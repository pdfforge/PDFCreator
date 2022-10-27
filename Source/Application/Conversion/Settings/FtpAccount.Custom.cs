using System.Text.RegularExpressions;
using pdfforge.DataStorage;

namespace pdfforge.PDFCreator.Conversion.Settings
{
    public partial class FtpAccount
    {
        public string AccountInfo => $"{FtpConnectionType.ToString().ToUpper()} {UserName}" + "@" + Server;

        private readonly Regex _hostNameRegex = new Regex(@"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])$");

        public void CopyTo(FtpAccount targetAccount)
        {
            var data = Data.CreateDataStorage();
            StoreValues(data, "");
            targetAccount.ReadValues(data, "");
        }

        public string GetHost()
        {
            var hostParts = Server.Split(':');
            return hostParts[0];
        }

        public bool IsHostValid()
        {
            var host = GetHost();
            var match = _hostNameRegex.Match(host);
                
            return match.Success;
        }

        public int? GetPort()
        {
            return GetPort(Server).Port;
        }
        public bool IsPortValid()
        {
            return GetPort(Server).IsValid;
        }

        private (bool IsValid, int? Port) GetPort(string host)
        {
            var hostParts = host.Split(new []{':'}, 2);
            if (hostParts.Length < 2)
                return (true,null);

            if (int.TryParse(hostParts[1], out var port))
            {
                var isValid = port >= 0 && port <= 65535;
                if (isValid)
                    return (true,port);
            }

            return (false, null);
        }
    }
}