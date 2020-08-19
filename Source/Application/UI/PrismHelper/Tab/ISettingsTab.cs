using pdfforge.PDFCreator.Core.ServiceLocator;
using Prism.Events;
using Prism.Regions;
using SimpleInjector;

namespace pdfforge.PDFCreator.UI.PrismHelper.Tab
{
    public interface ISettingsTab
    {
        void Register(IRegionManager regionManager, IWhitelistedServiceLocator serviceLocator, IEventAggregator eventAggregator, string regionName);

        void RegisterNavigationViews(Container container);
    }
}
