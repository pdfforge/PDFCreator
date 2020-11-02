using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.JobInfoQueue;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Core.Workflow.ComposeTargetFilePath;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Commands.ProfileCommands;
using pdfforge.PDFCreator.UI.Presentation.Events;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.UI.Presentation.Workflow;
using pdfforge.PDFCreator.Utilities;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public class PrintJobViewModel : TranslatableViewModelBase<PrintJobViewTranslation>, IWorkflowViewModel, IMountable
    {
        private TaskCompletionSource<object> _taskCompletionSource = new TaskCompletionSource<object>();
        public IGpoSettings GpoSettings { get; }
        private readonly ISettingsProvider _settingsProvider;
        private readonly ICommandLocator _commandLocator;
        private readonly ISelectedProfileProvider _selectedProfileProvider;
        private readonly ICurrentSettings<ObservableCollection<ConversionProfile>> _profilesProvider;
        private readonly ITargetFilePathComposer _targetFilePathComposer;
        private readonly IJobInfoManager _jobInfoManager;
        private readonly IChangeJobCheckAndProceedCommandBuilder _changeJobCheckAndProceedCommandBuilder;
        private readonly IEventAggregator _eventAggregator;
        private readonly IDispatcher _dispatcher;
        private readonly IJobDataUpdater _jobDataUpdater;
        private string _lastConfirmedFilePath = "";
        private readonly OutputFormatHelper _outputFormatHelper = new OutputFormatHelper();
        private readonly IJobInfoQueue _jobInfoQueue;

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
            IJobDataUpdater jobDataUpdater)
            : base(translationUpdater)
        {
            GpoSettings = gpoSettings;
            _settingsProvider = settingsProvider;
            _commandLocator = commandLocator;
            _eventAggregator = eventAggregator;
            _selectedProfileProvider = selectedProfileProvider;
            _profilesProvider = profilesProvider;
            _targetFilePathComposer = targetFilePathComposer;
            _jobInfoManager = jobInfoManager;

            _changeJobCheckAndProceedCommandBuilder = changeJobCheckAndProceedCommandBuilder;
            _dispatcher = dispatcher;
            _jobDataUpdater = jobDataUpdater;
            _changeJobCheckAndProceedCommandBuilder.Init(() => Job, CallFinishInteraction, () => _lastConfirmedFilePath, s => _lastConfirmedFilePath = s);

            SetOutputFormatCommand = new DelegateCommand<OutputFormat>(SetOutputFormatExecute);

            browseFileCommandBuilder.Init(() => Job, UpdateUiForJobOutputFileTemplate, () => _lastConfirmedFilePath, s => _lastConfirmedFilePath = s);
            BrowseFileCommand = browseFileCommandBuilder.BuildCommand();
            SetupEditProfileCommand(_commandLocator, eventAggregator);

            SetupSaveCommands(translationUpdater);

            EmailCommand = _changeJobCheckAndProceedCommandBuilder.BuildCommand(EnableEmailSettings);
            SetupSendDropDownCommands(translationUpdater);

            MergeCommand = new DelegateCommand(MergeExecute);
            var mergeAllAsyncCommand = new AsyncCommand(MergeAllExecuteAsync, o => jobInfoQueue.Count > 1);
            MergeDropDownCommands = new CommandCollection<PrintJobViewTranslation>(translationUpdater);
            MergeDropDownCommands.AddCommand(mergeAllAsyncCommand, t => t.MergeAll);
            CancelCommand = new DelegateCommand(CancelExecute);
            CancelDropDownCommands = new CommandCollection<PrintJobViewTranslation>(translationUpdater);
            CancelDropDownCommands.AddCommand(new DelegateCommand(CancelAllExecute, o => jobInfoQueue.Count > 1), t => t.CancelAll);

            DisableSaveTempOnlyCommand = new DelegateCommand(DisableSaveFileTemporary);

            jobInfoQueue.OnNewJobInfo += (sender, args) => UpdateNumberOfPrintJobsHint(jobInfoQueue.Count);
            _jobInfoQueue = jobInfoQueue;
            UpdateNumberOfPrintJobsHint(jobInfoQueue.Count);
        }

        private void ManagePrintJobEvent()
        {
            _dispatcher.BeginInvoke(() => MergeExecute(null));
        }

        private void EnableEmailSettings(object o)
        {
            Job.Profile.SaveFileTemporary = true;
            Job.Profile.EmailClientSettings.Enabled = true;
            Job.Profile.OpenViewer.Enabled = false;
            Job.Profile.ActionOrder.Insert(0, nameof(EmailClientSettings));
        }

        private void SetupSendDropDownCommands(ITranslationUpdater translationUpdater)
        {
            SendDropDownCommands = new CommandCollection<PrintJobViewTranslation>(translationUpdater);
            SendDropDownCommands.AddCommand(_changeJobCheckAndProceedCommandBuilder.BuildCommand(EnableSmtp), t => t.Smtp);
            SendDropDownCommands.AddCommand(_changeJobCheckAndProceedCommandBuilder.BuildCommand(EnableDropBox), t => t.Dropbox);
            SendDropDownCommands.AddCommand(_changeJobCheckAndProceedCommandBuilder.BuildCommand(EnableFtp), t => t.Ftp);
        }

        private void EnableDropBox(object o)
        {
            Job.Profile.SaveFileTemporary = true;
            Job.Profile.DropboxSettings.Enabled = true;
            Job.Profile.ActionOrder.Insert(0, nameof(DropboxSettings));
        }

        private void EnableSmtp(object o)
        {
            Job.Profile.SaveFileTemporary = true;
            Job.Profile.EmailSmtpSettings.Enabled = true;
            Job.Profile.ActionOrder.Insert(0, nameof(EmailSmtpSettings));
        }

        private void EnableFtp(object o)
        {
            Job.Profile.SaveFileTemporary = true;
            Job.Profile.Ftp.Enabled = true;
            Job.Profile.ActionOrder.Insert(0, nameof(Ftp));
        }

        private void DisableSaveFileTemporary(object obj)
        {
            SelectedProfile.SaveFileTemporary = false;
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
            SaveCommand = _changeJobCheckAndProceedCommandBuilder.BuildCommand(j => { });

            SaveDropDownCommands = new CommandCollection<PrintJobViewTranslation>(translationUpdater);
            SaveDropDownCommands.AddCommand(_changeJobCheckAndProceedCommandBuilder.BuildCommand(DisableSaveFileTemporary, BrowseFileCommand), t => t.SaveAs);
            SaveDropDownCommands.AddCommand(_changeJobCheckAndProceedCommandBuilder.BuildCommand(EnableSaveToDesktop), t => t.SaveToDesktop);
        }

        private void SetupEditProfileCommand(ICommandLocator commandsLocator, IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<EditSettingsFinishedEvent>().Subscribe(p =>
            {
                if (SelectedProfile != p)
                {
                    SelectedProfile = p;
                    SetSelectedProfile(this, null);
                }
            });

            if (commandsLocator != null)
            {
                EditProfileCommand = commandsLocator.CreateMacroCommand()
                    .AddCommand(new DelegateCommand(SetProfileForSelection))
                    .AddCommand<ShowLockLayerCommand>()
                    .AddCommand<OpenProfileCommand>()
                    .AddCommand<WaitProfileModificationCommand>()
                    .AddCommand(new DelegateCommand(x => eventAggregator.GetEvent<CloseMainWindowEvent>().Publish()))
                    .AddCommand<HideLockLayerCommand>()
                    .Build();
            }
        }

        private void SetProfileForSelection(object o)
        {
            _selectedProfileProvider.SelectedProfile = _profilesProvider.Settings.First(profile => profile.Guid == Job.Profile.Guid);
        }

        private void SetOutputFormatExecute(OutputFormat parameter)
        {
            OutputFormat = parameter;
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
            CancelDropDownCommands.RaiseEnabledChanged();
            MergeDropDownCommands.RaiseEnabledChanged();
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
                UpdateProfileData();
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

        public DelegateCommand<OutputFormat> SetOutputFormatCommand { get; }

        public IAsyncCommand SaveCommand { get; private set; }
        public CommandCollection<PrintJobViewTranslation> SaveDropDownCommands { get; private set; }

        public ICommand EmailCommand { get; }
        public CommandCollection<PrintJobViewTranslation> SendDropDownCommands { get; set; }

        public IMacroCommand BrowseFileCommand { get; }
        public ICommand MergeCommand { get; }
        public CommandCollection<PrintJobViewTranslation> MergeDropDownCommands { get; }

        public ICommand CancelCommand { get; }
        public CommandCollection<PrintJobViewTranslation> CancelDropDownCommands { get; }

        public ICommand EditProfileCommand { get; private set; }
        public ICommand DisableSaveTempOnlyCommand { get; set; }

        private ConversionProfileWrapper _selectedProfileWrapper = null;

        public bool IsUpdatingProfile { get; private set; }

        public ConversionProfileWrapper SelectedProfileWrapper
        {
            get { return _selectedProfileWrapper; }
            set
            {
                if (value != null)
                {
                    SetSelectedProfileAsync(value.ConversionProfile);
                    _selectedProfileWrapper = value;
                }
            }
        }

        public ObservableCollection<ConversionProfileWrapper> ProfilesWrapper { get; set; }

        public IEnumerable<OutputFormat> OutputFormats => System.Enum.GetValues(typeof(OutputFormat)) as OutputFormat[];

        public OutputFormat OutputFormat
        {
            get => Job?.Profile?.OutputFormat ?? OutputFormat.Pdf;
            set
            {
                Job.Profile.OutputFormat = value;
                OutputFilename = EnsureValidExtensionInFilename(OutputFilename, OutputFormat);
                RaisePropertyChanged();
            }
        }

        public string OutputFolder
        {
            get => Job == null ? "" : PathSafe.GetDirectoryName(Job.OutputFileTemplate);
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
                Job.OutputFileTemplate = PathSafe.Combine(OutputFolder, value);
                RaisePropertyChanged();
            }
        }

        public bool EditButtonEnabledByGpo => GpoSettings == null || !GpoSettings.DisableProfileManagement;

        private void SetSelectedProfile(object sender, EventArgs e)
        {
            InitCombobox();
        }

        private void InitCombobox()
        {
            if (SelectedProfile == null)
                return;

            ProfilesWrapper = _settingsProvider.Settings?.Copy().ConversionProfiles.Select(x => new ConversionProfileWrapper(x)).ToObservableCollection();

            SelectedProfileWrapper = ProfilesWrapper.FirstOrDefault(x => x.ConversionProfile.Guid == SelectedProfile.Guid)
                                  ?? ProfilesWrapper.FirstOrDefault();

            // Important: SelectedProfile must be raised before Profiles.
            // Otherwise, the UI will update the binding source and overwrite the selected profile.
            RaisePropertyChanged(nameof(SelectedProfileWrapper));
            RaisePropertyChanged(nameof(ProfilesWrapper));
        }

        public void MountView()
        {
            if (_selectedProfileProvider != null)
            {
                _selectedProfileProvider.SettingsChanged += SetSelectedProfile;
                _selectedProfileProvider.SelectedProfileChanged += SetSelectedProfile;
            }

            InitCombobox();

            _eventAggregator.GetEvent<ManagePrintJobEvent>().Subscribe(ManagePrintJobEvent);
        }

        public void UnmountView()
        {
            if (_selectedProfileProvider != null)
            {
                _selectedProfileProvider.SettingsChanged -= SetSelectedProfile;
                _selectedProfileProvider.SelectedProfileChanged -= SetSelectedProfile;
            }

            _eventAggregator.GetEvent<ManagePrintJobEvent>().Unsubscribe(ManagePrintJobEvent);
        }
    }
}
