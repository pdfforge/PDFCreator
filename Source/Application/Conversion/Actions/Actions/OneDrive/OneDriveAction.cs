using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Helper.Microsoft;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Mail;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.IO;
using pdfforge.PDFCreator.Utilities.Tokens;
using pdfforge.PDFCreator.Utilities.Web;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions.OneDrive;

public class OneDriveAction : ActionBase<OneDriveSettings>, IPostConversionAction
{
    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    private readonly IGraphManager _graphManager;
    private readonly IUniqueFilenameFactory _uniqueFilenameFactory;
    private readonly IWebLinkLauncher _webLinkLauncher;
    private readonly IPathUtil _pathUtil;
    private readonly MicrosoftActionHelper _microsoftActionHelper;
    private HttpClient _httpClient;

    public OneDriveAction(IGraphManager graphManager, IUniqueFilenameFactory uniqueFilenameFactory, MicrosoftActionHelper microsoftActionHelper, IWebLinkLauncher webLinkLauncher, IPathUtil pathUtil) : 
        base(p => p.OneDriveSettings)
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
        _logger.Debug("Starting OneDrive Action");

        var settings = new CurrentCheckSettings(job.AvailableProfiles, job.PrinterMappings, job.Accounts);
        var actionResult = Check(job.Profile, settings, CheckLevel.RunningJob);
        if (!actionResult)
            return actionResult;

        var account = job.Accounts.GetMicrosoftAccount(job.Profile);
        var createShareLink = job.Profile.OneDriveSettings.CreateShareLink;
        var ensureUniqueFilenames = job.Profile.OneDriveSettings.EnsureUniqueFilenames;
        var authenticationResult = _graphManager.GetAccessToken(account).GetAwaiter().GetResult();
        var destinationFolder = GetDestinationFolder(job);

        var (success, privateUrl, sharePath) = ProcessJobFiles(job, destinationFolder, authenticationResult.AccessToken, ensureUniqueFilenames).GetAwaiter().GetResult();
        if(!success)
            actionResult.Add(new ActionResult(ErrorCode.OneDrive_Upload_Failed));

        if (createShareLink)
        {
            var shareLink = CreateShareLink(job, sharePath, authenticationResult.AccessToken).GetAwaiter().GetResult();
            if (string.IsNullOrWhiteSpace(shareLink))
                return new ActionResult(ErrorCode.OneDrive_CouldNotCreateShareLink);
        }
        else
        {
            job.ShareLinks.OneDrivePrivateUrl = privateUrl;
        }

        if (job.Profile.OneDriveSettings.OpenUploadedFile)
        {
            _webLinkLauncher.Launch(string.IsNullOrEmpty(job.ShareLinks.OneDriveShareUrl) ? job.ShareLinks.OneDrivePrivateUrl : job.ShareLinks.OneDriveShareUrl);
        }

