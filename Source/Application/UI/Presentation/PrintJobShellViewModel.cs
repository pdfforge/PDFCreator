using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.UI.Presentation.Events;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities;
using Prism.Commands;
using Prism.Events;
using System.Windows;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation
{
    public class PrintJobShellViewModel : TranslatableViewModelBase<PrintJobShellTranslation>
    {
        private readonly IVersionHelper _versionHelper;
        private bool _showLockLayer;

        public ApplicationNameProvider ApplicationName { get; }
        public string ApplicationNameAndVersion => ApplicationName.ApplicationName + " " + _versionHelper.FormatWithThreeDigits();
        public IInteractionRequest InteractionRequest { get; }
        public ICommand DragEnterCommand { get; }

        public ICommand DragDropCommand { get; }

        public bool ShowLockLayer
        {
            get { return _showLockLayer; }
            set
            {
                _showLockLayer = value;
                RaisePropertyChanged(nameof(ShowLockLayer));
            }
        }

        public PrintJobShellViewModel(ApplicationNameProvider applicationNameProvider, IInteractionRequest interactionRequest,
            ITranslationUpdater updater, IEventAggregator eventAggregator, DragAndDropEventHandler dragAndDrop, IVersionHelper versionHelper) : base(updater)
        {
            _versionHelper = versionHelper;
            ApplicationName = applicationNameProvider;
            InteractionRequest = interactionRequest;

            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<ShowLockLayerEvent>().Subscribe(() => ShowLockLayer = true);
                eventAggregator.GetEvent<HideLockLayerEvent>().Subscribe(() => ShowLockLayer = false);
            }

            DragEnterCommand = new DelegateCommand<DragEventArgs>(dragAndDrop.HandleDragEnter);
            DragDropCommand = new DelegateCommand<DragEventArgs>(dragAndDrop.HandleDropEvent);
        }
    }
}
