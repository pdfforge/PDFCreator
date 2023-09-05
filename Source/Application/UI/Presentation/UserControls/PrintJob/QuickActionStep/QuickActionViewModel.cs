using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Commands.QuickActions;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.UI.Presentation.Workflow;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.QuickActionStep
{
    public class QuickActionViewModel : TranslatableViewModelBase<QuickActionTranslation>, IWorkflowViewModel
    {
        private readonly ICommandLocator _commandLocator;
        private readonly IReadableFileSizeFormatter _readableFileSizeHelper;
        private readonly ICurrentSettingsProvider _currentSettingsProvider;

        private ICurrentSettings<ObservableCollection<ConversionProfile>> _profilesProvider;
        private OutputFormat _outputFormat = OutputFormat.Pdf;
        private Job _job;

        private string _fileDirectory;
        private string _fileName;
        private string _fileSize;
        private readonly TaskCompletionSource<object> _taskCompletionSource = new TaskCompletionSource<object>();
        private readonly ICommand _saveChangedSettingsCommand;
        public ICommand OpenWithPdfArchitectCommand { get; set; }
        public ICommand OpenExplorerCommand { get; set; }
        public ICommand QuickActionOpenWithDefaultCommand { get; set; }
        public ICommand SendEmailCommand { get; set; }
        public ICommand PrintWithArchitectCommand { get; set; }

        public QuickActionViewModel(ITranslationUpdater translationUpdater, ICommandLocator commandLocator, IReadableFileSizeFormatter readableFileSizeHelper,
            ICurrentSettings<ObservableCollection<ConversionProfile>> profilesProvider, ICurrentSettingsProvider currentSettingsProvider) : base(translationUpdater)
        {
            _saveChangedSettingsCommand = commandLocator.GetCommand<ISaveChangedSettingsCommand>();

            _commandLocator = commandLocator;
            _readableFileSizeHelper = readableFileSizeHelper;
            _profilesProvider = profilesProvider;
            _currentSettingsProvider = currentSettingsProvider;
            OpenWithPdfArchitectCommand = _commandLocator.GetCommand<QuickActionOpenWithPdfArchitectCommand>();
            QuickActionOpenWithDefaultCommand = _commandLocator.GetCommand<QuickActionOpenWithDefaultCommand>();
            OpenExplorerCommand = _commandLocator.GetCommand<QuickActionOpenExplorerLocationCommand>();
            SendEmailCommand = _commandLocator.GetCommand<QuickActionOpenMailClientCommand>();
            PrintWithArchitectCommand = _commandLocator.GetCommand<QuickActionPrintWithPdfArchitectCommand>();
            FinishCommand = new DelegateCommand(OnFinish);
        }

        private void OnFinish(object obj)
        {
            _saveChangedSettingsCommand.Execute(null);
            StepFinished?.Invoke(this, EventArgs.Empty);
            _taskCompletionSource.SetResult(null);
        }

        public Task ExecuteWorkflowStep(Job job)
        {
            Job = job;
            OutputFormat = job.Profile.OutputFormat;
            var firstFile = job.OutputFiles.First();

            FileName = System.IO.Path.GetFileName(firstFile);
            FileDirectory = System.IO.Path.GetDirectoryName(firstFile);
            FileSize = _readableFileSizeHelper.GetFileSizeString(firstFile);
            RaisePropertyChanged(nameof(IsActive));

            IsSaveFileTemporary = job.Profile.SaveFileTemporary;
            RaisePropertyChanged(nameof(IsSaveFileTemporary));

            return _taskCompletionSource.Task;
        }

        public event EventHandler StepFinished;

        public Job Job
        {
            get => _job;
            private set
            {
                _job = value;
                RaisePropertyChanged(nameof(Job));
            }
        }

        public OutputFormat OutputFormat
        {
            get { return _outputFormat; }
            set
            {
                _outputFormat = value;
                RaisePropertyChanged(nameof(OutputFormat));
            }
        }

        public string FileDirectory
        {
            get { return _fileDirectory; }
            private set
            {
                _fileDirectory = value;
                RaisePropertyChanged(nameof(FileDirectory));
            }
        }

        public bool IsSaveFileTemporary { get; private set; }

        public string FileSize
        {
            get { return _fileSize; }
            set
            {
                _fileSize = value;
                RaisePropertyChanged(nameof(FileSize));
            }
        }

        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                RaisePropertyChanged(nameof(FileName));
            }
        }

        public bool IsActive
        {
            set
            {
                if (_job != null && _currentSettingsProvider != null)
                {
                    var conversionProfile = SettingsHelper.GetProfileByGuid(_profilesProvider.Settings, _job.Profile.Guid);
                    conversionProfile.ShowQuickActions = !value;
                    _job.Profile.ShowQuickActions = !value;
                    RaisePropertyChanged(nameof(IsActive));
                }
            }
            get
            {
                return _job?.Profile?.ShowQuickActions == false;
            }
        }

        public DelegateCommand FinishCommand { get; }
    }
}
