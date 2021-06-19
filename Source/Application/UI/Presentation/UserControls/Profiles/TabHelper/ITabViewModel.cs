using pdfforge.PDFCreator.Core.ServiceLocator;
using pdfforge.PDFCreator.Core.Services;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper
{
    public interface ITabViewModel : IWhitelisted, IMountable
    {
        string Title { get; }
        IconList Icon { get; }

        bool HiddenByGPO { get; }
        bool BlockedByGPO { get; }
    }
}
