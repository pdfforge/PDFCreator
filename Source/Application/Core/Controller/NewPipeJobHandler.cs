using Newtonsoft.Json;
using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Core.Communication;
using pdfforge.PDFCreator.Core.DirectConversion;
using pdfforge.PDFCreator.Core.JobInfoQueue;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Utilities.Threading;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace pdfforge.PDFCreator.Core.Controller
{
    public class NewPipeJobHandler : IPipeMessageHandler
    {
        private readonly IFileConversionAssistant _fileConversionAssistant;
        private readonly IJobInfoManager _jobInfoManager;
        private readonly IThreadManager _threadManager;
        private readonly IJobInfoQueueManager _jobInfoQueueManager;
        private readonly IJobInfoQueue _jobInfoQueue;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IMainWindowThreadLauncher _mainWindowThreadLauncher;
        private readonly ISettingsManager _settingsManager;

        public NewPipeJobHandler(IJobInfoQueue jobInfoQueue, ISettingsManager settingsManager,
            IFileConversionAssistant fileConversionAssistant, IMainWindowThreadLauncher mainWindowThreadLauncher,
            IJobInfoManager jobInfoManager, IThreadManager threadManager, IJobInfoQueueManager jobInfoQueueManager)
        {
            _jobInfoQueue = jobInfoQueue;
            _settingsManager = settingsManager;
            _fileConversionAssistant = fileConversionAssistant;
            _mainWindowThreadLauncher = mainWindowThreadLauncher;
            _jobInfoManager = jobInfoManager;
            _threadManager = threadManager;
            _jobInfoQueueManager = jobInfoQueueManager;
        }

        public void HandlePipeMessage(string message)
        {
            _logger.Debug("New Message received: " + message);
            if (message.StartsWith("NewJob|", StringComparison.OrdinalIgnoreCase))
            {
                HandleNewJobMessage(message);
            }
            else if (message.StartsWith("DragAndDrop|", StringComparison.OrdinalIgnoreCase))
            {
                HandleDroppedFileMessage(message);
            }
            else if (message.StartsWith("ShowMain|", StringComparison.OrdinalIgnoreCase))
            {
                _mainWindowThreadLauncher.LaunchMainWindow();
            }
            else if (message.StartsWith("ReloadSettings|", StringComparison.OrdinalIgnoreCase))
            {
                _logger.Info("Pipe Command: Reloading settings");
                _settingsManager.LoadAllSettings();
            }
        }

        private void HandleDroppedFileMessage(string message)
        {
            var messageParts = message.Split('|')
                .Skip(1)
                .ToList();

            var droppedFiles = messageParts
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Where(s => !s.StartsWith("{")); // remove parameters json

            var parametersJson = messageParts.SingleOrDefault(s => s.StartsWith("{"));
            var parameters = parametersJson != null
                ? JsonConvert.DeserializeObject<AppStartParameters>(parametersJson)
                : new AppStartParameters();

            if (parameters.ManagePrintJobs && _mainWindowThreadLauncher.IsPrintJobShellOpen())
                _mainWindowThreadLauncher.SwitchPrintJobShellToMergeWindow();
            else if (parameters.ManagePrintJobs)
                _jobInfoQueueManager.ManagePrintJobs();

            var threadStart = new ThreadStart(() => _fileConversionAssistant.HandleFileListWithoutTooManyFilesWarning(droppedFiles, parameters));

            _threadManager.StartSynchronizedUiThread(threadStart, "PipeDragAndDrop");
        }

        private void HandleNewJobMessage(string message)
        {
            var newJobs = message.Split('|')
                .Skip(1)
                .Where(s => !string.IsNullOrWhiteSpace(s));

            foreach (var infFile in newJobs)
            {
                try
                {
                    _logger.Debug("NewJob found: " + infFile);
                    if (!File.Exists(infFile))
                        return;

                    _logger.Trace("The given file exists.");
                    var jobInfo = _jobInfoManager.ReadFromInfFile(infFile);
                    _jobInfoQueue.Add(jobInfo);
                }
                catch (Exception ex)
                {
                    _logger.Warn(ex, "There was an Exception while adding the print job: ");
                }
            }
        }
    }
}