        _httpClient.Dispose();
        return actionResult;
    }

    private async Task<(bool success, string privatePath, string sharePath)> ProcessJobFiles(Job job, string destinationFolder, string accessToken, bool ensureUniqueFilenames)
    {
        var privatePath = "";
        var sharePath = "";
        var success = true;
        foreach (var filePath in job.OutputFiles)
        {
            var fileName = Path.GetFileName(filePath);
            fileName = _pathUtil.GetCleanFileNameWithoutUniqueCounter(fileName, job.OutputFileTemplate);

            if (ensureUniqueFilenames)
                fileName = await EnsureUniqueFileNames(fileName, job.OutputFileTemplate, destinationFolder, accessToken);

            var uploadSessionUrl = $"{GraphManager.BaseURL}/me/drive/root:/{destinationFolder}/{fileName}:/createUploadSession";
            var uploadResult = await _microsoftActionHelper.UploadFile(uploadSessionUrl, filePath, accessToken);

            if (uploadResult == null)
            {
                success = false;
                continue;
            }

            //first WebUrl for private link
            if (string.IsNullOrEmpty(privatePath))
            {
                privatePath = await GetItemWebUrl(uploadResult.ParentReference, accessToken);
            }

            //first (unique) filepath for share link 
            if (string.IsNullOrEmpty(sharePath))
            {
                sharePath = $"{destinationFolder}/{fileName}";
                if (job.OutputFiles.Count > 1)
                    sharePath = $"{destinationFolder}";
            }
        }

        return (success, privatePath, sharePath);
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

    private async Task<string> CreateShareLink(Job job, string sharePath, string accessToken)
    {
        var shareLink = await GetShareLink(sharePath, accessToken);

        if (!string.IsNullOrWhiteSpace(shareLink))
        {
            job.ShareLinks.OneDriveShareUrl = shareLink;
            job.TokenReplacer.AddToken(new StringToken(TokenNames.OneDriveShareLink, $"{shareLink}"));
            job.TokenReplacer.AddToken(new StringToken(TokenNames.OneDriveShareLinkHtml, $"<a href = '{shareLink}'>{shareLink}</a>"));
        }

        return shareLink;
    }

    private string GetDestinationFolder(Job job)
    {
        var destinationFolder = job.Profile.OneDriveSettings.SharedFolder;
        
        if (job.OutputFiles.Count > 1)
        {
            var nestedFolderName = Path.GetFileNameWithoutExtension(job.OutputFileTemplate);
            destinationFolder = $"{destinationFolder}/{nestedFolderName}";
        }
        
        return destinationFolder;
    }

    private async Task<string> EnsureUniqueFileNames(string fileName, string outputFilenameTemplate, string destinationFolder, string accessToken)
    {
        var fileExists = await DoesFileExistOnOneDrive(fileName, destinationFolder, accessToken);

        if (!fileExists) 
            return fileName;

        var uniquePath = _uniqueFilenameFactory.Build(fileName);
        return uniquePath.CreateUniqueFileName(UniqueFilenameCondition);

        bool UniqueFilenameCondition(string s)
        {
            return DoesFileExistOnOneDrive(s, destinationFolder, accessToken).GetAwaiter().GetResult();
        }
    }

    private async Task<bool> DoesFileExistOnOneDrive(string fileName, string destinationFolder, string accessToken)
    {
        var requestUrl = GraphManager.BaseURL + $"/me/drive/root:/{destinationFolder}/{fileName}";
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request);

        return response.StatusCode != HttpStatusCode.NotFound;
    }

    private async Task<string> GetShareLink(string itemPath, string accessToken)
    {
        var itemId = await GetItemIdFromPath(itemPath, accessToken);
        using var httpClient = new HttpClient();

        var relativeApiUrl = $"/me/drive/items/{itemId}/createLink";
        var requestMessage = GetRequestMessage(accessToken, relativeApiUrl);

        var response = await httpClient.SendAsync(requestMessage);

        if (!response.IsSuccessStatusCode)
            return "";

        return await GetShareLinkFromResponse(response);
    }

    private async Task<string> GetItemIdFromPath(string itemPath, string accessToken)
    {
        var url = GraphManager.BaseURL + $"/me/drive/root:/{itemPath}";
        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(httpRequest);

        return await GetItemIdFromResponse(response);
    }
    
    private static HttpRequestMessage GetRequestMessage(string accessToken, string relativeApiUrl)
    {
        var requestUrl = new Uri(GraphManager.BaseURL + relativeApiUrl);
        var httpRequest = new HttpRequestMessage();
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        httpRequest.RequestUri = requestUrl;
        httpRequest.Method = HttpMethod.Post;
        httpRequest.Content = new StringContent(
            JsonConvert.SerializeObject(
                new
                {
                    Type = "view", 
                    Scope = "anonymous"
                }), 
            Encoding.UTF8, 
            mediaType: "application/json");
        
        return httpRequest;
    }

    private static async Task<string> GetItemIdFromResponse(HttpResponseMessage response)
    {
        var responseContent = await response.Content.ReadAsStringAsync();
        var jsonObject = (JObject) JsonConvert.DeserializeObject(responseContent);
        return jsonObject?["id"]?.ToString();
    }

    private static async Task<string> GetShareLinkFromResponse(HttpResponseMessage response)
    {
        var responseContent = await response.Content.ReadAsStringAsync();
        var jsonObject = (JObject)JsonConvert.DeserializeObject(responseContent);
        return jsonObject?["link"]?["webUrl"]?.ToString();
    }

    public override void ApplyPreSpecifiedTokens(Job job)
    {
        job.Profile.OneDriveSettings.SharedFolder = job.TokenReplacer.ReplaceTokens(job.Profile.OneDriveSettings.SharedFolder).Replace("\\", "/");
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

        var account = settings.Accounts.GetMicrosoftAccount(profile);
        if (account == null)
        {
            result.Add(ErrorCode.Microsoft_Account_Missing);
            return result;
        }

        if (!account.HasPermissions(MicrosoftAccountPermission.FilesReadWrite))
            result.Add(ErrorCode.OneDrive_MissingPermissions);
        else if (account.GetExpirationDateTime() < DateTime.Now)
            result.Add(ErrorCode.Microsoft_Account_Expired);

        if (checkLevel == CheckLevel.EditingProfile)
        {
            if (TokenIdentifier.ContainsUserToken(profile.OneDriveSettings.SharedFolder))
            {
                if (!profile.UserTokens.Enabled)
                    result.Add(ErrorCode.OneDrive_SharedFolder_RequiresUserTokenAction);
                
                return result;
            }

            if (TokenIdentifier.ContainsTokens(profile.OneDriveSettings.SharedFolder))
                return result;
        }

        if (!ValidName.IsValidWebPath(profile.OneDriveSettings.SharedFolder))
            result.Add(ErrorCode.OneDrive_InvalidSharedFolder);

        return result;
    }
}
