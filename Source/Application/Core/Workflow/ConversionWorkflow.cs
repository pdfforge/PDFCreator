using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Services.JobEvents;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace pdfforge.PDFCreator.Core.Workflow
{
    /// <summary>
    ///     Defines the different stats the workflow can be in
    /// </summary>
    public enum WorkflowResultState
    {
        Init,
        AbortedByUser,
        Error,
        Finished
    }

    public class WorkflowResult
    {
        public static implicit operator WorkflowResultState(WorkflowResult workflowResult)
        {
            return workflowResult.State;
        }

        public WorkflowResultState State { get; }
        public ActionResult ActionResult { get; }

        private WorkflowResult(WorkflowResultState state, ActionResult actionResult)
        {
            State = state;
            ActionResult = actionResult;
        }

        public static WorkflowResult FromStateAndActionResult(WorkflowResultState state, ActionResult results)
        {
            return new WorkflowResult(state, results);
        }

        public static WorkflowResult FromState(WorkflowResultState state)
        {
            return new WorkflowResult(state, new ActionResult());
        }

        public static WorkflowResult FromError(ErrorCode errorCode)
        {
            return new WorkflowResult(WorkflowResultState.Error, new ActionResult(errorCode));
        }
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
        protected WorkflowResultState WorkflowResultState;

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
                WorkflowResultState = WorkflowResultState.AbortedByUser;
                // we need to clean up the job when it was cancelled
                _logger.Warn(ex.Message + " No output will be created.");

                SendJobEvents(job);
                return WorkflowResult.FromState(WorkflowResultState.AbortedByUser);
            }
            catch (WorkflowException ex)
            {
                WorkflowResultState = WorkflowResultState.Error;
                _logger.Error(ex.Message);

                SendJobEvents(job);
                return WorkflowResult.FromState(WorkflowResultState.Error);
            }
            catch (ProcessingException ex)
            {
                HandleProcessingException(ex, new ActionResult(ex.ErrorCode), true, false);

                SendJobEvents(job);
                return WorkflowResult.FromError(ex.ErrorCode);
            }
            catch (AggregateProcessingException ex)
            {
                HandleProcessingException(ex, ex.Result, false, true);

                SendJobEvents(job);
                return WorkflowResult.FromStateAndActionResult(WorkflowResultState.Finished, ex.Result);
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
                WorkflowResultState = WorkflowResultState.Error;
                _logger.Error(ex);
                SendJobEvents(job);

                throw;
            }

            return WorkflowResult.FromState(WorkflowResultState);
        }

        private void HandleProcessingException(Exception ex, ActionResult result, bool error, bool warn)
        {
            WorkflowResultState = error ? WorkflowResultState.Error : WorkflowResultState.Finished;
            var errorMessage = ex.Message + Environment.NewLine + string.Join(Environment.NewLine, result);
            if (ex.InnerException != null)
                errorMessage += Environment.NewLine + ex.InnerException;
            LastError = result.Last();
            if (warn)
            {
                _logger.Warn(errorMessage);
                HandleWarning(result);
            }

            if (error)
            {
                _logger.Error(errorMessage);
                HandleError(result.Last());
            }
        }

        private void SendJobEvents(Job job)
        {
            _stopwatch.Stop();
            var elapsedTime = TimeSpan.FromTicks(_stopwatch.ElapsedMilliseconds);

            switch (WorkflowResultState)
            {
                case WorkflowResultState.AbortedByUser:
                    JobEventsManager.RaiseJobFailed(job, elapsedTime, FailureReason.AbortedByUser);
                    break;

                case WorkflowResultState.Error:
                    JobEventsManager.RaiseJobFailed(job, elapsedTime, FailureReason.Error);
                    break;

                case WorkflowResultState.Finished:
                    JobEventsManager.RaiseJobCompleted(job, elapsedTime);
                    break;
            }
        }

        protected abstract void DoWorkflowWork(Job job);

        private void PrepareAndRun(Job job)
        {
            WorkflowResultState = WorkflowResultState.Init;

            _logger.Debug("Starting conversion...");

            var originalMetadata = job.JobInfo.Metadata.Copy();
            job.InitMetadataWithTemplatesFromProfile();

            JobDataUpdater.UpdateTokensAndMetadata(job);

            try
            {
                DoWorkflowWork(job);

                WorkflowResultState = WorkflowResultState.Finished;

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

        protected virtual void HandleWarning(ActionResult result)
        {
        }

        protected virtual void HandleError(ErrorCode errorCode)
        {
        }
    }
}
