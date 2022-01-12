using CommandLineParser;

namespace pdfforge.PDFCreator.UI.CLI.Commands
{
    public class StoreLicenseForAllUsersCommand : ICommand
    {
        public string LicenseServerCode { get; set; }
        public string LicenseKey { get; set; }
    }
}
