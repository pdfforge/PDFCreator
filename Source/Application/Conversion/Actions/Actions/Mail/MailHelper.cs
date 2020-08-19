using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement;
using System;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions
{
    public interface IMailHelper
    {
        MailInfo CreateMailInfo(Job job, IMailActionSettings mailSettings);
    }

    public class MailInfo
    {
        public string Subject;
        public string Body;
        public string Recipients;
        public string RecipientsCc;
        public string RecipientsBcc;
        public bool IsHtml;
        public IList<string> Attachments;
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

        public MailInfo CreateMailInfo(Job job, IMailActionSettings mailSettings)
        {
            _logger.Trace("Create MailInfo for " + mailSettings.GetType().Name.Replace("Settings", " Action."));

            var mailInfo = new MailInfo
            {
                Subject = job.TokenReplacer.ReplaceTokens(mailSettings.Subject),
                Body = BuildBody(job, mailSettings),
                Recipients = job.TokenReplacer.ReplaceTokens(mailSettings.Recipients.Replace(';', ',')),
                RecipientsCc = job.TokenReplacer.ReplaceTokens(mailSettings.RecipientsCc.Replace(';', ',')),
                RecipientsBcc = job.TokenReplacer.ReplaceTokens(mailSettings.RecipientsBcc.Replace(';', ',')),
                IsHtml = mailSettings.Html,

                Attachments = GetFileAttachmentList(job, mailSettings)
            };

            return mailInfo;
        }

        private string BuildBody(Job job, IMailActionSettings mailSettings)
        {
            var body = job.TokenReplacer.ReplaceTokens(mailSettings.Content);

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
