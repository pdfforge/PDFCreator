using System;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Abstractions;
using pdfforge.PDFCreator.Conversion.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions.Mail
{
    public interface IGraphManager
    {
        Task AcquireAccessToken(MicrosoftAccount account, List<MicrosoftAccountPermission> permissions);
        Task<AuthenticationResult> GetAccessToken(MicrosoftAccount account);
        ClientWrapper GetClient(MicrosoftAccount currentAccount);
    }

    public class GraphManager : IGraphManager
    {
        private const string AuthRecordCachePath = "http://localhost/";
        private const string ClientId = "f1d4e9fa-243b-496f-80aa-5d6baa4cf379";
        public const string BaseURL = "https://graph.microsoft.com/v1.0";

        private readonly List<ClientWrapper> _clients = new List<ClientWrapper>();

        public GraphManager()
        {
            // needed to load assembly
            var _ = new LogEntry();
        }

        public ClientWrapper GetClient(MicrosoftAccount account)
        {
            var fittingClients = _clients.Where(wrapper => wrapper.Account.AccountId == account.AccountId);
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

        public async Task AcquireAccessToken(MicrosoftAccount account, List<MicrosoftAccountPermission> permissions)
        {
            ClientWrapper clientWrapper;
            IPublicClientApplication client;

            clientWrapper = GetClient(account);
            client = clientWrapper.Client;
            var permissionScope = permissions.Select(permission => permission.ToPermissionString()).ToList();
            permissionScope.Add("offline_access");
            var authenticationResult = await client.AcquireTokenInteractive(permissionScope).ExecuteAsync();
            var newOffset = (new DateTimeOffset(DateTime.Now.AddDays(90))).ToUnixTimeSeconds();
            clientWrapper.Account.ExpirationDate = newOffset;
        }
    }
}
