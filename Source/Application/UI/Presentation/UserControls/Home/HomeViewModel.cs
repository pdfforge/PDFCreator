using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction.DialogInteractions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.DirectConversion;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.JobHistory;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Commands.QuickActions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.QuickActionStep;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Home
{
    public class HomeViewModel : TranslatableViewModelBase<HomeViewTranslation>, IMountable
    {
        private readonly IFileConversionAssistant _fileConversionAssistant;
        private readonly IPrinterHelper _printerHelper;
        private readonly ISettingsProvider _settingsProvider;
        private readonly IJobHistoryActiveRecord _jobHistoryActiveRecord;
        private readonly IDispatcher _dispatcher;
        private readonly ICommandLocator _commandLocator;
        private readonly IGpoSettings _gpoSettings;
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly ObservableCollection<HistoricJob> _jobHistoryList;

        public HomeViewModel(IInteractionInvoker interactionInvoker, IFileConversionAssistant fileConversionAssistant, ITranslationUpdater translationUpdater,
                             IPrinterHelper printerHelper, ISettingsProvider settingsProvider, IJobHistoryActiveRecord jobHistoryActiveRecord, IDispatcher dispatcher,
                             ICommandLocator commandLocator, IGpoSettings gpoSettings)
            : base(translationUpdater)
        {
            _interactionInvoker = interactionInvoker;
            _fileConversionAssistant = fileConversionAssistant;
            _printerHelper = printerHelper;
            _settingsProvider = settingsProvider;
            _jobHistoryActiveRecord = jobHistoryActiveRecord;
            _dispatcher = dispatcher;
            _commandLocator = commandLocator;
            _gpoSettings = gpoSettings;

            _jobHistoryList = new ObservableCollection<HistoricJob>();

            var viewSource = new CollectionViewSource();
            viewSource.SortDescriptions.Add(new SortDescription(nameof(HistoricJob.CreationTime), ListSortDirection.Descending));
            viewSource.Source = _jobHistoryList;

            JobHistory = viewSource.View;
            JobHistory.MoveCurrentTo(null); //unselect first item

            ConvertFileCommand = new DelegateCommand(o => ConvertFileExecute());

            ClearHistoryCommand = new DelegateCommand(o => jobHistoryActiveRecord.Delete());
            RefreshHistoryCommand = new DelegateCommand(o => RefreshHistory());
            ToggleHistoryEnabledCommand = new DelegateCommand<HistoricJob>(hj => HistoryEnabled = !HistoryEnabled);

            RemoveHistoricJobCommand = new DelegateCommand<HistoricJob>(jobHistoryActiveRecord.Remove);

            DeleteHistoricFilesCommand = commandLocator.CreateMacroCommand()
                .AddCommand<DeleteHistoricFilesCommand>()
                .AddCommand(new AsyncCommand(o => _jobHistoryActiveRecord.Refresh()))
                .Build();

            QuickActionOpenList = new List<DropDownButtonItem>
            {
                GetQuickActionItem<QuickActionOpenWithPdfArchitectCommand>(() =>Translation.OpenPDFArchitect),
                GetQuickActionItem<QuickActionOpenWithDefaultCommand>(() =>Translation.OpenDefaultProgram),
                GetQuickActionItem<QuickActionOpenExplorerLocationCommand>(() =>Translation.OpenExplorer),
                GetQuickActionItem<QuickActionPrintWithPdfArchitectCommand>(() =>Translation.PrintWithPDFArchitect),
                GetQuickActionItem<QuickActionOpenMailClientCommand>(() =>Translation.OpenMailClient)
            };
        }

        public void MountView()
        {
            _settingsProvider.Settings.ApplicationSettings.JobHistory.PropertyChanged += JobHistoryOnPropertyChanged;
            _jobHistoryActiveRecord.HistoryChanged += JobHistoryActiveRecordOnHistoryChanged;
            JobHistoryActiveRecordOnHistoryChanged(this, EventArgs.Empty);
            RaisePropertyChanged(nameof(HistoryEnabled));
            RaisePropertyChanged(nameof(NumberOfHistoricJobs));
        }

        public void UnmountView()
        {
            _settingsProvider.Settings.ApplicationSettings.JobHistory.PropertyChanged -= JobHistoryOnPropertyChanged;
            _jobHistoryActiveRecord.HistoryChanged -= JobHistoryActiveRecordOnHistoryChanged;
        }

        private void RefreshHistory()
        {
            var addedJobs = _jobHistoryActiveRecord.History.Except(_jobHistoryList);
            _jobHistoryList.AddRange(addedJobs);

            var removedJobs = _jobHistoryList.Except(_jobHistoryActiveRecord.History);
            foreach (var job in removedJobs.ToList())
            {
                _jobHistoryList.Remove(job);
            }

            RaisePropertyChanged(nameof(NumberOfHistoricJobs));
        }

        private void JobHistoryActiveRecordOnHistoryChanged(object sender, EventArgs e)
        {
            _dispatcher.BeginInvoke(RefreshHistory);
        }

        private void JobHistoryOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(HistoryEnabled));
        }

        private DropDownButtonItem GetQuickActionItem<TCommand>(Func<string> text) where TCommand : class, ICommand
        {
            return new DropDownButtonItem(text, () => JobHistory.CurrentItem, _commandLocator.GetCommand<TCommand>());
        }

        public IList<DropDownButtonItem> QuickActionOpenList { get; private set; }
        public ICollectionView JobHistory { get; }
        public ICommand ConvertFileCommand { get; set; }
        public ICommand ClearHistoryCommand { get; set; }
        public ICommand RefreshHistoryCommand { get; set; }
        public ICommand ToggleHistoryEnabledCommand { get; set; }
        public ICommand RemoveHistoricJobCommand { get; set; }
        public ICommand DeleteHistoricFilesCommand { get; set; }

        public int NumberOfHistoricJobs => _jobHistoryActiveRecord.History.Count;

        public bool HistoryEnabledByGpo => !_gpoSettings.DisableHistory;

        public bool HistoryEnabled
        {
            get
            {
                return _jobHistoryActiveRecord.HistoryEnabled;
            }
            set
            {
                _jobHistoryActiveRecord.HistoryEnabled = value;
                RaisePropertyChanged(nameof(HistoryEnabled));
            }
        }

        public string CallToActionText => Translation.FormatCallToAction(_printerHelper.GetApplicablePDFCreatorPrinter(_settingsProvider.Settings?.CreatorAppSettings?.PrimaryPrinter ?? ""));

        private void ConvertFileExecute()
        {
            var interaction = new OpenFileInteraction();

            _interactionInvoker.Invoke(interaction);

            if (!interaction.Success)
                return;

            var file = interaction.FileName;
            _fileConversionAssistant.HandleFileListWithoutTooManyFilesWarning(new List<string> { file }, new AppStartParameters());
        }

        protected override void OnTranslationChanged()
        {
            if (QuickActionOpenList != null)
            {
                foreach (var quickActionListItemVo in QuickActionOpenList)
                {
                    quickActionListItemVo.NotifyPropertyChanged();
                }
            }

            RaisePropertyChanged(nameof(CallToActionText));
        }
    }
}
