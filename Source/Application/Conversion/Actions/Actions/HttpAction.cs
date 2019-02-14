using NLog;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions
{
    public class HttpAction : RetypePasswordActionBase, IPostConversionAction, ICheckable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected override string PasswordText => "HTTP";

        public override void ApplyPreSpecifiedTokens(Job job)
        {
            //nothing to do here
        }

        /// <summary>
        ///     Check if the profile is configured properly for this action
        /// </summary>
        /// <param name="profile">The profile to check</param>
        /// <param name="accounts">Current accounts</param>
        /// <param name="checkLevel"></param>
        /// <returns>ActionResult with configuration problems</returns>
        public override ActionResult Check(ConversionProfile profile, Accounts accounts, CheckLevel checkLevel)
        {
            var actionResult = new ActionResult();

            if (!IsEnabled(profile))
                return actionResult;

            var httpAccount = accounts.GetHttpAccount(profile);
            if (httpAccount == null)
            {
                actionResult.Add(ErrorCode.HTTP_NoAccount);
                return actionResult;
            }

            Uri isValidUrl;
            if (!Uri.TryCreate(httpAccount.Url, UriKind.Absolute, out isValidUrl))
            {
                actionResult.Add(ErrorCode.HTTP_NoUrl);
            }
            else if (isValidUrl.Scheme != Uri.UriSchemeHttp && isValidUrl.Scheme != Uri.UriSchemeHttps)
            {
                actionResult.Add(ErrorCode.HTTP_MustStartWithHttp);
            }

            if (httpAccount.IsBasicAuthentication)
            {
                if (string.IsNullOrWhiteSpace(httpAccount.UserName))
                    actionResult.Add(ErrorCode.HTTP_NoUserNameForAuth);

                if (profile.AutoSave.Enabled && string.IsNullOrWhiteSpace(httpAccount.Password))
                    actionResult.Add(ErrorCode.HTTP_NoPasswordForAuthWithAutoSave);
            }

            return actionResult;
        }

        protected override ActionResult DoActionProcessing(Job job)
        {
            Logger.Debug("Launched httpRequest-Action");
            try
            {
                return HttpUpload(job);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception while upload file to http:\r\n" + ex.Message);

                return new ActionResult(ErrorCode.HTTP_Generic_Error);
            }
        }

        private ActionResult HttpUpload(Job job)
        {
            var result = new ActionResult();

            // setup and send request
            var uploadFileViaHttp = UploadFileViaHttp(job);

            // wait for the result
            var httpResponse = uploadFileViaHttp;
            if (httpResponse.IsSuccessStatusCode == false)
            {
                switch (httpResponse.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        result.Add(ErrorCode.PasswordAction_Login_Error);
                        break;

                    default:
                        result.Add(ErrorCode.HTTP_Generic_Error);
                        break;
                }
            }
            return result;
        }

        private HttpResponseMessage UploadFileViaHttp(Job job)
        {
            try
            {
                var multiContent = new MultipartContent();
                foreach (var jobOutputFile in job.OutputFiles)
                {
                    var fileStream = new FileStream(jobOutputFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    var outputFile = new StreamContent(fileStream);
                    var mimeMapping = MimeMapping.GetMimeMapping(jobOutputFile);
                    outputFile.Headers.ContentType = new MediaTypeHeaderValue(mimeMapping);
                    outputFile.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
                    outputFile.Headers.ContentDisposition.FileName = Path.GetFileName(jobOutputFile);
                    multiContent.Add(outputFile);
                }

                // do the Post request and wait for an answer
                return MakePostRequest(job, multiContent).Result;
            }
            catch (Exception e)
            {
                Logger.Error($"Exception during HTTP upload:\r\n{e.Message}");
                throw;
            }
        }

        private async Task<HttpResponseMessage> MakePostRequest(Job job, HttpContent message)
        {
            var account = job.Accounts.GetHttpAccount(job.Profile);
            var httpClient = new HttpClient();
            var timeout = account.Timeout;

            if (timeout < 0)
                timeout = 60;

            httpClient.Timeout = TimeSpan.FromSeconds(timeout);
            var uri = new Uri(account.Url);
            if (account.IsBasicAuthentication)
            {
                var asciiAuth = Encoding.ASCII.GetBytes($"{account.UserName}:{job.Passwords.HttpPassword}");
                var endcoded = Convert.ToBase64String(asciiAuth);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", endcoded);
            }
            return await httpClient.PostAsync(uri, message).ConfigureAwait(false);
        }

        protected override void SetPassword(Job job, string password)
        {
            job.Passwords.HttpPassword = password;
        }

        public override bool IsEnabled(ConversionProfile profile)
        {
            return profile.HttpSettings.Enabled;
        }
    }
}
