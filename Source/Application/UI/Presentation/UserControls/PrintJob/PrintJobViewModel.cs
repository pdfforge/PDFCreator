using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Query;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Core.Workflow.ComposeTargetFilePath;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using pdfforge.PDFCreator.Core.Workflow.Queries;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Commands.ProfileCommands;
using pdfforge.PDFCreator.UI.Presentation.Events;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.UI.Presentation.Workflow;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.IO;
using Prism.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public class PrintJobViewModel : TranslatableViewModelBase<PrintJobViewTranslation>, IWorkflowViewModel
    {
        private TaskCompletionSource<object> _taskCompletionSource = new TaskCompletionSource<object>();
        public IGpoSettings GpoSettings { get; }
        private readonly ISettingsProvider _settingsProvider;
        private readonly IFileNameQuery _saveFileQuery;
        private readonly IInteractionRequest _interactionRequest;
        private readonly ISelectedProfileProvider _selectedProfileProvider;
        private readonly ICurrentSettings<ObservableCollection<ConversionProfile>> _profilesProvider;
        private readonly IFile _file;
        private readonly ITempFolderProvider _tempFolderProvider;
        private readonly IDispatcher _dispatcher;
        private readonly IDirectoryHelper _directoryHelper;
        private readonly IInteractiveProfileChecker _interactiveProfileChecker;
        private readonly ITargetFilePathComposer _targetFilePathComposer;
        private string _outputFolder = "";
        private string _outputFilename = "";
        private string _latestDialogFilePath = "";
        private readonly OutputFormatHelper _outputFormatHelper = new OutputFormatHelper();

        public PrintJobViewModel(
            ISettingsProvider settingsProvider,
            ITranslationUpdater translationUpdater,
            IJobInfoQueue jobInfoQueue,
            IFileNameQuery saveFileQuery,
            IInteractionRequest interactionRequest,
            ICommandLocator commandsLocator,
            IEventAggregator eventAggregator,
            ISelectedProfileProvider selectedProfileProvider,
            ICurrentSettings<ObservableCollection<ConversionProfile>> profilesProvider,
            ITempFolderProvider tempFolderProvider,
            IFile file,
            IGpoSettings gpoSettings,
            IDispatcher dispatcher,
            IDirectoryHelper directoryHelper,
            IInteractiveProfileChecker interactiveProfileChecker,
            ITargetFilePathComposer targetFilePathComposer)

            : base(translationUpdater)
        {
            GpoSettings = gpoSettings;
            _settingsProvider = settingsProvider;
            _saveFileQuery = saveFileQuery;
            _interactionRequest = interactionRequest;
            _selectedProfileProvider = selectedProfileProvider;
            _profilesProvider = profilesProvider;
            _file = file;
            _tempFolderProvider = tempFolderProvider;
            _dispatcher = dispatcher;
            _directoryHelper = directoryHelper;
            _interactiveProfileChecker = interactiveProfileChecker;
            _targetFilePathComposer = targetFilePathComposer;
            SaveCommand = new DelegateCommand(SaveExecute);
            SendByEmailCommand = new DelegateCommand(EmailExecute);
            MergeCommand = new DelegateCommand(MergeExecute);
            CancelCommand = new DelegateCommand(CancelExecute);
            SetOutputFormatCommand = new DelegateCommand<OutputFormat>(SetOutputFormatExecute);
            BrowseFileCommandAsync = new AsyncCommand(BrowseFileExecute);

            SetupEditProfileCommand(commandsLocator, eventAggregator);

            var settings = settingsProvider.Settings?.CopyAndPreserveApplicationSettings();
            Profiles = settings?.ConversionProfiles;

            jobInfoQueue.OnNewJobInfo += (sender, args) => UpdateNumberOfPrintJobsHint(jobInfoQueue.Count);
            UpdateNumberOfPrintJobsHint(jobInfoQueue.Count);

            var profileListView = CollectionViewSource.GetDefaultView(Profiles);
            if (profileListView != null)
            {
                profileListView.CurrentChanged += (sender, args) =>
                {
                    if (Job != null)
                        OutputFormat = ((ConversionProfile)profileListView.CurrentItem).OutputFormat;
                };
            }
        }

        private void SetupEditProfileCommand(ICommandLocator commandsLocator, IEventAggregator eventAggregator)
        {
            if (commandsLocator != null)
            {
                EditProfileCommand = commandsLocator.CreateMacroCommand()
                    .AddCommand(new DelegateCommand(SetProfileForSelection))
                    .AddCommand<ShowLockLayerCommand>()
                    .AddCommand<OpenProfileCommand>()
                    .AddCommand<WaitProfileModificationCommand>()
                    .AddCommand(new DelegateCommand(x => eventAggregator.GetEvent<CloseMainWindowEvent>().Publish()))
                    .AddCommand(new DelegateCommand(UpdateJobProfileFromCurrentProperties))
                    .AddCommand<HideLockLayerCommand>()
                    .Build();
            }
        }

        private void UpdateJobProfileFromCurrentProperties(object obj)
        {
            _dispatcher.BeginInvoke(() =>
            {
                var settings = _settingsProvider.Settings?.Copy();
                Profiles = settings?.ConversionProfiles;
                if (Profiles != null)
                {
                    SelectedProfile = Profiles.First(profile => profile.Name == _selectedProfileProvider.SelectedProfile.Name);
                }
            });
        }

        private void SetProfileForSelection(object o)
        {
            _selectedProfileProvider.SelectedProfile = _profilesProvider.Settings.First(profile => profile.Name == Job.Profile.Name);
        }

        private void SetOutputFormatExecute(OutputFormat parameter)
        {
            OutputFormat = parameter;
        }

        private void EnsureValidExtensionInFilename()
        {
            if (Job?.Profile?.OutputFormat == null)
                return;

            var outputFormat = Job.Profile.OutputFormat;
            OutputFilename = _outputFormatHelper.EnsureValidExtension(OutputFilename, outputFormat);
        }

        private void SetOutputFilePathInJob()
        {
            Job.OutputFileTemplate = PathSafe.Combine(OutputFolder, OutputFilename);
        }

        private async Task BrowseFileExecute(object parameter)
        {
            _directoryHelper.CreateDirectory(OutputFolder);

            var result = await GetFileOrRetry();

            if (result.Success)
            {
                SetOutputFilenameAndFolder(result.Data.Filepath);
                _latestDialogFilePath = result.Data.Filepath;
                OutputFormat = result.Data.OutputFormat;
            }
        }

        private async Task<QueryResult<OutputFilenameResult>> GetFileOrRetry()
        {
            // Retry while there is a PathTooLongException
            while (true)
            {
                try
                {
                    return _saveFileQuery.GetFileName(OutputFolder, OutputFilename, OutputFormat);
                }
                catch (PathTooLongException)
                {
                    var interaction = new MessageInteraction(Translation.PathTooLongText, Translation.PathTooLongTitle, MessageOptions.OK, MessageIcon.Exclamation);
                    await _interactionRequest.RaiseAsync(interaction);
                }
            }
        }

        public Metadata Metadata => Job?.JobInfo?.Metadata;

        private void UpdateMetadata()
        {
            Job.InitMetadataWithTemplatesFromProfile();
            Job.ReplaceTokensInMetadata();
            RaisePropertyChanged(nameof(Metadata));
        }

        private void SetOutputFilenameAndFolder(string filenameTemplate)
        {
            OutputFilename = PathSafe.GetFileName(filenameTemplate);
            OutputFolder = PathSafe.GetDirectoryName(filenameTemplate);
        }

        private void UpdateNumberOfPrintJobsHint(int numberOfPrintJobs)
        {
            if (numberOfPrintJobs <= 1)
                NumberOfPrintJobsHint = "";
            else if (numberOfPrintJobs > 99)
            {
                NumberOfPrintJobsHint = "99+";
            }
            else
            {
                NumberOfPrintJobsHint = numberOfPrintJobs.ToString();
            }

            RaisePropertyChanged(nameof(NumberOfPrintJobsHint));
        }

        public Task ExecuteWorkflowStep(Job job)
        {
            SetJob(job);
            return _taskCompletionSource.Task;
        }

        private void SaveExecute(object obj)
        {
            EnsureValidExtensionInFilename(); //Ensure extension before the checks

            if (_interactiveProfileChecker.CheckWithErrorResultInOverlay(Job))
                if (CheckIfFileExistsElseNotifyUser())
                    CallFinishInteraction();
        }

        private bool CheckIfFileExistsElseNotifyUser()
        {
            var filePath = PathSafe.Combine(OutputFolder, OutputFilename);

            //Do not inform user, if SaveFileDialog already did
            if (filePath == _latestDialogFilePath)
                return true;

            if (!_file.Exists(filePath))
                return true;

            var title = Translation.ConfirmSaveAs.ToUpper(CultureInfo.CurrentCulture);
            var text = Translation.GetFileAlreadyExists(filePath);

            var interaction = new MessageInteraction(text, title, MessageOptions.YesNo, MessageIcon.Exclamation);

            _interactionRequest.Raise(interaction, NotifyFileExistsCallback);

            return false;
        }

        private void CallFinishInteraction()
        {
            Job.Passwords = JobPasswordHelper.GetJobPasswords(Job.Profile, Job.Accounts); //todo: Why here? Aren't we doing that already somewhere else?
            FinishInteraction();
        }

        private void NotifyFileExistsCallback(MessageInteraction interaction)
        {
            if (interaction.Response == MessageResponse.Yes)
            {
                CallFinishInteraction();
            }
            _latestDialogFilePath = "";
        }

        private void EmailExecute(object obj)
        {
            var tempDirectory = PathSafe.Combine(_tempFolderProvider.TempFolder,
                Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
            Directory.CreateDirectory(tempDirectory);

            Job.OutputFileTemplate = PathSafe.Combine(tempDirectory, OutputFilename);

            if (!_interactiveProfileChecker.CheckWithErrorResultInOverlay(Job))
            {
                try
                {
                    Directory.Delete(tempDirectory);
                }
                catch { }
            }

            Job.Profile.EmailClientSettings.Enabled = true;
            Job.Profile.OpenViewer = false;

            CallFinishInteraction();
        }

        private void CancelExecute(object obj)
        {
            // This needs to be called before the exceptions are thrown
            FinishInteraction();
            throw new AbortWorkflowException("User cancelled in the PrintJobView");
        }

        private void MergeExecute(object obj)
        {
            // This needs to be called before the exceptions are thrown
            FinishInteraction();
            throw new ManagePrintJobsException();
        }

        private void FinishInteraction()
        {
            _taskCompletionSource.SetResult(null);
        }

        public void SetJob(Job job)
        {
            if (job != null)
                Job = job;
        }

        private Job _job;

        public Job Job
        {
            get { return _job; }
            private set
            {
                _job = value;
                RaisePropertyChanged();
                SelectedProfile = _job.Profile;
            }
        }

        public ConversionProfile SelectedProfile
        {
            get { return Job?.Profile; }
            set
            {
                if (Job == null)
                    return;
                Job.Profile = value;
                RaisePropertyChanged();
                Job.OutputFileTemplate = _targetFilePathComposer.ComposeTargetFilePath(Job);
                SetOutputFilenameAndFolder(Job.OutputFileTemplate);
                EnsureValidExtensionInFilename();
                RaisePropertyChanged(nameof(OutputFormat));
                UpdateMetadata();
            }
        }

        public string NumberOfPrintJobsHint { get; private set; }

        public DelegateCommand<OutputFormat> SetOutputFormatCommand { get; }
        public DelegateCommand SaveCommand { get; }
        public ICommand SendByEmailCommand { get; }
        public IAsyncCommand BrowseFileCommandAsync { get; }
        public ICommand MergeCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand EditProfileCommand { get; private set; }

        public ObservableCollection<ConversionProfile> Profiles { get; private set; }

        public IEnumerable<OutputFormat> OutputFormats => System.Enum.GetValues(typeof(OutputFormat)) as OutputFormat[];

        public OutputFormat OutputFormat
        {
            get { return Job?.Profile?.OutputFormat ?? OutputFormat.Pdf; }
            set
            {
                Job.Profile.OutputFormat = value;
                EnsureValidExtensionInFilename();
                RaisePropertyChanged();
            }
        }

        public string OutputFolder
        {
            get { return _outputFolder; }
            set
            {
                SetProperty(ref _outputFolder, value);
                SetOutputFilePathInJob();
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public string OutputFilename
        {
            get { return _outputFilename; }
            set
            {
                SetProperty(ref _outputFilename, value);
                SetOutputFilePathInJob();
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public bool EditButtonEnabledByGpo => GpoSettings == null || !GpoSettings.DisableProfileManagement;
    }
}
