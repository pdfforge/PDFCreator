using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller.Routing;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Commands.EvaluateSettingsCommands;
using pdfforge.PDFCreator.UI.Presentation.Events;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.Routing;
using pdfforge.PDFCreator.UI.Presentation.UserControls;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.UpdateHint;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities;
using Prism.Events;
using Prism.Regions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation
{
    public class MainShellViewModel : TranslatableViewModelBase<MainShellTranslation>
    {
        private ApplicationNameProvider ApplicationName { get; }
        public IInteractionRequest InteractionRequest { get; }

        public string ApplicationNameAndVersion => ApplicationName.ApplicationName + " " + _versionHelper.FormatWithThreeDigits();

        private readonly IEventAggregator _aggregator;
        private string _activePath = MainRegionViewNames.HomeView;
        private readonly IDispatcher _dispatcher;
        private readonly IRegionManager _regionManager;
        private readonly IUpdateAssistant _updateAssistant;
        private readonly IStartupActionHandler _startupActionHandler;
        private readonly ICurrentSettings<UsageStatistics> _usageStatisticsProvider;
        private readonly IVersionHelper _versionHelper;
        private readonly SemaphoreSlim _interactionSemaphore = new SemaphoreSlim(1);

        public ICommand DismissUsageStatsInfoCommand { get; }
        public ICommand ReadMoreUsageStatsCommand { get; }
        public IGpoSettings GpoSettings { get; }

        private bool _showUpdate;
        private bool _updateInfoWasShown;

        public bool ShowUpdate
        {
            get => _showUpdate;
            set
            {
                _showUpdate = value;
                RaisePropertyChanged();
            }
        }

        public MainShellViewModel(DragAndDropEventHandler dragAndDrop, ITranslationUpdater translation,
            ApplicationNameProvider applicationName, IInteractionRequest interactionRequest,
            IEventAggregator aggregator, ICommandLocator commandLocator, IDispatcher dispatcher,
            IRegionManager regionManager, IGpoSettings gpoSettings, IUpdateAssistant updateAssistant, IEventAggregator eventAggregator,
            IStartupActionHandler startupActionHandler, ICurrentSettings<UsageStatistics> usageStatisticsProvider, IVersionHelper versionHelper)
            : base(translation)
        {
            _aggregator = aggregator;
            ApplicationName = applicationName;
            InteractionRequest = interactionRequest;

            _dispatcher = dispatcher;
            _regionManager = regionManager;
            _updateAssistant = updateAssistant;
            _startupActionHandler = startupActionHandler;
            _usageStatisticsProvider = usageStatisticsProvider;
            _versionHelper = versionHelper;
            ShowUpdate = updateAssistant.IsUpdateAvailable();
            GpoSettings = gpoSettings;

            NavigateCommand = commandLocator?.CreateMacroCommand()
                .AddCommand<EvaluateTabSwitchRelevantSettingsAndNotifyUserCommand>()
                .AddCommand<ISaveChangedSettingsCommand>()
                .AddCommand<NavigateToMainTabCommand>()
                .Build();

            CloseCommand = commandLocator?.CreateMacroCommand()
                .AddCommand<EvaluateTabSwitchRelevantSettingsAndNotifyUserCommand>()
                .AddCommand<ISaveChangedSettingsCommand>()
                .Build();

            DragEnterCommand = new DelegateCommand<DragEventArgs>(dragAndDrop.HandleDragEnter);
            DragDropCommand = new DelegateCommand<DragEventArgs>(dragAndDrop.HandleDropEvent);

            aggregator.GetEvent<NavigateToHomeEvent>().Subscribe(OnSettingsChanged);
            aggregator.GetEvent<NavigateMainTabEvent>().Subscribe(OnMainShellNavigated);
            aggregator.GetEvent<ForceMainShellNavigation>().Subscribe(OnForcedNavigation);
            aggregator.GetEvent<ExitApplicationEvent>().Subscribe(OnExitApplication);

            ShowUpdate = updateAssistant.ShowUpdate;
            eventAggregator.GetEvent<SetShowUpdateEvent>().Subscribe(value =>
            {
                ShowUpdate = updateAssistant.ShowUpdate;
            }, true);

            ReadMoreUsageStatsCommand = commandLocator?.CreateMacroCommand()
                .AddCommand(new DelegateCommand(_ => SetUsageStatsInfo()))
                .AddCommand(NavigateCommand)
                .Build();

            DismissUsageStatsInfoCommand = new DelegateCommand(o => SetUsageStatsInfo());

            _updateAssistant.TryShowUpdateInteraction += (sender, args) =>
            {
                if (_updateAssistant.ShowUpdate)
                {
                    _updateInfoWasShown = false;
                    _dispatcher.InvokeAsync(ShowUpdateVersionOverviewView);
                }
            };

            if (_updateAssistant != null && _updateAssistant.ShowUpdate)
            {
                _dispatcher.InvokeAsync(ShowUpdateVersionOverviewView);
            }
        }

        private void OnExitApplication()
        {
            _dispatcher.BeginInvoke(
                () =>
                {
                    CloseCommand.Execute(null);
                });
        }

        protected override void OnTranslationChanged()
        {
            base.OnTranslationChanged();
            RaisePropertyChanged(nameof(UsageStatisticsInfoText));
        }

        public string UsageStatisticsInfoText => Translation.FormatUsageStatisticsInfoText(ApplicationName.ApplicationNameWithEdition);

        private void SetUsageStatsInfo()
        {
            _usageStatisticsProvider.Settings.UsageStatsInfo = false;
            RaisePropertyChanged(nameof(ShowUsageStatsInfo));
        }

        public bool ShowUsageStatsInfo
        {
            get { return !GpoSettings.DisableUsageStatistics && _usageStatisticsProvider.Settings.UsageStatsInfo; }
            set
            {
                _usageStatisticsProvider.Settings.UsageStatsInfo = value;
                RaisePropertyChanged(nameof(ShowUsageStatsInfo));
            }
        }

        private void SetupActivePathInMainShell(StartupRoutine startupRoutine)
        {
            var startupNavigationActions = startupRoutine.GetActionByType<StartupNavigationAction>();
            foreach (var startupNavigationAction in startupNavigationActions)
            {
                if (startupNavigationAction.Region == RegionNames.MainRegion)
                {
                    ActivePath = startupNavigationAction.Target;
                }
            }
        }

        public void MainShellStartupAction(StartupRoutine startupRoutine)
        {
            SetupActivePathInMainShell(startupRoutine);

            _dispatcher.InvokeAsync(() => _startupActionHandler.HandleStartupActions(_regionManager, startupRoutine));

            if (_updateAssistant.OnlineVersion.VersionInfos.Count > 0)
            {
                _dispatcher.InvokeAsync(ShowUpdateVersionOverviewView);
            }
        }

        private async Task ShowUpdateVersionOverviewView()
        {
            if (!_updateInfoWasShown)
            {
                await _interactionSemaphore.WaitAsync();
                _updateInfoWasShown = true;
                await InteractionRequest.RaiseAsync(new UpdateOverviewInteraction());
                _interactionSemaphore.Release();
            }
        }

        private void OnForcedNavigation(string obj)
        {
            NavigateCommand.Execute(obj);
        }

        private void OnMainShellNavigated(string targetView)
        {
            _activePath = targetView;
            RaisePropertyChanged(nameof(ActivePath));
        }

        private void OnSettingsChanged()
        {
            NavigateCommand.Execute(MainRegionViewNames.HomeView);
        }

        public ICommand DragEnterCommand { get; }

        public ICommand DragDropCommand { get; }

        public ICommand NavigateCommand { get; set; }

        public IMacroCommand CloseCommand { get; set; }

        public string ActivePath
        {
            set
            {
                _activePath = value;
                RaisePropertyChanged();
            }
            get
            {
                return _activePath;
            }
        }

        public void PublishMainShellDone()
        {
            _aggregator.GetEvent<MainWindowOpenedEvent>().Publish();
        }

        public void OnClosed()
        {
            _aggregator.GetEvent<MainWindowClosedEvent>().Publish();
        }

        #region nothing to see here, move along!

        //      \
        //       \ji
        //       /.(((
        //      (,/"(((__,--.
        //          \  ) _( /{
        //          !||   :||
        //          !||   :||
        //          '''   '''

        #endregion nothing to see here, move along!
    }

    public class SetShowUpdateEvent : PubSubEvent<bool>
    {
    }
}
