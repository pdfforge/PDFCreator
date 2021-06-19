using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using pdfforge.PDFCreator.Core.SettingsManagement.DefaultSettings;
using pdfforge.PDFCreator.Core.SettingsManagement.Helper;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.Print
{
    public class PrintUserControlViewModel : ActionViewModelBase<PrintingAction, PrintTabTranslation>
    {
        public PrintUserControlViewModel(IActionLocator actionLocator,
            ISystemPrinterProvider systemPrinterProvider,
            ITranslationUpdater translationUpdater,
            ErrorCodeInterpreter errorCodeInterpreter,
            ICurrentSettingsProvider currentSettingsProvider,
            IDispatcher dispatcher,
            IDefaultSettingsBuilder defaultSettingsBuilder,
            IActionOrderHelper actionOrderHelper,
            ICurrentSettings<ObservableCollection<PrinterMapping>> printerMappings)
            : base(actionLocator, errorCodeInterpreter, translationUpdater, currentSettingsProvider, dispatcher, defaultSettingsBuilder, actionOrderHelper)
        {
            _systemPrinterProvider = systemPrinterProvider;
            _printerMappings = printerMappings;
        }

        private readonly ISystemPrinterProvider _systemPrinterProvider;
        private readonly ICurrentSettings<ObservableCollection<PrinterMapping>> _printerMappings;

        // ReSharper disable once MemberCanBePrivate.Global
        public bool PrinterDialogOptionEnabled { get; set; } = true;

        public IEnumerable<string> InstalledPrinters => _systemPrinterProvider.GetInstalledPrinterNames();

        public EnumTranslation<SelectPrinter>[] SelectPrinterValues => PrinterDialogOptionEnabled ?
            Translation.SelectPrinterValues :
            (EnumTranslation<SelectPrinter>[])Translation.SelectPrinterValues.Where(x => x.Value != SelectPrinter.ShowDialog).ToArray();

        protected override string SettingsPreviewString => Translation.GetPrinterText(CurrentProfile.Printing.SelectPrinter, CurrentProfile.Printing.PrinterName);

        public override void MountView()
        {
            SelectedPrinter = CurrentProfile.Printing.PrinterName;
        }

        public string SelectedPrinter
        {
            get => CurrentProfile?.Printing.PrinterName;
            set
            {
                CurrentProfile.Printing.PrinterName = value;
                RaisePropertyChanged(nameof(IsProfilePrinter));
                RaisePropertyChanged(nameof(StatusText));
            }
        }

        public SelectPrinter PrinterSelecting
        {
            get => CurrentProfile?.Printing.SelectPrinter ?? SelectPrinter.DefaultPrinter;
            set
            {
                CurrentProfile.Printing.SelectPrinter = value;
                RaisePropertyChanged(nameof(IsProfilePrinter));
                RaisePropertyChanged(nameof(StatusText));
            }
        }

        public bool IsProfilePrinter => _printerMappings.Settings.FirstOrDefault(mapping => mapping.PrinterName == SelectedPrinter) != null && CurrentProfile.Printing.SelectPrinter == SelectPrinter.SelectedPrinter;
    }
}
