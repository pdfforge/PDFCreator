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
using System.Collections.ObjectModel;
using System.Windows.Input;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using System.Windows.Data;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.EmailWeb
{
    public class MailWebActionViewModel : ActionViewModelBase<MailWebAction, OutlookWebTranslation>
    {
        private readonly IInteractionRequest _interactionRequest;
        private readonly ICurrentSettings<Conversion.Settings.Accounts> _accountProvider;
        private readonly IGpoSettings _gpoSettings;

        public TokenViewModel<ConversionProfile> RecipientsTokenViewModel { get; private set; }
        public TokenViewModel<ConversionProfile> RecipientsCcTokenViewModel { get; private set; }
        public TokenViewModel<ConversionProfile> RecipientsBccTokenViewModel { get; private set; }
        public SelectFilesUserControlViewModel AdditionalAttachmentsViewModel { get; private set; }

        public bool EditAccountsIsDisabled => _gpoSettings != null && _gpoSettings.DisableAccountsTab;
        public ObservableCollection<MicrosoftAccount> MicrosoftAccounts { get; set; }

        public ICommand AddMicrosoftAccountCommand { get; set; }
        public ICommand EditMicrosoftAccountCommand { get; set; }

        public DelegateCommand EditEmailTextCommand { get; set; }
        private EmailWebSettings EmailWebSettings => CurrentProfile?.EmailWebSettings;


        public bool SendingOptionNothing
        {
            get => CurrentProfile?.EmailWebSettings != null && !CurrentProfile.EmailWebSettings.ShowDraft && !CurrentProfile.EmailWebSettings.SendWebMailAutomatically;
            set
            {
                if (!value) return;
                CurrentProfile.EmailWebSettings.ShowDraft = false;
                CurrentProfile.EmailWebSettings.SendWebMailAutomatically = false;
            }
        }

        public bool IsServer { get; private set; } = false;

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
            ICurrentSettings<Conversion.Settings.Accounts> accountProvider,
            IGpoSettings gpoSettings,
            ICommandLocator commandLocator,
            EditionHelper editionHelper)
            : base(actionLocator, errorCodeInterpreter, translationUpdater, currentSettingsProvider, dispatcher, defaultSettingsBuilder, actionOrderHelper)
        {
            _interactionRequest = interactionRequest;
            _accountProvider = accountProvider;
            _gpoSettings = gpoSettings;
            IsServer = editionHelper.IsServer;

            CreateTokenViewModels(tokenViewModelFactory);

            MicrosoftAccounts = _accountProvider.Settings.MicrosoftAccounts;
            AddMicrosoftAccountCommand = commandLocator.CreateMacroCommand()
                .AddCommand<MicrosoftAccountEditCommand>()
                .AddCommand(new DelegateCommand(_ => RaisePropertyChanged(nameof(MicrosoftAccounts))))
                .AddCommand(new DelegateCommand(_ => SelectNewAccountInView()))
                .AddCommand(new DelegateCommand(_ => StatusChanged()))
                .Build();

            EditMicrosoftAccountCommand = commandLocator.CreateMacroCommand()
                .AddCommand<MicrosoftAccountEditCommand>()
                .AddCommand(new DelegateCommand(_ => StatusChanged()))
                .Build();

            EditEmailTextCommand = new DelegateCommand(EditEmailTextExecute);

            AdditionalAttachmentsViewModel = selectFilesUserControlViewModelFactory.Builder()
                .WithTitleGetter(() => Translation.MailAttachmentTitle)
                .WithAddFileButtonTextGetter(() => Translation.AddAttachmentFile)
                .WithFileListGetter(profile => profile.EmailWebSettings.AdditionalAttachments)
                .WithPropertyChanged(StatusChanged)
                .Build();
        }

        private void SelectNewAccountInView()
        {
            var accountToBeSelected = MicrosoftAccounts.LastOrDefault();
            var collectionView = CollectionViewSource.GetDefaultView(MicrosoftAccounts);
            collectionView.MoveCurrentTo(accountToBeSelected);
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

            RaisePropertyChanged(nameof(SendingOptionNothing));
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
