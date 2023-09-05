using pdfforge.Banners;
using System;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Utilities.Web
{
    public class TrackingParameters
    {
        private string Campaign { get; }
        private string Key1 { get; }
        private string Key2 { get; }
        private string Keyb { get; }
        private string LicenseKey { get; }

        public TrackingParameters(string campaign, string key1, string key2, string keyb, string licenseKey)
        {
            Campaign = campaign;
            Key1 = key1;
            Key2 = key2;
            Keyb = keyb;
            LicenseKey = licenseKey;
        }

        public IDictionary<string, string> ToParamList()
        {
            var parameters = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(Campaign))
                parameters["cmp"] = Campaign;

            if (!string.IsNullOrWhiteSpace(Key1))
                parameters["key1"] = Key1;

            if (!string.IsNullOrWhiteSpace(Key2))
                parameters["key2"] = Key2;

            if (!string.IsNullOrWhiteSpace(Keyb))
                parameters["keyb"] = Keyb;

            if (!string.IsNullOrWhiteSpace(LicenseKey))
                parameters["license_key"] = LicenseKey;

            return parameters;
        }

        public string CleanUpParamsAndAppendToUrl(string url)
        {
            var uri = new Uri(url);
            var cleanUri = uri.GetLeftPart(UriPartial.Path).TrimEnd('/');
            var uriParams = uri.Query.TrimStart('?').Split('&');
            var presentParams = GetPresentParams(uriParams);
            var trackingParamsDictionary = ToParamList();
            url = cleanUri;
            if (presentParams.Count > 0)
                url = UrlHelper.AddUrlParameters(cleanUri, presentParams);

            url = UrlHelper.AddUrlParameters(url, trackingParamsDictionary);
            return url + uri.Fragment;
        }

        private static IDictionary<string, string> GetPresentParams(IEnumerable<string> uriParams)
        {
            var presentParams = new Dictionary<string, string>();
            foreach (var urlParam in uriParams)
            {
                var paramSplit = urlParam.Split('=');
                if (paramSplit.Length != 2)
                    continue;

                // Avoid escaping data string multiple times
                var key = Uri.UnescapeDataString(paramSplit[0]);
                var value = Uri.UnescapeDataString(paramSplit[1]);

                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value) && !presentParams.ContainsKey(key))
                    presentParams.Add(key, value);
            }

            return presentParams;
        }
    }
}
