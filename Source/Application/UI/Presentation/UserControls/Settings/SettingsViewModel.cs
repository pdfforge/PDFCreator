using System.Threading.Tasks;
using System.Windows.Input;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Commands.EvaluateSettingsCommands;
using pdfforge.PDFCreator.UI.Presentation.Events;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using Prism.Events;
using Prism.Mvvm;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings
{
    public class SettingsViewModel :  TranslatableViewModelBase<ApplicationSettingsViewTranslation>, IMountableAsync
    {
        private readonly IGpoSettings _gpoSettings;
        private readonly IEventAggregator _eventAggregator;
        private readonly EditionHelper _editionHelper;

        public SettingsViewModel(
            IGpoSettings gpoSettings,
            ITranslationUpdater translationUpdater,
            IEventAggregator eventAggregator,
            ICommandLocator commandLocator,
            EditionHelper editionHelper) :base(translationUpdater)
        {
            _gpoSettings = gpoSettings;
            _eventAggregator = eventAggregator;
            _editionHelper = editionHelper;

            _eventAggregator.GetEvent<NavigateApplicationSettingsEvent>().Subscribe(targetView => ActivePath = targetView);

            NavigateCommand = commandLocator?.CreateMacroCommand()
                .AddCommand<SkipIfSameNavigationTargetCommand>()
                .AddCommand<EvaluateTabSwitchRelevantSettingsAndNotifyUserCommand>()
                .AddCommand<ISaveChangedSettingsCommand>()
                .AddCommand<NavigateApplicationSettingsTabCommand>()
                .Build();
        }
        
        public ICommand NavigateCommand { get; set; }

        public bool ShowLicense => !_editionHelper.IsFreeEdition && (_gpoSettings != null && !_gpoSettings.HideLicenseTab);
        public bool ApplicationSettingsIsDisabled => _gpoSettings is { DisableApplicationSettings: true };

        private string _activePath = RegionNames.GeneralSettingsTabContentRegion;
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

        public async Task MountViewAsync()
        {
            NavigateCommand.Execute(RegionViewName.GeneralSettingsRegionView);
        }
        

        public async Task UnmountViewAsync()
        {
        }
    }
}
