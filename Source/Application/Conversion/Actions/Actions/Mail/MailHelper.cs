using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Utilities.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions.Mail
{
    public interface IMailHelper
    {
        MailInfo CreateMailInfo(Job job, IMailActionSettings mailSettings);

        void ReplaceTokensInMailSettings(Job job, IMailActionSettings mailActionSettings);

        MailInfo CreateMailInfo(IList<string> files, IMailActionSettings mailActionSettings);
    }

    public class MailInfo
    {
        public string Subject = "";
        public string Body = "";
        public string Recipients = "";
        public string RecipientsCc = "";
        public string RecipientsBcc = "";
        public EmailFormatSetting Format;
        public IList<string> Attachments = new List<string>();
    }

    public class MailHelper : IMailHelper
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IMailSignatureHelper _mailSignatureHelper;

        public MailHelper(IMailSignatureHelper mailSignatureHelper)
        {
            _mailSignatureHelper = mailSignatureHelper;
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

        public MailInfo CreateMailInfo(IList<string> files, IMailActionSettings mailSettings)
        {
            _logger.Trace("Create MailInfo for " + mailSettings.GetType().Name.Replace("Settings", " Action."));

            var mailInfo = new MailInfo
            {
                Subject = mailSettings.Subject,
                Body = BuildBody(mailSettings),
                Recipients = mailSettings.Recipients.Replace(';', ','),
                RecipientsCc = mailSettings.RecipientsCc.Replace(';', ','),
                RecipientsBcc = mailSettings.RecipientsBcc.Replace(';', ','),
                Format = mailSettings.Format,
                Attachments = files
            };

            return mailInfo;
        }

        public MailInfo CreateMailInfo(Job job, IMailActionSettings mailSettings)
        {
            return CreateMailInfo(GetFileAttachmentList(job, mailSettings), mailSettings);
        }

        private string BuildBody(IMailActionSettings mailSettings)
        {
            var body = mailSettings.Content;

            if (mailSettings.AddSignature)
            {
                var signature = _mailSignatureHelper.ComposeMailSignature();
                if (mailSettings.Format.IsHtml())
                    signature = signature.Replace(Environment.NewLine, "<br>");

                body += signature;
            }

            return body;
        }

        private IList<string> GetFileAttachmentList(Job job, IMailActionSettings settings)
        {
            var attachmentList = new List<string>();

            if (!OneDriveShareLinksAreUsed(job, settings) && !DropboxShareLinksAreUsed(job, settings))
            {
                attachmentList.AddRange(job.OutputFiles);
            }
            // Additional attachments must always be added
            // If the user wants them as share link, he needs to upload them and put the share link in the mail content 
            attachmentList.AddRange(settings.AdditionalAttachments);

            return attachmentList;
        }

        private bool DropboxShareLinksAreUsed(Job job, IMailActionSettings mailSettings)
        {
            if (job.Profile.DropboxSettings.Enabled == false)
                return false;

            if (!TokenIdentifier.ContainsAnyToken(mailSettings.Content, TokenNames.DropboxFullLinks, TokenNames.DropboxHtmlLinks))
                return false;

            var mailTypeName = mailSettings.GetType().Name;
            if (!job.Profile.IsFirstActionBeforeSecond(nameof(DropboxSettings), mailTypeName))
            {
                _logger.Warn("To use the Dropbox share link instead of mail attachments, " +
                             "the Dropbox action must be executed before the mail action. " +
                             "For the current mail, the converted output will be attached.");
                return false;
            }

            if (job.Profile.DropboxSettings.CreateShareLink == false)
            {
                _logger.Warn("To use the Dropbox share link instead of mail attachments, " +
                             "a share link must be enabled in the Dropbox action. " +
                             "For the current mail, the converted output will be attached.");
                return false;
            }

            _logger.Info("Converted documents are not added to mail attachments because Dropbox share link token was set in content");
            return true;
        }

        private bool OneDriveShareLinksAreUsed(Job job, IMailActionSettings mailSettings)
        {
            if (job.Profile.OneDriveSettings.Enabled == false)
                return false;

            if (!TokenIdentifier.ContainsAnyToken(mailSettings.Content ,TokenNames.OneDriveShareLink, TokenNames.OneDriveShareLinkHtml))
                return false;

            var mailTypeName = mailSettings.GetType().Name;
            if (!job.Profile.IsFirstActionBeforeSecond(nameof(OneDriveSettings), mailTypeName))
            {
                _logger.Warn("To use the OneDrive share link instead of mail attachments, " +
                             "the OneDrive action must be executed before the mail action. " +
                             "For the current mail, the converted output will be attached.");
                return false;
            }

            if (job.Profile.OneDriveSettings.CreateShareLink == false)
            {
                _logger.Warn("To use the OneDrive share link instead of mail attachments, " +
                             "a share link must be enabled in the OneDrive action. " +
                             "For the current mail, the converted output will be attached.");
                return false;
            }

            _logger.Info("Converted documents are not added to mail attachments because OneDrive share link token was set in content");
            return true;
        }
    }
}
