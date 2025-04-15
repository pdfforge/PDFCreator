using NLog;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Interface;
using pdfforge.PDFCreator.Conversion.Actions.Queries;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.Helpers;
using pdfforge.PDFCreator.Core.SettingsManagementInterface;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions
{
    /// <summary>
    ///     OpenFileAction opens the output files in the default viewer
    /// </summary>
    public class OpenFileAction : ActionBase<OpenViewer>, IOpenFileAction
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
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
        public OpenFileAction(IFileAssoc fileAssoc, IRecommendArchitectAssistant recommendArchitectAssistant,
            IPdfArchitectCheck pdfArchitectCheck, ISettingsProvider settingsProvider,
            OutputFormatHelper outputFormatHelper, IProcessStarter processStarter,
            IDefaultViewerCheck defaultViewerCheck)
            : base(p => p.OpenViewer)
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
        /// <param name="processor"></param>
        /// <returns>An ActionResult to determine the success and a list of errors</returns>
        protected override ActionResult DoProcessJob(Job job, IPdfProcessor processor)
        {
            Logger.Debug("Launched Viewer-Action");
            var file = job.OutputFiles.First();

            return OpenOutputFile(file, job.Profile.OpenViewer.OpenWithPdfArchitect);
        }

        public ActionResult OpenWithArchitect(List<string> files)
        {
            if (!_pdfArchitectCheck.IsInstalled())
                return DoRecommendArchitect(PdfArchitectRecommendPurpose.NotInstalled);

            return DoOpenWithArchitect(files);
        }

        public ActionResult OpenOutputFile(string filePath, bool openWithPdfArchitect = false)
        {
            var outputFormatByPath = _outputFormatHelper.GetOutputFormatByPath(filePath);
            var defaultViewer = _settingsProvider.Settings.GetDefaultViewerByOutputFormat(outputFormatByPath);

            if (defaultViewer.IsActive)
                return DoOpenWithDefaultViewer(defaultViewer, filePath);

            if (!outputFormatByPath.IsPdf())
                return DoOpenWithSystemDefaultApplication(filePath);

            var hasOpen = _fileAssoc.HasOpen(".pdf");
            Logger.Debug($"HasOpen for .pdf is: {hasOpen}");

            if (_pdfArchitectCheck.IsInstalled())
            {
                if (openWithPdfArchitect || !hasOpen)
                    return DoOpenWithArchitect(new List<string> { filePath });
            }
            else
            {
                //No PDF Architect installed, but can be opened
                if (hasOpen)
                    if (_settingsProvider.Settings.CreatorAppSettings.DontRecommendArchitect)
                        return DoOpenWithSystemDefaultApplication(filePath);
                    else
                        return DoRecommendArchitect(PdfArchitectRecommendPurpose.NotInstalled);

                return DoRecommendArchitect(PdfArchitectRecommendPurpose.NoPdfViewer);
            }

            return DoOpenWithSystemDefaultApplication(filePath);
        }

        private ActionResult DoRecommendArchitect(PdfArchitectRecommendPurpose purpose)
        {
            _recommendArchitectAssistant.Show(purpose);
            return new ActionResult();
        }

        private ActionResult DoOpenWithArchitect(IList<string> files)
        {
            Logger.Debug("Open with PDF Architect");

            try
            {
                var architectPath = _pdfArchitectCheck.GetInstallationPath();
                foreach (var file in files)
                {
                    _processStarter.Start(architectPath, "\"" + file + "\"");
                    Logger.Trace("Openend: " + file);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "PDF Architect could not open file(s):\r\n" + string.Join("\r\n", files));
            }

            return new ActionResult();
        }

        private ActionResult DoOpenWithDefaultViewer(DefaultViewer defaultViewer, string filePath)
        {
            var result = _defaultViewerCheck.Check(defaultViewer);
            if (!result)
                return result;

            try
            {
                Logger.Debug($"Open \"{filePath}\" with default viewer: {defaultViewer.Path} ");
                _processStarter.Start(defaultViewer.Path, defaultViewer.Parameters + " \"" + filePath + "\"");
                return new ActionResult();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Could not open file with default viewer.");
                return new ActionResult(ErrorCode.Viewer_CouldNotOpenOutputFileWithDefaultViewer);
            }
        }

        private ActionResult DoOpenWithSystemDefaultApplication(string filePath)
        {
            try
            {
                Logger.Debug("Open file with system default application: " + filePath);
                _processStarter.Start(filePath, true);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "File could not be opened.");
            }
            return new ActionResult();
        }

        public override void ApplyPreSpecifiedTokens(Job job)
        {
            //Nothing to do here
        }

        public override ActionResult Check(ConversionProfile profile, CurrentCheckSettings settings, CheckLevel checkLevel)
        {
            //todo:
            //ProfileLevel: Maybe check default viewer installed?
            //              Maybe check Architect installed?

            //JobLevel: Nothing to do
            return new ActionResult();
        }

        public override bool IsRestricted(ConversionProfile profile)
        {
            return false;
        }

        protected override void ApplyActionSpecificRestrictions(Job job)
        { }
    }
}
