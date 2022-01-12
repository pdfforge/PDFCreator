using CommandLineParser;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.UI.CLI.Commands
{
    public class MergeAndProcessFilesCommand : ICommand
    {
        public string Profile { get; set; }
        public string OutputFile { get; set; }
        public List<string> Files { get; set; }
    }
}
