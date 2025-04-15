using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities;
using Prism.Commands;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews.Microsoft;

public class MicrosoftAccountViewModel : AccountViewModelBase<MicrosoftAccountInteraction, MicrosoftTranslation>, IMountable
{
    public enum MicrosoftActions
    {
        MailDraft,
        MailSend,
        OneDrive,
        Sharepoint
    }

    private readonly ICurrentSettings<Conversion.Settings.Accounts> _accountProvider;
    private readonly EditionHelper _editionHelper;
    private MicrosoftAccount _account;
    private bool _initialHasOneDrive;
    private bool _initialHasSharepoint;
    private bool _initialHasOWA;
    private bool _initialHasOWASend;

    public MicrosoftAccountViewModel(ITranslationUpdater translationUpdater, ICommandLocator commandLocator, 
        ICurrentSettings<Conversion.Settings.Accounts> accountProvider, EditionHelper editionHelper) : base(translationUpdater)
    {
        _accountProvider = accountProvider;
        _editionHelper = editionHelper;
        RequestPermissionCommand = commandLocator.CreateMacroCommand()
            .AddCommand<MicrosoftAccountRequestPermissionCommand>()
            .AddCommand(new DelegateCommand(SaveExecute))
            .Build();
    }

    public bool IsSharePointSupported => !_editionHelper.IsFreeEdition; 

    public IMacroCommand RequestPermissionCommand { get; set; }

    public MicrosoftAccount Account
    {
        get => _account;
        set
        {
            if (Equals(value, _account)) return;
            _account = value;
            SetInitialStates();
            RaisePropertyChanged(nameof(Account));
            RaisePropertyChanged(nameof(AccountExists));
            RaisePropertyChanged(nameof(PermissionsExpired));
            RaisePropertyChanged(nameof(MicrosoftAccountPermissionsPayload));
        }
    }

    public MicrosoftAccountPermissionsPayload MicrosoftAccountPermissionsPayload => new(Account, GetPermissions());

    public bool AccountExists
    {
        get
        {
            if (Account == null)
                return false;

            return !string.IsNullOrEmpty(Account.AccountInfo);
        }
    }

    public List<string> RequiredPermissions { get; set; } = new();

    public bool HasOneDrive
    {
        get
        {
            if (Account?.Actions == null)
                return false;

            return HasAction(MicrosoftActions.OneDrive);
        }

        set
        {
            var action = MicrosoftActions.OneDrive;
            if (HasAction(action) == value)
                return;

            UpdateAction(action, value);
            UpdateData(Account);
        }
    }

    public bool HasSharepoint
    {
        get
        {
            if (Account?.Actions == null)
                return false;

            return HasAction(MicrosoftActions.Sharepoint);
        }

        set
        {
            var action = MicrosoftActions.Sharepoint;
            if (HasAction(action) == value)
                return;

            UpdateAction(action, value);
            UpdateData(Account);
        }
    }

    public bool HasOWA
    {
        get
        {
            if (Account?.Actions == null)
                return false;

            return HasAction(MicrosoftActions.MailDraft);
        }

        set
        {
            var action = MicrosoftActions.MailDraft;
            if (HasAction(action) == value)
                return;

            UpdateAction(action, value);

            if (value == false)
                RemoveAction(MicrosoftActions.MailSend);

            UpdateData(Account);
        }
    }

    public bool HasOWASend
    {
        get
        {
            if (Account?.Actions == null)
                return false;

            return HasAction(MicrosoftActions.MailSend);
        }

        set
        {
            var action = MicrosoftActions.MailSend;
            if (HasAction(action) == value)
                return;

            UpdateAction(action, value);
            UpdateData(Account);
        }
    }

    public bool PermissionsExpired
    {
        get
        {
            if (Account == null)
                return false;
            return Account.HasExpiredPermissions(DateTime.Now);
        }
    }

    public bool HasModifiedCheckboxState =>
        _initialHasOneDrive != HasOneDrive ||
        _initialHasSharepoint != HasSharepoint ||
        _initialHasOWA != HasOWA ||
        _initialHasOWASend != HasOWASend;

    public bool HasPermissionSelected => HasOWA || HasOWASend || HasOneDrive || HasSharepoint || HasModifiedCheckboxState;

