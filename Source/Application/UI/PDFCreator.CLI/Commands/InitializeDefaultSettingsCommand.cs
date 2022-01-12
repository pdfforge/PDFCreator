using CommandLineParser;

namespace pdfforge.PDFCreator.UI.CLI.Commands
{
    public class InitializeDefaultSettingsCommand : ICommand
    {
        public string SettingsFile { get; set; }
    }
}
