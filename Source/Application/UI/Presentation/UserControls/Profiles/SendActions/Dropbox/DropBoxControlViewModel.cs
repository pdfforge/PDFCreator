using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Dropbox;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Controls;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities.Tokens;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.Dropbox
{
    public class DropBoxControlViewModel : ProfileUserControlViewModel<DropboxTranslation>
    {
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly IDropboxService _dropboxService;
        private Helper.SynchronizedCollection<DropboxAccount> _accountsCollection;
        private Conversion.Settings.Accounts _accounts;

        public DropBoxControlViewModel(IInteractionInvoker interactionInvoker, IDropboxService dropboxService, TokenHelper tokenHelper, ITranslationUpdater translationUpdater, ISelectedProfileProvider profile) : base(translationUpdater, profile)
        {
            _interactionInvoker = interactionInvoker;
            _dropboxService = dropboxService;
            AddDropboxAccountCommand = new DelegateCommand(AuthoriseDropboxUser);
            _accountsCollection = new Helper.SynchronizedCollection<DropboxAccount>(new List<DropboxAccount>());
            RemoveDropboxAccountCommand = new DelegateCommand(RemoveDropboxAccount, RemoveDropboxCanExecute);
            if (tokenHelper != null)
            {
                TokenReplacer = tokenHelper.TokenReplacerWithPlaceHolders;
                TokenViewModel = new TokenViewModel(x => CurrentProfile.DropboxSettings.SharedFolder = x, () => CurrentProfile?.DropboxSettings.SharedFolder, tokenHelper.GetTokenListForDirectory(), ReplaceTokens);
            }
        }

        private bool RemoveDropboxCanExecute(object o)
        {
            return DropboxAccounts.Any();
        }

        public TokenReplacer TokenReplacer { get; set; }

        public TokenViewModel TokenViewModel { get; set; }

        public DelegateCommand AddDropboxAccountCommand { get; set; }
        public DelegateCommand RemoveDropboxAccountCommand { get; set; }
        public ObservableCollection<DropboxAccount> DropboxAccounts => _accountsCollection.ObservableCollection;

        private void RemoveDropboxAccount(object obj)
        {
            var currentSelectedAccount = DropboxAccounts.FirstOrDefault(item => item.AccountId == CurrentProfile.DropboxSettings.AccountId);
            if (currentSelectedAccount != null)
            {
                DropboxAccounts.Remove(currentSelectedAccount);
                var account = Accounts.DropboxAccounts.FirstOrDefault();
                CurrentProfile.DropboxSettings.AccountId = account == null ? "" : account.AccountId;
                _dropboxService.RevokeToken(currentSelectedAccount.AccessToken);
                RaisePropertyChanged(nameof(Accounts));
            }

            var collectionView = CollectionViewSource.GetDefaultView(DropboxAccounts);
            collectionView.MoveCurrentToLast();
        }

        public Conversion.Settings.Accounts Accounts
        {
            get { return _accounts; }
            set
            {
                _accounts = value;
                SetAccounts(_accounts);
            }
        }

        protected void SetAccounts(Conversion.Settings.Accounts accounts)
        {
            _accountsCollection = new Helper.SynchronizedCollection<DropboxAccount>(Accounts.DropboxAccounts);
            _accountsCollection.ObservableCollection.CollectionChanged += (sender, args) => RemoveDropboxAccountCommand.RaiseCanExecuteChanged();
            RaisePropertyChanged(nameof(DropboxAccounts));
            RemoveDropboxAccountCommand.RaiseCanExecuteChanged();
        }

        protected override void OnCurrentProfileChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            base.OnCurrentProfileChanged(sender, propertyChangedEventArgs);

            TokenViewModel.RaiseTextChanged();
        }

        /// <summary>
        ///     Calling dropbox authorisation api
        /// </summary>
        /// <param name="obj"></param>
        private void AuthoriseDropboxUser(object obj)
        {
            var interaction = new DropboxInteraction();

            _interactionInvoker.Invoke(interaction);
            if (interaction.Success)
            {
                if (DropboxAccounts.Any(item => item.AccountId.Equals(interaction.AccountId)) == false)
                    DropboxAccounts.Add(new DropboxAccount
                    {
                        AccessToken = interaction.AccessToken,
                        AccountId = interaction.AccountId,
                        AccountInfo = interaction.AccountInfo
                    });

                CurrentProfile.DropboxSettings.AccountId = interaction.AccountId;
                var collectionView = CollectionViewSource.GetDefaultView(DropboxAccounts);
                collectionView.MoveCurrentToLast();
            }
        }

        private string ReplaceTokens(string s)
        {
            if (s != null)
            {
                return TokenReplacer.ReplaceTokens(s);
            }
            return string.Empty;
        }
    }

    public class DesignTimeDropBoxControlViewModel : DropBoxControlViewModel
    {
        public DesignTimeDropBoxControlViewModel() : base(null, null, null, new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider())
        {
        }
    }
}
