using Prism.Regions;

namespace pdfforge.PDFCreator.UI.Presentation
{
    public interface IRegionHelper
    {
        string CurrentRegionName { get; }
    }

    public class RegionHelper : IRegionHelper
    {
        private readonly IRegionManager _regionManager;

        public RegionHelper(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public string CurrentRegionName => _regionManager.Regions[RegionNames.MainRegion].NavigationService.Journal?.CurrentEntry?.Uri?.OriginalString;
    }
}
