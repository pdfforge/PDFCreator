using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Overlay.Encryption
{
    public class EncryptionPasswordUserControlViewModel : OverlayViewModelBase<EncryptionPasswordInteraction, EncryptionPasswordsUserControlTranslation>
    {
        public bool AllowConversionInterrupts { get; set; } = true;

        public EncryptionPasswordUserControlViewModel(ITranslationUpdater updater) : base(updater)
        {
            OkCommand = new DelegateCommand(ExecuteOk, CanExecuteOk);
            RemoveCommand = new DelegateCommand(ExecuteRemove);
            SkipCommand = new DelegateCommand(ExecuteSkip);
            CancelCommand = new DelegateCommand(ExecuteCancel);
        }

        public DelegateCommand OkCommand { get; protected set; }
        public DelegateCommand RemoveCommand { get; protected set; }
        public DelegateCommand SkipCommand { get; protected set; }
        public DelegateCommand CancelCommand { get; protected set; }

        public string OwnerPassword
        {
            get => Interaction.OwnerPassword;
            set
            {
                Interaction.OwnerPassword = value;
                OkCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(nameof(OwnerPassword));
            }
        }

        public string UserPassword
        {
            get => Interaction.UserPassword;
            set
            {
                Interaction.UserPassword = value;
                OkCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(nameof(UserPassword));
            }
        }

        protected override void HandleInteractionObjectChanged()
        {
            OwnerPassword = Interaction.OwnerPassword;
            UserPassword = Interaction.UserPassword;
        }

        public override string Title => Translation.Title;

        private void ExecuteOk(object obj)
        {
            Interaction.Response = PasswordResult.StorePassword;
            FinishInteraction();
        }

        private bool CanExecuteOk(object obj)
        {
            if (Interaction == null)
                return false;

            if (!Interaction.Skip && AllowConversionInterrupts)
                return true;

            if (Interaction.AskOwnerPassword)
                if (string.IsNullOrWhiteSpace(Interaction.OwnerPassword))
                    return false;

            if (Interaction.AskUserPassword)
                if (string.IsNullOrWhiteSpace(Interaction.UserPassword))
                    return false;

            return true;
        }

        private void ExecuteSkip(object obj)
        {
            Interaction.OwnerPassword = "";
            Interaction.UserPassword = "";
            Interaction.Response = PasswordResult.Skip;
            FinishInteraction();
        }

        private void ExecuteCancel(object obj)
        {
            Interaction.OwnerPassword = "";
            Interaction.UserPassword = "";
            Interaction.Response = PasswordResult.Cancel;
            FinishInteraction();
        }

        private void ExecuteRemove(object obj)
        {
            Interaction.OwnerPassword = "";
            Interaction.UserPassword = "";
            Interaction.Response = PasswordResult.RemovePassword;
            FinishInteraction();
        }
    }

    public class DesignTimeEncryptionPasswordUserControlViewModel : EncryptionPasswordUserControlViewModel
    {
        public DesignTimeEncryptionPasswordUserControlViewModel() : base(new DesignTimeTranslationUpdater())
        {
        }
    }
}
