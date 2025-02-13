﻿using NLog;
using pdfforge.Mail;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Mail;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System;
using System.Collections.Generic;
using pdfforge.PDFCreator.Utilities.Tokens;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions
{
    public interface IEMailClientAction : IPostConversionAction
    {
        ActionResult OpenEmptyClient(IList<string> files, EmailClientSettings settings);
    }

    public class MailClientAction : ActionBase<EmailClientSettings>, IEMailClientAction
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IEmailClientFactory _emailClientFactory;
        private readonly IFile _file;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IMailHelper _mailHelper;

        public MailClientAction(IEmailClientFactory emailClientFactory, IFile file, IMailHelper mailHelper)
            : base(p => p.EmailClientSettings)
        {
            _emailClientFactory = emailClientFactory;
            _file = file;
            _mailHelper = mailHelper;
        }

        public ActionResult OpenEmptyClient(IList<string> files, EmailClientSettings settings)
        {
            var mailInfo = _mailHelper.CreateMailInfo(files, settings);
            return ProcessMailInfo(mailInfo);
        }

        protected override ActionResult DoProcessJob(Job job, IPdfProcessor processor)
        {
            ApplyPreSpecifiedTokens(job);
            var mailInfo = _mailHelper.CreateMailInfo(job, job.Profile.EmailClientSettings);

            return ProcessMailInfo(mailInfo);
        }

        private Email CreateEmail(MailInfo mailInfo)
        {
            var mapEmailFormatSetting = new Func<EmailFormatSetting, EmailFormat>((emailFormatSetting) =>
           {
               switch (emailFormatSetting)
               {
                   case EmailFormatSetting.Auto:
                       return EmailFormat.Auto;

                   case EmailFormatSetting.Html:
                       return EmailFormat.Html;

                   case EmailFormatSetting.Text:
                       return EmailFormat.Text;

                   default:
                       return EmailFormat.Auto;
               }
           });

            var mail = new Email
            {
                Format = mapEmailFormatSetting(mailInfo.Format),
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
                _logger.Info("Launch email client action");

                var mailClient = _emailClientFactory.CreateEmailClient();
                if (mailClient == null)
                {
                    _logger.Error("No compatible email client installed.");
                    return new ActionResult(ErrorCode.MailClient_NoCompatibleEmailClientInstalled);
                }

                var email = CreateEmail(mailInfo);

                var success = mailClient.ShowEmailClient(email);

                if (!success)
                {
                    _logger.Warn("Could not start email client");
                    return new ActionResult(ErrorCode.MailClient_GenericError);
                }

                _logger.Info("Done starting email client");
                return new ActionResult();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception in email client Action ");
                return new ActionResult(ErrorCode.MailClient_GenericError);
            }
        }

        public override void ApplyPreSpecifiedTokens(Job job)
        {
            _mailHelper.ReplaceTokensInMailSettings(job, job.Profile.EmailClientSettings);
        }

        public override ActionResult Check(ConversionProfile profile, CurrentCheckSettings settings, CheckLevel checkLevel)
        {
            var result = new ActionResult();

            if (_emailClientFactory.CreateEmailClient() == null)
                result.Add(ErrorCode.MailClient_NoCompatibleEmailClientInstalled);

            if (checkLevel == CheckLevel.EditingProfile && !profile.UserTokens.Enabled)
            {
                if (TokenIdentifier.ContainsUserToken(profile.EmailClientSettings.Recipients))
                    result.Add(ErrorCode.MailClient_Recipients_RequiresUserToken);
                if (TokenIdentifier.ContainsUserToken(profile.EmailClientSettings.RecipientsCc))
                    result.Add(ErrorCode.MailClient_RecipientsCc_RequiresUserToken);
                if (TokenIdentifier.ContainsUserToken(profile.EmailClientSettings.RecipientsBcc))
                    result.Add(ErrorCode.MailClient_RecipientsBcc_RequiresUserToken);
                if (TokenIdentifier.ContainsUserToken(profile.EmailClientSettings.Subject))
                    result.Add(ErrorCode.MailClient_Subject_RequiresUserToken);
                if (TokenIdentifier.ContainsUserToken(profile.EmailClientSettings.Content))
                    result.Add(ErrorCode.MailClient_Content_RequiresUserToken);
                
                foreach (var path in profile.EmailClientSettings.AdditionalAttachments)
                {
                    if (TokenIdentifier.ContainsUserToken(path))
                    {
                        result.Add(ErrorCode.MailClient_AdditionalAttachment_RequiresUserToken);
                        break;
                    }
                }
            }
            else if (checkLevel == CheckLevel.RunningJob)
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

        public override bool IsRestricted(ConversionProfile profile)
        {
            return false;
        }

        protected override void ApplyActionSpecificRestrictions(Job job)
        { }
    }
}
