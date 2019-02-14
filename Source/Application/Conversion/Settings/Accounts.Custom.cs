using System.Linq;

namespace pdfforge.PDFCreator.Conversion.Settings
{
    partial class Accounts
    {
        public DropboxAccount GetDropboxAccount(ConversionProfile profile)
        {
            var dropboxAccountId = profile.DropboxSettings.AccountId;

            if (string.IsNullOrWhiteSpace(dropboxAccountId))
                return null;

            return DropboxAccounts.FirstOrDefault(x => x.AccountId == dropboxAccountId);
        }

        public HttpAccount GetHttpAccount(ConversionProfile profile)
        {
            var httpAccountId = profile.HttpSettings.AccountId;

            if (string.IsNullOrWhiteSpace(httpAccountId))
                return null;

            return HttpAccounts.FirstOrDefault(x => x.AccountId == httpAccountId);
        }

        public FtpAccount GetFtpAccount(ConversionProfile profile)
        {
            var ftpAccountId = profile.Ftp.AccountId;

            if (string.IsNullOrWhiteSpace(ftpAccountId))
                return null;

            return FtpAccounts.FirstOrDefault(x => x.AccountId == ftpAccountId);
        }

        public SmtpAccount GetSmtpAccount(ConversionProfile profile)
        {
            var smtpAccountId = profile.EmailSmtpSettings.AccountId;

            if (string.IsNullOrWhiteSpace(smtpAccountId))
                return null;

            return SmtpAccounts.FirstOrDefault(x => x.AccountId == smtpAccountId);
        }

        public TimeServerAccount GetTimeServerAccount(ConversionProfile profile)
        {
            var timeServerAccountId = profile.PdfSettings.Signature.TimeServerAccountId;

            if (string.IsNullOrWhiteSpace(timeServerAccountId))
                return null;

            return TimeServerAccounts.FirstOrDefault(x => x.AccountId == timeServerAccountId);
        }


    }
}
