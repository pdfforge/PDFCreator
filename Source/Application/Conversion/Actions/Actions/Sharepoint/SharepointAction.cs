using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Helper.Microsoft;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Interface;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Mail;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.IO;
using pdfforge.PDFCreator.Utilities.Tokens;
using pdfforge.PDFCreator.Utilities.Web;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions.Sharepoint;

public class SharepointAction : ActionBase<SharepointSettings>, IPostConversionAction, IBusinessFeatureAction
{
    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    private readonly IGraphManager _graphManager;
    private readonly IUniqueFilenameFactory _uniqueFilenameFactory;
    private readonly IWebLinkLauncher _webLinkLauncher;
    private readonly IPathUtil _pathUtil;
    private readonly MicrosoftActionHelper _microsoftActionHelper;
    private HttpClient _httpClient;

    public SharepointAction(IGraphManager graphManager, IUniqueFilenameFactory uniqueFilenameFactory, MicrosoftActionHelper microsoftActionHelper, IWebLinkLauncher webLinkLauncher, IPathUtil pathUtil) : 
        base(p => p.SharepointSettings)
    {
        _graphManager = graphManager;
        _uniqueFilenameFactory = uniqueFilenameFactory;
        _microsoftActionHelper = microsoftActionHelper;
        _webLinkLauncher = webLinkLauncher;
        _pathUtil = pathUtil;
    }

    protected override ActionResult DoProcessJob(Job job, IPdfProcessor processor)
    {
        _httpClient = new HttpClient();
        _logger.Debug("Starting Sharepoint Action");

        var settings = new CurrentCheckSettings(job.AvailableProfiles, job.PrinterMappings, job.Accounts);
        var actionResult = Check(job.Profile, settings, CheckLevel.RunningJob);
        if (!actionResult)
            return actionResult;

        var account = job.Accounts.GetSharepointAccount(job.Profile);
        var ensureUniqueFilenames = job.Profile.SharepointSettings.EnsureUniqueFilenames;
        var authenticationResult = _graphManager.GetAccessToken(account).GetAwaiter().GetResult();
        var destinationFolder = GetDestinationFolder(job);

        var (success, privateUrl) = ProcessJobFiles(job, destinationFolder, authenticationResult.AccessToken, ensureUniqueFilenames).GetAwaiter().GetResult();
        if(!success)
            actionResult.Add(new ActionResult(ErrorCode.Sharepoint_Upload_Failed));

        job.ShareLinks.SharepointPrivateUrl = privateUrl;
        

        if (job.Profile.SharepointSettings.OpenUploadedFile)
        {
            _webLinkLauncher.Launch(job.ShareLinks.SharepointPrivateUrl);
        }

        _httpClient.Dispose();
        return actionResult;
    }

    private async Task<(bool success, string privatePath)> ProcessJobFiles(Job job, string destinationFolder, string accessToken, bool ensureUniqueFilenames)
    {
        var privatePath = "";
        var success = true;
        var sharepointSettingsDriveId = job.Profile.SharepointSettings.DriveId;

        foreach (var filePath in job.OutputFiles)
        {
            var fileName = Path.GetFileName(filePath);
            fileName = _pathUtil.GetCleanFileNameWithoutUniqueCounter(fileName, job.OutputFileTemplate);

            if (ensureUniqueFilenames)
                fileName = await EnsureUniqueFileNames(job, fileName, destinationFolder, accessToken);

            var uploadSessionUrl = $"{GraphManager.BaseURL}/drives/{sharepointSettingsDriveId}/root:/{destinationFolder}/{fileName}:/createUploadSession";
            var uploadResult = await _microsoftActionHelper.UploadFile(uploadSessionUrl, filePath, accessToken);

            if (uploadResult == null)
            {
                success = false;
                continue;
            }

            privatePath = await GetItemWebUrl(uploadResult.ParentReference, accessToken);
        }

        return (success, privatePath);
    }
     
