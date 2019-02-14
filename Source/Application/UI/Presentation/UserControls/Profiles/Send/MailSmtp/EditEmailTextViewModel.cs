using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities.Tokens;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.MailSmtp
{
    public class EditEmailTextViewModel : OverlayViewModelBase<EditEmailTextInteraction, SmtpTranslation>
    {
        private readonly string _signatureText;
        public TokenViewModel<EditEmailTextInteraction> SubjectTokenViewModel { get; set; }
        public TokenViewModel<EditEmailTextInteraction> ContentTokenViewModel { get; set; }

        private readonly TokenReplacer _tokenReplacer;

        public EditEmailTextViewModel(ITranslationUpdater translationUpdater, IMailSignatureHelper mailSignatureHelper, ITokenHelper tokenHelper, TokenViewModelFactory tokenViewModelFactory)
            : base(translationUpdater)
        {
            _tokenReplacer = tokenHelper.TokenReplacerWithPlaceHolders;

            var builder = tokenViewModelFactory
                .Builder<EditEmailTextInteraction>()
                .WithInitialValue(Interaction)
                .WithTokenList(tokenHelper.GetTokenListForEmail());

            SubjectTokenViewModel = builder
                .WithSelector(i => i.Subject)
                .WithDefaultTokenReplacerPreview()
                .Build();

            ContentTokenViewModel = builder
                .WithSelector(i => i.Content)
                .WithTokenCustomPreview(ReplaceTokensAddSignature)
                .Build();

            _signatureText = mailSignatureHelper.ComposeMailSignature();

            OkCommand = new DelegateCommand(ExecuteOk);
            CancelCommand = new DelegateCommand(o => FinishInteraction());
        }

        private string ReplaceTokensAddSignature(string s)
        {
            if (s == null)
                return string.Empty;

            var contentString = _tokenReplacer.ReplaceTokens(s);

            if (AddSignature)
                contentString += _signatureText;

            return contentString;
        }

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        private void ExecuteOk(object o)
        {
            Interaction.Success = true;

            FinishInteraction();
        }

        public bool AddSignature
        {
            get { return Interaction?.AddSignature == true; }
            set
            {
                Interaction.AddSignature = value;
                RaisePropertyChanged(nameof(AddSignature));
                ContentTokenViewModel.RaiseTextChanged();
            }
        }

        protected override void HandleInteractionObjectChanged()
        {
            RaisePropertyChanged(nameof(ContentTokenViewModel));
            RaisePropertyChanged(nameof(SubjectTokenViewModel));
            RaisePropertyChanged(nameof(AddSignature));
            RaisePropertyChanged(nameof(Interaction));
            SubjectTokenViewModel.CurrentValue = Interaction;
            ContentTokenViewModel.CurrentValue = Interaction;
            ContentTokenViewModel?.RaiseTextChanged();
        }

        public override string Title => Translation.EditMailText;
    }
}
