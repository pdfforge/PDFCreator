using System;
using System.Collections.Generic;
using System.Linq;

namespace pdfforge.PDFCreator.Utilities.Web
{
    public class TrackingParameters
    {
        private string Campaign { get; }
        private string Key1 { get; }
        private string Key2 { get; }
        private string Keywords { get; }

        public TrackingParameters(string campaign, string key1, string key2, string keywords)
        {
            Campaign = campaign;
            Key1 = key1;
            Key2 = key2;
            Keywords = keywords;
        }

        private IDictionary<string, string> ToParamList()
        {
            var parameters = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(Campaign))
                parameters["cmp"] = Campaign;

            if (!string.IsNullOrWhiteSpace(Key1))
                parameters["key1"] = Key1;

            if (!string.IsNullOrWhiteSpace(Key2))
                parameters["key2"] = Key2;

            if (!string.IsNullOrWhiteSpace(Keywords))
                parameters["keyb"] = Keywords;

            return parameters;
        }

        public string CleanUpParamsAndAppendToUrl(string url)
        {
            var uri = new Uri(url);
            var cleanUrl = uri.GetLeftPart(UriPartial.Path).TrimEnd('/');
            var splitQuery = uri.Query.TrimStart('?').Split('&');
            var presentParams = new Dictionary<string, string>();

            foreach (var item in splitQuery)
            {
                var splitItem = item.Split('=');
                var itemKey = splitItem.First();
                var itemValue = splitItem.Length > 1 ? splitItem.Skip(1).First() : string.Empty;

                if (!string.IsNullOrWhiteSpace(itemKey) && !string.IsNullOrWhiteSpace(itemValue) && !presentParams.ContainsKey(itemKey))
                    presentParams.Add(itemKey, itemValue);
            }

            var trackingParamsDictionary = ToParamList();

            trackingParamsDictionary.Keys.Except(presentParams.Keys).ToList()
                 .ForEach(k => presentParams.Add(k, trackingParamsDictionary[k]));

            var cleanedParamString = "";
            if (presentParams.Count > 0)
                cleanedParamString = "?" + ToParamString(presentParams);

            return cleanUrl + cleanedParamString;
        }

        private string ToParamString(Dictionary<string, string> dict)
        {
            var paramList = dict.Select(p => Uri.EscapeDataString(p.Key) + "=" + Uri.EscapeDataString(p.Value));
            return string.Join("&", paramList);
        }
    }
}
