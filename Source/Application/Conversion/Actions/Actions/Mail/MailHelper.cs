using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions
{
    public interface IMailHelper
    {
        MailInfo CreateMailInfo(Job job, IMailActionSettings mailSettings);

        void ReplaceTokensInMailSettings(Job job, IMailActionSettings mailActionSettings);
    }

    public class MailInfo
    {
        public string Subject = "";
        public string Body = "";
        public string Recipients = "";
        public string RecipientsCc = "";
        public string RecipientsBcc = "";
        public bool IsHtml;
        public IList<string> Attachments = new List<string>();
    }

    public class MailHelper : IMailHelper
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IMailSignatureHelper _mailSignatureHelper;
        private readonly IActionOrderHelper _actionOrderHelper;

        public MailHelper(IMailSignatureHelper mailSignatureHelper, IActionOrderHelper actionOrderHelper)
        {
            _mailSignatureHelper = mailSignatureHelper;
            _actionOrderHelper = actionOrderHelper;
        }

        public void ReplaceTokensInMailSettings(Job job, IMailActionSettings mailActionSettings)
        {
            mailActionSettings.Subject = job.TokenReplacer.ReplaceTokens(mailActionSettings.Subject);
            mailActionSettings.Content = job.TokenReplacer.ReplaceTokens(mailActionSettings.Content);

            mailActionSettings.Recipients = job.TokenReplacer.ReplaceTokens(mailActionSettings.Recipients)
                .Replace(';', ',');
            mailActionSettings.RecipientsCc = job.TokenReplacer.ReplaceTokens(mailActionSettings.RecipientsCc)
                .Replace(';', ',');
            mailActionSettings.RecipientsBcc = job.TokenReplacer.ReplaceTokens(mailActionSettings.RecipientsBcc)
                .Replace(';', ',');

            mailActionSettings.AdditionalAttachments = mailActionSettings.AdditionalAttachments
                .Select(aA => job.TokenReplacer.ReplaceTokens(aA))
                .ToList();
        }

        public MailInfo CreateMailInfo(Job job, IMailActionSettings mailSettings)
        {
            _logger.Trace("Create MailInfo for " + mailSettings.GetType().Name.Replace("Settings", " Action."));

            var mailInfo = new MailInfo
            {
                Subject = mailSettings.Subject,
                Body = BuildBody(mailSettings),
                Recipients = mailSettings.Recipients.Replace(';', ','),
                RecipientsCc = mailSettings.RecipientsCc.Replace(';', ','),
                RecipientsBcc = mailSettings.RecipientsBcc.Replace(';', ','),
                IsHtml = mailSettings.Html,

                Attachments = GetFileAttachmentList(job, mailSettings)
            };

            return mailInfo;
        }

        private string BuildBody(IMailActionSettings mailSettings)
        {
            var body = mailSettings.Content;

            if (mailSettings.AddSignature)
            {
                var signature = _mailSignatureHelper.ComposeMailSignature();
                if (mailSettings.Html)
                    signature = signature.Replace(Environment.NewLine, "<br>");

                body += signature;
            }

            return body;
        }

        private IList<string> GetFileAttachmentList(Job job, IMailActionSettings settings)
        {
            var attachmentList = new List<string>();

            if (!DropboxShareLinksAreUsed(job, settings))
            {
                attachmentList.AddRange(job.OutputFiles);
                attachmentList.AddRange(settings.AdditionalAttachments);
            }

            return attachmentList;
        }

        private bool DropboxShareLinksAreUsed(Job job, IMailActionSettings mailSettings)
        {
            if (job.Profile.DropboxSettings.Enabled == false || job.Profile.DropboxSettings.CreateShareLink == false)
                return false;

            if (mailSettings.Content.IndexOf("<Dropbox", StringComparison.InvariantCultureIgnoreCase) < 0)
                return false;

            var mailTypeName = mailSettings.GetType().Name;
            if (!_actionOrderHelper.IsFirstActionBeforeSecond(job.Profile, nameof(DropboxSettings), mailTypeName))
            {
                _logger.Warn("To use the share links instead of mail attachments, the Dropbox action must be executed before the mail action.");
                return false;
            }

            return true;
        }
    }
}
