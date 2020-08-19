using pdfforge.PDFCreator.Core.ServiceLocator;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Helper;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper
{
    public interface ITabViewModel : IHasNotSupportedFeatures, IWhitelisted, IMountable
    {
        string Title { get; }
        IconList Icon { get; }

        bool HiddenByGPO { get; }
        bool BlockedByGPO { get; }
    }

    public interface IHasNotSupportedFeatures
    {
        bool HasNotSupportedFeatures { get; }
    }
}
