using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Core.Communication;
using pdfforge.PDFCreator.Core.JobInfoQueue;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.JobHistory;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.Utilities.Threading;
using System;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Core.Startup.AppStarts
{
    public interface IMaybePipedApplicationStarter
    {
        ICheckAllStartupConditions StartupConditions { get; }

        Task<bool> SendMessageOrStartApplication(Func<string> composePipeMessage, Func<bool> startApplication, bool startManagePrintJobs);
    }

    public class MaybePipedApplicationStarter : IMaybePipedApplicationStarter
    {
        private readonly IPdfCreatorFolderCleanUp _folderCleanUp;
        private readonly IJobInfoQueue _jobInfoQueue;
        private readonly IJobInfoQueueManager _jobInfoQueueManager;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPipeServerManager _pipeServerManager;
        private readonly ISettingsManager _settingsManager;
        private readonly ISpooledJobFinder _spooledJobFinder;
        private readonly IJobHistoryActiveRecord _jobHistoryActiveRecord;
        private readonly IThreadManager _threadManager;
        private readonly IUpdateHelper _updateHelper;

        public MaybePipedApplicationStarter(ISettingsManager settingsManager, IUpdateHelper updateHelper,
            ICheckAllStartupConditions startupConditions, IThreadManager threadManager,
            IPipeServerManager pipeServerManager, IJobInfoQueueManager jobInfoQueueManager, IJobInfoQueue jobInfoQueue,
            IStaticPropertiesHack staticPropertiesHack, IPdfCreatorFolderCleanUp folderCleanUp, ISpooledJobFinder spooledJobFinder,
            IJobHistoryActiveRecord jobHistoryActiveRecord)
        {
            StartupConditions = startupConditions;
            _jobInfoQueue = jobInfoQueue;
            _folderCleanUp = folderCleanUp;
            _spooledJobFinder = spooledJobFinder;
            _jobHistoryActiveRecord = jobHistoryActiveRecord;
            _settingsManager = settingsManager;
            _updateHelper = updateHelper;
            _threadManager = threadManager;
            _pipeServerManager = pipeServerManager;
            _jobInfoQueueManager = jobInfoQueueManager;

            staticPropertiesHack.SetStaticProperties();
        }

        public int Retries { get; set; } = 5;

        public ICheckAllStartupConditions StartupConditions { get; }

        public async Task<bool> SendMessageOrStartApplication(Func<string> composePipeMessage, Func<bool> startApplication, bool startManagePrintJobs)
        {
            _settingsManager.LoadAllSettings();
            var loadHistoryTask = Task.Run(() => _jobHistoryActiveRecord.Load());

            var pipeMessage = composePipeMessage();

            var retry = 0;
            var success = false;

            // Make n Attempts: Look if a pipe server exists, if so, send a message. If that fails, retry (and maybe do the job yourself)
            while (!success && (retry++ < Retries))
            {
                _logger.Debug("Starting attempt {0}: ", retry);
                if (_pipeServerManager.IsServerRunning())
                {
                    success = TrySendPipeMessage(pipeMessage);
                    _logger.Debug("TrySendPipeMessage: " + success);
                }
                else
                {
                    success = TryStartApplication(startApplication, startManagePrintJobs);
                    _logger.Debug("TryStartApplication: " + success);
                }
            }

            if (success)
            {
                _logger.Debug("Starting update check thread");
                StartUpdateCheck();

                _logger.Debug("Starting Cleanup thread");
                StartCleanUpThread();
            }

            await Shutdown();
            await loadHistoryTask;

            return success;
        }

        private async Task Shutdown()
        {
            _logger.Debug("Waiting for all threads to finish");
            await _threadManager.WaitForThreads();

            _pipeServerManager.PrepareShutdown();

            _threadManager.Shutdown();

            _settingsManager.SaveCurrentSettings();
            _jobHistoryActiveRecord.Save();

            _pipeServerManager.Shutdown();
        }

        private bool TrySendPipeMessage(string message)
        {
            _logger.Debug("Found another running instance of PDFCreator, so we send our data there");

            if (_pipeServerManager.TrySendPipeMessage(message))
            {
                _logger.Debug("Pipe message successfully sent");
                return true;
            }

            _logger.Warn("There was an error while communicating through the pipe");
            return false;
        }

        private bool TryStartApplication(Func<bool> startApplication, bool startManagePrintJobs)
        {
            var success = false;
            try
            {
                _logger.Debug("Starting pipe server");

                success = _pipeServerManager.StartServer();

                if (!success)
                    return false;

                _logger.Debug("Reloading settings");
                // Settings may have changed as this may have not been the only instance till now
                _settingsManager.LoadAllSettings();

                if (startManagePrintJobs)
                    _jobInfoQueueManager.ManagePrintJobs();

                _logger.Debug("Finding spooled jobs");

                var spooledJobs = _spooledJobFinder.GetJobs();
                _jobInfoQueue.Add(spooledJobs);

                success = startApplication();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "There was an error while starting the application");
                success = false;
                throw;
            }
            finally
            {
                if (!success)
                    _pipeServerManager.Shutdown();
            }
            return success;
        }

        private void StartCleanUpThread()
        {
            _threadManager.StartSynchronizedThread(CleanUp, "CleanUpThread");
        }

        private void CleanUp()
        {
            _folderCleanUp.CleanSpoolFolder(TimeSpan.FromDays(1));
            _folderCleanUp.CleanTempFolder(TimeSpan.FromDays(1));
        }

        private void StartUpdateCheck()
        {
            Task.Run(async () => await _updateHelper.UpdateCheckAsync(true));
        }
    }
}
