namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.WorkflowEditor
{
    public interface IWorkflowEditorSubViewProvider
    {
        string SaveOverlay { get; }
        string MetaDataOverlay { get; }
        string OutputFormatOverlay { get; }
    }

    public class WorkflowEditorSubViewProvider : IWorkflowEditorSubViewProvider
    {
        public string SaveOverlay { get; }
        public string MetaDataOverlay { get; }
        public string OutputFormatOverlay { get; }

        public WorkflowEditorSubViewProvider(string saveOverlay, string metaDataOverlay, string outputFormatOverlay)
        {
            SaveOverlay = saveOverlay;
            MetaDataOverlay = metaDataOverlay;
            OutputFormatOverlay = outputFormatOverlay;
        }
    }
}
