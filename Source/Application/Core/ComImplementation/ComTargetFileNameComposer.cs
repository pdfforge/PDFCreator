using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Workflow.ComposeTargetFilePath;

namespace pdfforge.PDFCreator.Core.ComImplementation
{
    public class ComTargetFilePathComposer : ITargetFilePathComposer
    {
        private readonly string _targetFilename;

        public ComTargetFilePathComposer(string targetFilename)
        {
            _targetFilename = targetFilename;
        }

        public string ComposeTargetFilePath(Job job)
        {
            return _targetFilename;
        }
    }
}
