using NLog;
using pdfforge.Mail;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Interface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using System;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions
{
    public class EMailClientAction : IEMailClientAction
    {
        private readonly IEmailClientFactory _emailClientFactory;
        private readonly IMailSignatureHelper _mailSignatureHelper;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public EMailClientAction(IEmailClientFactory emailClientFactory, IMailSignatureHelper mailSignatureHelper)
        {
            _emailClientFactory = emailClientFactory;
            _mailSignatureHelper = mailSignatureHelper;
        }

        public ActionResult ProcessJob(Job job)
        {
            _logger.Info("Launched client e-mail action");

            var subject = job.TokenReplacer.ReplaceTokens(job.Profile.EmailClientSettings.Subject);
            var body = job.TokenReplacer.ReplaceTokens(job.Profile.EmailClientSettings.Content);
            var isHtml = job.Profile.EmailClientSettings.Html;
            var hasSignature = job.Profile.EmailClientSettings.AddSignature;
            var signature = string.Empty;

            if (hasSignature)
                signature = _mailSignatureHelper.ComposeMailSignature();

            var recipientsTo = job.TokenReplacer.ReplaceTokens(job.Profile.EmailClientSettings.Recipients).Replace(';', ',');
            var recipientsCc = job.TokenReplacer.ReplaceTokens(job.Profile.EmailClientSettings.RecipientsCc).Replace(';', ',');
            var recipientsBcc = job.TokenReplacer.ReplaceTokens(job.Profile.EmailClientSettings.RecipientsBcc).Replace(';', ',');

            var hasAttachments = !SkipFileAttachments(job);
            IList<string> attachments = hasAttachments ? job.OutputFiles : null;

            return Process(subject, body, isHtml, hasSignature, signature, recipientsTo, recipientsCc, recipientsBcc, hasAttachments, attachments);
        }

        public ActionResult Process(string subject, string body, bool isHtml, bool hasSignature, string signature, string recipientsTo, string recipientsCc, string recipientsBcc, bool hasAttachments, IEnumerable<string> attachedFiles)
        {
            try
            {
                _logger.Info("Launched client e-mail action");

                var message = new Email();

                message.Subject = subject;
                message.Body = body;
                message.Html = isHtml;

                if (hasSignature)
                {
                    message.Body += isHtml ? signature.Replace(Environment.NewLine, "<br>") : signature;
                }

                message.Recipients.AddTo(recipientsTo);
                message.Recipients.AddCc(recipientsCc);
                message.Recipients.AddBcc(recipientsBcc);

                if (hasAttachments)
                {
                    foreach (var file in attachedFiles)
                    {
                        message.Attachments.Add(new Attachment(file));
                    }
                }

                _logger.Info("Starting e-mail client");

                var mailClient = _emailClientFactory.CreateEmailClient();

                if (mailClient == null)
                {
                    _logger.Error("No compatible e-mail client installed");
                    return new ActionResult(ErrorCode.MailClient_NoCompatibleEmailClientInstalled);
                }

                var success = mailClient.ShowEmailClient(message);

                if (!success)
                {
                    _logger.Warn("Could not start e-mail client");
                    return new ActionResult(ErrorCode.MailClient_GenericError);
                }

                _logger.Info("Done starting e-mail client");
                return new ActionResult();
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in e-mail client Action \r\n" + ex.Message);
                return new ActionResult(ErrorCode.MailClient_GenericError);
            }
        }

        private bool SkipFileAttachments(Job job)
        {
            if (job.Profile.DropboxSettings.Enabled == false || job.Profile.DropboxSettings.CreateShareLink == false)
                return false;

            return job.Profile.EmailClientSettings.Content.IndexOf("<Dropbox", StringComparison.InvariantCultureIgnoreCase) >= 0;
        }

        public bool IsEnabled(ConversionProfile profile)
        {
            return profile.EmailClientSettings.Enabled;
        }

        public bool CheckEmailClientInstalled()
        {
            return _emailClientFactory.CreateEmailClient() != null;
        }
    }
}
