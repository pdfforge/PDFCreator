﻿using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting;
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
        
        public ICommand NavigateCommand { get; set; }

        public bool ShowLicense => !_editionHelper.IsFreeEdition && (_gpoSettings != null && !_gpoSettings.HideLicenseTab);

        private string _activePath = RegionNames.GeneralSettingsTabContentRegion;

        public string ActivePath
        {
            set
            {
                _activePath = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(ActivePathIsLockedByGpo));
            }
            get
            {
                return _activePath;
            }
        }

        public bool ActivePathIsLockedByGpo
        {
            get
            {
                if (_gpoSettings == null)
                    return false;

                if (_gpoSettings.DisableApplicationSettings)
                    return true;

                if (ActivePath.Equals(RegionViewName.TitleReplacementsRegionView))
                    return _gpoSettings.DisableTitleTab;
                if (ActivePath.Equals(RegionViewName.DebugSettingRegionView))
                    return _gpoSettings.DisableDebugTab;

                return false;
            } 
        }

        public async Task MountViewAsync()
        {
            await _dispatcher.InvokeAsync(() =>
            {
                var selectedView = _regionManager.Regions[RegionNames.ApplicationSettingsTabsRegion].Views.First();
                ActivePath = selectedView.GetType().Name;
                RaisePropertyChanged(nameof(ActivePath));
            });
        }


        public async Task UnmountViewAsync()
        {
        }
    }
}