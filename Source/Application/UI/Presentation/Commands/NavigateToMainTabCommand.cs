using pdfforge.PDFCreator.UI.Presentation.Events;
using Prism.Events;
using Prism.Regions;
using System;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class NavigateToMainTabCommand : ICommand
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _aggregator;

        public NavigateToMainTabCommand(IRegionManager regionManager, IEventAggregator aggregator)
        {
            _regionManager = regionManager;
            _aggregator = aggregator;
        }

        //public set to overwrite MainRegionName for PDFCreator Server
        public string MainRegionName { get; set; } = RegionNames.MainRegion;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _regionManager.RequestNavigate(MainRegionName, parameter.ToString(), OnNavigateMainTab);
        }

        private void OnNavigateMainTab(NavigationResult navigationResult)
        {
            var navigateMainTabEvent = _aggregator.GetEvent<NavigateMainTabEvent>();
            navigateMainTabEvent.Publish(navigationResult.Context.Uri.ToString());
        }

#pragma warning disable 67

        public event EventHandler CanExecuteChanged;

#pragma warning restore 67
    }
}
