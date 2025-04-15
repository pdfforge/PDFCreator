using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Commands.EvaluateSettingsCommands;
using pdfforge.PDFCreator.UI.Presentation.Events;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities;
using Prism.Events;
using Prism.Regions;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings
{
    public class CreatorSettingsButtonsViewModel :  TranslatableViewModelBase<ApplicationSettingsViewTranslation>, IMountableAsync
    {
        private readonly IGpoSettings _gpoSettings;
        private readonly IEventAggregator _eventAggregator;
        private readonly EditionHelper _editionHelper;
        protected readonly IDispatcher _dispatcher;
        private readonly IRegionManager _regionManager;

        public CreatorSettingsButtonsViewModel(
            IGpoSettings gpoSettings,
            ITranslationUpdater translationUpdater,
            IEventAggregator eventAggregator,
            ICommandLocator commandLocator,
            EditionHelper editionHelper,
            IDispatcher dispatcher,
            IRegionManager regionManager) :base(translationUpdater)
        {
            _gpoSettings = gpoSettings;
            _eventAggregator = eventAggregator;
            _editionHelper = editionHelper;
            _dispatcher = dispatcher;
            _regionManager = regionManager;

            _eventAggregator.GetEvent<NavigateApplicationSettingsEvent>().Subscribe(targetView => ActivePath = targetView);
          
            NavigateCommand = commandLocator?.CreateMacroCommand()
                .AddCommand<SkipIfSameNavigationTargetCommand>()
                .AddCommand<EvaluateTabSwitchRelevantSettingsAndNotifyUserCommand>()
                .AddCommand<ISaveChangedSettingsCommand>()
                .AddCommand<NavigateApplicationSettingsTabCommand>()
                .Build();
            
        }


        protected override void OnTranslationChanged()
        {
            base.OnTranslationChanged();
            RaisePropertyChanged(nameof(IsRegionNameLong));
        }

        public ICommand NavigateCommand { get; set; }

        public bool ShowLicense
        {
            get
            {
                var needsLicense = !(_editionHelper.IsFreeEdition || _editionHelper.IsCustom);
                var isAllowedByGpo = (_gpoSettings != null && !_gpoSettings.HideLicenseTab);

                return  needsLicense && isAllowedByGpo;
            }
        } 

        private string _activePath = RegionNames.GeneralSettingsTabContentRegion;

        public bool IsRegionNameLong
        {
            get
            {
                if (Translation.General.Length>14
                    || Translation.Debug.Length > 14
                    || Translation.Viewer.Length > 14
                    || Translation.Title.Length > 14
                    || Translation.DirectImageConversion.Length > 14
                    || Translation.License.Length > 14
                    )
                    return true;
                return false;
            }

        }

    public string ActivePath
        {
            set
            {
                _activePath = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsRegionNameLong));
            }
            get
            {
                return _activePath;
            }
        }

        public async Task MountViewAsync()
        {
            await _dispatcher.InvokeAsync(() =>
            {
                var selectedView = _regionManager.Regions[RegionNames.ApplicationSettingsTabsRegion].Views.FirstOrDefault();
                if (selectedView == null)
                {
                    NavigateCommand.Execute(RegionViewName.GeneralSettings);
                    ActivePath = RegionViewName.GeneralSettings;
                }
                else
                    ActivePath = selectedView.GetType().Name;
                
                RaisePropertyChanged(nameof(ActivePath));
                RaisePropertyChanged(nameof(IsRegionNameLong));
            });
        }


        public Task UnmountViewAsync()
        {
            return Task.CompletedTask;
        }
    }
}
