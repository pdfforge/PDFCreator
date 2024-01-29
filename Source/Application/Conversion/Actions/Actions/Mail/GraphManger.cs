using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Abstractions;
using pdfforge.PDFCreator.Conversion.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions.Mail
{
    public interface IGraphManager
    {
        Task AddMicrosoftAccount(MicrosoftAccount account);

        Task<AuthenticationResult> GetAccessToken(MicrosoftAccount account);
    }

    public class GraphManager : IGraphManager
    {
        private const string AuthRecordCachePath = "http://localhost/";
        private const string ClientId = "f1d4e9fa-243b-496f-80aa-5d6baa4cf379";
        private static readonly string[] GraphUserScopes = new[] { "mail.readwrite", "offline_access" };
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
            return await client.AcquireTokenSilent(GraphUserScopes, accountsAsync).ExecuteAsync();
        }

        public async Task AddMicrosoftAccount(MicrosoftAccount account)
        {
            ClientWrapper clientWrapper;
            IPublicClientApplication client;
            if (account != null && !string.IsNullOrEmpty(account.AccountId))
            {
                clientWrapper = GetClient(account);
                client = clientWrapper.Client;
                var accountsAsync = (await client.GetAccountsAsync()).FirstOrDefault();
                throw new Exception("Account already exists");
            }

            clientWrapper = GetClient(account);
            client = clientWrapper.Client;
            await client.AcquireTokenInteractive(GraphUserScopes).ExecuteAsync();
        }
    }
}
