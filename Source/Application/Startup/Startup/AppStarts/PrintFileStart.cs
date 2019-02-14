using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Core.DirectConversion;
using pdfforge.PDFCreator.Core.Printing.Printing;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.StartupInterface;
using System.IO;

namespace pdfforge.PDFCreator.Core.Startup.AppStarts
{
    public class PrintFileStart : AppStartBase
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPrintFileHelper _printFileHelper;
        private readonly ISettingsManager _settingsManager;
        private readonly IStoredParametersManager _storedParametersManager;

        public PrintFileStart(ICheckAllStartupConditions startupConditions,
            IPrintFileHelper printFileHelper,
            ISettingsManager settingsManager,
            IStoredParametersManager storedParametersManager)
            : base(startupConditions)
        {
            _printFileHelper = printFileHelper;
            _settingsManager = settingsManager;
            _storedParametersManager = storedParametersManager;
        }

        public string PrintFile { get; internal set; }
        public AppStartParameters AppStartParameters { get; internal set; }

        public override ExitCode Run()
        {
            _logger.Info("Launched printjob with PrintFile command.");

            if (string.IsNullOrEmpty(PrintFile))
            {
                _logger.Error("PrintFile Parameter has no argument");
                return ExitCode.PrintFileParameterHasNoArgument;
            }

            if (!File.Exists(PrintFile))
            {
                _logger.Error("The file \"{0}\" does not exist!", PrintFile);
                return ExitCode.PrintFileDoesNotExist;
            }

            _settingsManager.LoadAllSettings();

            _printFileHelper.PdfCreatorPrinter = GetValidPrinterName();

            if (!_printFileHelper.AddFile(PrintFile))
            {
                _logger.Warn("The file \"{0}\" is not printable!", PrintFile);
                return ExitCode.PrintFileNotPrintable;
            }

            //todo: Test
            // Ignore Profile if Printer is set.
            // After the printing we can't determine if the printer was selected by the user or was the primary printer
            var profile = "";
            if (string.IsNullOrWhiteSpace(AppStartParameters.Printer))
                profile = AppStartParameters.Profile;
            _storedParametersManager.SaveParameterSettings(AppStartParameters.OutputFile, profile, PrintFile);

            if (!_printFileHelper.PrintAll())
            {
                _logger.Error("The file \"{0}\" could not be printed!", PrintFile);
                return ExitCode.PrintFileCouldNotBePrinted;
            }

            return ExitCode.Ok;
        }

        private string GetValidPrinterName()
        {
            if (!string.IsNullOrWhiteSpace(AppStartParameters.Printer))
                return AppStartParameters.Printer;

            var settingsProvider = _settingsManager.GetSettingsProvider();
            return settingsProvider.Settings.CreatorAppSettings.PrimaryPrinter;
        }
    }
}
