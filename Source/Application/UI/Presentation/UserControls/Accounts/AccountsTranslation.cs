using pdfforge.PDFCreator.Conversion.Settings.Enums;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts
{
    public class AccountsTranslation : ITranslatable
    {
        private IPluralBuilder PluralBuilder { get; set; } = new DefaultPluralBuilder();

        public string Accounts { get; protected set; } = "Accounts";
        public string AccountLabel { get; protected set; } = "Account:";
        public string ManageAccountsDescription { get; protected set; } = "Manage your credentials for corresponding actions. Added accounts and changes will be available for all profiles.";
        public string ManageAccounts { get; protected set; } = "Manage accounts";
        public string AddFtpAccount { get; protected set; } = "Add FTP account";
        public string FtpAccount { get; protected set; } = "FTP account";
        public string TestFtpAccount { get; protected set; } = "Test FTP account";
        public string SuccessfulConnectionTest { get; protected set; } = "A connection has been established";
        public string UnsuccessfulConnectionTest { get; protected set; } = "Couldn't establish a connection with the provided credentials";
        public string AddSmtpAccount { get; protected set; } = "Add SMTP account";
        public string SmtpAccount { get; protected set; } = "SMTP Account";
        public string MicrosoftAccount { get; protected set; } = "Microsoft Account";
        public string HttpAccount { get; protected set; } = "HTTP account";
        public string AddHttpAccount { get; protected set; } = "Add HTTP account";
        public string AddDropboxAccount { get; protected set; } = "Add Dropbox account";
        public string AddMicrosoftAccount { get; protected set; } = "Add Microsoft account";
        public string SelectMicrosoftAccount { get; set; } = "Please select a Microsoft account";
        public string DropboxAccount { get; protected set; } = "Dropbox Account";
        public string AddTimeServerAccount { get; protected set; } = "Add time server account";
        public string TimeServerAccount { get; protected set; } = "Time server account";
        public string DontSavePassword { get; protected set; } = "Don't save password and request it during conversion (this is not possible for automatic saving)";
        public string DirectoryLabel { get; protected set; } = "Directory:";
        public string EnsureUniqueFilenames { get; protected set; } = "Don't overwrite files (adds an increasing number in case of conflict)";
        public string Cancel { get; protected set; } = "Cancel";
        public string PasswordLabel { get; protected set; } = "Password:";
        public string PasswordRequiredForConnectionTest { get; protected set; } = "In order to test the connection the password needs to be set temporarily";
        public string PassphraseRequired { get; protected set; } = "Key file requires passphrase";
        public string Save { get; protected set; } = "Save";
        public string Ok { get; protected set; } = "Ok";
        public string RequestPermissions { get; protected set; } = "Request permissions";
        public string ServerLabel { get; protected set; } = "Server:";
        public string UserNameLabel { get; protected set; } = "User name:";
        public string AddAccount { get; protected set; } = "Add account";
        public string SelectOrAddAccount { get; protected set; } = "Select an account or create a new one ->";

        public string SureYouWantToDeleteAccount { get; protected set; } = "Are you sure you want to delete this account?";

        protected string[] AccountIsUsed { get; set; } = { "The account is used in the following profile:", "The account is used in the following profiles:" };

        public string TimeoutLabel { get; protected set; } = "Timeout in seconds:";
        public string SendModeLabel { get; protected set; } = "Sending mode:";
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

        public string RestrictedHint { get; set; } = "This feature is not supported by the selected output format";
        public string EnabledHint { get; set; } = "This feature is already enabled for the selected profile";
        public string MailReadWrite{ get; set; } = "Read and write emails";
        public string FilesReadWrite { get; set; } = "Read and write files";
        public string MailSend { get; set; } = "Send emails";
        public string MailPermissionsLabel { get; set; } = "OWA Permissions";
        public string SiteAndReadLabel { get; set; } = "Read site infos on SharePoint";
        public string SharepointPermissionsLabel { get; set; } = "SharePoint Permissions";
        public string OneDrivePermissionsLabel { get; set; } = "OneDrive Permissions";
        public string OneDriveTitle { get; set; } = "OneDrive";
        public string SharepointTitle { get; set; } = "SharePoint";
        public string OWATitle { get; set; } = "Outlook Web Access";
        public string OWASendTitle { get; set; } = "Send directly";
    }
}
