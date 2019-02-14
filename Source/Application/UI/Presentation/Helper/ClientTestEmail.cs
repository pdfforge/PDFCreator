using NLog;
using pdfforge.Mail;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using System;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    public interface IClientTestEmail
    {
        bool SendTestEmail(EmailClientSettings clientSettings);
    }

    public class ClientTestEmail : IClientTestEmail
    {
        private readonly IEmailClientFactory _emailClientFactory;
        private readonly IMailSignatureHelper _mailSignatureHelper;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPath _path;
        private readonly IDirectory _directory;
        private readonly IFile _file;
        private readonly ITokenHelper _tokenHelper;

        public ClientTestEmail(IEmailClientFactory emailClientFactory, IMailSignatureHelper mailSignatureHelper, IPath path, IDirectory directory, IFile file, ITokenHelper tokenHelper)
        {
            _emailClientFactory = emailClientFactory;
            _mailSignatureHelper = mailSignatureHelper;
            _path = path;
            _directory = directory;
            _file = file;
            _tokenHelper = tokenHelper;
        }

        public bool SendTestEmail(EmailClientSettings clientSettings)
        {
            var emailClient = _emailClientFactory.CreateEmailClient();

            if (emailClient == null)
            {
                return false;
            }

            var eMail = new Email();
            var tokenReplacer = _tokenHelper.TokenReplacerWithPlaceHolders;

            var recipientsTo = tokenReplacer.ReplaceTokens(clientSettings.Recipients);
            var recipientsCc = tokenReplacer.ReplaceTokens(clientSettings.RecipientsCc);
            var recipientsBcc = tokenReplacer.ReplaceTokens(clientSettings.RecipientsBcc);

            eMail.Subject = tokenReplacer.ReplaceTokens(clientSettings.Subject);
            eMail.Html = clientSettings.Html;
            eMail.Body = tokenReplacer.ReplaceTokens(clientSettings.Content);
            eMail.Recipients.AddTo(recipientsTo);
            eMail.Recipients.AddCc(recipientsCc);
            eMail.Recipients.AddBcc(recipientsBcc);

            if (clientSettings.AddSignature)
            {
                var signature = _mailSignatureHelper.ComposeMailSignature();
                if (clientSettings.Html)
                    signature = signature.Replace(Environment.NewLine, "<br/>");
                eMail.Body += signature;
            }

            try
            {
                var tempFolder = _path.GetTempPath();
                var tmpTestFolder = PathSafe.Combine(tempFolder, "PDFCreator-Test\\SendSmtpTestmail");
                _directory.CreateDirectory(tmpTestFolder);
                var tmpFile = PathSafe.Combine(tmpTestFolder, "PDFCreator Mail Client Test.pdf");
                _file.WriteAllText(tmpFile, "");
                eMail.Attachments.Add(new Attachment(tmpFile));

                emailClient.ShowEmailClient(eMail);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Exception while creating mail");
                return false;
            }
            return true;
        }
    }
}
