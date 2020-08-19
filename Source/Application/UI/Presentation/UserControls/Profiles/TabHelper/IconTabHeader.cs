using System.Windows;
using Prism.Regions;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper
{
    public class IconTabHeader : FrameworkContentElement, IRegionMemberLifetime
    {
        public bool KeepAlive { get; } = true;
    }
}
