using CommandLineParser;

namespace pdfforge.PDFCreator.UI.CLI.Commands
{
    public class ProcessFileCommand : ICommand
    {
        public string Profile { get; set; }
        public string OutputFile { get; set; }
        public string File { get; set; }
    }
}
