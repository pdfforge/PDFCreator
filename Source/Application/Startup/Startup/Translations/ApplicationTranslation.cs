using Translatable;

namespace pdfforge.PDFCreator.Core.Startup.Translations
{
    public class StartupApplicationTranslation : ITranslatable
    {
        public string RepairPrinterFailed { get; private set; } = "PDFCreator was not able to repair your printers. Please contact your administrator or the support to assist you in with this problem.";
        private string SpoolFolderUnableToRepair { get; set; } = "PDFCreator was not able to repair your spool folder. Please contact your administrator or the support to assist you in changing the permissions of the path '{0}'.";

        public string GetSpoolFolderUnableToRepairMessage(string spoolFolder)
        {
            return string.Format(SpoolFolderUnableToRepair, spoolFolder);
        }

    }
}
