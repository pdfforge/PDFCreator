using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public class OutputFormatViewModel : ProfileUserControlViewModel<OutputFormatTranslation>, IStatusHintViewModel
    {
        public OutputFormatViewModel(ITranslationUpdater updater, ISelectedProfileProvider selectedProfile, IDispatcher dispatcher) : base(updater, selectedProfile, dispatcher)
        {
            SetOutputFormatCommand = new DelegateCommand<OutputFormat>(SetOutputFormatExecute);
        }

        public bool HideStatusInOverlay => true;
        public string StatusText => "";
        public bool HasWarning => false;

        // Required for SelectOutputFormatContextMenuButton
        public OutputFormat OutputFormat => CurrentProfile?.OutputFormat ?? OutputFormat.Pdf;

        public DelegateCommand<OutputFormat> SetOutputFormatCommand { get; }

        private void SetOutputFormatExecute(OutputFormat parameter)
        {
            CurrentProfile.OutputFormat = parameter;
            RaisePropertyChanged(nameof(OutputFormat));
        }

        private void OnCurrentProfileChanged(object sender, EventArgs args)
        {
            RaisePropertyChanged(nameof(OutputFormat));
        }

        public override void MountView()
        {
            base.MountView();
            CurrentProfileChanged += OnCurrentProfileChanged;
        }

        public override void UnmountView()
        {
            base.UnmountView();
            CurrentProfileChanged -= OnCurrentProfileChanged;
        }
    }
}
