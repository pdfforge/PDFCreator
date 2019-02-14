using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.JobEvents;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Core.Workflow.ComposeTargetFilePath;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using pdfforge.PDFCreator.Core.Workflow.Output;
using System;

namespace pdfforge.PDFCreator.UnitTest.Core.Workflow
{
    [TestFixture]
    public class ConversionWorkflowTest
    {
        private Job _job;
        private JobInfo _jobInfo;
        private ConversionProfile _profile;
        private IProfileChecker _profileChecker;
        private readonly ActionResult _validActionResult = new ActionResult();

        private ConversionWorkflow _workflow;
        private ITargetFilePathComposer _query;
        private IJobRunner _jobRunner;
        private IJobDataUpdater _jobDataUpdater;
        private IJobEventsManager _jobEventsManager;
        private INotificationService _notificationService;

        [SetUp]
        public void SetUp()
        {
            _jobInfo = new JobInfo
            {
                Metadata = new Metadata
                {
                    Title = "Test"
                }
            };

            _profile = new ConversionProfile();
            _job = new Job(_jobInfo, _profile, new Accounts());
            _job.OutputFiles.Add("X:\\test.pdf");
            _profileChecker = Substitute.For<IProfileChecker>();
            _profileChecker.CheckJob(Arg.Any<Job>()).Returns(_validActionResult);

            _query = Substitute.For<ITargetFilePathComposer>();
            _jobRunner = Substitute.For<IJobRunner>();
            _jobDataUpdater = Substitute.For<IJobDataUpdater>();
            _jobEventsManager = Substitute.For<IJobEventsManager>();
            _notificationService = Substitute.For<INotificationService>();

            _workflow = new AutoSaveWorkflow(_jobDataUpdater, _jobRunner, _profileChecker, _query, null, _notificationService, _jobEventsManager);
        }

        private void SetUpConditionsForCompleteWorkflow()
        {
            _profileChecker.CheckJob(Arg.Any<Job>()).Returns(_validActionResult);
        }

        [Test]
        public void DoWorkflow_OnFailedJob_CallsErrorNotifier()
        {
            SetUpConditionsForCompleteWorkflow();
            _jobRunner.When(x => x.RunJob(_job, Arg.Any<IOutputFileMover>())).Do(x => { throw new ProcessingException("", ErrorCode.Conversion_UnknownError); });

            _workflow.RunWorkflow(_job);

            Assert.AreEqual(ErrorCode.Conversion_UnknownError, _workflow.LastError);
        }

        [Test]
        public void DoWorkflow_OnSuccessfulJob_DoesNotCallErrorNotifier()
        {
            SetUpConditionsForCompleteWorkflow();
            _jobRunner.When(x => x.RunJob(_job, Arg.Any<IOutputFileMover>())).Do(x => { _job.Completed = true; });

            _workflow.RunWorkflow(_job);

            Assert.IsTrue(_job.Completed);
            Assert.IsNull(_workflow.LastError);
        }

        [Test]
        public void RunWorkFlow_AbortWorkflowExceptionGetsCatched_WorkflowStepIsAbortedByUser()
        {
            SetUpConditionsForCompleteWorkflow();
            _query.When(x => x.ComposeTargetFilePath(Arg.Any<Job>())).Do(x => { throw new AbortWorkflowException("message"); });

            var workflowResult = _workflow.RunWorkflow(_job);
            Assert.AreEqual(WorkflowResult.AbortedByUser, workflowResult);
        }

        [Test]
        public void RunWorkFlow_CheckOrderOfCallsAndActionAssignmentsForCompleteProcess()
        {
            SetUpConditionsForCompleteWorkflow();
            _workflow.RunWorkflow(_job);

            Received.InOrder(() =>
            {
                _jobDataUpdater.UpdateTokensAndMetadata(_job);
                _query.ComposeTargetFilePath(_job);
                _profileChecker.CheckJob(_job);
                _jobRunner.RunJob(_job, Arg.Any<IOutputFileMover>());
            });
        }

        [Test]
        public void
            RunWorkFlow_QueryTargetFileThrowsManagePrintJobsException_ThrowsManagePrintJobsExceptionAndMetadataGetsReverted
            ()
        {
            var titleToRevert = "My Title to revert!";

            Job job = null;
            _jobDataUpdater
                .When(x => x.UpdateTokensAndMetadata(Arg.Any<Job>()))
                .Do(x =>
                {
                    job = x.Arg<Job>();
                    job.JobInfo.Metadata.Title = titleToRevert;
                });

            _query.When(x => x.ComposeTargetFilePath(Arg.Any<Job>())).Do(x => { throw new ManagePrintJobsException(); });
            Assert.Throws<ManagePrintJobsException>(() => _workflow.RunWorkflow(_job), "Did not throw exception");
            Assert.AreNotEqual(titleToRevert, job.JobInfo.Metadata.Title, "Metadata not reverted");
        }