    public string RequestOrSaveButtonText =>
        HasModifiedCheckboxState && GetPermissions().IsOnlyOfflinePermission() ? // No new permission checkbox is selected.
            Translation.Save : Translation.RequestPermissions;

    public void MountView()
    {
        _accountProvider.Settings.MicrosoftAccounts.CollectionChanged += MicrosoftAccountsOnCollectionChanged;
        UpdateData(Interaction.MicrosoftAccount);
    }

    public void UnmountView()
    {
    }


    private bool HasAction(MicrosoftActions action)
    {
        return Account.Actions.Contains(action.ToString());
    }

    private void AddAction(MicrosoftActions action)
    {
        if (!Account.Actions.Contains(action.ToString()))
        {
            if (string.IsNullOrEmpty(Account.Actions))
            {
                Account.Actions = action.ToString();
                return;
            }

            var stringList = Account.Actions.Split(',').ToList();
            stringList.Add(action.ToString());
            Account.Actions = string.Join(",", stringList);
        }
    }

    private void RemoveAction(MicrosoftActions action)
    {
        if (Account.Actions.Contains(action.ToString()))
        {
            var stringList = Account.Actions.Split(',').ToList();
            stringList.Remove(stringList.FirstOrDefault(s => s.Equals(action.ToString())));
            Account.Actions = string.Join(",", stringList);
        }
    }

    private void UpdateAction(MicrosoftActions action, bool newValue)
    {
        if (!HasAction(action) && newValue)
            AddAction(action);
        else
            RemoveAction(action);
    }

    protected override void SaveExecute()
    {
        Interaction.MicrosoftAccount = Account;
        Interaction.Success = true;
        FinishInteraction();
    }

    protected override bool SaveCanExecute()
    {
        if (_accountProvider.Settings.MicrosoftAccounts.FirstOrDefault() is { } account) return !string.IsNullOrEmpty(account.MicrosoftJson);
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

    private List<string> GetPermissionsString()
    {
        return GetPermissions().Select(permission => permission.ToPermissionString()).ToList();
    }

    private List<MicrosoftAccountPermission> GetPermissions()
    {
        var permissions = new List<MicrosoftAccountPermission> { MicrosoftAccountPermission.OfflineAccess };

        if (HasOWA)
            permissions.Add(MicrosoftAccountPermission.MailReadWrite);

        if (HasOWASend)
            permissions.Add(MicrosoftAccountPermission.MailSend);

        if (HasSharepoint)
        {
            permissions.Add(MicrosoftAccountPermission.SitesReadAll);
            permissions.Add(MicrosoftAccountPermission.FilesReadWriteAll);
        }

        if (HasOneDrive)
            if (!permissions.Contains(MicrosoftAccountPermission.FilesReadWrite))
                permissions.Add(MicrosoftAccountPermission.FilesReadWrite);

        return permissions;
    }

    private void UpdateData(MicrosoftAccount account = null)
    {
        if (account == null)
            return;

        RequiredPermissions = GetPermissionsString();
        RaisePropertyChanged(nameof(RequiredPermissions));
        RaisePropertyChanged(nameof(Account));
        RaisePropertyChanged(nameof(HasOWA));
        RaisePropertyChanged(nameof(HasOWASend));
        RaisePropertyChanged(nameof(HasOneDrive));
        RaisePropertyChanged(nameof(HasSharepoint));
        RaisePropertyChanged(nameof(MicrosoftAccountPermissionsPayload));
        RaisePropertyChanged(nameof(HasPermissionSelected));
        RaisePropertyChanged(nameof(SaveCanExecute));
        RaisePropertyChanged(nameof(RequestOrSaveButtonText));
        RaisePropertyChanged(nameof(HasModifiedCheckboxState));
        SaveCommand.RaiseCanExecuteChanged();
    }

    private void MicrosoftAccountsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems?[0] is MicrosoftAccount account)
            UpdateData(account);
    }
    private void SetInitialStates()
    {
        _initialHasOneDrive = HasOneDrive;
        _initialHasSharepoint = HasSharepoint;
        _initialHasOWA = HasOWA;
        _initialHasOWASend = HasOWASend;
    }
}
