using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.Core.Workflow.ComposeTargetFilePath
{
    public class TargetFilePathComposer : TargetFilePathComposerBase
    {
        private readonly ILastSaveDirectoryHelper _lastSaveDirectoryHelper;

        public TargetFilePathComposer(IPathUtil pathUtil, ILastSaveDirectoryHelper lastSaveDirectoryHelper, 
            ISplitDocumentFilePathHelper splitDocumentFilePathHelper, OutputFormatHelper outputFormatHelper,
            ITempFolderProvider tempFolderProvider)
            : base(pathUtil, splitDocumentFilePathHelper, outputFormatHelper, tempFolderProvider)
        {
            _lastSaveDirectoryHelper = lastSaveDirectoryHelper;
        }

        protected override string ConsiderLastSaveDirectory(string outputFolder, Job job)
        {
            return _lastSaveDirectoryHelper.IsEnabled(job) ? _lastSaveDirectoryHelper.GetDirectory() : outputFolder;
        }
    }
}
