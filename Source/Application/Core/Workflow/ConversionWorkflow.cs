using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Services.JobEvents;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using System;
using System.Diagnostics;
using System.Threading;

namespace pdfforge.PDFCreator.Core.Workflow
{
    /// <summary>
    ///     Defines the different stats the workflow can be in
    /// </summary>
    public enum WorkflowResult
    {
        Init,
        AbortedByUser,
        Error,
        Finished
    }

    /// <summary>
    ///     The ConversionWorkflow class handles all required steps to convert a PostScript file.
    ///     If required (i.e. during interactive conversion), the respective requests are invoked
    ///     on the IWorkflowInformationQuery implementation.
    /// </summary>
    public abstract class ConversionWorkflow : IConversionWorkflow
    {
        protected abstract IJobEventsManager JobEventsManager { get; }
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     The step the workflow currently is in
        /// </summary>
        protected WorkflowResult WorkflowResult;

        protected abstract IJobDataUpdater JobDataUpdater { get; }

        public ErrorCode? LastError { get; private set; }

        public event EventHandler JobFinished;

        private Stopwatch _stopwatch;

        /// <summary>
        ///     Runs all steps and user interaction that is required during the conversion
        /// </summary>
        public WorkflowResult RunWorkflow(Job job)
        {
            try
            {
                _stopwatch = new Stopwatch();
                _stopwatch.Start();
                JobEventsManager.RaiseJobStarted(job, Thread.CurrentThread.ManagedThreadId.ToString());

                PrepareAndRun(job);
            }
            catch (AbortWorkflowException ex)
            {
                // we need to clean up the job when it was cancelled
                _logger.Warn(ex.Message + " No output will be created.");
                WorkflowResult = WorkflowResult.AbortedByUser;

                SendJobEvents(job);
            }
            catch (WorkflowException ex)
            {
                _logger.Error(ex.Message);
                WorkflowResult = WorkflowResult.Error;

                SendJobEvents(job);
            }
            catch (ProcessingException ex)
            {
                var errorMessage = ex.ErrorCode + " / " + ex.Message;
                if (ex.InnerException != null)
                    errorMessage += Environment.NewLine + ex.InnerException;
                _logger.Error(errorMessage);

                LastError = ex.ErrorCode;

                HandleError(ex.ErrorCode);
                WorkflowResult = WorkflowResult.Error;

                SendJobEvents(job);
            }
            catch (ManagePrintJobsException)
            {
                throw;
            }
            catch (InterruptWorkflowException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                WorkflowResult = WorkflowResult.Error;
                SendJobEvents(job);

                throw;
            }

            return WorkflowResult;
        }

        private void SendJobEvents(Job job)
        {
            _stopwatch.Stop();
            var elapsedTime = TimeSpan.FromTicks(_stopwatch.ElapsedMilliseconds);

            switch (WorkflowResult)
            {
                case WorkflowResult.AbortedByUser:
                    JobEventsManager.RaiseJobFailed(job, elapsedTime, FailureReason.AbortedByUser);
                    break;

                case WorkflowResult.Error:
                    JobEventsManager.RaiseJobFailed(job, elapsedTime, FailureReason.Error);
                    break;

                case WorkflowResult.Finished:
                    JobEventsManager.RaiseJobCompleted(job, elapsedTime);
                    break;
            }
        }

        protected abstract void DoWorkflowWork(Job job);

        private void PrepareAndRun(Job job)
        {
            WorkflowResult = WorkflowResult.Init;

            _logger.Debug("Starting conversion...");

            var originalMetadata = job.JobInfo.Metadata.Copy();
            job.InitMetadataWithTemplatesFromProfile();

            JobDataUpdater.UpdateTokensAndMetadata(job);

            try
            {
                DoWorkflowWork(job);

                WorkflowResult = WorkflowResult.Finished;

                SendJobEvents(job);
            }
            catch (ManagePrintJobsException)
            {
                // revert metadata changes and rethrow exception
                job.JobInfo.Metadata = originalMetadata;
                throw;
            }
            finally
            {
                OnJobFinished(EventArgs.Empty);
            }
        }

        protected void OnJobFinished(EventArgs e)
        {
            JobFinished?.Invoke(this, e);
        }

        protected virtual void HandleError(ErrorCode errorCode)
        {
        }
    }
}
