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
using System.Text;
using System.Threading.Tasks;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using SystemInterface.IO;

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

        public MailWebAction(IFile file, IGraphManager graphManager, IMailHelper mailHelper, IWebLinkLauncher webLinkLauncher)
            : base(p => p.EmailWebSettings)
        {
            _file = file;
            _graphManager = graphManager;
            _mailHelper = mailHelper;
            _webLinkLauncher = webLinkLauncher;
        }

        protected override ActionResult DoProcessJob(Job job, IPdfProcessor processor)
        {
            // check if there is an account present
            if (job.Accounts.MicrosoftAccounts.Count == 0)
                return new ActionResult(ErrorCode.Outlook_Web_Account_Missing);

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

        private StringContent CreateJsonStringContent(string message)
        {
            return new StringContent(message, Encoding.UTF8, "application/json");
        }

        private async Task<ActionResult> ProcessMailInfo(Job job, MailInfo mailInfo)
        {
            var actionResult = new ActionResult();
            if (job == null)
                return new ActionResult(ErrorCode.Outlook_Web_General_Error);

            try
            {
                var account = job.Accounts.GetMicrosoftAccount(job.Profile);
                var messageString = CreateMessageJsonString(mailInfo);
                var httpContent = CreateJsonStringContent(messageString);

                var httpClient = new HttpClient();
                var authenticationAccountResult = await _graphManager.GetAccessToken(account);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authenticationAccountResult.AccessToken);

                // Create draft message
                var createdDraftResponse = await httpClient.PostAsync($"{GraphManager.BaseURL}/me/messages", httpContent);
                if (createdDraftResponse.StatusCode != HttpStatusCode.Created)
                {
                    return new ActionResult(ErrorCode.Outlook_Web_Account_Missing);
                }

                // Add Attachment to draft
                var createDraftResponseString = await createdDraftResponse.Content.ReadAsStringAsync();
                var createDraftResponse = JsonConvert.DeserializeObject<CreateMessageResponse>(createDraftResponseString);
                var messageId = createDraftResponse.Id;

                if (!await UploadAttachments(job, messageId, httpClient, actionResult)) return actionResult;

                if (!job.Profile.AutoSave.Enabled) // is it interactive?
                    _webLinkLauncher.Launch(createDraftResponse.WebLink);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, "General error during web mail action", e);
                actionResult.Add(new ActionResult(ErrorCode.Outlook_Web_General_Error));
                return actionResult;
            }
            return actionResult;
        }

        private async Task<bool> UploadAttachments(Job job, string messageId, HttpClient httpClient, ActionResult actionResult)
        {
            if (job.OutputFiles.Count == 0)
            {
                {
                    actionResult.Add(new ActionResult(ErrorCode.Processing_OutputFileMissing));
                    return false;
                }
            }

            foreach (var jobOutputFile in job.OutputFiles)
            {
                if (!await UploadFile(messageId, httpClient, actionResult, jobOutputFile))
                {
                    return false;
                }
            }

            foreach (var jobOutputFile in job.Profile.EmailWebSettings.AdditionalAttachments)
            {
                if (!await UploadFile(messageId, httpClient, actionResult, jobOutputFile))
                {
                    return false;
                }
            }

            return true;
        }

        private async Task<bool> UploadFile(string messageId, HttpClient httpClient, ActionResult actionResult, string filePath)
        {
            int chunk = 4 * 1024 * 1024; // max chuck size is 4mb
            var uploadSessionUrl = $"{GraphManager.BaseURL}/me/messages/{messageId}/attachments/createUploadSession";

            var fInfo = new FileInfo(filePath);
            var file = File.OpenRead(filePath);
            try
            {
                var uploadSessionMessage = CreateUploadSessionMessage(fInfo, file);

                var stringContent = CreateJsonStringContent(JsonConvert.SerializeObject(uploadSessionMessage));

                var createUploadSessionResponse = await httpClient.PostAsync(uploadSessionUrl, stringContent);
                var readAsStringAsync = await createUploadSessionResponse.Content.ReadAsStringAsync();
                var createUploadSessionResponseContent = JsonConvert.DeserializeObject<CreateUploadSessionResponse>(readAsStringAsync);

                int totalChunks = ((int)uploadSessionMessage.AttachmentItem.Size) / chunk;
                for (int i = 0; i <= totalChunks; i++)
                {
                    int chunkStartingPosition = i * chunk;
                    int chunkArraySize = (int)Math.Min(file.Length - chunkStartingPosition, chunk); //  either read the rest which is filesize - chunkStartingPosition or read a full chunk dependent on what is smaller
                    int lastArrayIndex = chunkStartingPosition + chunkArraySize - 1;
                    byte[] buffer = new byte[chunkArraySize];
                    await file.ReadAsync(buffer, 0, chunkArraySize);

                    var contentPiece = new ByteArrayContent(buffer, 0, chunkArraySize);

                    contentPiece.Headers.ContentRange = new ContentRangeHeaderValue(chunkStartingPosition, lastArrayIndex, file.Length);
                    var newClient = new HttpClient();
                    var uploadResponse = await newClient.PutAsync(createUploadSessionResponseContent.UploadUrl, contentPiece);

                    if (!uploadResponse.IsSuccessStatusCode)
                    {
                        actionResult.Add(new ActionResult(ErrorCode.Outlook_Web_Upload_Error));
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, "Error while uploading attachments to Outlook.", e);
                actionResult.Add(new ActionResult(ErrorCode.Outlook_Web_Upload_Error));
                return false;
            }
            finally
            {
                file.Close();
            }

            return true;
        }

        private static CreateUploadSessionRequest CreateUploadSessionMessage(FileInfo fInfo, FileStream file)
        {
            return new CreateUploadSessionRequest
            {
                AttachmentItem = new GraphMailAttachmentItem()
                {
                    AttachmentType = "file",
                    Name = fInfo.Name,
                    Size = file.Length
                }
            };
        }

        public override void ApplyPreSpecifiedTokens(Job job)
        {
            _mailHelper.ReplaceTokensInMailSettings(job, job.Profile.EmailWebSettings);
        }

        public override ActionResult Check(ConversionProfile profile, CurrentCheckSettings settings, CheckLevel checkLevel)
        {
            var result = new ActionResult();

            if (settings.Accounts.MicrosoftAccounts.Count == 0)
            {
                return new ActionResult(ErrorCode.Outlook_Web_Account_Missing);
            }

            if (checkLevel == CheckLevel.RunningJob)
            {
                foreach (var attachmentFile in profile.EmailWebSettings.AdditionalAttachments)
                {
                    if (!_file.Exists(attachmentFile))
                    {
                        Logger.Error("Can't find web mail attachment " + attachmentFile + ".");
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
