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
    public interface IEMailClientAction : IPostConversionAction
    {
        ActionResult OpenEmptyClient(IList<string> files, string signature);
    }

    public class EMailClientAction : IEMailClientAction
    {
        private readonly IEmailClientFactory _emailClientFactory;
        private readonly IFile _file;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private IMailHelper _mailHelper;

        public EMailClientAction(IEmailClientFactory emailClientFactory, IMailSignatureHelper mailSignatureHelper, IFile file, IMailHelper mailHelper)
        {
            _emailClientFactory = emailClientFactory;
            _file = file;
            _mailHelper = mailHelper;
        }

        public ActionResult OpenEmptyClient(IList<string> files, string signature)
        {
            var mailInfo = new MailInfo { Attachments = files, Body = signature };

            return Process(mailInfo);
        }

        public ActionResult ProcessJob(Job job)
        {
            _logger.Info("Launched client e-mail action");

            var mailInfo = _mailHelper.CreateMailInfo(job, job.Profile.EmailClientSettings);

            return Process(mailInfo);
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

            AddOutputFilesAsAttachmentsForEmailClientAction(mail, mailInfo.Attachments);

            return mail;
        }

        private void AddOutputFilesAsAttachmentsForEmailClientAction(Email mail, IList<string> outputFiles)
        {
            var i = 1;
            foreach (var file in outputFiles)
            {
                if (_file.Exists(file))
                {
                    var attachment = new Attachment(file);
                    mail.Attachments.Add(attachment);
                    _logger.Debug("Attachment " + i + "/" + outputFiles.Count + ":" + file);
                    i++;
                }
                else
                {
                    _logger.Error("E-Mail Attachment \"" + file + "\" not found.");
                }
            }
        }

        private ActionResult Process(MailInfo mailInfo)
        {
            var email = CreateEmail(mailInfo);

            try
            {
                _logger.Info("Launched e-mail client action");

                var mailClient = _emailClientFactory.CreateEmailClient();

                if (mailClient == null)
                {
                    _logger.Error("No compatible e-mail client installed");
                    return new ActionResult(ErrorCode.MailClient_NoCompatibleEmailClientInstalled);
                }

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

        public bool CheckEmailClientInstalled()
        {
            return _emailClientFactory.CreateEmailClient() != null;
        }
    }
}
