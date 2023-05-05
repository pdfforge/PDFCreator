using pdfforge.PDFCreator.UI.Presentation.Events;
using Prism.Events;
using Prism.Regions;
using System;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class NavigateApplicationSettingsTabCommand : ICommand
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _aggregator;

        public NavigateApplicationSettingsTabCommand(IRegionManager regionManager, IEventAggregator aggregator)
        {
            _regionManager = regionManager;
            _aggregator = aggregator;
        }

        //public set to overwrite MainRegionName for PDFCreator Server
        public string RegionName { get; set; } = RegionNames.ApplicationSettingsTabsRegion;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _regionManager.RequestNavigate(RegionName, parameter.ToString(), OnNavigateSettingsTab);
        }

        private void OnNavigateSettingsTab(NavigationResult navigationResult)
        {
            var navigateApplicationSettingsEvent = _aggregator.GetEvent<NavigateApplicationSettingsEvent>();
            navigateApplicationSettingsEvent.Publish(navigationResult.Context.Uri.ToString());
        }

#pragma warning disable 67

        public event EventHandler CanExecuteChanged;

#pragma warning restore 67
    }
}
