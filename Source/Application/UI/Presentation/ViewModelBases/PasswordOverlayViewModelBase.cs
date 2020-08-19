using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Controls;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;
using System;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.ViewModelBases
{
    public abstract class PasswordOverlayViewModelBase<TInteraction, TTranslation> : OverlayViewModelBase<TInteraction, TTranslation>, IPasswordButtonViewModel
        where TTranslation : ITranslatable, new()
        where TInteraction : BasicPasswordOverlayInteraction
    {
        public Action RaiseOkCanExecuteChanged { get; set; }

        public string Password
        {
            get { return PasswordButtonController.Password; }
            set { PasswordButtonController.Password = value; }
        }

        protected PasswordOverlayViewModelBase(ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
            PasswordButtonController = new PasswordButtonController(translationUpdater, this, false, true);
        }

        protected override void HandleInteractionObjectChanged()
        {
            Password = Interaction.Password;
            RaisePropertyChanged(nameof(Password));
        }

        public virtual void SetPassword(string values)
        {
            Interaction.Password = Password;
        }

        public PasswordButtonController PasswordButtonController { get; }

        public void ClearPasswordFields()
        {
            Password = string.Empty;
        }

        public void FinishedHook()
        {
            FinishInteraction();
        }

        public void SkipHook()
        {
            Interaction.Result = PasswordResult.Skip;
        }

        public void CancelHook()
        {
            Interaction.Result = PasswordResult.Cancel;
        }

        public void OkHook()
        {
            Interaction.Result = PasswordResult.StorePassword;
        }

        public void RemoveHook()
        {
            Interaction.Result = PasswordResult.RemovePassword;
        }

        public virtual bool CanExecuteHook()
        {
            return !string.IsNullOrEmpty(Password);
        }
    }
}
