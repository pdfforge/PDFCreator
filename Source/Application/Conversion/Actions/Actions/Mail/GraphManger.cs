using System;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Abstractions;
using pdfforge.PDFCreator.Conversion.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions.Mail
{
    public interface IGraphManager
    {
        Task AcquireAccessToken(MicrosoftAccount microsoftAccount, List<MicrosoftAccountPermission> permissions);
        Task<AuthenticationResult> GetAccessToken(MicrosoftAccount account);
        ClientWrapper GetClient(MicrosoftAccount currentAccount);
    }

    public class GraphManager : IGraphManager
    {
        private const string AuthRecordCachePath = "http://localhost/";
        private const string ClientId = "26528e78-9272-4506-b396-20ecb072d10b";
        public const string BaseURL = "https://graph.microsoft.com/v1.0";

        private readonly List<ClientWrapper> _clients = new List<ClientWrapper>();

        public GraphManager()
        {
            // needed to load assembly
            var _ = new LogEntry();
        }

        public ClientWrapper GetClient(MicrosoftAccount account)
        {
            var fittingClients = _clients.Where(wrapper => 
                wrapper.Account.AccountId == account.AccountId 
                && wrapper.Account.Actions == account.Actions);

            if (fittingClients.Any())
            {
                return fittingClients.First();
            }

            var client = PublicClientApplicationBuilder
                .Create(ClientId)
                .WithRedirectUri(AuthRecordCachePath)
                .Build();
            
            var clientWrapper = new ClientWrapper(client, account);
            _clients.Add(clientWrapper);
            return clientWrapper;
        }

        public async Task<AuthenticationResult> GetAccessToken(MicrosoftAccount account)
        {
            var clientWrapper = GetClient(account);
            var client = clientWrapper.Client;

            var accountsAsync = (await client.GetAccountsAsync()).FirstOrDefault();

            var permissionList = account.PermissionScopes.Split(',').ToList();
            var authenticationResult = await client.AcquireTokenSilent(permissionList, accountsAsync).ExecuteAsync();

            var newOffset =  (new DateTimeOffset(DateTime.Now.AddDays(90))).ToUnixTimeSeconds();
            clientWrapper.Account.ExpirationDate = newOffset;
            return authenticationResult;
        }

        public async Task AcquireAccessToken(MicrosoftAccount microsoftAccount, List<MicrosoftAccountPermission> permissions)
        {
            ClientWrapper clientWrapper;
            IPublicClientApplication client;

            clientWrapper = GetClient(microsoftAccount);
            client = clientWrapper.Client;
            var permissionScope = permissions.Select(permission => permission.ToPermissionString()).ToList();
            permissionScope.Add("offline_access");

            if (string.IsNullOrEmpty(microsoftAccount.AccountId))
            {
                var authenticationResult = await client.AcquireTokenInteractive(permissionScope).ExecuteAsync();
            }
            else
            {
                var account =  await client.GetAccountAsync(microsoftAccount.AccountId);
                var authenticationResult = await client.AcquireTokenInteractive(permissionScope).WithAccount(account).WithPrompt(Prompt.Consent).ExecuteAsync();
            }

            var newOffset = (new DateTimeOffset(DateTime.Now.AddDays(90))).ToUnixTimeSeconds();
            clientWrapper.Account.ExpirationDate = newOffset;
        }
    }
}
