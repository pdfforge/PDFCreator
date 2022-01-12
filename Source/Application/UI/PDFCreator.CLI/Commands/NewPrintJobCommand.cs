using CommandLineParser;

namespace pdfforge.PDFCreator.UI.CLI.Commands
{
    internal class NewPrintJobCommand : ICommand
    {
        public string InfFilePath { get; set; }
    }
}
