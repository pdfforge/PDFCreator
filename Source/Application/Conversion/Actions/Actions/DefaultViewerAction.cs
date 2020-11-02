using NLog;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Interface;
using pdfforge.PDFCreator.Conversion.Actions.Queries;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions
{
    /// <summary>
    ///     DefaultViewerAction opens the output files in the default viewer
    /// </summary>
    public class DefaultViewerAction : IDefaultViewerAction
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IPdfArchitectCheck _pdfArchitectCheck;
        private readonly ISettingsProvider _settingsProvider;
        private readonly OutputFormatHelper _outputFormatHelper;
        private readonly IProcessStarter _processStarter;
        private readonly IFileAssoc _fileAssoc;
        private readonly IRecommendArchitectAssistant _recommendArchitectAssistant;
        private readonly IDefaultViewerCheck _defaultViewerCheck;

        /// <summary>
        ///     Creates a new default viewer action.
        /// </summary>
        public DefaultViewerAction(IFileAssoc fileAssoc, IRecommendArchitectAssistant recommendArchitectAssistant,
            IPdfArchitectCheck pdfArchitectCheck, ISettingsProvider settingsProvider,
            OutputFormatHelper outputFormatHelper, IProcessStarter processStarter,
            IDefaultViewerCheck defaultViewerCheck)
        {
            _fileAssoc = fileAssoc;
            _recommendArchitectAssistant = recommendArchitectAssistant;
            _pdfArchitectCheck = pdfArchitectCheck;
            _settingsProvider = settingsProvider;
            _outputFormatHelper = outputFormatHelper;
            _processStarter = processStarter;
            _defaultViewerCheck = defaultViewerCheck;
        }

        /// <summary>
        ///     Open all files in the default viewer
        /// </summary>
        /// <param name="job">Job information</param>
        /// <returns>An ActionResult to determine the success and a list of errors</returns>
        public ActionResult ProcessJob(Job job)
        {
            Logger.Debug("Launched Viewer-Action");
            var file = job.OutputFiles.First();

            return OpenOutputFile(file, job.Profile.OpenViewer.OpenWithPdfArchitect);
        }

        public ActionResult OpenWithArchitect(List<string> files)
        {
            var architectPath = _pdfArchitectCheck.GetInstallationPath();

            Logger.Debug("Open with PDF Architect");

            if (_pdfArchitectCheck.IsInstalled())
            {
                try
                {
                    foreach (var file in files)
                    {
                        _processStarter.Start(architectPath, "\"" + file + "\"");
                        Logger.Trace("Openend: " + file);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "PDF Architect could not open file(s):\r\n" + string.Join("\r\n", files));
                    return new ActionResult(ErrorCode.Viewer_ArchitectCouldNotOpenOutput);
                }
            }
            else
            {
                _recommendArchitectAssistant.Show();
            }

            return new ActionResult();
        }

        public bool IsEnabled(ConversionProfile profile)
        {
            return profile.OpenViewer.Enabled;
        }

        private bool ShouldOpenWithArchitect(OutputFormat outputFormat, bool openWithPdfArchitectSetting)
        {
            if (!outputFormat.IsPdf())
                return false;

            if (_pdfArchitectCheck.IsInstalled())
            {
                if (openWithPdfArchitectSetting)
                    return true;

                if (!_fileAssoc.HasOpen(".pdf"))
                    return true;
            }

            return false;
        }

        private bool ShouldRecommendArchitect(OutputFormat outputFormat)
        {
            if (!outputFormat.IsPdf())
                return false;

            if (_pdfArchitectCheck.IsInstalled())
                return false;

            if (!_fileAssoc.HasOpen(".pdf"))
                return true;

            return false;
        }

        public ActionResult OpenOutputFile(string filePath, bool openWithPdfArchitect = false)
        {
            var outputFormatByPath = _outputFormatHelper.GetOutputFormatByPath(filePath);
            var defaultViewer = _settingsProvider.Settings.GetDefaultViewerByOutputFormat(outputFormatByPath);
            try
            {
                if (defaultViewer.IsActive)
                {
                    var result = _defaultViewerCheck.Check(defaultViewer);
                    if (!result)
                        return result;

                    Logger.Debug($"Open \"{filePath}\" with default viewer: {defaultViewer.Path} ");
                    _processStarter.Start(defaultViewer.Path, "\"" + filePath + "\"");
                    return new ActionResult();
                }

                if (ShouldOpenWithArchitect(outputFormatByPath, openWithPdfArchitect))
                {
                    return OpenWithArchitect(new List<string> { filePath });
                }

                if (ShouldRecommendArchitect(outputFormatByPath))
                {
                    _recommendArchitectAssistant.Show();
                }
                else
                {
                    Logger.Debug("Open file with system default application: " + filePath);
                    _processStarter.Start(filePath);
                }
            }
            catch
            {
                Logger.Error("File could not be opened.");
            }

            return new ActionResult();
        }
    }
}
