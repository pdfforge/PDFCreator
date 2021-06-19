using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using System;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews
{
    public class FtpActionTranslation : AccountsTranslation, IActionTranslation
    {
        private IPluralBuilder PluralBuilder { get; set; } = new DefaultPluralBuilder();

        public string SelectFtpAccount { get; private set; } = "Please select a FTP account:";
        public string UploadWithFtp { get; private set; } = "Upload with FTP";
        public string RemoveFtpAccount { get; private set; } = "Remove FTP account";
        public string EditFtpAccount { get; private set; } = "Edit FTP Account";
        public string FtpUploadTitle { get; private set; } = "FTP Upload";
        public string FtpAccountColon { get; private set; } = "FTP Account:";
        public string FtpServerPassword { get; private set; } = "FTP Server Password:";
        public string Title { get; set; } = "FTP";
        public string InfoText { get; set; } = "Uploads the document with FTP.";

        private string[] FtpGetsDisabled { get; set; } = { "The FTP action will be disabled for this profile.", "The FTP action will be disabled for this profiles." };
        public string ErrorCustomViewNotFoundTitle { get; private set; } = "Viewer not found";
        public string ErrorCustomViewNotFoundDesc { get; private set; } = "Viewer was not found, please check your settings.";

        private string FtpConnection { get; set; } = "{0} via FTP";
        private string SftpConnection { get; set; } = "{0} via SFTP";

        public string GetFtpGetsDisabledMessage(int numberOfProfiles)
        {
            return PluralBuilder.GetFormattedPlural(numberOfProfiles, FtpGetsDisabled);
        }

        public string FormatFtpConnectionName(string serverName, FtpConnectionType ftpConnectionType)
        {
            switch (ftpConnectionType)
            {
                case FtpConnectionType.Ftp:
                    return string.Format(FtpConnection, serverName);

                case FtpConnectionType.Sftp:
                    return string.Format(SftpConnection, serverName);

                default: throw new Exception($"The FTP connection type {ftpConnectionType} is unknown here");
            }
        }
    }
}
