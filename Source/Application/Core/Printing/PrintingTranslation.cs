using Translatable;

namespace pdfforge.PDFCreator.Core.Printing
{
    public class PrintingTranslation : ITranslatable
    {
        public string Error { get; private set; } = "Error";
        public string RepairPrinterAskUserUac { get; private set; } = "You do not have any PDFCreator printers installed. Most likely there was a problem during the setup or the installation has been altered afterwards.\nDo you want to fix this by reinstalling the PDFCreator printers?\n\nNote: You might be asked to grant admin privileges while fixing the problem.";
        public string RepairPrinterNoPrintersInstalled { get; private set; } = "No printers installed";
        private string SetupFileMissing { get; set; } = "An important PDFCreator file is missing ('{0}'). Please reinstall PDFCreator!";
        
        public string GetSetupFileMissingMessage(string file)
        {
            return string.Format(SetupFileMissing, file);
        }
    }
}
