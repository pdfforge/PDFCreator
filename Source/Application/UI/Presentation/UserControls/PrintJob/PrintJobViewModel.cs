using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Conversion.Settings.Helpers;
using pdfforge.PDFCreator.Core.JobInfoQueue;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.Core.Services.Trial;
using pdfforge.PDFCreator.Core.SettingsManagementInterface;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Core.Workflow.ComposeTargetFilePath;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Commands.ProfileCommands;
using pdfforge.PDFCreator.UI.Presentation.Events;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Overlay;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.UI.Presentation.Workflow;
using pdfforge.PDFCreator.Utilities;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public class PrintJobViewModel : TranslatableViewModelBase<PrintJobViewTranslation>, IWorkflowViewModel, IMountable
    {
        private readonly TaskCompletionSource<object> _taskCompletionSource = new();
        private IGpoSettings GpoSettings { get; }
        private readonly ISettingsProvider _settingsProvider;
        private readonly ISelectedProfileProvider _selectedProfileProvider;
        private readonly ICurrentSettings<ObservableCollection<ConversionProfile>> _profilesProvider;
        private readonly ITargetFilePathComposer _targetFilePathComposer;
        private readonly IJobInfoManager _jobInfoManager;
        private readonly IChangeJobCheckAndProceedCommandBuilder _changeJobCheckAndProceedCommandBuilder;
        private readonly IEventAggregator _eventAggregator;
        private readonly IDispatcher _dispatcher;
        private readonly IJobDataUpdater _jobDataUpdater;
        private readonly IInteractionRequest _interactionRequest;
        private readonly IProfileChecker _profileChecker;
        private readonly ApplicationNameProvider _applicationNameProvider;
        private string _lastConfirmedFilePath = "";
        private readonly OutputFormatHelper _outputFormatHelper = new OutputFormatHelper();
        private readonly IJobInfoQueue _jobInfoQueue;

        public ICampaignHelper CampaignHelper { get; private set; }

        public string TrialExtendLink => CampaignHelper.GetTrialExtendLink(ExtendLicenseFallbackUrl);

        private string ExtendLicenseFallbackUrl => Urls.GetExtendLicenseFallbackUrl(_applicationNameProvider.EditionName);

        public bool SaveFileTemporaryIsEnabled => SelectedProfile?.SaveFileTemporary ?? false;

        public PrintJobViewModel(
            ISettingsProvider settingsProvider,
            ITranslationUpdater translationUpdater,
            IJobInfoQueue jobInfoQueue,
            ICommandLocator commandLocator,
            IEventAggregator eventAggregator,
            ISelectedProfileProvider selectedProfileProvider,
            ICurrentSettings<ObservableCollection<ConversionProfile>> profilesProvider,
            IGpoSettings gpoSettings,
            ITargetFilePathComposer targetFilePathComposer,
            IJobInfoManager jobInfoManager,
            IChangeJobCheckAndProceedCommandBuilder changeJobCheckAndProceedCommandBuilder,
            IBrowseFileCommandBuilder browseFileCommandBuilder,
            IDispatcher dispatcher,
            IJobDataUpdater jobDataUpdater,
            IInteractionRequest interactionRequest,
            ICampaignHelper campaignHelper,
            IProfileChecker profileChecker,
            ITempFolderProvider tempFolderProvider,
            ApplicationNameProvider applicationNameProvider)
            : base(translationUpdater)
        {
            GpoSettings = gpoSettings;
            _settingsProvider = settingsProvider;
            var commandLocator1 = commandLocator;
            _eventAggregator = eventAggregator;
            _selectedProfileProvider = selectedProfileProvider;
            _profilesProvider = profilesProvider;
            _targetFilePathComposer = targetFilePathComposer;
            _jobInfoManager = jobInfoManager;
            _changeJobCheckAndProceedCommandBuilder = changeJobCheckAndProceedCommandBuilder;
            _dispatcher = dispatcher;
            _jobDataUpdater = jobDataUpdater;
            _interactionRequest = interactionRequest;
            _applicationNameProvider = applicationNameProvider;
            _profileChecker = profileChecker;
            _changeJobCheckAndProceedCommandBuilder.Init(() => Job, CallFinishInteraction, () => _lastConfirmedFilePath, s => _lastConfirmedFilePath = s);
            

            CampaignHelper = campaignHelper;
            SetOutputFormatCommand = new DelegateCommand(SetOutputFormatExecute);

            browseFileCommandBuilder.Init(() => Job, UpdateUiForJobOutputFileTemplate, () => _lastConfirmedFilePath, s => _lastConfirmedFilePath = s);
            var existingFileBehaviourQueryCommand = new AsyncCommand(ExistingFileBehaviourQuery);
            var browseFileCommand = browseFileCommandBuilder.BuildCommand(new List<ICommand> { existingFileBehaviourQueryCommand });

            BrowseFileCommand = browseFileCommand;
            SetupEditProfileCommand(commandLocator1, eventAggregator);

            SetupSaveCommands(translationUpdater);

            EmailCommand = _changeJobCheckAndProceedCommandBuilder.BuildCommand(EnableEmailSettings);
            SendEmailWithoutSavingCommand = _changeJobCheckAndProceedCommandBuilder.BuildCommand(SendEmailWithoutSavingExecute);

            MergeCommand = new DelegateCommand(MergeExecute);
            MergeAllCommand = new AsyncCommand(MergeAllExecuteAsync, o => jobInfoQueue.Count > 1);
            CancelCommand = new DelegateCommand(CancelExecute);
            CancelAllCommand = new DelegateCommand(CancelAllExecute, o => jobInfoQueue.Count > 1);

            DisableSaveTempOnlyCommand = new DelegateCommand(DisableSaveFileTemporaryExecute);
            OpenUrlCommand = commandLocator1.GetCommand<UrlOpenCommand>();

            jobInfoQueue.OnNewJobInfo += (sender, args) => UpdateNumberOfPrintJobsHint(jobInfoQueue.Count);
            _jobInfoQueue = jobInfoQueue;
            UpdateNumberOfPrintJobsHint(jobInfoQueue.Count);
            OnTranslationChanged();
        }

        private async Task ExistingFileBehaviourQuery(object obj)
        {
            // file does not exist
            if (!File.Exists(_lastConfirmedFilePath))
                return;

            var interaction = new OverwriteOrAppendInteraction { MergeIsSupported = Job.Profile.OutputFormat.IsPdf() };

            var result = await _interactionRequest.RaiseAsync(interaction);
            switch (result.Chosen)
            {
                case ExistingFileBehaviour.Overwrite:
                    Job.ExistingFileBehavior = ExistingFileBehaviour.Overwrite;
                    break;

                case ExistingFileBehaviour.Merge:
                    Job.ExistingFileBehavior = ExistingFileBehaviour.Merge;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (result.Cancel)
                await BrowseFileCommand.ExecuteAsync(null);
        }

        private void ManagePrintJobEvent()
        {
            _dispatcher.BeginInvoke(() => MergeExecute(null));
        }

        private void EnableEmailSettings(object o)
        {
            Job.Profile.EmailClientSettings.Enabled = true;
            Job.Profile.OpenViewer.Enabled = false;
            if (!Job.Profile.ActionOrder.Contains(nameof(EmailClientSettings)))
                Job.Profile.ActionOrder.Insert(0, nameof(EmailClientSettings));
        }

        private void SendEmailWithoutSavingExecute(object o)
        {
            //Set FilenameTemplate and SaveFileTemporary to consider it in following ComposeTargetFilePath
            Job.Profile.SaveFileTemporary = true;
            Job.Profile.FileNameTemplate = OutputFilename;
            Job.OutputFileTemplate = _targetFilePathComposer.ComposeTargetFilePath(Job);
            
            Job.Profile.EmailClientSettings.Enabled = true;
            Job.Profile.OpenViewer.Enabled = false;
            if (!Job.Profile.ActionOrder.Contains(nameof(EmailClientSettings)))
                Job.Profile.ActionOrder.Insert(0, nameof(EmailClientSettings));
        }

        private void DisableSaveFileTemporaryExecute(object obj)
        {
            SelectedProfile.SaveFileTemporary = false;
            OutputFolder = Job.Profile.TargetDirectory;
            RaisePropertyChanged(nameof(SaveFileTemporaryIsEnabled));
        }

        private void EnableSaveToDesktop(Job job)
        {
            job.Profile.SaveFileTemporary = false;
            var filename = PathSafe.GetFileName(job.OutputFileTemplate);
            var desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            job.OutputFileTemplate = PathSafe.Combine(desktopFolder, filename);
        }

        private void SetupSaveCommands(ITranslationUpdater translationUpdater)
        {
            SaveCommand = _changeJobCheckAndProceedCommandBuilder.BuildCommand(j =>
            {
                _jobDataUpdater.UpdateTokensAndMetadata(j);
            });

            SaveAsCommand = _changeJobCheckAndProceedCommandBuilder.BuildCommand(DisableSaveFileTemporaryExecute, BrowseFileCommand);
            SaveToDesktopCommand = _changeJobCheckAndProceedCommandBuilder.BuildCommand(EnableSaveToDesktop);
        }

        private void SetupEditProfileCommand(ICommandLocator commandsLocator, IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<EditSettingsFinishedEvent>().Subscribe(p =>
            {
                if (Job == null)
                    return;

                Job.Profile = p;
                InitCombobox();
                UpdateAfterEditing(null);
            });

            if (commandsLocator != null)
            {
                EditProfileCommand = commandsLocator.CreateMacroCommand()
                    .AddCommand(new DelegateCommand(x => eventAggregator.GetEvent<CloseMainWindowEvent>().Publish()))
                    .AddCommand(new DelegateCommand(StartEditing))
                    .AddCommand<ShowLockLayerCommand>()
                    .AddCommand<WaitMainShellClosedCommand>()
                    .AddCommand<OpenProfileCommand>()
                    .AddCommand<WaitProfileModificationCommand>()
                    .AddCommand(new DelegateCommand(x => eventAggregator.GetEvent<CloseMainWindowEvent>().Publish()))
                    .AddCommand<HideLockLayerCommand>()
                    .Build();
            }
        }

        private void StartEditing(object obj)
        {
            _selectedProfileProvider.SelectedProfile = _profilesProvider.Settings.First(profile => profile.Guid == Job.Profile.Guid);

            ProfilesWrapper = null;
        }

        private void UpdateAfterEditing(object obj)
        {
            _dispatcher.BeginInvoke(() =>
            {
                if (SelectedProfile == null)
                    return;

                ProfilesWrapper = _settingsProvider.Settings?.ConversionProfiles.Select(x => new ConversionProfileWrapper(x)).ToObservableCollection();

                SelectedProfileWrapper = ProfilesWrapper?.FirstOrDefault(x => x.ConversionProfile.Guid == SelectedProfile.Guid)
                                         ?? ProfilesWrapper?.FirstOrDefault();

                Job.CurrentSettings.Accounts = _settingsProvider?.Settings?.ApplicationSettings.Accounts.Copy();

                // Important: RaisePropertyChanged for ProfilesWrapper must be called at the end.
                // Otherwise, the UI will update the binding source and overwrite the selected profile.
                RaisePropertyChanged(nameof(ProfilesWrapper));
            });
        }

        private void SetOutputFormatExecute(object parameter)
        {
            OutputFormat = (OutputFormat)parameter;
        }

        private string EnsureValidExtensionInFilename(string fileName, OutputFormat format)
        {
            if (Job?.Profile?.OutputFormat == null)
                return "";

            return _outputFormatHelper.EnsureValidExtension(fileName, format);
        }

        public Metadata Metadata => Job?.JobInfo?.Metadata;

        private void UpdateMetadata()
        {
            Job.InitMetadataWithTemplatesFromProfile();
            Job.ReplaceTokensInMetadata();
            RaisePropertyChanged(nameof(Metadata));
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
            SetNewJob(job);
            return _taskCompletionSource.Task;
        }

        private void CallFinishInteraction()
        {
            Job.Passwords = JobPasswordHelper.GetJobPasswords(Job.Profile, Job.Accounts); // Set passwords in case the profile has changed
            FinishInteraction();
        }

        private void CancelExecute(object obj)
        {
            // This needs to be called before the exceptions are thrown
            FinishInteraction();
            throw new AbortWorkflowException("User cancelled in the PrintJobView");
        }

        private void CancelAllExecute(object obj)
        {
            _jobInfoQueue.Clear();
            CancelExecute(obj);
        }

        private void MergeExecute(object obj)
        {
            // This needs to be called before the exceptions are thrown
            FinishInteraction();
            throw new ManagePrintJobsException();
        }

        private async Task MergeAllExecuteAsync(object arg)
        {
            await Task.Run(MergeAllExecute);

            UpdateNumberOfPrintJobsHint(_jobInfoQueue.JobInfos.Count);
        }

        private bool MergeAllExecute()
        {
            var jobInfosCopy = _jobInfoQueue.JobInfos.ToList();
            var first = jobInfosCopy.First();

            foreach (var jobObject in jobInfosCopy.Skip(1))
            {
                var job = (JobInfo)jobObject;
                if (job.JobType != first.JobType)
                    continue;

                _jobInfoManager.Merge(first, job);
                _jobInfoQueue.Remove(job, false);
                _jobInfoManager.DeleteInf(job);
            }

            _jobInfoManager.SaveToInfFile(first);

            return true;
        }

        private void FinishInteraction()
        {
            _taskCompletionSource.SetResult(null);
        }

        public void SetNewJob(Job job)
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

        public void UpdateUiForJobOutputFileTemplate()
        {
            RaisePropertyChanged(nameof(OutputFormat));
            RaisePropertyChanged(nameof(OutputFilename));
            RaisePropertyChanged(nameof(OutputFolder));
        }

        public ConversionProfile SelectedProfile
        {
            get { return Job?.Profile; }
            set
            {
                if (Job == null)
                    return;
                Job.Profile = value.Copy();
                _dispatcher.InvokeAsync(async () => await UpdateProfileData());
                RaisePropertyChanged();
            }
        }

        public async Task SetSelectedProfileAsync(ConversionProfile profile)
        {
            IsUpdatingProfile = true;
            RaisePropertyChanged(nameof(IsUpdatingProfile));

            Job.Profile = profile.Copy();
            await UpdateProfileData();

            IsUpdatingProfile = false;
            RaisePropertyChanged(nameof(IsUpdatingProfile));
        }

        private async Task UpdateProfileData()
        {
            await _jobDataUpdater.UpdateTokensAndMetadataAsync(Job);

            RaisePropertyChanged(nameof(SelectedProfile));
            Job.OutputFileTemplate = _targetFilePathComposer.ComposeTargetFilePath(Job);
            OutputFilename = EnsureValidExtensionInFilename(OutputFilename, OutputFormat);
            UpdateUiForJobOutputFileTemplate();

            RaisePropertyChanged(nameof(SaveFileTemporaryIsEnabled));
            UpdateMetadata();
        }

        public string NumberOfPrintJobsHint { get; private set; }

        public DelegateCommand SetOutputFormatCommand { get; }

        public ICommand OpenUrlCommand { get; }

        public IAsyncCommand SaveCommand { get; private set; }
        public IAsyncCommand SaveAsCommand { get; private set; }
        public IAsyncCommand SaveToDesktopCommand { get; private set; }

        public ICommand EmailCommand { get; }
        public ICommand SendEmailWithoutSavingCommand { get; }

        public IMacroCommand BrowseFileCommand { get; }
        public ICommand MergeCommand { get; }
        public AsyncCommand MergeAllCommand { get; }

        public ICommand CancelCommand { get; }
        public DelegateCommand CancelAllCommand { get; }

        public ICommand EditProfileCommand { get; private set; }
        public ICommand DisableSaveTempOnlyCommand { get; set; }

        private ConversionProfileWrapper _selectedProfileWrapper = null;

        public bool IsUpdatingProfile { get; private set; }

        public ConversionProfileWrapper SelectedProfileWrapper
        {
            get { return _selectedProfileWrapper; }
            set
            {
                if (value == null)
                    return;

                _dispatcher.InvokeAsync(async () => await SetSelectedProfileAsync(value.ConversionProfile));
                _selectedProfileWrapper = value;
                AreActionsRestricted = _profileChecker.DoesProfileContainRestrictedActions(value.ConversionProfile);
                RaisePropertyChanged(nameof(SelectedProfileWrapper));
            }
        }

        public ObservableCollection<ConversionProfileWrapper> ProfilesWrapper { get; set; }

        public OutputFormat OutputFormat
        {
            get => Job?.Profile?.OutputFormat ?? OutputFormat.Pdf;
            set
            {
                Job.Profile.OutputFormat = value;
                OutputFilename = EnsureValidExtensionInFilename(OutputFilename, OutputFormat);
                AreActionsRestricted = _profileChecker.DoesProfileContainRestrictedActions(Job.Profile);
                RaisePropertyChanged();
            }
        }

        public string OutputFolder
        {
            get
            {
                if (Job == null)
                    return "";

                if (Job.Profile.SaveFileTemporary)
                    return "";

                return PathSafe.GetDirectoryName(Job.OutputFileTemplate);
            }
            set
            {
                Job.OutputFileTemplate = PathSafe.Combine(value, OutputFilename);
                RaisePropertyChanged();
            }
        }

        public string OutputFilename
        {
            get => Job == null ? "" : PathSafe.GetFileName(Job.OutputFileTemplate);
            set
            {
                if (Job.Profile.SaveFileTemporary)
                    Job.OutputFileTemplate = PathSafe.Combine(Job.OutputFileTemplate, value);
                else
                    Job.OutputFileTemplate = PathSafe.Combine(OutputFolder, value);
                RaisePropertyChanged();
            }
        }

        public bool EditButtonEnabledByGpo => GpoSettings == null || !GpoSettings.DisableProfileManagement;

        private bool _hasBanner;

        public bool HasBanner
        {
            get => _hasBanner;
            set
            {
                _hasBanner = value;
                RaisePropertyChanged();
            }
        }

        public string TrialRemainingDaysInfoText => Translation.GetTrialRemainingDaysInfoText(CampaignHelper.TrialRemainingDays);

        public bool ShowTrialRemainingDaysInfo => CampaignHelper.IsTrial;

        private bool _areActionsRestricted;

        public bool AreActionsRestricted
        {
            get => _areActionsRestricted;
            set
            {
                _areActionsRestricted = value;
                RaisePropertyChanged();
            }
        }

        private void InitCombobox()
        {
            if (SelectedProfile == null)
                return;

            ProfilesWrapper = _settingsProvider.Settings?.ConversionProfiles.Select(x => new ConversionProfileWrapper(x)).ToObservableCollection();

            SelectedProfileWrapper = ProfilesWrapper.FirstOrDefault(x => x.ConversionProfile.Guid == SelectedProfile.Guid)
                                  ?? ProfilesWrapper.FirstOrDefault();

            // Important: RaisePropertyChanged for ProfilesWrapper must be called at the end.
            // Otherwise, the UI will update the binding source and overwrite the selected profile.
            RaisePropertyChanged(nameof(ProfilesWrapper));
        }

        public void MountView()
        {
            InitCombobox();

            _eventAggregator.GetEvent<ManagePrintJobEvent>().Subscribe(ManagePrintJobEvent);
            _eventAggregator.GetEvent<TrialStatusChangedEvent>().Subscribe(OnTrialStatusChanged);
        }

        private void OnTrialStatusChanged()
        {
            RaisePropertyChanged(nameof(ShowTrialRemainingDaysInfo));
            RaisePropertyChanged(nameof(HasBanner));
        }

        public void UnmountView()
        {
            _eventAggregator.GetEvent<ManagePrintJobEvent>().Unsubscribe(ManagePrintJobEvent);
            _eventAggregator.GetEvent<TrialStatusChangedEvent>().Unsubscribe(OnTrialStatusChanged);
        }

        protected override void OnTranslationChanged()
        {
            if (CampaignHelper != null)
                RaisePropertyChanged(TrialRemainingDaysInfoText);
        }
    }
}
