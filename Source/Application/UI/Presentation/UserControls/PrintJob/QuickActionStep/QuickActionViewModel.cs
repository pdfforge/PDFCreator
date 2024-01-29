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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Identity.Client;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using NLog;
using Logger = NLog.Logger;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.QuickActionStep
{
    public class QuickActionViewModel : TranslatableViewModelBase<QuickActionTranslation>, IWorkflowViewModel
    {
        private readonly ICommandLocator _commandLocator;
        private readonly IReadableFileSizeFormatter _readableFileSizeHelper;
        private readonly ICurrentSettingsProvider _currentSettingsProvider;
        private readonly IAttachToOutlookItemAssistant _attachToOutlookItemAssistant;

        private ICurrentSettings<ObservableCollection<ConversionProfile>> _profilesProvider;
        private OutputFormat _outputFormat = OutputFormat.Pdf;
        private Job _job;

        private string _fileDirectory;
        private string _fileName;
        private string _fileSize;
        private readonly TaskCompletionSource<object> _taskCompletionSource = new TaskCompletionSource<object>();
        private readonly ICommand _saveChangedSettingsCommand;

        public ObservableCollection<MenuItem> SendMenuItems { get; set; }

        public ICommand OpenWithPdfArchitectCommand { get; }
        public ICommand OpenExplorerCommand { get; }
        public ICommand QuickActionOpenWithDefaultCommand { get; }
        public IAsyncCommand UpdateSendContextMenuButtonItemsCommand { get; }
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public QuickActionViewModel(ITranslationUpdater translationUpdater, ICommandLocator commandLocator, IReadableFileSizeFormatter readableFileSizeHelper,
            ICurrentSettings<ObservableCollection<ConversionProfile>> profilesProvider, ICurrentSettingsProvider currentSettingsProvider,
            IAttachToOutlookItemAssistant attachToOutlookItemAssistant) : base(translationUpdater)
        {
            _saveChangedSettingsCommand = commandLocator.GetCommand<ISaveChangedSettingsCommand>();

            _commandLocator = commandLocator;
            _readableFileSizeHelper = readableFileSizeHelper;
            _profilesProvider = profilesProvider;
            _currentSettingsProvider = currentSettingsProvider;
            _attachToOutlookItemAssistant = attachToOutlookItemAssistant;
            OpenWithPdfArchitectCommand = _commandLocator.GetCommand<QuickActionOpenWithPdfArchitectCommand>();
            QuickActionOpenWithDefaultCommand = _commandLocator.GetCommand<QuickActionOpenWithDefaultCommand>();
            OpenExplorerCommand = _commandLocator.GetCommand<QuickActionOpenExplorerLocationCommand>();
            UpdateSendContextMenuButtonItemsCommand = new AsyncCommand(UpdateSendContextMenuButtonItemsExecute);
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

            _numberOfFixedSendMenuItems = InitSendContextMenuButtonItems();

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

        private int _numberOfFixedSendMenuItems = 2;

        private int InitSendContextMenuButtonItems()
        {
            SendMenuItems = new ObservableCollection<MenuItem>();
            var printWithArchitectMenuItem = new MenuItem
            {
                Header = Translation.PrintFileWithArchitect,
                Command = _commandLocator.GetCommand<QuickActionPrintWithPdfArchitectCommand>(),
                CommandParameter = Job
            };
            SendMenuItems.Add(printWithArchitectMenuItem);

            var sendEmailMenuItem = new MenuItem
            {
                Header = Translation.SendEmail,
                Command = _commandLocator.GetCommand<QuickActionOpenMailClientCommand>(),
                CommandParameter = Job
            };
            SendMenuItems.Add(sendEmailMenuItem);
            
            RaisePropertyChanged(nameof(SendMenuItems));
            return SendMenuItems.Count;
        }


        private async Task UpdateSendContextMenuButtonItemsExecute(object obj)
        {
            for (int i = SendMenuItems.Count; i > _numberOfFixedSendMenuItems; i--)
                SendMenuItems.RemoveAt(i - 1);

            List<string> activeCaptions = new List<string>();

            await Task.Run(() =>
            {
                try
                {
                    activeCaptions.AddRange(_attachToOutlookItemAssistant.GetOutlookItemCaptions());
                }
                catch (Exception e)
                {
                    Logger.Warn("Issues with Outlook");
                    activeCaptions = new List<string>();
                }
            });

            foreach (var caption in activeCaptions)
            {
                var attachToOutlookItem = new MenuItem
                {
                    Header = Translation.AttachTo + " " + caption,
                    Command = new DelegateCommand(o =>
                        _attachToOutlookItemAssistant.ExportToOutlookItem(caption, Job.OutputFiles))
                };
                SendMenuItems.Add(attachToOutlookItem);
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
