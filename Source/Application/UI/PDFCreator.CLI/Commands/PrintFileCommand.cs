using CommandLineParser;

namespace pdfforge.PDFCreator.UI.CLI.Commands
{
    public class PrintFileCommand : ICommand
    {
        public bool AllowSwitchDefaultPrinter { get; set; }
        public string Printer { get; set; }
        public string Profile { get; set; }
        public string File { get; set; }
        public string OutputFile { get; set; }
    }
}
