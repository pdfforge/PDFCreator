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
        private readonly IRecommendArchitect _recommendArchitect;
        private readonly IDefaultViewerCheck _defaultViewerCheck;

        /// <summary>
        ///     Creates a new default viewer action.
        /// </summary>
        public DefaultViewerAction(IFileAssoc fileAssoc, IRecommendArchitect recommendArchitect,
            IPdfArchitectCheck pdfArchitectCheck, ISettingsProvider settingsProvider,
            OutputFormatHelper outputFormatHelper, IProcessStarter processStarter,
            IDefaultViewerCheck defaultViewerCheck)
        {
            _fileAssoc = fileAssoc;
            _recommendArchitect = recommendArchitect;
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

            var isPdf = job.Profile.OutputFormat.IsPdf();
            if (isPdf && job.Profile.OpenWithPdfArchitect && _pdfArchitectCheck.IsInstalled())
                return OpenWithArchitect(file);

            return OpenOutputFile(file);
        }

        public ActionResult OpenWithArchitect(string filePath)
        {
            string architectPath = _pdfArchitectCheck.GetInstallationPath();

            Logger.Debug("Open with PDF Architect");
            try
            {
                _processStarter.Start(architectPath, "\"" + filePath + "\"");
                Logger.Trace("Openend: " + filePath);
            }
            catch
            {
                Logger.Error("PDF Architect could not open file: " + filePath);
                return new ActionResult(ErrorCode.Viewer_ArchitectCouldNotOpenOutput);
            }
            return new ActionResult();
        }

        public bool IsEnabled(ConversionProfile profile)
        {
            return profile.OpenViewer;
        }

        public ActionResult OpenOutputFile(string filePath)
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

                if (outputFormatByPath.IsPdf() && !_fileAssoc.HasOpen(".pdf"))
                {
                    if (_pdfArchitectCheck.IsInstalled())
                        return OpenWithArchitect(filePath);

                    _recommendArchitect.Show();
                    return new ActionResult(); //return true, to avoid another message window.
                }

                Logger.Debug("Open file with system default application: " + filePath);
                _processStarter.Start(filePath);
            }
            catch
            {
                Logger.Error("File could not be opened.");
            }

            return new ActionResult();
        }
    }
}
