using System;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using Prism.Commands;
using pdfforge.PDFCreator.Core.Services.Macros;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews.Microsoft
{
    public class MicrosoftAccountViewModel : AccountViewModelBase<MicrosoftAccountInteraction, MicrosoftTranslation>, IMountable
    {
        private readonly ICurrentSettings<Conversion.Settings.Accounts> _accountProvider;
        private bool _hasMailReadWritePermission;
        private bool _hasMailSendPermission;
        private bool _hasFilesReadWritePermission;
        
        private MicrosoftAccount _account;
        public IMacroCommand RequestPermissionCommand { get; set; }

        public MicrosoftAccount Account
        {
            get => _account;
            set
            {
                if (Equals(value, _account)) return;
                _account = value;
                RaisePropertyChanged(nameof(Account));
                RaisePropertyChanged(nameof(AccountExists));
                RaisePropertyChanged(nameof(PermissionsExpired));
                RaisePropertyChanged(nameof(MicrosoftAccountPermissionsPayload));
            }
        }

        public bool AccountExists
        {
            get
            {
                if (Account == null)
                    return false;

                return !string.IsNullOrEmpty(Account.AccountInfo);
            }
        }

        public bool PermissionsExpired
        {
            get
            {
                if (!AccountExists)
                    return false;

                return Account.GetExpirationDateTime() < DateTime.Now;
            }
        }

        public bool HasMailReadWritePermission
        {
            get => _hasMailReadWritePermission;
            set
            {
                if (value == _hasMailReadWritePermission) return;
                _hasMailReadWritePermission = value;
                RaisePropertyChanged(nameof(MicrosoftAccountPermissionsPayload));
                RaisePropertyChanged(nameof(HasMailReadWritePermission));
            }
        }

        public bool HasMailSendPermission   
        {
            get => _hasMailSendPermission;
            set
            {
                if (value == _hasMailSendPermission) return;
                _hasMailSendPermission = value;
                RaisePropertyChanged(nameof(MicrosoftAccountPermissionsPayload));
                RaisePropertyChanged(nameof(HasMailSendPermission));
            }
        }

        public bool HasFilesReadWritePermission
        {
            get => _hasFilesReadWritePermission;
            set
            {
                if(value == _hasFilesReadWritePermission) return;
                _hasFilesReadWritePermission = value;
                RaisePropertyChanged(nameof(MicrosoftAccountPermissionsPayload));
                RaisePropertyChanged(nameof(HasFilesReadWritePermission));
            }
        }

        public MicrosoftAccountPermissionsPayload MicrosoftAccountPermissionsPayload
        {
            get
            {
                var permissions = new List<MicrosoftAccountPermission>();
                if(HasMailReadWritePermission)
                    permissions.Add(MicrosoftAccountPermission.MailReadWrite);
                if (HasMailSendPermission)
                    permissions.Add(MicrosoftAccountPermission.MailSend);
                if(HasFilesReadWritePermission)
                    permissions.Add(MicrosoftAccountPermission.FilesReadWrite);

                return new MicrosoftAccountPermissionsPayload(Account?? new MicrosoftAccount(), permissions);
            }
        }

        public MicrosoftAccountViewModel(ITranslationUpdater translationUpdater, ICommandLocator commandLocator, ICurrentSettings<Conversion.Settings.Accounts> accountProvider) : base(translationUpdater)
        {
            _accountProvider = accountProvider;
            RequestPermissionCommand =  commandLocator.CreateMacroCommand()
                .AddCommand<MicrosoftAccountRequestPermissionCommand>()
                .AddCommand(new DelegateCommand(SaveExecute))
                .Build();
        }

        protected override void SaveExecute()
        {
            Interaction.MicrosoftAccount = Account;
            Interaction.Success = true;
            FinishInteraction();
        }

        protected override bool SaveCanExecute()
        {
            if (_accountProvider.Settings.MicrosoftAccounts.FirstOrDefault() is { } account)
            {
                return !string.IsNullOrEmpty(account.MicrosoftJson);
            }
            return false;
        }

        protected override void ClearPassword()
        {
        }

        protected override void HandleInteractionObjectChanged()
        {
            base.HandleInteractionObjectChanged();
            Account = Interaction.MicrosoftAccount;
        }

        public void MountView()
        {
            _accountProvider.Settings.MicrosoftAccounts.CollectionChanged += MicrosoftAccountsOnCollectionChanged;
            UpdateData(_accountProvider.Settings.MicrosoftAccounts.FirstOrDefault());
            
        }

        private void UpdateData(MicrosoftAccount account = null)
        {
            if (account == null)
                return;

            HasMailReadWritePermission = account.PermissionScopes.Contains(MicrosoftAccountPermission.MailReadWrite.ToPermissionString());
            HasMailSendPermission = account.PermissionScopes.Contains(MicrosoftAccountPermission.MailSend.ToPermissionString());
            HasFilesReadWritePermission = account.PermissionScopes.Contains(MicrosoftAccountPermission.FilesReadWrite.ToPermissionString());
            RaisePropertyChanged(nameof(Account));
            RaisePropertyChanged(nameof(MicrosoftAccountPermissionsPayload));
            RaisePropertyChanged(nameof(SaveCanExecute));
            SaveCommand.RaiseCanExecuteChanged();
        }

        private void MicrosoftAccountsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.NewItems?[0] is MicrosoftAccount account)
                UpdateData(account);
        }

        public void UnmountView()
        {
        }
    }
}
