using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.UI.Presentation.Wrapper;
using System.Threading;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Overlay.Encryption
{
    public class EncryptionPasswordUserControlViewModel : OverlayViewModelBase<EncryptionPasswordInteraction, EncryptionPasswordsUserControlTranslation>
    {
        private readonly ZxcvbnProvider _zxcvbnProvider;
        public bool AllowConversionInterrupts { get; set; } = true;

        public EncryptionPasswordUserControlViewModel(ITranslationUpdater updater, ZxcvbnProvider zxcvbnProvider) : base(updater)
        {
            _zxcvbnProvider = zxcvbnProvider;

            OkCommand = new DelegateCommand(ExecuteOk, CanExecuteOk);
            RemoveCommand = new DelegateCommand(ExecuteRemove);
            SkipCommand = new DelegateCommand(ExecuteSkip);
            CancelCommand = new DelegateCommand(ExecuteCancel);
        }

        public DelegateCommand OkCommand { get; protected set; }
        public DelegateCommand RemoveCommand { get; protected set; }
        public DelegateCommand SkipCommand { get; protected set; }
        public DelegateCommand CancelCommand { get; protected set; }

        public double EntropyPercentageOwner { get; set; }
        public double EntropyPercentageUser { get; set; }

        private double EntropyCheck(string password)
        {
            if (password == null)
                return 0.0;

            var evaluator = _zxcvbnProvider.GetInstanceAsync().Result;
            return evaluator.EvaluatePassword(password, CancellationToken.None).Entropy;
        }

        public string OwnerPassword
        {
            get => Interaction?.OwnerPassword;
            set
            {
                EntropyPercentageOwner = EntropyCheck(value);
                Interaction.OwnerPassword = value;
                OkCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(nameof(OwnerPassword));
                RaisePropertyChanged(nameof(EntropyPercentageOwner));
            }
        }

        public string UserPassword
        {
            get => Interaction?.UserPassword;
            set
            {
                EntropyPercentageUser = EntropyCheck(value);
                Interaction.UserPassword = value;
                OkCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(nameof(UserPassword));
                RaisePropertyChanged(nameof(EntropyPercentageUser));
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
        public DesignTimeEncryptionPasswordUserControlViewModel() : base(new DesignTimeTranslationUpdater(), null)
        {
        }
    }
}
