using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Services.JobHistory;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using pdfforge.PDFCreator.Utilities.Threading;
using System;
using System.IO;
using System.Threading;
using IJobInfoQueue = pdfforge.PDFCreator.Core.JobInfoQueue.IJobInfoQueue;
using NewJobInfoEventArgs = pdfforge.PDFCreator.Core.JobInfoQueue.NewJobInfoEventArgs;

namespace pdfforge.PDFCreator.Core.Workflow
{
    public interface IJobInfoQueueManager
    {
        void ManagePrintJobs();

        bool AutoStartProcessing { get; set; }
    }

    /// <summary>
    ///     The JobRunner class manages the thread that processes the print jobs. It listens for new jobs in the
    ///     <see cref="JobInfoQueue" /> and creates the processing thread if required.
    /// </summary>
    public class JobInfoQueueManager : IJobInfoQueueManager
    {
        private readonly IJobInfoQueue _jobInfoQueue;
        private readonly IJobBuilder _jobBuilder;
        private readonly ISettingsProvider _settingsProvider;
        private readonly IJobHistoryActiveRecord _jobHistoryActiveRecord;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IManagePrintJobExceptionHandler _managePrintJobExceptionHandler;
        private readonly IThreadManager _threadManager;
        private readonly IWorkflowFactory _workflowFactory;

        private bool _managePrintJobs;
        private ISynchronizedThread _processingThread;

        public JobInfoQueueManager(IManagePrintJobExceptionHandler managePrintJobExceptionHandler, IThreadManager threadManager, IWorkflowFactory workflowFactory,
                                   IJobInfoQueue jobInfoQueue, IJobBuilder jobBuilder, ISettingsProvider settingsProvider, IJobHistoryActiveRecord jobHistoryActiveRecord)
        {
            _managePrintJobExceptionHandler = managePrintJobExceptionHandler;
            _threadManager = threadManager;
            _workflowFactory = workflowFactory;
            _jobInfoQueue = jobInfoQueue;
            _jobBuilder = jobBuilder;
            _settingsProvider = settingsProvider;
            _jobHistoryActiveRecord = jobHistoryActiveRecord;

            _jobInfoQueue.OnNewJobInfo += JobInfoQueue_OnNewJobInfo;
        }

        public bool AutoStartProcessing { get; set; } = true;

        public void ManagePrintJobs()
        {
            if (_processingThread != null)
                return;
            _managePrintJobs = true;
            StartProcessing();
        }

        private void JobInfoQueue_OnNewJobInfo(object sender, NewJobInfoEventArgs e)
        {
            if (_processingThread == null && AutoStartProcessing)
            {
                StartProcessing();
            }
        }

        private void StartProcessing()
        {
            if (_jobInfoQueue.IsEmpty && !_managePrintJobs)
            {
                return;
            }

            _processingThread = new SynchronizedThread(ProcessJobs) { Name = "ProcessingThread" };
            _processingThread.SetApartmentState(ApartmentState.STA);

            _threadManager.StartSynchronizedThread(_processingThread);
        }

        [STAThread]
        private void ProcessJobs()
        {
            try
            {
                while (!_jobInfoQueue.IsEmpty || _managePrintJobs)
                {
                    try
                    {
                        if (_managePrintJobs)
                        {
                            throw new ManagePrintJobsException();
                        }

                        var jobInfo = _jobInfoQueue.NextJob;

                        if (jobInfo.SourceFiles.Count == 0)
                        {
                            _logger.Info("JobInfo has no source files and will be skipped");
                            _jobInfoQueue.Remove(jobInfo, true);
                            continue;
                        }

                        _logger.Debug("New PrintJob {0} from Printer {1}", jobInfo.InfFile,
                            jobInfo.SourceFiles[0].PrinterName);

                        var repeatJob = true;

                        try
                        {
                            ProcessJob(jobInfo);
                            // If Job was processed without ManagePrintJobsException, it can be removed
                            repeatJob = false;
                        }
                        catch (InvalidDataException ex)
                        {
                            _logger.Error("There was an invalid data exception while parsing the ps file: " + ex);
                            repeatJob = false;
                        }
                        finally
                        {
                            if (!repeatJob)
                            {
                                // ensure that the current job is removed even if an exception is thrown
                                _logger.Trace("Removing job from Queue");
                                _jobInfoQueue.Remove(jobInfo, true);
                            }
                        }
                    }
                    catch (ManagePrintJobsException)
                    {
                        _managePrintJobs = false;
                        _logger.Trace("Managing print jobs");
                        _managePrintJobExceptionHandler.HandleException();
                    }
                }
            }
            catch (InterruptWorkflowException)
            {
                _logger.Warn("Interrupted workflow");
            }
            catch (Exception ex)
            {
                _logger.Error("There was an error while processing the print jobs: " + ex);

                throw;
            }
            finally
            {
                if (!_jobInfoQueue.IsEmpty)
                    _logger.Warn("Processing finishes while there are print jobs left.");

                _processingThread = null;
            }
        }

        /// <summary>
        ///     Process a single job
        /// </summary>
        /// <param name="jobInfo">The jobinfo to process</param>
        /// <returns>True if the job was processed. If the user decided to manage the print jobs instead, this returns false</returns>
        private void ProcessJob(JobInfo jobInfo)
        {
            _logger.Trace("Creating job workflow");

            var job = _jobBuilder.BuildJobFromJobInfo(jobInfo, _settingsProvider.Settings);

            var mode = DetermineMode(job);
            var workflow = _workflowFactory.CreateWorkflow(mode);

            _logger.Trace("Running workflow");
            var workflowResult = workflow.RunWorkflow(job);

            if (workflowResult != WorkflowResultState.Finished)
            {
                if (workflowResult == WorkflowResultState.AbortedByUser)
                {
                    _logger.Info("The job '{0}' was aborted by the user.",
                        job.JobInfo.Metadata.Title);
                }
                else
                {
                    _logger.Error("The job '{0}' terminated at step {1} and did not end successfully.",
                        job.JobInfo.Metadata.Title, workflowResult.State);
                }
            }
            else
            {
                _jobHistoryActiveRecord.Add(job);
            }
        }

        private WorkflowModeEnum DetermineMode(Job job)
        {
            return job.Profile.AutoSave.Enabled ? WorkflowModeEnum.Autosave : WorkflowModeEnum.Interactive;
        }
    }
}
