using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Jobs;
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
using pdfforge.PDFCreator.UI.Presentation.Helper.Version;
using pdfforge.PDFCreator.UI.Presentation.Routing;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.UpdateHint;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities;
using Prism.Events;
using Prism.Regions;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation
{
    public class MainShellViewModel : TranslatableViewModelBase<MainShellTranslation>, IMountableAsync
    {
        private ApplicationNameProvider ApplicationName { get; }
        public IInteractionRequest InteractionRequest { get; }

        public string ApplicationNameAndVersion => ApplicationName.ApplicationName + " " + _versionHelper.FormatWithThreeDigits();

        private readonly IEventAggregator _aggregator;
        private string _activePath = RegionNames.HomeView;
        private readonly IDispatcher _dispatcher;
        private readonly IRegionManager _regionManager;
        private readonly IUpdateHelper _updateHelper;
        private readonly IEventAggregator _eventAggregator;
        private readonly IStartupActionHandler _startupActionHandler;
        private readonly ICurrentSettings<Conversion.Settings.UsageStatistics> _usageStatisticsProvider;
        private readonly IVersionHelper _versionHelper;
        private readonly IOnlineVersionHelper _onlineVersionHelper;
        private readonly SemaphoreSlim _interactionSemaphore = new SemaphoreSlim(1);

        public ICommand DismissUsageStatsInfoCommand { get; }
        public ICommand ReadMoreUsageStatsCommand { get; }

        public IGpoSettings GpoSettings { get; }

        private bool _showUpdate;
        private bool _updateInfoWasShown;
        private readonly IStartupRoutine _startupRoutine;

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
            IRegionManager regionManager, IGpoSettings gpoSettings, IUpdateHelper updateHelper, IEventAggregator eventAggregator,
            IStartupActionHandler startupActionHandler, ICurrentSettings<Conversion.Settings.UsageStatistics> usageStatisticsProvider,
            IVersionHelper versionHelper, IOnlineVersionHelper onlineVersionHelper,
            IStartupRoutine startupActions)
            : base(translation)
        {
            _aggregator = aggregator;
            ApplicationName = applicationName;
            InteractionRequest = interactionRequest;

            _startupRoutine = startupActions;
            _dispatcher = dispatcher;
            _regionManager = regionManager;
            _updateHelper = updateHelper;
            _eventAggregator = eventAggregator;
            _startupActionHandler = startupActionHandler;
            _usageStatisticsProvider = usageStatisticsProvider;
            _versionHelper = versionHelper;
            _onlineVersionHelper = onlineVersionHelper;
            GpoSettings = gpoSettings;

            NavigateCommand = commandLocator?.CreateMacroCommand()
                .AddCommand<SkipIfSameNavigationTargetCommand>()
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

            ReadMoreUsageStatsCommand = commandLocator?.CreateMacroCommand()
                .AddCommand(new DelegateCommand(_ => SetUsageStatsInfo()))
                .AddCommand(NavigateCommand)
                .Build();

            DismissUsageStatsInfoCommand = new DelegateCommand(o => SetUsageStatsInfo());
        }

        private void OnCloseMainWindow()
        {
            _dispatcher.BeginInvoke(() => _closeViewAction?.Invoke());
        }

        public void Init(Action close)
        {
            _closeViewAction = close;
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

        private void SetupActivePathInMainShell(IStartupRoutine startupRoutine)
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
            NavigateCommand.Execute(RegionNames.HomeView);
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

        private SubscriptionToken _showUpdateInteractionEventToken;
        private SubscriptionToken _setShowUpdateEventToken;
        private Action _closeViewAction;

        public async Task MountViewAsync()
        {
            SetupActivePathInMainShell(_startupRoutine);

            _startupActionHandler.HandleStartupActions(_regionManager, _startupRoutine);

            var onlineVersionAsync = _onlineVersionHelper.GetOnlineVersion();

            if (onlineVersionAsync.VersionInfos.Count > 0)
            {
                await _dispatcher.InvokeAsync(ShowUpdateVersionOverviewView);
            }

            _aggregator.GetEvent<CloseMainWindowEvent>().Subscribe(OnCloseMainWindow);

            _showUpdateInteractionEventToken = _eventAggregator.GetEvent<ShowUpdateInteractionEvent>().Subscribe(() =>
            {
                if (!_updateHelper.UpdateShouldBeShown()) return;
                _updateInfoWasShown = false;
                _dispatcher.InvokeAsync(ShowUpdateVersionOverviewView);
            });

            ShowUpdate = _updateHelper.UpdateShouldBeShown();
            _setShowUpdateEventToken = _eventAggregator.GetEvent<SetShowUpdateEvent>().Subscribe(
                value => ShowUpdate = value
                );
        }

        public async Task UnmountViewAsync()
        {
            await Task.FromResult(false);
            _eventAggregator.GetEvent<ShowUpdateInteractionEvent>().Unsubscribe(_showUpdateInteractionEventToken);
            _eventAggregator.GetEvent<SetShowUpdateEvent>().Unsubscribe(_setShowUpdateEventToken);

            _aggregator.GetEvent<CloseMainWindowEvent>().Unsubscribe(OnCloseMainWindow);
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

    public class ShowUpdateInteractionEvent : PubSubEvent
    {
    }
}
