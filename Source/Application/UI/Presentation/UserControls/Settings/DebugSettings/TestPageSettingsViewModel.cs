using System.Threading.Tasks;
using System.Windows.Input;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Logging;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Commands.EvaluateSettingsCommands;
using pdfforge.PDFCreator.UI.Presentation.Helper.TestPage;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings
{
    public class TestPageSettingsViewModelBase : ADebugSettingsItemControlModel
    {
        public TestPageSettingsViewModelBase(ITranslationUpdater translationUpdater, IGpoSettings gpoSettings) : base(translationUpdater, gpoSettings)
        {
        }

        public virtual bool CanBeShown => true;
    }

    public class ServerTestPageSettingsViewModel : TestPageSettingsViewModelBase
    {
        public ServerTestPageSettingsViewModel(ITranslationUpdater translationUpdater, IGpoSettings gpoSettings) : base(translationUpdater, gpoSettings)
        {
        }

        public override bool CanBeShown => false;
    }

    public class CreatorTestPageSettingsViewModel : TestPageSettingsViewModelBase
    {
        protected readonly IPrinterHelper _printerHelper;
        private readonly IPdfProcessor _pdfProcessor;
        protected readonly ITestPageHelper _testPageHelper;
        protected readonly ICurrentSettings<CreatorAppSettings> _settingsProvider;
        protected readonly ICurrentSettings<ApplicationSettings> _applicationSettingsProvider;

        public CreatorTestPageSettingsViewModel(
            ITestPageHelper testPageHelper,
            ICurrentSettings<CreatorAppSettings> settingsProvider,
            ICurrentSettings<ApplicationSettings> applicationSettingsProvider,
            IPrinterHelper printerHelper,
            ITranslationUpdater translationUpdater,
            IGpoSettings gpoSettings,
            IPdfProcessor pdfProcessor) :
            base(translationUpdater, gpoSettings)
        {
            PrintPdfCreatorTestPageCommand = new AsyncCommand(PdfCreatorTestPageExecute);
            PrintWindowsTestPageCommand = new AsyncCommand(WindowsTestPageExecute);
            _printerHelper = printerHelper;
            _pdfProcessor = pdfProcessor;
            _testPageHelper = testPageHelper;
            _settingsProvider = settingsProvider;
            _applicationSettingsProvider = applicationSettingsProvider;
        }

        public ICommand PrintPdfCreatorTestPageCommand { get; protected set; }
        public ICommand PrintWindowsTestPageCommand { get; }

        protected virtual async Task PdfCreatorTestPageExecute(object o)
        {

            await Task.Run(() =>
            {
                LoggingHelper.ChangeLogLevel(_applicationSettingsProvider.Settings.LoggingLevel);
                _testPageHelper.CreateAndPrintTestPage();
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
                                                .AddCommand<EvaluateSavingRelevantSettingsAndNotifyUserCommand>()
                                                .AddCommand<AskForSavingCommand>()
                                                .AddCommand<ISaveChangedSettingsCommand>()
                                                .AddCommand<IPrintTestPageAsyncCommand>()
                                                .Build();
        }

        public IMacroCommand PrintPdfCreatorTestPageCommand { get; private set; }
    }
}