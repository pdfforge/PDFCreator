using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Query;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.IO;
using System.Threading.Tasks;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.Workflow.Output
{
    public class AutosaveOutputFileMover : OutputFileMoverBase
    {
        public AutosaveOutputFileMover(IDirectory directory, IFile file, IPathUtil pathUtil, IDirectoryHelper directoryHelper)
        {
            Directory = directory;
            File = file;
            PathUtil = pathUtil;
            DirectoryHelper = directoryHelper;
        }

        protected override IDirectory Directory { get; }
        protected override IDirectoryHelper DirectoryHelper { get; }
        protected override IFile File { get; }
        protected override IPathUtil PathUtil { get; }

        protected override Task<QueryResult<string>> HandleInvalidRootedPath(string filename, OutputFormat outputFormat)
        {
            return Task.FromResult(new QueryResult<string>(false, null));
        }

        protected override Task<QueryResult<string>> HandleFirstFileFailed(string filename, OutputFormat outputFormat)
        {
            return Task.FromResult(new QueryResult<string>(false, null));
        }

        protected override HandleCopyErrorResult QueryHandleCopyError(int fileNumber)
        {
            return HandleCopyErrorResult.EnsureUniqueFilename;
        }

        protected override bool ApplyUniqueFilename(Job job)
        {
            return job.Profile.AutoSave.EnsureUniqueFilenames;
        }
    }
}
