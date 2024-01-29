using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Controller.Routing;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.Core.Services.Trial;
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
using System.ComponentModel;
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

        public string ApplicationNameAndVersion => ApplicationName.ApplicationNameWithEdition + " " + _versionHelper.FormatWithThreeDigits();

        private readonly IEventAggregator _aggregator;
        private readonly IDispatcher _dispatcher;
        private readonly IRegionManager _regionManager;
        private readonly IUpdateHelper _updateHelper;
        private readonly IEventAggregator _eventAggregator;
        private readonly IStartupActionHandler _startupActionHandler;
        private readonly ICurrentSettings<Conversion.Settings.UsageStatistics> _usageStatisticsProvider;
        private readonly IVersionHelper _versionHelper;
        private readonly IOnlineVersionHelper _onlineVersionHelper;
        private readonly SemaphoreSlim _interactionSemaphore = new(1);
        public ICommand DismissUsageStatsInfoCommand { get; }
        public ICommand ReadMoreUsageStatsCommand { get; }

        public IGpoSettings GpoSettings { get; }

        private bool _showUpdate;
        private bool _updateInfoWasShown;
        private readonly IStartupRoutine _startupRoutine;
        private readonly IPdfEditorHelper _pdfEditorHelper;
        private readonly ICampaignHelper _campaignHelper;
        private readonly ApplicationNameProvider _applicationNameProvider;

        public ICommand DismissTrialExpireInfoCommand { get; }

        public bool HidePdfArchitectInfo => GpoSettings.HidePdfArchitectInfo || _pdfEditorHelper.UseSodaPdf;

        public bool ShowUpdate
        {
            get => _showUpdate;
            set
            {
                _showUpdate = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(UpdateBadge));
            }
        }

        public string UpdateBadge => ShowUpdate ? "!" : "";

        public MainShellViewModel(DragAndDropEventHandler dragAndDrop, ITranslationUpdater translation,
            ApplicationNameProvider applicationName, IInteractionRequest interactionRequest,
            IEventAggregator aggregator, ICommandLocator commandLocator, IDispatcher dispatcher,
            IRegionManager regionManager, IGpoSettings gpoSettings, IUpdateHelper updateHelper, IEventAggregator eventAggregator,
            IStartupActionHandler startupActionHandler, ICurrentSettings<Conversion.Settings.UsageStatistics> usageStatisticsProvider,
            IVersionHelper versionHelper, IOnlineVersionHelper onlineVersionHelper,
            IStartupRoutine startupActions, IPdfEditorHelper pdfEditorHelper,
            ICampaignHelper campaignHelper,
            ApplicationNameProvider applicationNameProvider)
            : base(translation)
        {
            _aggregator = aggregator;
            ApplicationName = applicationName;
            InteractionRequest = interactionRequest;

            _startupRoutine = startupActions;
            _pdfEditorHelper = pdfEditorHelper;
            _dispatcher = dispatcher;
            _regionManager = regionManager;
            _updateHelper = updateHelper;
            _eventAggregator = eventAggregator;
            _startupActionHandler = startupActionHandler;
            _usageStatisticsProvider = usageStatisticsProvider;
            _versionHelper = versionHelper;
            _onlineVersionHelper = onlineVersionHelper;
            GpoSettings = gpoSettings;
            _campaignHelper = campaignHelper;
            _applicationNameProvider = applicationNameProvider;

            NavigateCommand = commandLocator.CreateMacroCommand()
                .AddCommand<SkipIfSameNavigationTargetCommand>()
                .AddCommand<EvaluateTabSwitchRelevantSettingsAndNotifyUserCommand>()
                .AddCommand<ISaveChangedSettingsCommand>()
                .AddCommand<NavigateToMainTabCommand>()
                .Build();

            CloseCommand = commandLocator.CreateMacroCommand()
                .AddCommand<EvaluateTabSwitchRelevantSettingsAndNotifyUserCommand>()
                .AddCommand<ISaveChangedSettingsCommand>()
                .AddCommand<IDeleteTempFolderCommand>()
                .Build();

            DragEnterCommand = new DelegateCommand<DragEventArgs>(dragAndDrop.HandleDragEnter);
            DragDropCommand = new DelegateCommand<DragEventArgs>(dragAndDrop.HandleDropEvent);

            aggregator.GetEvent<NavigateToHomeEvent>().Subscribe(OnSettingsChanged);
            aggregator.GetEvent<NavigateMainTabEvent>().Subscribe(OnMainShellNavigated);
            aggregator.GetEvent<ForceMainShellNavigation>().Subscribe(OnForcedNavigation);
            aggregator.GetEvent<ExitApplicationEvent>().Subscribe(OnExitApplication);

            ReadMoreUsageStatsCommand = commandLocator.CreateMacroCommand()
                .AddCommand(new DelegateCommand(_ => { ShowUsageStatsInfo = false; }))
                .AddCommand(NavigateCommand)
                .Build();

            DismissUsageStatsInfoCommand = new DelegateCommand(_ => { ShowUsageStatsInfo = false; });

            DismissTrialExpireInfoCommand = new DelegateCommand(_ => { ShowTrialRemainingDaysInfo = false; });

            OpenUrlCommand = commandLocator.GetCommand<UrlOpenCommand>();
        }

        private void OnCloseMainWindow()
        {
            _dispatcher.BeginInvoke(() => _closeViewAction?.Invoke());
        }

        public void Init(Action close)
        {
            _closeViewAction = close;
            RaisePropertyChanged(nameof(ShowTrialRemainingDaysInfo));
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
            RaisePropertyChanged(nameof(TrialRemainingDaysInfoText));
        }

        public string UsageStatisticsInfoText => Translation.FormatUsageStatisticsInfoText(ApplicationName.ApplicationNameWithEdition);

        public bool ShowUsageStatsInfo
        {
            get => !GpoSettings.DisableUsageStatistics && _usageStatisticsProvider.Settings.UsageStatsInfo;
            set
            {
                _usageStatisticsProvider.Settings.UsageStatsInfo = value;
                RaisePropertyChanged();
            }
        }

        public string TrialRemainingDaysInfoText => Translation.GetTrialRemainingDaysInfoText(_campaignHelper.TrialRemainingDays);

        public bool ShowTrialHint { get; set; } = true;

        public bool ShowTrialRemainingDaysInfo
        {
            get => _campaignHelper.IsTrial && ShowTrialHint;
            set
            {
                ShowTrialHint = value;
                RaisePropertyChanged();
            }
        }

        private string FallbackUrl => Urls.GetExtendLicenseFallbackUrl(_applicationNameProvider.EditionName);

        public string TrialExtendLink => _campaignHelper.GetTrialExtendLink(FallbackUrl);

        public ICommand OpenUrlCommand { get; }

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
            NavigateCommand.Execute(RegionViewName.HomeView);
        }

        public ICommand DragEnterCommand { get; }

        public ICommand DragDropCommand { get; }

        public ICommand NavigateCommand { get; }

        public IMacroCommand CloseCommand { get; }

        private string _activePath = RegionViewName.HomeView;

        public string ActivePath
        {
            set
            {
                _activePath = value;
                RaisePropertyChanged();
            }
            get => _activePath;
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
        private SubscriptionToken _showTrialRemainingDaysEventToken;

        public async Task MountViewAsync()
        {
            SetupActivePathInMainShell(_startupRoutine);

            _startupActionHandler.HandleStartupActions(_regionManager, _startupRoutine);

            var onlineVersionAsync = _onlineVersionHelper.GetOnlineVersion();

            if (onlineVersionAsync.ReleaseInfos.Count > 0 && _updateHelper.UpdateShouldBeShown())
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

            _showTrialRemainingDaysEventToken = _eventAggregator.GetEvent<TrialStatusChangedEvent>().Subscribe(
                () => RaisePropertyChanged(nameof(ShowTrialRemainingDaysInfo))
                );

            _campaignHelper.PropertyChanged += CampaignHelperOnPropertyChanged;
            _eventAggregator.GetEvent<TrialStatusChangedEvent>().Subscribe(
                () => RaisePropertyChanged(nameof(TrialRemainingDaysInfoText)));
        }

        private void CampaignHelperOnPropertyChanged(object o, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ICampaignHelper.ExtendLicenseUrl))
                RaisePropertyChanged(nameof(TrialExtendLink));
        }

        public async Task UnmountViewAsync()
        {
            await Task.FromResult(false);
            _eventAggregator.GetEvent<ShowUpdateInteractionEvent>().Unsubscribe(_showUpdateInteractionEventToken);
            _eventAggregator.GetEvent<SetShowUpdateEvent>().Unsubscribe(_setShowUpdateEventToken);

            _aggregator.GetEvent<CloseMainWindowEvent>().Unsubscribe(OnCloseMainWindow);
            _eventAggregator.GetEvent<TrialStatusChangedEvent>().Unsubscribe(_showTrialRemainingDaysEventToken);

            _campaignHelper.PropertyChanged -= CampaignHelperOnPropertyChanged;
            _eventAggregator.GetEvent<TrialStatusChangedEvent>().Unsubscribe(
                () => RaisePropertyChanged(nameof(TrialRemainingDaysInfoText)));
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
