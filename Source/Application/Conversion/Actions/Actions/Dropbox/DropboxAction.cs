using NLog;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Tokens;
using System.Linq;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions.Dropbox
{
    public class DropboxAction : IPostConversionAction, ICheckable
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IDropboxService _dropboxService;

        public DropboxAction(IDropboxService dropboxService)
        {
            _dropboxService = dropboxService;
        }

        public ActionResult ProcessJob(Job job)
        {
            _logger.Debug("Launched Dropbox Action");

            ApplyPreSpecifiedTokens(job);
            var actionResult = Check(job.Profile, job.Accounts, CheckLevel.Job);
            if (!actionResult)
                return actionResult;

            var sharedLink = job.Profile.DropboxSettings.CreateShareLink;
            var shareFolder = job.TokenReplacer.ReplaceTokens(job.Profile.DropboxSettings.SharedFolder);

            var currentDropBoxAccount = job.Accounts.GetDropboxAccount(job.Profile);
            if (sharedLink)
            {
                try
                {
                    var shareLink = _dropboxService.UploadFileWithSharing(
                        currentDropBoxAccount.AccessToken,
                        shareFolder, job.OutputFiles,
                        job.Profile.DropboxSettings.EnsureUniqueFilenames, job.OutputFileTemplate);
                    if (shareLink == null)
                    {
                        return new ActionResult(ErrorCode.Dropbox_Upload_And_Share_Error);
                    }

                    job.ShareLinks.DropboxShareUrl = shareLink.ShareUrl;

                    shareLink.Filename = shareLink.Filename.Split('/').Last();
                    job.TokenReplacer.AddToken(new StringToken("DROPBOXFULLLINKS", $"{shareLink.Filename} ( {shareLink.ShareUrl} )"));
                    job.TokenReplacer.AddToken(new StringToken("DROPBOXHTMLLINKS", $"<a href = '{shareLink.ShareUrl}'>{shareLink.Filename}</a>"));
                }
                catch
                {
                    return new ActionResult(ErrorCode.Dropbox_Upload_And_Share_Error);
                }
            }
            else
            {
                var result = _dropboxService.UploadFiles(currentDropBoxAccount.AccessToken, shareFolder, job.OutputFiles, job.Profile.DropboxSettings.EnsureUniqueFilenames, job.OutputFileTemplate);
                if (result == false)
                {
                    return new ActionResult(ErrorCode.Dropbox_Upload_Error);
                }
            }

            return new ActionResult();
        }

        public bool IsEnabled(ConversionProfile profile)
        {
            return profile.DropboxSettings.Enabled;
        }

        public void ApplyPreSpecifiedTokens(Job job)
        {
            job.Profile.DropboxSettings.SharedFolder = job.TokenReplacer.ReplaceTokens(job.Profile.DropboxSettings.SharedFolder)
                                                                        .Replace("\\", "/");
        }

        public ActionResult Check(ConversionProfile profile, Accounts accounts, CheckLevel checkLevel)
        {
            if (!IsEnabled(profile))
                return new ActionResult();

            var isJobLevelCheck = checkLevel == CheckLevel.Job;

            var account = accounts.GetDropboxAccount(profile);

            if (account == null)
                return new ActionResult(ErrorCode.Dropbox_AccountNotSpecified);

            var accessToken = account.AccessToken;

            if (string.IsNullOrEmpty(accessToken))
                return new ActionResult(ErrorCode.Dropbox_AccessTokenNotSpecified);

            if (!isJobLevelCheck && TokenIdentifier.ContainsTokens(profile.DropboxSettings.SharedFolder))
                return new ActionResult();

            if (!ValidName.IsValidDropboxFolder(profile.DropboxSettings.SharedFolder))
                return new ActionResult(ErrorCode.Dropbox_InvalidFolderName);

            return new ActionResult();
        }
    }
}
