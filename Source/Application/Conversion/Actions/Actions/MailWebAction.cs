using Newtonsoft.Json;
using NLog;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Mail;
using pdfforge.PDFCreator.Conversion.Actions.Actions.WebMail;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Utilities.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Helper.Microsoft;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using SystemInterface.IO;
using pdfforge.PDFCreator.Utilities.Tokens;
using SystemInterface;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions
{
    public interface IMailWebAction : IPostConversionAction
    {
    }

    public class MailWebAction : ActionBase<EmailWebSettings>, IMailWebAction
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IFile _file;
        private readonly IGraphManager _graphManager;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IMailHelper _mailHelper;
        private readonly IWebLinkLauncher _webLinkLauncher;
        private readonly MicrosoftActionHelper _microsoftActionHelper;

        public MailWebAction(IFile file, IGraphManager graphManager, IMailHelper mailHelper, 
            IWebLinkLauncher webLinkLauncher, MicrosoftActionHelper microsoftActionHelper)
            : base(p => p.EmailWebSettings)
        {
            _file = file;
            _graphManager = graphManager;
            _mailHelper = mailHelper;
            _webLinkLauncher = webLinkLauncher;
            _microsoftActionHelper = microsoftActionHelper;
        }

        protected override ActionResult DoProcessJob(Job job, IPdfProcessor processor)
        {
            // check if there is an account present
            if (job.Accounts.MicrosoftAccounts.Count == 0)
                return new ActionResult(ErrorCode.Microsoft_Account_Missing);

            var attachmentsList = new List<string>();
            attachmentsList.AddRange(job.OutputFiles);
            attachmentsList.AddRange(job.Profile.EmailWebSettings.AdditionalAttachments);

            // check for too many attachments
            if (attachmentsList.Count > 100)
                return new ActionResult(ErrorCode.Outlook_Web_Attachment_Limit_Count);

            // check for general size
            long size = attachmentsList.ToList().Sum(fileName => new FileInfo(fileName).Length);
            if (size > 150 * 1024 * 1024) // max allowed size is 150mb
                return new ActionResult(ErrorCode.Outlook_Web_Attachment_Limit_Size);

            ApplyPreSpecifiedTokens(job);
            var mailInfo = _mailHelper.CreateMailInfo(job, job.Profile.EmailWebSettings);

            return ProcessMailInfo(job, mailInfo).Result;
        }

        private string CreateMessageJsonString(MailInfo mailInfo)
        {
            try
            {
                var toRecipients = CreateRecipientArrayFromString(mailInfo.Recipients);
                var ccRecipients = CreateRecipientArrayFromString(mailInfo.RecipientsCc);
                var bccToRecipients = CreateRecipientArrayFromString(mailInfo.RecipientsBcc);

                var message = new GraphMailMessage()
                {
                    Subject = mailInfo.Subject,
                    Body = new GraphMailBody()
                    {
                        Content = mailInfo.Body,
                        ContentType = mailInfo.Format.IsHtml() ? "HTML" : "text",
                    },
                    ToRecipients = toRecipients,
                    CcRecipients = ccRecipients,
                    BccRecipients = bccToRecipients
                };

                return JsonConvert.SerializeObject(message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not parse Object to JSON");
                return "";
            }
        }

        private GraphMailRecipient[] CreateRecipientArrayFromString(string recipientsString)
        {
            var recipients = new List<GraphMailRecipient>();
            var splitString = recipientsString.Split(',', ';');
            foreach (var address in splitString)
            {
                recipients.Add(new GraphMailRecipient()
                {
                    EmailAddress = new GraphMailEmailAddress
                    {
                        Address = address
                    }
                });
            }

            return recipients.ToArray();
        }

        private async Task<ActionResult> ProcessMailInfo(Job job, MailInfo mailInfo)
        {
            if (job == null)
                return new ActionResult(ErrorCode.Outlook_Web_General_Error);

            var settings = job.Profile.EmailWebSettings;
            var account = job.Accounts.GetMicrosoftAccount(job.Profile);

            var actionResult = new ActionResult();

            if (!account.HasPermissions(MicrosoftAccountPermission.MailReadWrite))
                return new ActionResult(ErrorCode.Outlook_Web_MissingReadWritePermissions);

            try
            {
                var messageString = CreateMessageJsonString(mailInfo);
                var httpContent = _microsoftActionHelper.CreateJsonStringContent(messageString);

                using var httpClient = new HttpClient();
                var authenticationAccountResult = await _graphManager.GetAccessToken(account);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authenticationAccountResult.AccessToken);

                // Create draft message
                var createdDraftResponse = await httpClient.PostAsync($"{GraphManager.BaseURL}/me/messages", httpContent);
                if (createdDraftResponse.StatusCode != HttpStatusCode.Created)
                {
                    return new ActionResult(ErrorCode.Microsoft_Account_Missing);
                }

                // Add Attachment to draft
                var createDraftResponseString = await createdDraftResponse.Content.ReadAsStringAsync();
                var createDraftResponse = JsonConvert.DeserializeObject<CreateMessageResponse>(createDraftResponseString);
                var messageId = createDraftResponse.Id;
                var requestUri = $"{GraphManager.BaseURL}/me/messages/{messageId}/send";

                if (!await UploadAttachments(job, messageId, authenticationAccountResult.AccessToken, actionResult)) return actionResult;
                if (settings.SendWebMailAutomatically && account.HasPermissions(MicrosoftAccountPermission.MailSend))
                {
                    try
                    {
                        var sendResponse = await httpClient.PostAsync(requestUri, new StringContent(string.Empty));
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Error while sending "+ e.Message, e);
                        return new ActionResult(ErrorCode.Outlook_Web_General_Error);
                    }
                }
                else
                {
                    if (job.Profile.EmailWebSettings.ShowDraft) // is it interactive and not send directly?
                    {
                        _webLinkLauncher.Launch(createDraftResponse.WebLink);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, "General error during web mail action", e);
                actionResult.Add(new ActionResult(ErrorCode.Outlook_Web_General_Error));
                return actionResult;
            }
            return actionResult;
        }

        private async Task<bool> UploadAttachments(Job job, string messageId, string accessToken, ActionResult actionResult)
        {
            if (job.OutputFiles.Count == 0)
            {
                {
                    actionResult.Add(new ActionResult(ErrorCode.Processing_OutputFileMissing));
                    return false;
                }
            }

            var uploadSessionUrl = $"{GraphManager.BaseURL}/me/messages/{messageId}/attachments/createUploadSession";

            foreach (var jobOutputFile in job.OutputFiles)
            {
                var uploadResult = await _microsoftActionHelper.UploadFile(uploadSessionUrl, jobOutputFile, accessToken, true);
                if (uploadResult == null)
                {
                    actionResult.Add(new ActionResult(ErrorCode.Outlook_Web_Upload_Error));
                    return false;
                }
            }

            foreach (var jobOutputFile in job.Profile.EmailWebSettings.AdditionalAttachments)
            {
                var uploadResult = await _microsoftActionHelper.UploadFile(uploadSessionUrl, jobOutputFile, accessToken, true);
                if (uploadResult == null)
                {
                        actionResult.Add(new ActionResult(ErrorCode.Outlook_Web_Upload_Error));
                        return false;
                }
            }

            return true;
        }
        
        public override void ApplyPreSpecifiedTokens(Job job)
        {
            _mailHelper.ReplaceTokensInMailSettings(job, job.Profile.EmailWebSettings);
        }

        public override ActionResult Check(ConversionProfile profile, CurrentCheckSettings settings, CheckLevel checkLevel)
        {
            var result = new ActionResult();

            var account = settings.Accounts.MicrosoftAccounts.FirstOrDefault(microsoftAccount => microsoftAccount.AccountId == profile.EmailWebSettings.AccountId);
            if (account == null)
            {
                result.Add(ErrorCode.Microsoft_Account_Missing);
            }
            else
            {
                if (!account.HasPermissions(MicrosoftAccountPermission.MailReadWrite))
                    result.Add(ErrorCode.Outlook_Web_MissingReadWritePermissions);
                else if (profile.EmailWebSettings.SendWebMailAutomatically && !account.HasPermissions(MicrosoftAccountPermission.MailSend))
                    result.Add(ErrorCode.Outlook_Web_MissingSendPermission);
                else if (account.GetExpirationDateTime() < DateTime.Now)
                    result.Add(ErrorCode.Microsoft_Account_Expired);
            }

            if (checkLevel == CheckLevel.EditingProfile && !profile.UserTokens.Enabled)
            {
                if (TokenIdentifier.ContainsUserToken(profile.EmailWebSettings.Recipients))
                    result.Add(ErrorCode.Outlook_Web_Recipients_RequiresUserToken);
                if (TokenIdentifier.ContainsUserToken(profile.EmailWebSettings.RecipientsCc))
                    result.Add(ErrorCode.Outlook_Web_RecipientsCc_RequiresUserToken);
                if (TokenIdentifier.ContainsUserToken(profile.EmailWebSettings.RecipientsBcc))
                    result.Add(ErrorCode.Outlook_Web_RecipientsBcc_RequiresUserToken);
                if (TokenIdentifier.ContainsUserToken(profile.EmailWebSettings.Subject))
                    result.Add(ErrorCode.Outlook_Web_Subject_RequiresUserToken);
                if (TokenIdentifier.ContainsUserToken(profile.EmailWebSettings.Content))
                    result.Add(ErrorCode.Outlook_Web_Content_RequiresUserToken);
                
                foreach (var path in profile.EmailWebSettings.AdditionalAttachments)
                {
                    if (TokenIdentifier.ContainsUserToken(path))
                    {
                        result.Add(ErrorCode.Outlook_Web_AdditionalAttachment_RequiresUserToken);
                        break;
                    }
                }
            }

            if (checkLevel == CheckLevel.RunningJob)
            {
                foreach (var attachmentFile in profile.EmailWebSettings.AdditionalAttachments)
                {
                    if (!_file.Exists(attachmentFile))
                    {
                        Logger.Error("Can't find web mail attachment " + attachmentFile + ".");
                        result.Add(ErrorCode.Outlook_Web_InvalidAttachmentFiles);
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
