using NLog;
using pdfforge.PDFCreator.Conversion.Actions.Queries;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;
using System;
using System.Diagnostics;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.QuickActions
{
    public class QuickActionPrintWithPdfArchitectCommand : QuickActionCommandBase<FtpActionTranslation>
    {
        private readonly IPdfArchitectCheck _architectCheck;
        private readonly IRecommendArchitectAssistant _recommendArchitectAssistant;
        private readonly IProcessStarter _processStarter;
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly Version MinimumArchitectPrintDialogVersion = new Version(6, 1, 24, 0);

        public QuickActionPrintWithPdfArchitectCommand(ITranslationUpdater translationUpdater, IPdfArchitectCheck architectCheck, IRecommendArchitectAssistant recommendArchitectAssistant, IProcessStarter processStarter) : base(translationUpdater)
        {
            _architectCheck = architectCheck;
            _recommendArchitectAssistant = recommendArchitectAssistant;
            _processStarter = processStarter;
        }

        public override void Execute(object obj)
        {
            if (_architectCheck.IsInstalled())
            {
                var files = GetPaths(obj);

                string architectPath = _architectCheck.GetInstallationPath();
                FileVersionInfo info = FileVersionInfo.GetVersionInfo(architectPath);
                var version = new Version(info.ProductVersion);

                if (version >= MinimumArchitectPrintDialogVersion)
                {
                    foreach (var file in files)
                    {
                        PrintWithArchitect(file);
                    }
                }
                else
                {
                    _recommendArchitectAssistant.Show(PdfArchitectRecommendPurpose.UpdateRequired);
                }
            }
            else
            {
                _recommendArchitectAssistant.Show(PdfArchitectRecommendPurpose.NotInstalled);
            }
        }

        private void PrintWithArchitect(string filePath)
        {
            string architectPath = _architectCheck.GetInstallationPath();
            try
            {
                try
                {
                    _processStarter.Start(architectPath, "--print-dlg \"" + filePath + "\"");
                    Logger.Trace("Print: " + filePath);
                }
                catch
                {
                    Logger.Error("PDF Architect could not print file: " + filePath);
                    throw;
                }
            }
            catch
            {
                Logger.Error("PDF Architect exe could not be read");
                throw;
            }

            Logger.Debug("Open with PDF Architect");
        }
    }
}
