using pdfforge.PDFCreator.Core.ServiceLocator;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper
{
    public interface ITabViewModel : IHasNotSupportedFeatures, IWhitelisted
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
