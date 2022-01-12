using CommandLineParser;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.UI.CLI.Commands
{
    public class PrintFilesCommand : ICommand
    {
        public bool AllowSwitchDefaultPrinter { get; set; }
        public string Printer { get; set; }
        public List<string> Files { get; set; }
    }
}
