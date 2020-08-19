using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.JobEvents;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Core.Workflow.ComposeTargetFilePath;
using pdfforge.PDFCreator.Core.Workflow.Queries;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.Utilities.IO;

namespace pdfforge.PDFCreator.UI.Presentation.Workflow
{
    public sealed class InteractiveWorkflow : ConversionWorkflow
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IShellManager _shellManager;
        private readonly IErrorNotifier _errorNotifier;
        private readonly ISettingsProvider _settingsProvider;
        private readonly ICommandLocator _commandLocator;
        private readonly ILastSaveDirectoryHelper _lastSaveDirectoryHelper;
        private readonly IDirectoryHelper _directoryHelper;
        private readonly IInteractiveProfileChecker _interactiveProfileChecker;
        private readonly ITargetFilePathComposer _targetFilePathComposer;

        public InteractiveWorkflow(IShellManager shellManager, ITargetFilePathComposer targetFilePathComposer, IJobDataUpdater jobDataUpdater,
                                   IErrorNotifier errorNotifier, ISettingsProvider settingsProvider,
                                   ICommandLocator commandLocator, ILastSaveDirectoryHelper lastSaveDirectoryHelper,
                                   IDirectoryHelper directoryHelper, IInteractiveProfileChecker interactiveProfileChecker,
                                   IJobEventsManager jobEventsManager
            )
        {
            _shellManager = shellManager;
            _errorNotifier = errorNotifier;
            _settingsProvider = settingsProvider;
            _commandLocator = commandLocator;
            _lastSaveDirectoryHelper = lastSaveDirectoryHelper;
            _directoryHelper = directoryHelper;
            _interactiveProfileChecker = interactiveProfileChecker;
            _targetFilePathComposer = targetFilePathComposer;

            JobDataUpdater = jobDataUpdater;
            JobEventsManager = jobEventsManager;
            _targetFilePathComposer = targetFilePathComposer;
        }

        protected override IJobEventsManager JobEventsManager { get; }
        protected override IJobDataUpdater JobDataUpdater { get; }

        protected override void DoWorkflowWork(Job job)
        {
            job.OutputFileTemplate = _targetFilePathComposer.ComposeTargetFilePath(job);

            job.Passwords = JobPasswordHelper.GetJobPasswords(job.Profile, job.Accounts);  // Set passwords for a skipped print job window

            if (job.Profile.SkipPrintDialog)
            {
                if (!job.Profile.SaveFileTemporary && _interactiveProfileChecker.CheckWithErrorResultInWindow(job))
                {
                    _commandLocator.GetCommand<SkipPrintDialogCommand>().Execute(job);
                }
                else
                {
                    //Enable PrintJobView for invalid profiles
                    job.Profile.SkipPrintDialog = false;
                }
            }

            _logger.Debug("Starting PrintJobWindow");

            try
            {
                _shellManager.ShowPrintJobShell(job);
                _settingsProvider.Settings.CreatorAppSettings.LastUsedProfileGuid = job.Profile.Guid;

                if (job.IsSuccessful)
                {
                    _lastSaveDirectoryHelper.Save(job);
                }
            }
            finally
            {
                _directoryHelper.DeleteCreatedDirectories();
            }
        }

        protected override void HandleError(ErrorCode errorCode)
        {
            _errorNotifier.Notify(new ActionResult(errorCode));
        }
    }
}
