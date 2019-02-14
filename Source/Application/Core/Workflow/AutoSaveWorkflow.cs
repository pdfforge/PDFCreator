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
        private readonly IProfileChecker _profileChecker;
        private readonly ITargetFilePathComposer _targetFilePathComposer;
        private readonly AutosaveOutputFileMover _outputFileMover;
        private readonly INotificationService _notificationService;

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

        protected override IJobEventsManager JobEventsManager { get; }
        protected override IJobDataUpdater JobDataUpdater { get; }

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

                // Can throw ProcessingException
                _jobRunner.RunJob(job, _outputFileMover).Wait();

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
