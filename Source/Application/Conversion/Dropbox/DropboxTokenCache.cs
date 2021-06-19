using System;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Conversion.Dropbox
{
    public interface IDropboxTokenCache
    {
        void RefreshAccessToken(string accessToken, DateTime expirationDate);

        bool NeedsRefresh(string accessToken);

        DateTime GetExpirationDate(string accessToken);
    }

    public class DropboxTokenCache : IDropboxTokenCache
    {
        private static readonly Dictionary<string, DateTime> TokenToExpirationDictionary = new Dictionary<string, DateTime>();

        public void RefreshAccessToken(string accessToken, DateTime expirationDate)
        {
            TokenToExpirationDictionary[accessToken] = expirationDate;
        }

        public bool NeedsRefresh(string accessToken)
        {
            // if it expires in the future
            var expirationDate = GetExpirationDate(accessToken);

            // until now dropbox refreshes a token for 4 hours, we reduce it to 3 as a buffer
            return expirationDate <= DateTime.Now.AddHours(1);
        }

        public DateTime GetExpirationDate(string accessToken)
        {
            if (TokenToExpirationDictionary.TryGetValue(accessToken, out var tokenExpirationDate))
            {
                return tokenExpirationDate;
            }
            // if not cached return an expired ExpirationDate
            return DateTime.Now.AddHours(-1);
        }
    }
}
