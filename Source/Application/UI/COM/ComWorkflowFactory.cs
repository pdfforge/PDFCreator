using pdfforge.PDFCreator.Core.ComImplementation;
using pdfforge.PDFCreator.Core.Services.JobEvents;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Core.Workflow.Output;
using SimpleInjector;

namespace pdfforge.PDFCreator.UI.COM
{
    internal class ComWorkflowFactory : IComWorkflowFactory
    {
        private readonly Container _container;

        public ComWorkflowFactory(Container container)
        {
            _container = container;
        }

        public IConversionWorkflow BuildWorkflow(string targetFileName)
        {
            var profileChecker = _container.GetInstance<IProfileChecker>();
            var targetFileNameComposer = new ComTargetFilePathComposer(targetFileName);
            var jobRunner = _container.GetInstance<IJobRunner>();
            var jobDataUpdater = _container.GetInstance<IJobDataUpdater>();
            var jobEventsManager = _container.GetInstance<IJobEventsManager>();
            var outputFileMover = _container.GetInstance<AutosaveOutputFileMover>();
            var notificationService = _container.GetInstance<DisabledNotificationService>();

            return new AutoSaveWorkflow(jobDataUpdater, jobRunner, profileChecker, targetFileNameComposer, outputFileMover, notificationService, jobEventsManager);
        }
    }
}
