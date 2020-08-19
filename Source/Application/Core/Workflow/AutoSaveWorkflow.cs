using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Services.JobEvents;
using pdfforge.PDFCreator.Core.Workflow.ComposeTargetFilePath;
using pdfforge.PDFCreator.Core.Workflow.Output;
using System;
using System.IO;
using System.Linq;

namespace pdfforge.PDFCreator.Core.Workflow
{
    public sealed class AutoSaveWorkflow : ConversionWorkflow
    {
        private readonly IJobRunner _jobRunner;
        private readonly INotificationService _notificationService;
        private readonly AutosaveOutputFileMover _outputFileMover;
        private readonly IProfileChecker _profileChecker;
        private readonly ITargetFilePathComposer _targetFilePathComposer;

        public AutoSaveWorkflow(IJobDataUpdater jobDataUpdater, IJobRunner jobRunner, IProfileChecker profileChecker,
            ITargetFilePathComposer targetFilePathComposer, AutosaveOutputFileMover outputFileMover,
            INotificationService notificationService, IJobEventsManager jobEventsManager)
        {
            JobDataUpdater = jobDataUpdater;
            JobEventsManager = jobEventsManager;
            _jobRunner = jobRunner;
            _profileChecker = profileChecker;
            _targetFilePathComposer = targetFilePathComposer;
            _outputFileMover = outputFileMover;
            _notificationService = notificationService;
        }

        protected override IJobDataUpdater JobDataUpdater { get; }
        protected override IJobEventsManager JobEventsManager { get; }

        protected override void DoWorkflowWork(Job job)
        {
            var documentName = job.JobInfo.Metadata.Title;
            var currentProfile = job.Profile;

            try
            {
                job.OutputFileTemplate = _targetFilePathComposer.ComposeTargetFilePath(job);

                var result = _profileChecker.CheckJob(job);
                if (!result)
                    throw new ProcessingException("Invalid Profile", result[0]);

                job.Passwords = JobPasswordHelper.GetJobPasswords(job.Profile, job.Accounts);

                job.CleanUpOnError = true;
                // Can throw ProcessingException. Use GetAwaiter().GetResult() to unwrap an occuring AggregateException.
                _jobRunner.RunJob(job, _outputFileMover).GetAwaiter().GetResult();

                WorkflowResult = WorkflowResult.Finished;
                OnJobFinished(EventArgs.Empty);

                documentName = Path.GetFileName(job.OutputFiles.First());

                if (currentProfile.ShowAllNotifications && !currentProfile.ShowOnlyErrorNotifications)
                    _notificationService?.ShowInfoNotification(documentName, job.OutputFiles.First());
            }
            catch (Exception)
            {
                if (currentProfile.ShowAllNotifications || currentProfile.ShowOnlyErrorNotifications)
                    _notificationService?.ShowErrorNotification(documentName);

                throw;
            }
        }
    }
}
