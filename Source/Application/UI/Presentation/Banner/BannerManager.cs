using System.Threading.Tasks;
using System.Windows;

namespace pdfforge.PDFCreator.UI.Presentation.Banner
{
    public interface IBannerManager
    {
        Task<UIElement> GetBanner(string slot);
    }

    public class BannerManagerDefault : IBannerManager
    {
        public Task<UIElement> GetBanner(string slot)
        {
            return Task.FromResult((UIElement)null);
        }
    }
}
