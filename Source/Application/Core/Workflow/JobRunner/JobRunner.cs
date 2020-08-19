using NLog;
using pdfforge.PDFCreator.Conversion.ConverterInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Workflow.Output;
using pdfforge.PDFCreator.Utilities.IO;
using System;
using System.IO;
using System.Threading.Tasks;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.Workflow
{
    public interface IJobRunner
    {
        /// <summary>
        ///     Runs the job and all actions
        /// </summary>
        Task RunJob(Job abstractJob, IOutputFileMover outputFileMover);
    }

    public class JobRunner : IJobRunner
    {
        private readonly IConverterFactory _converterFactory;
        private readonly IDirectory _directory;
        private readonly IDirectoryHelper _directoryHelper;
        private readonly IPdfProcessingHelper _processingHelper;
        private readonly IActionExecutor _actionExecutor;
        private readonly IJobCleanUp _jobClean;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ITempFolderProvider _tempFolderProvider;
        private readonly ITokenReplacerFactory _tokenReplacerFactory;

        public JobRunner(ITokenReplacerFactory tokenReplacerFactory,
            IConverterFactory converterFactory, IJobCleanUp jobClean, ITempFolderProvider tempFolderProvider, IDirectory directory,
            IDirectoryHelper directoryHelper, IPdfProcessingHelper processingHelper, IActionExecutor actionExecutor)
        {
            _tokenReplacerFactory = tokenReplacerFactory;
            _converterFactory = converterFactory;
            _jobClean = jobClean;
            _tempFolderProvider = tempFolderProvider;
            _directory = directory;
            _directoryHelper = directoryHelper;
            _processingHelper = processingHelper;
            _actionExecutor = actionExecutor;
        }

        /// <summary>
        ///     Runs the job and all actions
        /// </summary>
        public async Task RunJob(Job job, IOutputFileMover outputFileMover) //todo: Store OutputFileMover somewhere workflow dependant
        {
            _logger.Trace("Starting job");

            _logger.Debug("Output filename template is: {0}", job.OutputFileTemplate);
            _logger.Debug("Output format is: {0}", job.Profile.OutputFormat);
            _logger.Info("Converting " + job.OutputFileTemplate);

            SetTempFolders(job);

            try
            {
                // TODO: Use async/await
                _actionExecutor.CallPreConversionActions(job);

                _processingHelper.ApplyFormatRestrictionsToProfile(job);

                var converter = _converterFactory.GetConverter(job.JobInfo.JobType);
                var reportProgress = new EventHandler<ConversionProgressChangedEventArgs>((sender, args) => job.ReportProgress(args.Progress));
                converter.OnReportProgress += reportProgress;

                var isPdf = job.Profile.OutputFormat.IsPdf();
                var isProcessingRequired = _processingHelper.IsProcessingRequired(job);
                converter.Init(isPdf, isProcessingRequired);

                converter.FirstConversionStep(job);

                _actionExecutor.CallConversionActions(job);

                converter.SecondConversionStep(job);

                converter.OnReportProgress -= reportProgress;

                await outputFileMover.MoveOutputFiles(job);

                if (job.OutputFiles.Count == 0)
                {
                    _logger.Error("No output files were created for unknown reason");
                    throw new ProcessingException("No output files were created for unknown reason", ErrorCode.Conversion_UnknownError);
                }

                LogOutputFiles(job);

                job.TokenReplacer = _tokenReplacerFactory.BuildTokenReplacerWithOutputfiles(job);

                _actionExecutor.CallPostConversionActions(job);

                CleanUp(job);

                job.IsSuccessful = true;
                _logger.Trace("Job finished successfully");
            }
            catch (Exception ex)
            {
                if (ex is ProcessingException processingException)
                    _logger.Error($"The job failed: {processingException.Message} ({processingException.ErrorCode})");
                else
                    _logger.Error(ex, $"The job failed: {ex.Message}");

                if (job.CleanUpOnError)
                    CleanUp(job);

                throw;
            }
            finally
            {
                _logger.Trace("Calling job completed event");
                job.CallJobCompleted();
            }
        }

        private void LogOutputFiles(Job job)
        {
            _logger.Trace("Created {0} output files.", job.OutputFiles.Count);
            var i = 1;
            foreach (var file in job.OutputFiles)
            {
                _logger.Trace(i + ". Output file: {0}", file);
                i++;
            }
        }

        /// <summary>
        ///     Clean up all temporary files that have been generated during the Job
        /// </summary>
        private void CleanUp(Job job)
        {
            _logger.Debug("Cleaning up after the job");
            _jobClean.DoCleanUp(job.JobTempFolder, job.JobInfo.SourceFiles, job.JobInfo.InfFile);
            _directoryHelper.DeleteCreatedDirectories();
        }

        private void SetTempFolders(Job job)
        {
            job.JobTempFolder = PathSafe.Combine(_tempFolderProvider.TempFolder,
                "Job_" + PathSafe.GetFileNameWithoutExtension(Path.GetRandomFileName()));
            _directory.CreateDirectory(job.JobTempFolder);
            job.JobTempOutputFolder = PathSafe.Combine(job.JobTempFolder, "tempoutput");
            _directory.CreateDirectory(job.JobTempOutputFolder);
            job.IntermediateFolder = PathSafe.Combine(job.JobTempFolder, "intermediate");
            _directory.CreateDirectory(job.IntermediateFolder);

            // Shorten the temp folder for GS compatibility
            job.JobTempFolder = PathHelper.GetShortPathName(job.JobTempFolder);
            //TODO remove this after upgrade to GS 9.19

            if (job.Profile.SaveFileTemporary)
            {
                var tempPath = PathSafe.Combine(_tempFolderProvider.TempFolder,
                    "Job_tempsave_" + PathSafe.GetFileNameWithoutExtension(Path.GetRandomFileName()));

                job.OutputFileTemplate = PathSafe.Combine(tempPath, PathSafe.GetFileNameWithoutExtension(job.OutputFileTemplate));
            }
        }
    }
}
