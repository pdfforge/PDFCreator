using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Microsoft.Office.Interop.Outlook;
using Newtonsoft.Json;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Helper.Microsoft;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Mail;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions.Helper.Sharepoint
{
    public class SharepointHelper
    {
        private readonly MicrosoftActionHelper _microsoftHelper;
        private readonly IGraphManager _graphManager;


        private async Task<HttpClient> SetupHttpClient(MicrosoftAccount account)
        {
            var authenticationResult = await _graphManager.GetAccessToken(account);
            var httpClient = new HttpClient();
            _microsoftHelper.SetupHttpClient(httpClient, authenticationResult.AccessToken);
            return httpClient;
        }
        
        public SharepointHelper(MicrosoftActionHelper microsoftHelper, IGraphManager graphManager)
        {
            _microsoftHelper = microsoftHelper;
            _graphManager = graphManager;
        }

        public async Task<List<SharepointSite>> GetSiteFor(MicrosoftAccount account)
        {
            var httpClient = await SetupHttpClient(account);
            var result = await httpClient.GetAsync(GetAllSitesRequest.RequestURL());
            var s = await result.Content.ReadAsStringAsync();
            var deserializedGetSiteForResult = JsonSerializer.Create().Deserialize<GetAllSitesRequest>(new JsonTextReader(new StringReader(s)));

            return deserializedGetSiteForResult?.value ?? new List<SharepointSite>();
        }

        public async Task<List<SharepointDrive>> GetDrivesForSite(MicrosoftAccount account, SharepointSite site)
        {
            var httpClient = await SetupHttpClient(account);
            var result = await httpClient.GetAsync(GetAllDrivesForSiteRequest.RequestURL(site));
            var s = await result.Content.ReadAsStringAsync();
            var deserializeGetAllDrivesForSiteResult = JsonSerializer.Create().Deserialize<GetAllDrivesForSiteRequest>(new JsonTextReader(new StringReader(s)));
            return deserializeGetAllDrivesForSiteResult?.value ?? new List<SharepointDrive>();
        }
    }

}
