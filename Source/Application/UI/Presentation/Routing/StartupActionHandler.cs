using pdfforge.PDFCreator.Core.Controller.Routing;
using Prism.Regions;
using System.Linq;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.Routing
{
    public interface IStartupActionHandler
    {
        void HandleStartupActions(IRegionManager regionManager, StartupRoutine startupRoutine);
    }

    public class StartupActionHandler : IStartupActionHandler
    {
        public void HandleStartupActions(IRegionManager regionManager, StartupRoutine startupRoutine)
        {
            foreach (var action in startupRoutine.GetAllActions())
            {
                switch (action)
                {
                    case StartupNavigationAction navigationAction:
                        regionManager.RequestNavigate(navigationAction.Region, navigationAction.Target);
                        break;

                    case StartupSelectTabAction tabAction:
                        var region = regionManager.Regions[tabAction.TabRegion];
                        var view = region.Views.Where(v => v is TabItem).Cast<TabItem>().FirstOrDefault(t => t.Name == tabAction.TabName);

                        if (view != null)
                            region.Activate(view);
                        else
                            regionManager.RequestNavigate(tabAction.TabRegion, tabAction.TabName);
                        break;
                }
            }
        }
    }
}
