using Translatable;

namespace pdfforge.PDFCreator.Utilities.Spool
{
    public class SpoolApplicationTranslation : ITranslatable
    {
        public string RepairToolNotFound { get; private set; } = "RepairTool not found";
        private string SetupFileMissing { get; set; } = "An important PDFCreator file is missing ('{0}'). Please reinstall PDFCreator!";
        public string SpoolFolderAccessDenied { get; private set; } = "Access Denied";
        private string SpoolFolderAskToRepair { get; set; } = "The temporary path where PDFCreator stores the print jobs can't be accessed. This is a configuration problem on your machine and needs to be fixed. Do you want PDFCreator to attempt repairing it?\nYour spool folder is: {0}";
        private string SpoolFolderUnableToRepair { get; set; } = "PDFCreator was not able to repair your spool folder. Please contact your administrator or the support to assist you in changing the permissions of the path '{0}'.";

        public string GetSpoolFolderUnableToRepairMessage(string spoolFolder)
        {
            return string.Format(SpoolFolderUnableToRepair, spoolFolder);
        }

        public string GetSetupFileMissingMessage(string file)
        {
            return string.Format(SetupFileMissing, file);
        }

        public string GetSpoolFolderAskToRepairMessage(string spoolFolder)
        {
            return string.Format(SpoolFolderAskToRepair, spoolFolder);
        }
    }
}
