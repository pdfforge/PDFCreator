using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews
{
    public interface IDropboxCodeExchanger
    {
        Task<string> ExchangeCodeTokenAsync(string code, string codeVerifier, string appKey, string redirectUri);
    }

    public class DropboxCodeExchanger : IDropboxCodeExchanger
    {
        public async Task<string> ExchangeCodeTokenAsync(string code, string codeVerifier, string appKey, string redirectUri)
        {
            using (var httpClient = new HttpClient())
            {
                var apiUri = "https://api.dropbox.com/oauth2/token";
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), apiUri))
                {
                    var contentList = new List<string>
                    {
                        $"code={code}",
                        "grant_type=authorization_code",
                        $"code_verifier={codeVerifier}",
                        $"client_id={appKey}",
                        $"redirect_uri={redirectUri}"
                    };

                    request.Content = new StringContent(string.Join("&", contentList));
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                    var response = await httpClient.SendAsync(request);
                    var content = response.Content.ReadAsStringAsync();

                    return response.StatusCode == HttpStatusCode.OK ? content.Result : throw new DropboxAuthException($"An error occured while exchanging code for token. StatusCode={response.StatusCode}");
                }
            }
        }
    }
}
