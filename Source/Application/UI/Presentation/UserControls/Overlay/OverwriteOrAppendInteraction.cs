using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Overlay
{
    public class OverwriteOrAppendInteraction : IInteraction
    {
        public ExistingFileBehaviour Chosen { get; set; }
        public bool Cancel { get; set; } = true;
        public bool MergeIsSupported { get; set; }
    }
}
