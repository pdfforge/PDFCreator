using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Query;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Workflow.Queries;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.IO;
using System;
using System.Threading.Tasks;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.Workflow.Output
{
    public class InteractiveOutputFileMover : OutputFileMoverBase
    {
        private readonly IDispatcher _dispatcher;
        private readonly IPdfProcessor _pdfProcessor;

        public InteractiveOutputFileMover(IUniqueFilenameFactory uniqueFilenameFactory, IFile file, IPathUtil pathUtil, IRetypeFileNameQuery retypeFileNameQuery, IDispatcher dispatcher, IDirectoryHelper directoryHelper, IPdfProcessor pdfProcessor)
        {
            _dispatcher = dispatcher;
            _pdfProcessor = pdfProcessor;
            UniqueFilenameFactory = uniqueFilenameFactory;
            DirectoryHelper = directoryHelper;
            File = file;
            PathUtil = pathUtil;
            RetypeFileNameQuery = retypeFileNameQuery;
        }

        protected override IUniqueFilenameFactory UniqueFilenameFactory { get; }
        protected override IDirectoryHelper DirectoryHelper { get; }
        protected override IFile File { get; }
        protected override IPathUtil PathUtil { get; }
        private IRetypeFileNameQuery RetypeFileNameQuery { get; }

        protected override Task<QueryResult<string>> HandleInvalidRootedPath(string filename, OutputFormat outputFormat)
        {
            var result = _dispatcher.InvokeAsync(() => RetypeFileNameQuery.RetypeFileNameQuery(filename, outputFormat, RetypeReason.InvalidRootedPath));
            return result;
        }

        protected override Task<QueryResult<string>> HandleFirstFileFailed(string filename, OutputFormat outputFormat)
        {
            var result = _dispatcher.InvokeAsync(() => RetypeFileNameQuery.RetypeFileNameQuery(filename, outputFormat, RetypeReason.CopyError));
            return result;
        }

        protected override HandleCopyErrorResult QueryHandleCopyError(int fileNumber)
        {
            if (fileNumber == 1)
                return HandleCopyErrorResult.Requery;

            return HandleCopyErrorResult.EnsureUniqueFilename;
        }

        protected override bool ShouldApplyUniqueFilename(Job job)
        {
            return false;
        }

        protected override bool ShouldApplyMerger(Job job)
        {
            return job.ExistingFile == ExistingFileBehaviour.Merge;
        }

        protected override bool AppendFile(string tempFile, string outputFile)
        {
            try
            {
                _pdfProcessor.MergePDFs(outputFile, tempFile);
                Logger.Debug("Append output file \"{0}\" \r\ninto \"{1}\"", tempFile, outputFile);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Warn("Error while append into target file.\r\nfrom\"{0}\" \r\nto \"{1}\"\r\n{2}", tempFile, outputFile, ex.Message);
            }
            return false;
        }
    }
}
