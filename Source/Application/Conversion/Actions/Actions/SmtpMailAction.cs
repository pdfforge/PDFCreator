using NLog;
using pdfforge.Mail;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using System;
using System.Net;
using System.Net.Mail;
using Attachment = System.Net.Mail.Attachment;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions
{
    public interface ISmtpMailAction : IPostConversionAction, ICheckable
    { }

    public class SmtpMailAction : RetypePasswordActionBase, ISmtpMailAction
    {
        private readonly IMailSignatureHelper _mailSignatureHelper;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected override string PasswordText => "SMTP";

        public SmtpMailAction(IMailSignatureHelper mailSignatureHelper)
        {
            _mailSignatureHelper = mailSignatureHelper;
        }

        public override void ApplyPreSpecifiedTokens(Job job)
        {
            job.Profile.EmailSmtpSettings.Recipients = job.TokenReplacer.ReplaceTokens(job.Profile.EmailSmtpSettings.Recipients)
                                                                        .Replace(';', ',');
            job.Profile.EmailSmtpSettings.RecipientsCc = job.TokenReplacer.ReplaceTokens(job.Profile.EmailSmtpSettings.RecipientsCc)
                                                                          .Replace(';', ',');
            job.Profile.EmailSmtpSettings.RecipientsBcc = job.TokenReplacer.ReplaceTokens(job.Profile.EmailSmtpSettings.RecipientsBcc)
                                                                           .Replace(';', ',');
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

            MailMessage mail;
            try
            {
                mail = new MailMessage(smtpAccount.Address, job.Profile.EmailSmtpSettings.Recipients);
            }
            catch (Exception e) when (e is FormatException || e is ArgumentException)
            {
                Logger.Error($"\'{job.Profile.EmailSmtpSettings.Recipients}\' is no valid SMTP e-mail recipient: " + e.Message);
                return new ActionResult(ErrorCode.Smtp_InvalidRecipients);
            }

            // these blocks have to be seperated, because we want to log the offending recipients
            // (AddRecipients does this already, but can't be reused for the constructor)
            try
            {
                AddRecipients(mail, RecipientType.Cc, job.Profile.EmailSmtpSettings.RecipientsCc);
                AddRecipients(mail, RecipientType.Bcc, job.Profile.EmailSmtpSettings.RecipientsBcc);
            }
            catch
            {
                return new ActionResult(ErrorCode.Smtp_InvalidRecipients);
            }

            mail.Subject = job.TokenReplacer.ReplaceTokens(job.Profile.EmailSmtpSettings.Subject);
            mail.IsBodyHtml = job.Profile.EmailSmtpSettings.Html;
            mail.Body = job.TokenReplacer.ReplaceTokens(job.Profile.EmailSmtpSettings.Content);

            if (job.Profile.EmailSmtpSettings.AddSignature)
            {
                var mailSignature = _mailSignatureHelper.ComposeMailSignature();

                // if html option is checked replace newLine with <br />
                if (job.Profile.EmailSmtpSettings.Html)
                    mailSignature = mailSignature.Replace(Environment.NewLine, "<br>");

                mail.Body += mailSignature;
            }

            Logger.Debug("Created new Mail"
                         + "\r\nFrom: " + mail.From
                         + "\r\nTo: " + mail.To
                         + "\r\nSubject: " + mail.Subject
                         + "\r\nContent: " + mail.Body
            );

            if (!SkipFileAttachments(job))
            {
                var i = 1;
                foreach (var file in job.OutputFiles)
                {
                    var attach = new Attachment(file);
                    //attach.NameEncoding = System.Text.Encoding.ASCII;
                    mail.Attachments.Add(attach);
                    Logger.Debug("Attachement " + i + "/" + job.OutputFiles.Count + ":" + file);
                    i++;
                }
            }

            var smtp = new SmtpClient(smtpAccount.Server, smtpAccount.Port);
            smtp.EnableSsl = smtpAccount.Ssl;

            Logger.Debug("Created new SmtpClient:"
                         + "\r\nHost: " + smtp.Host
                         + "\r\nPort: " + smtp.Port
            );

            return SendEmail(job, smtp, mail);
        }

        private MailAddressCollection GetAddressCollection(MailMessage mail, RecipientType recipientType)
        {
            switch (recipientType)
            {
                case RecipientType.To: return mail.To;
                case RecipientType.Cc: return mail.CC;
                case RecipientType.Bcc: return mail.Bcc;
                default: throw new ArgumentException("Invalid recipient type!", nameof(recipientType));
            }
        }

        private void AddRecipients(MailMessage mail, RecipientType recipientType, string recipients)
        {
            if (string.IsNullOrWhiteSpace(recipients))
                return;

            var addressCollection = GetAddressCollection(mail, recipientType);
            try
            {
                addressCollection.Add(recipients);
            }
            catch (Exception e) when (e is FormatException || e is ArgumentException)
            {
                Logger.Error($"\'{recipients}\' is no valid SMTP e-mail recipient: " + e.Message);
                throw;
            }
        }

        private bool SkipFileAttachments(Job job)
        {
            if (job.Profile.DropboxSettings.Enabled == false || job.Profile.DropboxSettings.CreateShareLink == false)
                return false;

            return job.Profile.EmailSmtpSettings.Content.IndexOf("<Dropbox", StringComparison.InvariantCultureIgnoreCase) >= 0;
        }

        private ActionResult SendEmail(Job job, SmtpClient smtp, MailMessage mail)
        {
            var smtpAccount = job.Accounts.GetSmtpAccount(job.Profile);

            var credentials = new NetworkCredential(smtpAccount.UserName, job.Passwords.SmtpPassword);
            smtp.Credentials = credentials;

            try
            {
                smtp.Send(mail);
            }
            catch (SmtpFailedRecipientsException ex)
            {
                Logger.Error("The message could not be delivered to one or more of the recipients\r\n" + ex.Message);
                return new ActionResult(ErrorCode.Smtp_EmailNotDelivered);
            }
            catch (SmtpException ex)
            {
                Logger.Warn("Could not authorize to host.\r\n" + ex.Message);
                return new ActionResult(ErrorCode.PasswordAction_Login_Error);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception while sending mail over smtp:\r\n" + ex.Message);

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
