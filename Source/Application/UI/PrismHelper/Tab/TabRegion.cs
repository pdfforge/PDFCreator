using pdfforge.PDFCreator.Core.ServiceLocator;
using Prism.Events;
using Prism.Regions;
using SimpleInjector;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.UI.PrismHelper.Tab
{
    public interface ITabRegion
    {
        string RegionName { get; }
        IList<ISettingsTab> Tabs { get; }

        void RegisterTabs(IRegionManager regionManager, IWhitelistedServiceLocator serviceLocator, IEventAggregator eventAggregator);

        void RegisterNavigationViews(Container container);

        void Add(ISettingsTab tab);
    }

    public class TabRegion : ITabRegion
    {
        public string RegionName { get; }
        public IList<ISettingsTab> Tabs { get; } = new List<ISettingsTab>();

        public TabRegion(string regionName)
        {
            RegionName = regionName;
        }

        public void RegisterTabs(IRegionManager regionManager, IWhitelistedServiceLocator serviceLocator, IEventAggregator eventAggregator)
        {
            foreach (var pageTab in Tabs)
            {
                pageTab.Register(regionManager, serviceLocator, eventAggregator, RegionName);
            }
        }

        public void RegisterNavigationViews(Container container)
        {
            foreach (var pageTab in Tabs)
            {
                pageTab.RegisterNavigationViews(container);
            }
        }

        public void Add(ISettingsTab tab)
        {
            Tabs.Add(tab);
        }
    }
}
