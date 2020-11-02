using NLog;
using pdfforge.Mail;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using System;
using System.Collections.Generic;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions
{
    public interface IEMailClientAction : IPostConversionAction, ICheckable
    {
        ActionResult OpenEmptyClient(IList<string> files, string signature);
    }

    public class EMailClientAction : IEMailClientAction
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IEmailClientFactory _emailClientFactory;
        private readonly IFile _file;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IMailHelper _mailHelper;

        public EMailClientAction(IEmailClientFactory emailClientFactory, IFile file, IMailHelper mailHelper)
        {
            _emailClientFactory = emailClientFactory;
            _file = file;
            _mailHelper = mailHelper;
        }

        public ActionResult OpenEmptyClient(IList<string> files, string signature)
        {
            var mailInfo = new MailInfo { Attachments = files, Body = signature };

            return ProcessMailInfo(mailInfo);
        }

        public ActionResult ProcessJob(Job job)
        {
            var mailInfo = _mailHelper.CreateMailInfo(job, job.Profile.EmailClientSettings);

            return ProcessMailInfo(mailInfo);
        }

        private Email CreateEmail(MailInfo mailInfo)
        {
            var mail = new Email
            {
                Html = mailInfo.IsHtml,
                Subject = mailInfo.Subject,
                Body = mailInfo.Body,
            };

            mail.Recipients.AddTo(mailInfo.Recipients);
            mail.Recipients.AddCc(mailInfo.RecipientsCc);
            mail.Recipients.AddBcc(mailInfo.RecipientsBcc);

            foreach (var file in mailInfo.Attachments)
            {
                var attachment = new Attachment(file);
                mail.Attachments.Add(attachment);
                _logger.Debug("Added mail attachment " + file);
            }

            return mail;
        }

        private ActionResult ProcessMailInfo(MailInfo mailInfo)
        {
            try
            {
                _logger.Info("Launch e-mail client action");

                var mailClient = _emailClientFactory.CreateEmailClient();
                if (mailClient == null)
                {
                    _logger.Error("No compatible e-mail client installed.");
                    return new ActionResult(ErrorCode.MailClient_NoCompatibleEmailClientInstalled);
                }

                var email = CreateEmail(mailInfo);

                var success = mailClient.ShowEmailClient(email);

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
                _logger.Error(ex, "Exception in e-mail client Action ");
                return new ActionResult(ErrorCode.MailClient_GenericError);
            }
        }

        public bool IsEnabled(ConversionProfile profile)
        {
            return profile.EmailClientSettings.Enabled;
        }

        public void ApplyPreSpecifiedTokens(Job job)
        {
            _mailHelper.ReplaceTokensInMailSettings(job, job.Profile.EmailClientSettings);
        }

        public ActionResult Check(ConversionProfile profile, Accounts accounts, CheckLevel checkLevel)
        {
            var result = new ActionResult();

            if (_emailClientFactory.CreateEmailClient() == null)
                result.Add(ErrorCode.MailClient_NoCompatibleEmailClientInstalled);

            if (checkLevel == CheckLevel.Job)
            {
                foreach (var attachmentFile in profile.EmailClientSettings.AdditionalAttachments)
                {
                    if (!_file.Exists(attachmentFile))
                    {
                        Logger.Error("Can't find client mail attachment " + attachmentFile + ".");
                        result.Add(ErrorCode.MailClient_InvalidAttachmentFiles);
                        break;
                    }
                }
            }

            return result;
        }
    }
}