        [Test]
        public void RunWorkFlow_WorkflowExceptionGetsCatched_WorkflowStepIsError()
        {
            SetUpConditionsForCompleteWorkflow();
            _query.When(x => x.ComposeTargetFilePath(Arg.Any<Job>())).Do(x => { throw new WorkflowException("message"); });

            var workflowResult = _workflow.RunWorkflow(_job);
            Assert.AreEqual(WorkflowResult.Error, workflowResult);
        }

        [Test]
        public void RunWorkFlow_WithErrorsInProfileCheck_ThrowsProcessingException()
        {
            var errorResult = new ActionResult(ErrorCode.Conversion_UnknownError);
            SetUpConditionsForCompleteWorkflow();
            _profileChecker.CheckJob(_job).Returns(errorResult);

            var workflowResult = _workflow.RunWorkflow(_job);
            Assert.AreEqual(WorkflowResult.Error, workflowResult);
        }

        [Test]
        public void AutoSaveWorkflow_NotificationWasCalled()
        {
            var workflowResult = _workflow.RunWorkflow(_job);

            _notificationService.Received().ShowInfoNotification(Arg.Any<string>(), Arg.Any<string>());
            _notificationService.DidNotReceive().ShowErrorNotification(Arg.Any<string>());
        }

        [Test]
        public void AutoSaveWorkflow_InfoNotificationsDisabled_DoesNotCallNotifications()
        {
            _job.Profile.ShowAllNotifications = false;
            _job.Profile.ShowOnlyErrorNotifications = true;

            var workflowResult = _workflow.RunWorkflow(_job);

            _notificationService.DidNotReceive().ShowInfoNotification(Arg.Any<string>(), Arg.Any<string>());
            _notificationService.DidNotReceive().ShowErrorNotification(Arg.Any<string>());
        }

        [Test]
        public void AutoSaveWorkflow_OnlyErrorNotificationsEnabled_DoesNotCallNotifications()
        {
            _job.Profile.ShowAllNotifications = false;
            _job.Profile.ShowOnlyErrorNotifications = true;

            var workflowResult = _workflow.RunWorkflow(_job);

            _notificationService.DidNotReceive().ShowInfoNotification(Arg.Any<string>(), Arg.Any<string>());
            _notificationService.DidNotReceive().ShowErrorNotification(Arg.Any<string>());
        }

        [Test]
        public void AutoSaveWorkflow_DoWorkflowThrowsError_NotificationErrorWasCalled()
        {
            var errorResult = new ActionResult(ErrorCode.Conversion_UnknownError);
            SetUpConditionsForCompleteWorkflow();
            _profileChecker.CheckJob(_job).Returns(errorResult);

            var workflowResult = _workflow.RunWorkflow(_job);

            _notificationService.DidNotReceive().ShowInfoNotification(Arg.Any<string>(), Arg.Any<string>());
            _notificationService.Received().ShowErrorNotification(Arg.Any<string>());
        }

        [Test]
        public void AutoSaveWorkflow_DoWorkflowThrowsErrorAndNotificationsDisabled_DoesNotCallNotifications()
        {
            _job.Profile.ShowAllNotifications = false;
            _job.Profile.ShowOnlyErrorNotifications = false;

            var errorResult = new ActionResult(ErrorCode.Conversion_UnknownError);
            SetUpConditionsForCompleteWorkflow();
            _profileChecker.CheckJob(_job).Returns(errorResult);

            var workflowResult = _workflow.RunWorkflow(_job);

            _notificationService.DidNotReceive().ShowInfoNotification(Arg.Any<string>(), Arg.Any<string>());
            _notificationService.DidNotReceive().ShowErrorNotification(Arg.Any<string>());
        }

        [Test]
        public void RunWorkflow_CallsJobEventsManager()
        {
            var workflowResult = _workflow.RunWorkflow(_job);

            _jobEventsManager.Received().RaiseJobCompleted(Arg.Any<Job>(), Arg.Any<TimeSpan>());
        }

        [Test]
        public void RunWorkflow_DoWorkflowThrowsError_CallsJobEventsManager()
        {
            var errorResult = new ActionResult(ErrorCode.Conversion_UnknownError);
            SetUpConditionsForCompleteWorkflow();
            _profileChecker.CheckJob(_job).Returns(errorResult);

            var workflowResult = _workflow.RunWorkflow(_job);

            _jobEventsManager.Received().RaiseJobFailed(Arg.Any<Job>(), Arg.Any<TimeSpan>(), FailureReason.Error);
        }
    }
}
