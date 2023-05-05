using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.JobHistory;
using pdfforge.PDFCreator.Core.SettingsManagementInterface;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Commands.QuickActions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Home
{
    public class HomeViewModel : TranslatableViewModelBase<HomeViewTranslation>, IMountable
    {
        private readonly IPrinterHelper _printerHelper;
        private readonly ISettingsProvider _settingsProvider;
        private readonly IJobHistoryActiveRecord _jobHistoryActiveRecord;
        private readonly IDispatcher _dispatcher;
        private readonly IGpoSettings _gpoSettings;
        private readonly CollectionViewSource _collectionViewSource;
        private readonly ObservableCollection<HistoricJob> _jobHistoryList;

        public HomeViewModel(ITranslationUpdater translationUpdater, IPrinterHelper printerHelper, ISettingsProvider settingsProvider, 
                             IJobHistoryActiveRecord jobHistoryActiveRecord, IDispatcher dispatcher,
                             ICommandLocator commandLocator, IGpoSettings gpoSettings)
            : base(translationUpdater)
        {
            _printerHelper = printerHelper;
            _settingsProvider = settingsProvider;
            _jobHistoryActiveRecord = jobHistoryActiveRecord;
            _dispatcher = dispatcher;
            _gpoSettings = gpoSettings;

            _jobHistoryList = new ObservableCollection<HistoricJob>();

            _collectionViewSource = new CollectionViewSource();
            _collectionViewSource.SortDescriptions.Add(new SortDescription(nameof(HistoricJob.CreationTime), ListSortDirection.Descending));
            _collectionViewSource.Source = _jobHistoryList;

            JobHistory = _collectionViewSource.View;
            JobHistory.MoveCurrentTo(null); //unselect first item

            ConvertFileCommand = commandLocator.GetCommand<SelectFileViaDialogAndConvertCommand>();

            ClearHistoryCommand = new DelegateCommand(o => jobHistoryActiveRecord.Delete());
            RefreshHistoryCommand = new DelegateCommand(o => RefreshHistory());
            ToggleHistoryEnabledCommand = new DelegateCommand<HistoricJob>(hj => HistoryEnabled = !HistoryEnabled);

            RemoveHistoricJobCommand = new DelegateCommand<HistoricJob>(jobHistoryActiveRecord.Remove);

            DeleteHistoricFilesCommand = commandLocator.CreateMacroCommand()
                .AddCommand<DeleteHistoricFilesCommand>()
                .AddCommand(new AsyncCommand(o => _jobHistoryActiveRecord.Refresh()))
                .Build();

            HistoryQuickActionMenuItems = new List<MenuItem>
            {
                new MenuItem
                {
                    Header = Translation.DeleteFileFromHistory,
                    Command = DeleteHistoricFilesCommand
                },

                new MenuItem
                {
                    Header = Translation.OpenPDFArchitect,
                    Command = commandLocator.GetCommand<QuickActionOpenWithPdfArchitectCommand>()
                },

                new MenuItem
                {
                    Header = Translation.OpenDefaultProgram,
                    Command = commandLocator.GetCommand<QuickActionOpenWithDefaultCommand>()
                },

                new MenuItem
                {
                    Header = Translation.OpenExplorer,
                    Command = commandLocator.GetCommand<QuickActionOpenExplorerLocationCommand>()
                },
                new MenuItem
                {
                    Header = Translation.PrintWithPDFArchitect,
                    Command = commandLocator.GetCommand<QuickActionPrintWithPdfArchitectCommand>()
                },
                new MenuItem
                {
                    Header = Translation.OpenMailClient,
                    Command = commandLocator.GetCommand<QuickActionOpenMailClientCommand>()
                }
            };
        }

        public void MountView()
        {
            _collectionViewSource.Source = _jobHistoryList;
            _settingsProvider.Settings.ApplicationSettings.JobHistory.PropertyChanged += JobHistoryOnPropertyChanged;
            _jobHistoryActiveRecord.HistoryChanged += JobHistoryActiveRecordOnHistoryChanged;
            JobHistoryActiveRecordOnHistoryChanged(this, EventArgs.Empty);
            RaisePropertyChanged(nameof(HistoryEnabled));
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
        }

        private void JobHistoryActiveRecordOnHistoryChanged(object sender, EventArgs e)
        {
            _dispatcher.BeginInvoke(RefreshHistory);
        }

        private void JobHistoryOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(HistoryEnabled));
        }

        public IEnumerable<MenuItem> HistoryQuickActionMenuItems { get; private set; }
        public ICollectionView JobHistory { get; }
        public ICommand ConvertFileCommand { get; set; }
        public ICommand ClearHistoryCommand { get; set; }
        public ICommand RefreshHistoryCommand { get; set; }
        public ICommand ToggleHistoryEnabledCommand { get; set; }
        public ICommand RemoveHistoricJobCommand { get; set; }
        public ICommand DeleteHistoricFilesCommand { get; set; }

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

        protected override void OnTranslationChanged()
        {
            RaisePropertyChanged(nameof(CallToActionText));
            RaisePropertyChanged(nameof(HistoryQuickActionMenuItems));
        }
    }
}
