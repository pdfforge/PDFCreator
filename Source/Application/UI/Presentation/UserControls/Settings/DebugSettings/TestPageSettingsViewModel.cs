using System.Windows.Input;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.Services.Logging;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings
{
    public class TestPageSettingsViewModel : ADebugSettingsItemControlModel
    {
        private readonly IPrinterHelper _printerHelper;
        private readonly ITestPageHelper _testPageHelper;
        private readonly ICurrentSettings<CreatorAppSettings> _settingsProvider;
        private readonly ICurrentSettings<ApplicationSettings> _applicationSettingsProvider;

        public TestPageSettingsViewModel(
            ITestPageHelper testPageHelper,
            ICurrentSettings<CreatorAppSettings> settingsProvider,
            ICurrentSettings<ApplicationSettings> applicationSettingsProvider,
            IPrinterHelper printerHelper,
            ITranslationUpdater translationUpdater,
            IGpoSettings gpoSettings) :
            base(translationUpdater, gpoSettings)
        {
            PrintPdfCreatorTestpageCommand = new DelegateCommand(PdfCreatorTestpageExecute);
            PrintWindowsTestpageCommand = new DelegateCommand(WindowsTestpageExecute);
            _printerHelper = printerHelper;
            _testPageHelper = testPageHelper;
            _settingsProvider = settingsProvider;
            _applicationSettingsProvider = applicationSettingsProvider;
        }

        public ICommand PrintPdfCreatorTestpageCommand { get; }
        public ICommand PrintWindowsTestpageCommand { get; }

        private void PdfCreatorTestpageExecute(object o)
        {
            LoggingHelper.ChangeLogLevel(_applicationSettingsProvider.Settings.LoggingLevel);
            _testPageHelper.CreateTestPage();
        }

        private void WindowsTestpageExecute(object o)
        {
            LoggingHelper.ChangeLogLevel(_applicationSettingsProvider.Settings.LoggingLevel);
            _printerHelper.PrintWindowsTestPage(_settingsProvider.Settings.PrimaryPrinter);
        }
    }
}