using Newtonsoft.Json;
using NLog;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Dropbox;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews
{
    public interface IDropboxUserInfoManager
    {
        Task<DropboxUserInfo> GetDropboxUserInfo();
    }

    public class DropboxUserInfoManager : IDropboxUserInfoManager
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IDropboxService _dropboxService;
        private readonly DropboxAppData _dropboxAppData;
        private readonly IDropboxHttpListener _dropboxHttpListener;
        private readonly IDropboxCodeExchanger _dropboxCodeExchanger;

        public DropboxUserInfoManager(IDropboxService dropboxService, DropboxAppData dropboxAppData,
            IDropboxHttpListener dropboxHttpListener, IDropboxCodeExchanger dropboxCodeExchanger)
        {
            _dropboxService = dropboxService;
            _dropboxAppData = dropboxAppData;
            _dropboxHttpListener = dropboxHttpListener;
            _dropboxCodeExchanger = dropboxCodeExchanger;
        }

        private async Task<T> DoOrRetryPortSelectionAsync<T>(IEnumerable<int> portList, Func<int, Task<T>> action)
        {
            using (var portEnumerator = portList.GetEnumerator())
            {
                portEnumerator.MoveNext();

                while (true)
                {
                    try
                    {
                        return await action(portEnumerator.Current);
                    }
                    catch (DropboxAccessDeniedException)
                    {
                        throw;
                    }
                    catch (Exception)
                    {
                        if (!portEnumerator.MoveNext())
                            throw;
                    }
                }
            }
        }

        private async Task<string> GetDropboxToken()
        {
            var portsList = _dropboxAppData.PortsList;
            var appKey = _dropboxAppData.AppKey;
            var (challenge, verifier) = GenerateChallengeAndVerifier();
            var code_challenge_method = "S256";
            var redirectUri = "";

            var code = await DoOrRetryPortSelectionAsync(portsList, async port =>
            {
                redirectUri = $"http://{IPAddress.Loopback }:{port}/oauth2/redirect_receiver";
                var authUri = _dropboxService.GetAuthorizeUri(appKey, redirectUri);
                var authUrl = $"{authUri}&code_challenge={challenge}&code_challenge_method={code_challenge_method}";

                return await _dropboxHttpListener.StartAsync(authUrl, port);
            });

            var token = await _dropboxCodeExchanger.ExchangeCodeTokenAsync(code, verifier, appKey, redirectUri);
            return token;
        }

        public async Task<DropboxUserInfo> GetDropboxUserInfo()
        {
            try
            {
                var token = await GetDropboxToken();
                var tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(token);
                var dbUserInfo = _dropboxService.GetDropUserInfo(tokenEndpointDecoded["access_token"], tokenEndpointDecoded["refresh_token"]);
                return dbUserInfo;
            }
            catch (DropboxAuthException ex)
            {
                _logger.Error(ex, "An error in DropboxUserInfoManager occured.");
                throw;
            }
        }

        private (string challenge, string verifier) GenerateChallengeAndVerifier()
        {
            var randomNumberGenerator = RandomNumberGenerator.Create();

            var bytes = new byte[32];
            randomNumberGenerator.GetBytes(bytes);

            var verifier = Convert.ToBase64String(bytes)
               .TrimEnd('=')
               .Replace('+', '-')
               .Replace('/', '_');

            using (var sha256 = SHA256.Create())
            {
                var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(verifier));
                var challenge = Convert.ToBase64String(challengeBytes)
                    .TrimEnd('=')
                    .Replace('+', '-')
                    .Replace('/', '_');

                return (challenge, verifier);
            }
        }
    }
}
