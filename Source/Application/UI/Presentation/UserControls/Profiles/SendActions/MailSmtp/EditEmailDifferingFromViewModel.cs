using System.Windows.Input;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.MailSmtp
{
    public class EditEmailDifferingFromInteraction : IInteraction
    {
        public bool Success { get; set; } = false;
        public EmailSmtpSettings EmailSmtpSettings { get; }

        public EditEmailDifferingFromInteraction(EmailSmtpSettings smtpSettingsCopy)
        {
            EmailSmtpSettings = smtpSettingsCopy;
        }
    }

    public class EditEmailDifferingFromViewModel : OverlayViewModelBase<EditEmailDifferingFromInteraction, MailTranslation>
    {
        private readonly EditionHelper _editionHelper;
        public override string Title => Translation.SpecifySender;

        public string OkBack => _editionHelper.IsFreeEdition ? Translation.Back : Translation.OK;

        public TokenViewModel<EditEmailDifferingFromInteraction> OnBehalfOfTokenViewModel { get; private set; }
        public TokenViewModel<EditEmailDifferingFromInteraction> DisplayNameTokenViewModel { get; private set; }
        public TokenViewModel<EditEmailDifferingFromInteraction> ReplyToTokenViewModel { get; private set; }

        public DelegateCommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public EditEmailDifferingFromViewModel(ITranslationUpdater translationUpdater, ITokenViewModelFactory tokenViewModelFactory, EditionHelper editionHelper) : base(translationUpdater)
        {
            _editionHelper = editionHelper;

            var builder = tokenViewModelFactory
                .Builder<EditEmailDifferingFromInteraction>()
                .WithInitialValue(Interaction)
                .WithTokenList(th => th.GetTokenListForEmailRecipients())
                .WithDefaultTokenReplacerPreview();

            OnBehalfOfTokenViewModel = builder
                .WithSelector(i => i.EmailSmtpSettings.OnBehalfOf)
                .Build();

            DisplayNameTokenViewModel = builder
                .WithSelector(i => i.EmailSmtpSettings.DisplayName)
                .Build();

            ReplyToTokenViewModel = builder
                .WithSelector(i => i.EmailSmtpSettings.ReplyTo)
                .Build();

            OkCommand = new DelegateCommand(OkBackExecute);
            CancelCommand = new DelegateCommand(o => FinishInteraction());
        }

        protected override void HandleInteractionObjectChanged()
        {
            OnBehalfOfTokenViewModel.CurrentValue = Interaction;
            RaisePropertyChanged(nameof(OnBehalfOfTokenViewModel));
            DisplayNameTokenViewModel.CurrentValue = Interaction;
            RaisePropertyChanged(nameof(DisplayNameTokenViewModel));
            ReplyToTokenViewModel.CurrentValue = Interaction;
            RaisePropertyChanged(nameof(ReplyToTokenViewModel));
        }

        private void OkBackExecute(object obj)
        {
            Interaction.Success = !_editionHelper.IsFreeEdition;
            FinishInteraction();
        }
    }
}
