using System.Threading.Tasks;
using System.Windows.Input;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Logging;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings
{
    public class TestPageSettingsViewModel : ADebugSettingsItemControlModel
    {
        protected readonly IPrinterHelper _printerHelper;
        protected readonly ITestPageHelper _testPageHelper;
        protected readonly ICurrentSettings<CreatorAppSettings> _settingsProvider;
        protected readonly ICurrentSettings<ApplicationSettings> _applicationSettingsProvider;

        public TestPageSettingsViewModel(
            ITestPageHelper testPageHelper,
            ICurrentSettings<CreatorAppSettings> settingsProvider,
            ICurrentSettings<ApplicationSettings> applicationSettingsProvider,
            IPrinterHelper printerHelper,
            ITranslationUpdater translationUpdater,
            IGpoSettings gpoSettings) :
            base(translationUpdater, gpoSettings)
        {
            PrintPdfCreatorTestPageCommand = new AsyncCommand(PdfCreatorTestPageExecute);
            PrintWindowsTestPageCommand = new AsyncCommand(WindowsTestPageExecute);
            _printerHelper = printerHelper;
            _testPageHelper = testPageHelper;
            _settingsProvider = settingsProvider;
            _applicationSettingsProvider = applicationSettingsProvider;
        }

        public ICommand PrintPdfCreatorTestPageCommand { get; protected set; }
        public ICommand PrintWindowsTestPageCommand { get; }

        protected virtual async Task PdfCreatorTestPageExecute(object o)
        {
            await Task.Run(() =>{
                LoggingHelper.ChangeLogLevel(_applicationSettingsProvider.Settings.LoggingLevel);
                _testPageHelper.CreateTestPage();
            });
        }

        private async Task WindowsTestPageExecute(object o)
        {
            await Task.Run(() =>
            {
                LoggingHelper.ChangeLogLevel(_applicationSettingsProvider.Settings.LoggingLevel);
                _printerHelper.PrintWindowsTestPage(_settingsProvider.Settings.PrimaryPrinter);
            });
        }
    }

    public class WorkflowTestPageSettingsViewModel :  TranslatableViewModelBase<DebugSettingsTranslation>
    {
        public WorkflowTestPageSettingsViewModel(
            ICommandLocator commandLocator,
            ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
            PrintPdfCreatorTestPageCommand = commandLocator
                                                .CreateMacroCommand()
                                                .AddCommand<AskForSavingCommand>()
                                                .AddCommand<ISaveChangedSettingsCommand>()
                                                .AddCommand<IPrintTestPageAsyncCommand>()
                                                .Build();
        }

        public IMacroCommand PrintPdfCreatorTestPageCommand { get; private set; }
    }
}