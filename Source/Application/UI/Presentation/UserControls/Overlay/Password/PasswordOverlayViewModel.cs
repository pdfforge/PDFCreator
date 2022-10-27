using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities.Threading;
using System;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Overlay.Password
{
    public class PasswordOverlayViewModel : OverlayViewModelBase<PasswordOverlayInteraction, PasswordOverlayTranslation>
    {
        public PasswordOverlayViewModel(ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
            OkCommand = new DelegateCommand(OkExecute, OkCanExecute);
            RemoveCommand = new DelegateCommand(ExecuteRemove);
            CancelCommand = new DelegateCommand(ExecuteCancel);
        }

        public DelegateCommand OkCommand { get; protected set; }
        public DelegateCommand RemoveCommand { get; protected set; }
        public DelegateCommand CancelCommand { get; protected set; }

        public string Password
        {
            set
            {
                Interaction.Password = value;
                OkCommand.RaiseCanExecuteChanged();
            }
        }

        public bool ShowIntroText => !string.IsNullOrWhiteSpace(Interaction?.IntroText);
        public bool CanRemovePassword => Interaction?.MiddleButtonAction == PasswordMiddleButton.Remove;

        public Action<string> SetPasswordAction { get; set; }

        protected override void HandleInteractionObjectChanged()
        {
            SetPasswordAction?.Invoke(Interaction.Password);
            OkCommand.RaiseCanExecuteChanged();

            RaisePropertyChanged(nameof(CanRemovePassword));
            RaisePropertyChanged(nameof(ShowIntroText));
        }

        public override string Title => Interaction.Title;

        private void ExecuteCancel(object obj)
        {
            Interaction.Password = "";
            Interaction.Result = PasswordResult.Cancel;
            FinishInteraction();
        }

        private void ExecuteRemove(object obj)
        {
            Interaction.Password = "";
            Interaction.Result = PasswordResult.RemovePassword;
            FinishInteraction();
        }

        private bool OkCanExecute(object obj)
        {
            return !string.IsNullOrEmpty(Interaction?.Password);
        }

        private void OkExecute(object obj)
        {
            Interaction.Result = PasswordResult.StorePassword;
            FinishInteraction();
        }
    }
}
