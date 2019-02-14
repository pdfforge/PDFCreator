using NLog;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.ConverterInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Core.Workflow.Output;
using pdfforge.PDFCreator.Utilities.IO;
using System.Collections.Generic;
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
        private readonly IActionManager _actionManager;
        private readonly IConverterFactory _converterFactory;
        private readonly IDirectory _directory;
        private readonly IDirectoryHelper _directoryHelper;
        private readonly IJobCleanUp _jobClean;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPdfProcessor _processor;
        private readonly ITempFolderProvider _tempFolderProvider;
        private readonly ITokenReplacerFactory _tokenReplacerFactory;

        public JobRunner(ITokenReplacerFactory tokenReplacerFactory,
            IPdfProcessor processor, IConverterFactory converterFactory, IActionManager actionManager,
            IJobCleanUp jobClean, ITempFolderProvider tempFolderProvider, IDirectory directory, IDirectoryHelper directoryHelper)
        {
            _tokenReplacerFactory = tokenReplacerFactory;
            _processor = processor;
            _converterFactory = converterFactory;
            _actionManager = actionManager;
            _jobClean = jobClean;
            _tempFolderProvider = tempFolderProvider;
            _directory = directory;
            _directoryHelper = directoryHelper;
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
                _processor.Init(job);

                CallPreConversionActions(job);
                Convert(job);
                Process(job);

                await outputFileMover.MoveOutputFiles(job);

                if (job.OutputFiles.Count == 0)
                {
                    _logger.Error("No output files were created for unknown reason");
                    throw new ProcessingException("No output files were created for unknown reason", ErrorCode.Conversion_UnknownError);
                }

                LogOutputFiles(job);

                job.TokenReplacer = _tokenReplacerFactory.BuildTokenReplacerWithOutputfiles(job);

                CallPostConversionActions(job);

                CleanUp(job);

                job.IsSuccessful = true;
                _logger.Trace("Job finished successfully");
            }
            catch (ProcessingException ex)
            {
                _logger.Error($"The job failed: {ex.Message} ({ex.ErrorCode})");
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
            job.JobTempFolder = Path.Combine(_tempFolderProvider.TempFolder,
                "Job_" + Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
            job.JobTempOutputFolder = Path.Combine(job.JobTempFolder, "tempoutput");
            _directory.CreateDirectory(job.JobTempFolder);
            _directory.CreateDirectory(job.JobTempOutputFolder);

            // Shorten the temp folder for GS compatibility
            job.JobTempFolder = PathHelper.GetShortPathName(job.JobTempFolder);
            //TODO remove this after upgrade to GS 9.19
        }

        private void CallActions(Job job, IEnumerable<IAction> actions)
        {
            _logger.Trace("Starting Actions");
            foreach (var action in actions)
            {
                var result = action.ProcessJob(job);
                if (result)
                    _logger.Trace("Action {0} completed", action.GetType().Name);
                else
                    throw new ProcessingException("An action failed.", result[0]);
            }
        }

        /// <summary>
        ///     Call all pre conversion actions according to the configuration
        /// </summary>
        /// <param name="job"></param>
        private void CallPreConversionActions(Job job)
        {
            _logger.Trace("Setting up pre conversion actions");
            var preConversionActions = _actionManager.GetApplicablePreConversionActions(job);

            CallActions(job, preConversionActions);
        }

        /// <summary>
        ///     Call all post conversion actions according to the configuration
        /// </summary>
        /// <param name="job"></param>
        private void CallPostConversionActions(Job job)
        {
            _logger.Trace("Setting up post conversion actions");
            var postConversionActions = _actionManager.GetApplicablePostConversionActions(job);

            CallActions(job, postConversionActions);
        }

        /// <summary>
        ///     Apply all post conversion actions according to the configuration
        /// </summary>
        /// <param name="job"></param>
        private IEnumerable<IAction> SetUpPostConversionActions(Job job)
        {
            _logger.Trace("Setting up post conversion actions");
            var actions = _actionManager.GetApplicablePostConversionActions(job);
            return actions;
        }

        private void Convert(Job job)
        {
            var converter = _converterFactory.GetCorrectConverter(job.JobInfo.JobType);
            converter.OnReportProgress += (sender, args) => job.ReportProgress(args.Progress);

            converter.DoConversion(job);

            _logger.Debug(converter.ConverterOutput);
        }

        private void Process(Job job)
        {
            if (!_processor.ProcessingRequired(job.Profile))
            {
                _logger.Debug("No pdf processing required.");
                return;
            }
            _processor.ProcessPdf(job);
        }
    }
}
