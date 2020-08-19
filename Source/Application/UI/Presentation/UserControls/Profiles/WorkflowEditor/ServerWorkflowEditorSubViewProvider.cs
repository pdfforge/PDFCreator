namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.WorkflowEditor
{
    public class ServerWorkflowEditorSubViewProvider : WorkflowEditorSubViewProvider
    {
        public string PrinterOverlay { get; }

        public ServerWorkflowEditorSubViewProvider(string saveOverlay, string metaDataOverlay, string outputFormatOverlay, string printerOverlay) : base(saveOverlay, metaDataOverlay, outputFormatOverlay)
        {
            PrinterOverlay = printerOverlay;
        }
    }
}
