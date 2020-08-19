using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Presentation.Workflow;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public interface IChangeJobCheckAndProceedCommandBuilder
    {
        void Init(Func<Job> getJob, Action callFinishInteraction, Func<string> getLatestConfirmedPath, Action<string> setLatestConfirmedPath);

        IAsyncCommand BuildCommand(Action<Job> changeJobAction, IMacroCommand preSaveCommand = null);
    }

    public class ChangeJobCheckAndProceedCommandBuilder : IChangeJobCheckAndProceedCommandBuilder
    {
        private readonly IInteractiveProfileChecker _interactiveProfileChecker;
        private readonly IInteractiveFileExistsChecker _interactiveFileExistsChecker;
        private Func<Job> _getJob;
        private Action _callFinishInteraction;
        private Func<string> _getLatestConfirmedPath;
        private Action<string> _setLatestConfirmedPath;

        public ChangeJobCheckAndProceedCommandBuilder(
            IInteractiveProfileChecker interactiveProfileChecker,
            IInteractiveFileExistsChecker interactiveFileExistsChecker)
        {
            _interactiveProfileChecker = interactiveProfileChecker;
            _interactiveFileExistsChecker = interactiveFileExistsChecker;
        }

        public void Init(Func<Job> getJob, Action callFinishInteraction, Func<string> getLatestConfirmedPath, Action<string> setLatestConfirmedPath)
        {
            _getJob = getJob;
            _callFinishInteraction = callFinishInteraction;
            _getLatestConfirmedPath = getLatestConfirmedPath;
            _setLatestConfirmedPath = setLatestConfirmedPath;
        }

        public IAsyncCommand BuildCommand(Action<Job> changeJobAction, IMacroCommand preSaveCommand = null)
        {
            if (_getJob == null || _callFinishInteraction == null || _getLatestConfirmedPath == null || _setLatestConfirmedPath == null)
                throw new InvalidOperationException($"Call {nameof(ProceedWithChangedJobCommand)}.Init first!");

            return new ProceedWithChangedJobCommand(
                _interactiveProfileChecker,
                _interactiveFileExistsChecker,
                _getJob,
                _callFinishInteraction,
                _getLatestConfirmedPath,
                _setLatestConfirmedPath,
                changeJobAction,
                preSaveCommand);
        }
    }

    public class ProceedWithChangedJobCommand : AsyncCommandBase
    {
        private readonly IInteractiveProfileChecker _interactiveProfileChecker;
        private readonly IInteractiveFileExistsChecker _interactiveFileExistsChecker;
        private readonly Func<Job> _getJob;
        private readonly Action<Job> _changeJobAction;
        private readonly Action _callFinishInteraction;
        private readonly Func<string> _getLatestConfirmedPath;
        private readonly Action<string> _setLatestConfirmedPath;
        private readonly IMacroCommand _preProcessingCommand;

        private readonly OutputFormatHelper _outputFormatHelper = new OutputFormatHelper();

        public ProceedWithChangedJobCommand(
            IInteractiveProfileChecker interactiveProfileChecker,
            IInteractiveFileExistsChecker interactiveFileExistsChecker,
            Func<Job> getJob,
            Action callFinishInteraction,
            Func<string> getLatestConfirmedPath,
            Action<string> setLatestConfirmedPath,
            Action<Job> changeJobAction,
            IMacroCommand preProcessingCommand = null)
        {
            _interactiveProfileChecker = interactiveProfileChecker;
            _interactiveFileExistsChecker = interactiveFileExistsChecker;
            _getJob = getJob;
            _changeJobAction = changeJobAction;
            _callFinishInteraction = callFinishInteraction;

            _getLatestConfirmedPath = getLatestConfirmedPath;
            _setLatestConfirmedPath = setLatestConfirmedPath;
            _preProcessingCommand = preProcessingCommand;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        private ConversionProfile _storedProfile;
        private Metadata _storedMetadata;
        private string _storedOutputFileTemplate;

        private void StoreOriginalJob(Job job)
        {
            _storedProfile = job.Profile.Copy();
            _storedMetadata = job.JobInfo.Metadata.Copy();
            _storedOutputFileTemplate = job.OutputFileTemplate;
        }

        private void RestoreOriginalJob(Job job)
        {
            job.Profile = _storedProfile;
            job.JobInfo.Metadata = _storedMetadata;
            job.OutputFileTemplate = _storedOutputFileTemplate;
        }

        public override async Task ExecuteAsync(object parameter)
        {
            var job = _getJob();
            StoreOriginalJob(job);
            _changeJobAction(job);

            var success = true;
            if (_preProcessingCommand != null)
                success = await _preProcessingCommand.ExecuteAsync(parameter) == ResponseStatus.Success;
            if (!success)
            {
                RestoreOriginalJob(job);
                return;
            }

            if (await CheckJob(job))
                _callFinishInteraction();
            else
                RestoreOriginalJob(job);
        }

        private async Task<bool> CheckJob(Job job)
        {
            //Ensure extension before the checks
            job.OutputFileTemplate = _outputFormatHelper.EnsureValidExtension(job.OutputFileTemplate, job.Profile.OutputFormat);

            if (!_interactiveProfileChecker.CheckWithErrorResultInOverlay(job))
                return false;

            var fileExistsResult = await _interactiveFileExistsChecker.CheckIfFileExistsWithResultInOverlay(job, _getLatestConfirmedPath());
            _setLatestConfirmedPath(fileExistsResult.ConfirmedPath);
            return fileExistsResult.Success;
        }
    }
}
