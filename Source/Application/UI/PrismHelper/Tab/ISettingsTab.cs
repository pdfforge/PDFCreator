using pdfforge.PDFCreator.Core.ServiceLocator;
using Prism.Regions;
using SimpleInjector;

namespace pdfforge.PDFCreator.UI.PrismHelper.Tab
{
    public interface ISettingsTab
    {
        void Register(IRegionManager regionManager, IWhitelistedServiceLocator serviceLocator, string regionName);

        void RegisterNavigationViews(Container container);
    }
}
