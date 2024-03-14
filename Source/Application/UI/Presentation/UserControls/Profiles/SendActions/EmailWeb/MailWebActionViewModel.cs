using System.Linq;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement.DefaultSettings;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SelectFiles;
using System.Text;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.EmailWeb
{
    public class MailWebActionViewModel : ActionViewModelBase<MailWebAction, OutlookWebTranslation>
    {
        private readonly IInteractionRequest _interactionRequest;
        private readonly ICurrentSettings<Conversion.Settings.Accounts> _accountProvider;

        public TokenViewModel<ConversionProfile> RecipientsTokenViewModel { get; private set; }
        public TokenViewModel<ConversionProfile> RecipientsCcTokenViewModel { get; private set; }
        public TokenViewModel<ConversionProfile> RecipientsBccTokenViewModel { get; private set; }
        public SelectFilesUserControlViewModel AdditionalAttachmentsViewModel { get; private set; }

        public DelegateCommand EditEmailTextCommand { get; set; }
        private EmailWebSettings EmailWebSettings => CurrentProfile?.EmailWebSettings;

        public MailWebActionViewModel(
            IActionLocator actionLocator,
            ErrorCodeInterpreter errorCodeInterpreter,
            ICurrentSettingsProvider currentSettingsProvider,
            IInteractionRequest interactionRequest,
            ITranslationUpdater translationUpdater,
            ITokenViewModelFactory tokenViewModelFactory,
            IDispatcher dispatcher,
            ISelectFilesUserControlViewModelFactory selectFilesUserControlViewModelFactory,
            IDefaultSettingsBuilder defaultSettingsBuilder,
            IActionOrderHelper actionOrderHelper,
            ICurrentSettings<Conversion.Settings.Accounts> accountProvider)
            : base(actionLocator, errorCodeInterpreter, translationUpdater, currentSettingsProvider, dispatcher, defaultSettingsBuilder, actionOrderHelper)
        {
            _interactionRequest = interactionRequest;
            _accountProvider = accountProvider;

            CreateTokenViewModels(tokenViewModelFactory);

            EditEmailTextCommand = new DelegateCommand(EditEmailTextExecute);

            AdditionalAttachmentsViewModel = selectFilesUserControlViewModelFactory.Builder()
                .WithTitleGetter(() => Translation.MailAttachmentTitle)
                .WithFileListGetter(profile => profile.EmailWebSettings.AdditionalAttachments)
                .WithPropertyChanged(StatusChanged)
                .Build();
        }

        private void CreateTokenViewModels(ITokenViewModelFactory tokenViewModelFactory)
        {
            var builder = tokenViewModelFactory
                .BuilderWithSelectedProfile()
                .WithDefaultTokenReplacerPreview(th => th.GetTokenListForEmailRecipients());

            RecipientsTokenViewModel = builder
                .WithSelector(p => p.EmailWebSettings.Recipients)
                .Build();

            RecipientsCcTokenViewModel = builder
                .WithSelector(p => p.EmailWebSettings.RecipientsCc)
                .Build();

            RecipientsBccTokenViewModel = builder
                .WithSelector(p => p.EmailWebSettings.RecipientsBcc)
                .Build();
        }

        protected override string SettingsPreviewString
        {
            get
            {
                var preview = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(CurrentProfile.EmailWebSettings.Recipients))
                    preview.Append(Translation.RecipientsToText).Append(" ").Append(CurrentProfile.EmailWebSettings.Recipients);
                else
                    preview.Append(Translation.BlankToField);

                if (!string.IsNullOrEmpty(CurrentProfile.EmailWebSettings.RecipientsCc))
                    preview.AppendLine().Append(Translation.RecipientsCcText).Append(" ").Append(CurrentProfile.EmailWebSettings.RecipientsCc);

                if (!string.IsNullOrEmpty(CurrentProfile.EmailWebSettings.RecipientsBcc))
                    preview.AppendLine().Append(Translation.RecipientsBccText).Append(" ").Append(CurrentProfile.EmailWebSettings.RecipientsBcc);

                return preview.ToString();
            }
        }

        public override void MountView()
        {
            RecipientsTokenViewModel.MountView();
            RecipientsCcTokenViewModel.MountView();
            RecipientsBccTokenViewModel.MountView();
            AdditionalAttachmentsViewModel.MountView();
            if (_accountProvider.Settings.MicrosoftAccounts.Count > 0)
            {
                EmailWebSettings.AccountId = _accountProvider.Settings.MicrosoftAccounts.First().AccountId;
            }
            base.MountView();
        }

        public override void UnmountView()
        {
            base.UnmountView();

            RecipientsTokenViewModel.UnmountView();
            RecipientsCcTokenViewModel.UnmountView();
            RecipientsBccTokenViewModel.UnmountView();
            AdditionalAttachmentsViewModel.UnmountView();
        }

        private void EditEmailTextExecute(object obj)
        {
            var interaction = new EditEmailTextInteraction(EmailWebSettings);

            _interactionRequest.Raise(interaction, EditEmailTextCallback);
        }

        private void EditEmailTextCallback(EditEmailTextInteraction interaction)
        {
            if (!interaction.Success)
                return;

            EmailWebSettings.AddSignature = interaction.AddSignature;
            EmailWebSettings.Content = interaction.Content;
            EmailWebSettings.Subject = interaction.Subject;
            EmailWebSettings.Format = interaction.Format;
            
        }
    }
}
