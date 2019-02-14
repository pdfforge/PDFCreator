using pdfforge.PDFCreator.UI.Presentation.ServiceLocator;
using Prism.Regions;
using SimpleInjector;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Editions.EditionBase.Tab
{
    public interface ITabRegion
    {
        string RegionName { get; }
        IList<ISettingsTab> Tabs { get; }

        void RegisterTabs(IRegionManager regionManager, IWhitelistedServiceLocator serviceLocator);

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

        public void RegisterTabs(IRegionManager regionManager, IWhitelistedServiceLocator serviceLocator)
        {
            foreach (var pageTab in Tabs)
            {
                pageTab.Register(regionManager, serviceLocator, RegionName);
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