    private async Task<string> GetItemWebUrl(ItemReference itemReference, string accessToken)
    {
        var itemUrl = $"{GraphManager.BaseURL}/drives/{itemReference.DriveId}/items/{itemReference.Id}";
        var request = new HttpRequestMessage(HttpMethod.Get, itemUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        var resultString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<FileUploadResult>(resultString);
        return result.WebUrl;
    }


    private string GetDestinationFolder(Job job)
    {
        var destinationFolder = job.Profile.SharepointSettings.SharedFolder;
        
        if (job.OutputFiles.Count > 1)
        {
            var nestedFolderName = Path.GetFileNameWithoutExtension(job.OutputFileTemplate);
            destinationFolder = $"{destinationFolder}/{nestedFolderName}";
        }
        
        return destinationFolder;
    }

    private async Task<string> EnsureUniqueFileNames(Job job, string fileName, string destinationFolder, string accessToken)
    {
        var fileExists = await DoesFileExistOnDrive(job, fileName, destinationFolder, accessToken);

        if (!fileExists) 
            return fileName;

        var uniquePath = _uniqueFilenameFactory.Build(fileName);
        return uniquePath.CreateUniqueFileName(UniqueFilenameCondition);

        bool UniqueFilenameCondition(string s)
        {
            return DoesFileExistOnDrive(job,s, destinationFolder, accessToken).GetAwaiter().GetResult();
        }
    }

    private async Task<bool> DoesFileExistOnDrive(Job job, string fileName, string destinationFolder, string accessToken)
    {
        var sharepointSettingsDriveId = job.Profile.SharepointSettings.DriveId;
        var requestUrl = GraphManager.BaseURL + $"/drives/{sharepointSettingsDriveId}/root:/{destinationFolder}/{fileName}";
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request);

        return response.StatusCode != HttpStatusCode.NotFound;
    }

    public override void ApplyPreSpecifiedTokens(Job job)
    {
        job.Profile.SharepointSettings.SharedFolder = job.TokenReplacer.ReplaceTokens(job.Profile.SharepointSettings.SharedFolder).Replace("\\", "/");
    }

    public override bool IsRestricted(ConversionProfile profile)
    {
        return false;
    }

    protected override void ApplyActionSpecificRestrictions(Job job)
    { }

    public override ActionResult Check(ConversionProfile profile, CurrentCheckSettings settings, CheckLevel checkLevel)
    {
        var result = new ActionResult();

        var account = settings.Accounts.GetSharepointAccount(profile);
        if (account == null)
        {
            result.Add(ErrorCode.Microsoft_Account_Missing);
            return result;
        }

        if (!account.HasPermissions(MicrosoftAccountPermission.FilesReadWriteAll))
            result.Add(ErrorCode.Sharepoint_MissingPermissions);
        else if (account.HasExpiredPermissions(DateTime.Now))
            result.Add(ErrorCode.Microsoft_Account_Expired);

        if (checkLevel == CheckLevel.EditingProfile)
        {
            if (TokenIdentifier.ContainsUserToken(profile.SharepointSettings.SharedFolder))
            {
                if (!profile.UserTokens.Enabled)
                    result.Add(ErrorCode.Sharepoint_SharedFolder_RequiresUserTokenAction);
                
                return result;
            }

            if (TokenIdentifier.ContainsTokens(profile.SharepointSettings.SharedFolder))
                return result;
        }

        if(string.IsNullOrEmpty(profile.SharepointSettings.AccountId))
            result.Add(ErrorCode.Sharepoint_Missing_Account);

        if (string.IsNullOrEmpty(profile.SharepointSettings.SiteId))
            result.Add(ErrorCode.Sharepoint_Missing_Site);

        if (string.IsNullOrEmpty(profile.SharepointSettings.DriveId))
            result.Add(ErrorCode.Sharepoint_Missing_Drive);

        if (!ValidName.IsValidWebPath(profile.SharepointSettings.SharedFolder))
            result.Add(ErrorCode.Sharepoint_InvalidSharedFolder);

        return result;
    }
}
