using pdfforge.PDFCreator.Core.Services.Update;
using pdfforge.PDFCreator.UI.Presentation.Assistants.Update;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper
{
    public class DesignTimeUpdateLauncher : IUpdateLauncher
    {
        public Task LaunchUpdateAsync(IApplicationVersion version)
        {
            return Task.FromResult((object)null);
        }
    }
}
