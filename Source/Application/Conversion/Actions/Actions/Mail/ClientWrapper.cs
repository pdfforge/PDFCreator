using Microsoft.Identity.Client;
using pdfforge.PDFCreator.Conversion.Settings;
using System;
using System.Text;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions.Mail
{
    public class ClientWrapper
    {
        public IPublicClientApplication Client { get; }
        public MicrosoftAccount Account { get; }

        public ClientWrapper(IPublicClientApplication client, MicrosoftAccount account)
        {
            Client = client;
            Account = account;
            client.UserTokenCache.SetAfterAccess(AfterAccessingTokenCache);
            client.UserTokenCache.SetBeforeAccess(BeforeAccessingTokenCache);
        }

        private void BeforeAccessingTokenCache(TokenCacheNotificationArgs args)
        {
            if (Account != null)
            {
                args.TokenCache.DeserializeMsalV3(Encoding.UTF8.GetBytes(Account.MicrosoftJson));
            }
        }

        private void AfterAccessingTokenCache(TokenCacheNotificationArgs args)
        {
            if (Account == null)
            {
                throw new Exception("Missing Microsoft account while writing to Token Cache");
            }

            var stringJson = Encoding.UTF8.GetString(args.TokenCache.SerializeMsalV3());

            if (args.Account != null)
            {
                Account.AccountInfo = args.Account.Username;
                Account.AccountId = args.Account.HomeAccountId.Identifier;
                Account.PermissionScopes = string.Join(",", args.RequestScopes);
            }

            Account.MicrosoftJson = stringJson;
        }
    }
}
