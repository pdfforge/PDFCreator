using NLog;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using System;
using System.Net;
using System.Net.Mail;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions
{
    public interface ISmtpMailAction : IPostConversionAction, ICheckable
    { }

    public class SmtpMailAction : RetypePasswordActionBase, ISmtpMailAction
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IFile _file;
        private readonly IMailHelper _mailHelper;

        protected override string PasswordText => "SMTP";

        public SmtpMailAction(IFile file, IMailHelper mailHelper)
        {
            _file = file;
            _mailHelper = mailHelper;
        }

        public override void ApplyPreSpecifiedTokens(Job job)
        {
            _mailHelper.ReplaceTokensInMailSettings(job, job.Profile.EmailSmtpSettings);
        }

        public override ActionResult Check(ConversionProfile profile, Accounts accounts, CheckLevel checkLevel)
        {
            var actionResult = new ActionResult();

            if (!IsEnabled(profile))
                return actionResult;

            var smtpAccount = accounts.GetSmtpAccount(profile);

            if (smtpAccount == null)
            {
                Logger.Error($"The specified SMTP account with ID \"{profile.EmailSmtpSettings.AccountId}\" is not configured.");
                actionResult.Add(ErrorCode.Smtp_NoAccount);
                return actionResult;
            }

            if (string.IsNullOrWhiteSpace(smtpAccount.Address))
            {
                Logger.Error("No SMTP email address is specified.");
                actionResult.Add(ErrorCode.Smtp_NoEmailAddress);
            }

            if (string.IsNullOrWhiteSpace(smtpAccount.Server))
            {
                Logger.Error("No SMTP host is specified.");
                actionResult.Add(ErrorCode.Smtp_NoServerSpecified);
            }

            if (smtpAccount.Port < 0)
            {
                Logger.Error("Invalid SMTP port.");
                actionResult.Add(ErrorCode.Smtp_InvalidPort);
            }

            if (string.IsNullOrWhiteSpace(smtpAccount.UserName))
            {
                Logger.Error("No SMTP UserName is specified.");
                actionResult.Add(ErrorCode.Smtp_NoUserSpecified);
            }

            if (string.IsNullOrWhiteSpace(profile.EmailSmtpSettings.Recipients))
            {
                Logger.Error("No SMTP email recipients are specified.");
                actionResult.Add(ErrorCode.Smtp_NoRecipients);
            }

            if (profile.AutoSave.Enabled)
            {
                if (string.IsNullOrWhiteSpace(smtpAccount.Password))
                {
                    Logger.Error("No SMTP password for automatic saving.");
                    actionResult.Add(ErrorCode.Smtp_NoPasswordSpecified);
                }
            }

            if (checkLevel == CheckLevel.Job)
            {
                foreach (var attachmentFile in profile.EmailSmtpSettings.AdditionalAttachments)
                {
                    if (!_file.Exists(attachmentFile))
                    {
                        actionResult.Add(ErrorCode.Smtp_InvalidAttachmentFiles);
                        Logger.Error("Can't find SMTP attachment " + attachmentFile + ".");
                        break;
                    }
                }
            }

            return actionResult;
        }

        protected override ActionResult DoActionProcessing(Job job)
        {
            var smtpAccount = job.Accounts.GetSmtpAccount(job.Profile);

            if (string.IsNullOrEmpty(job.Passwords.SmtpPassword))
            {
                Logger.Error("SendMailOverSmtp canceled. Action launched without Password.");
                return new ActionResult(ErrorCode.Smtp_NoPasswordSpecified);
            }

            var result = CreateMailMessage(job, job.Profile.EmailSmtpSettings, smtpAccount, out var message);
            if (!result)
                return result;

            Logger.Debug("Created new Mail"
                         + "\r\nFrom: " + message.From
                         + "\r\nTo: " + message.To
                         + "\r\nSubject: " + message.Subject
                         + "\r\nContent: " + message.Body
            );

            var smtp = new SmtpClient(smtpAccount.Server, smtpAccount.Port)
            {
                EnableSsl = smtpAccount.Ssl
            };

            Logger.Debug("Created new SmtpClient:"
                         + "\r\nHost: " + smtp.Host
                         + "\r\nPort: " + smtp.Port
            );

            return SendEmail(smtp, job.Passwords.SmtpPassword, message, smtpAccount);
        }

        private ActionResult CreateMailMessage(Job job, EmailSmtpSettings mailSettings, SmtpAccount sender, out MailMessage mailMessage)
        {
            var mailInfo = _mailHelper.CreateMailInfo(job, mailSettings);

            mailMessage = null;
            try
            {
                mailMessage = mailMessage = new MailMessage(sender.Address, mailInfo.Recipients)
                {
                    Subject = mailInfo.Subject,
                    Body = mailInfo.Body,
                    IsBodyHtml = mailInfo.IsHtml
                };

                if (!string.IsNullOrWhiteSpace(mailInfo.RecipientsBcc)) //Prevent MailMessage from throwing and exception if BCC is not set
                    mailMessage.Bcc.Add(mailInfo.RecipientsBcc);

                if (!string.IsNullOrWhiteSpace(mailInfo.RecipientsCc)) //Prevent MailMessage from throwing and exception if CC is not set
                    mailMessage.CC.Add(mailInfo.RecipientsCc);
            }
            catch
            {
                return new ActionResult(ErrorCode.Smtp_InvalidRecipients);
            }

            foreach (var attachment in mailInfo.Attachments)
            {
                try
                {
                    mailMessage.Attachments.Add(new Attachment(attachment));
                }
                catch
                {
                    Logger.Error("Invalid SMTP attachment: " + attachment);
                    return new ActionResult(ErrorCode.Smtp_InvalidAttachmentFiles);
                }
            }

            return new ActionResult();
        }

        private ActionResult SendEmail(SmtpClient smtpClient, string smtpPassword, MailMessage mail, SmtpAccount smtpAccount)
        {
            var credentials = new NetworkCredential(smtpAccount.UserName, smtpPassword);
            smtpClient.Credentials = credentials;

            try
            {
                smtpClient.Send(mail);
            }
            catch (SmtpFailedRecipientsException ex)
            {
                Logger.Error(ex, "The message could not be delivered to one or more of the recipients ");
                return new ActionResult(ErrorCode.Smtp_EmailNotDelivered);
            }
            catch (SmtpException ex)
            {
                Logger.Warn("Could not authorize to host.\r\n" + ex.Message);
                return new ActionResult(ErrorCode.PasswordAction_Login_Error);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception while sending mail over smtp: ");

                return new ActionResult(ErrorCode.Smtp_GenericError);
            }
            finally
            {
                mail.Dispose();
            }

            return new ActionResult();
        }

        protected override void SetPassword(Job job, string password)
        {
            job.Passwords.SmtpPassword = password;
        }

        public override bool IsEnabled(ConversionProfile profile)
        {
            return profile.EmailSmtpSettings.Enabled;
        }
    }
}
