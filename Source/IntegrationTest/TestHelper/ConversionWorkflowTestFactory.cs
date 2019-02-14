using pdfforge.PDFCreator.Core.Services.JobEvents;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Core.Workflow.ComposeTargetFilePath;
using pdfforge.PDFCreator.Core.Workflow.Output;

namespace PDFCreator.TestUtilities
{
    public class ConversionWorkflowTestFactory
    {
        private readonly IJobDataUpdater _jobDataUpdater;
        private readonly IJobEventsManager _jobEventsManager;
        private readonly AutosaveOutputFileMover _outputFileMover;
        private readonly IJobRunner _jobRunner;
        private readonly IProfileChecker _profileChecker;
        private readonly ITargetFilePathComposer _targetFilePathComposer;

        public ConversionWorkflowTestFactory(IProfileChecker profileChecker, ITargetFilePathComposer targetFilePathComposer,
            IJobRunner jobRunner, IJobDataUpdater jobDataUpdater, AutosaveOutputFileMover outputFileMover, IJobEventsManager jobEventsManager)
        {
            _profileChecker = profileChecker;
            _targetFilePathComposer = targetFilePathComposer;
            _jobRunner = jobRunner;
            _jobDataUpdater = jobDataUpdater;
            _jobEventsManager = jobEventsManager;
            _outputFileMover = outputFileMover;
        }

        public ConversionWorkflow BuildWorkflow()
        {
            return new AutoSaveWorkflow(_jobDataUpdater, _jobRunner, _profileChecker, _targetFilePathComposer,
                                        _outputFileMover, new TestNotificationService(), _jobEventsManager);
        }
    }

    internal class TestNotificationService : INotificationService
    {
        public void ShowInfoNotification(string documentName, string documentPath)
        {
        }

        public void ShowErrorNotification(string documentName)
        {
        }
    }
}
