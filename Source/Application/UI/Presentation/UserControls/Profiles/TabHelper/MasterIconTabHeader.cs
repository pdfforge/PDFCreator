using System.Windows;
using Prism.Regions;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper
{
    public class MasterIconTabHeader : FrameworkContentElement, IRegionMemberLifetime
    {
        public bool KeepAlive { get; } = true;
        public MasterIconTabHeader(string regionName)
        {
            RegionName = regionName;
        }

        public string RegionName { get; }
    }
}
