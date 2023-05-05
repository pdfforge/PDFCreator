using pdfforge.PDFCreator.Conversion.Settings.Enums;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts
{
    public class AccountsTranslation : ITranslatable
    {
        private IPluralBuilder PluralBuilder { get; set; } = new DefaultPluralBuilder();

        public string Accounts { get; protected set; } = "Accounts";
        public string ManageAccountsDescription { get; protected set; } = "Manage your credentials for corresponding actions. Added accounts and changes will be available for all profiles.";
        public string ManageAccounts { get; protected set; } = "Manage Accounts";
        public string AddFtpAccount { get; protected set; } = "Add FTP account";
        public string FtpAccount { get; protected set; } = "FTP Account";
        public string TestFtpAccount { get; protected set; } = "Test FTP Account";
        public string SuccessfulConnectionTest { get; protected set; } = "A connection has been established";
        public string UnsuccessfulConnectionTest { get; protected set; } = "Couldn't establish a connection with the provided credentials";
        public string AddSmtpAccount { get; protected set; } = "Add SMTP account";
        public string SmtpAccount { get; protected set; } = "SMTP Account";
        public string HttpAccount { get; protected set; } = "HTTP Account";
        public string AddHttpAccount { get; protected set; } = "Add HTTP account";
        public string AddDropboxAccount { get; protected set; } = "Add Dropbox account";
        public string DropboxAccount { get; protected set; } = "Dropbox Account";
        public string AddTimeServerAccount { get; protected set; } = "Add Time Server account";
        public string TimeServerAccount { get; protected set; } = "Time Server Account";
        public string DontSavePassword { get; protected set; } = "Don't save password and request it during conversion (this is not possible for automatic saving)";
        public string DirectoryLabel { get; protected set; } = "Directory:";
        public string EnsureUniqueFilenames { get; protected set; } = "Don't overwrite files (adds an increasing number in case of conflict)";
        public string Cancel { get; protected set; } = "Cancel";
        public string PasswordLabel { get; protected set; } = "Password:";
        public string PasswordRequiredForConnectionTest { get; protected set; } = "In order to test the connection the password needs to be set temporarily";
        public string PassphraseRequired { get; protected set; } = "Key file requires passphrase";
        public string Save { get; protected set; } = "Save";
        public string Ok { get; protected set; } = "Ok";
        public string ServerLabel { get; protected set; } = "Server:";
        public string UserNameLabel { get; protected set; } = "User Name:";
        public string AddAccount { get; protected set; } = "Add account";
        public string SelectOrAddAccount { get; protected set; } = "Select account or create a new one ->";

        public string SureYouWantToDeleteAccount { get; protected set; } = "Are you sure you want to delete this account?";

        protected string[] AccountIsUsed { get; set; } = { "The account is used in the following profile:", "The account is used in the following profiles:" };

        public string TimeoutLabel { get; protected set; } = "Timeout in seconds:";
        public string SendModeLabel { get; protected set; } = "Sending Mode:";
        public string FtpProtocol { get; protected set; } = "Protocol:";
        public string PrivateKeyFile { get; protected set; } = "Private key file:";
        public string Authentication { get; protected set; } = "Authentication";
        public string UserName { get; protected set; } = "User name";
        public string KeyFile { get; protected set; } = "Key file";
        public string SelectKeyFile { get; protected set; } = "Select key file";
        public string KeyFiles { get; protected set; } = "key files";
        public string AllFiles { get; protected set; } = "All files";

        public EnumTranslation<FtpConnectionType>[] FtpConnectionTypes { get; protected set; } = EnumTranslation<FtpConnectionType>.CreateDefaultEnumTranslation();

        public string GetAccountIsUsedInFollowingMessage(int numberOfProfiles)
        {
            return PluralBuilder.GetFormattedPlural(numberOfProfiles, AccountIsUsed);
        }

        public string RestrictedHint { get; set; } = "This feature is not supported by the selected output format.";
    }
}
