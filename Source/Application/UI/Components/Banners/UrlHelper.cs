using System;
using System.Collections.Generic;
using System.Linq;

namespace Banners
{
    internal static class UrlHelper
    {
        public static string AddUrlParameters(string url, Dictionary<string, string> parameters)
        {
            var escapedParameters = parameters.Select(pair => Uri.EscapeDataString(pair.Key) + "=" + Uri.EscapeDataString(pair.Value));
            var parameterString = string.Join("&", escapedParameters);
            var separator = url.Contains("?") ? "&" : "?";

            return url + separator + parameterString;
        }

        public static string AddUrlParameters(string url, string key, string value)
        {
            var parameters = new Dictionary<string, string>() { { key, value } };

            return AddUrlParameters(url, parameters);
        }
    }
}