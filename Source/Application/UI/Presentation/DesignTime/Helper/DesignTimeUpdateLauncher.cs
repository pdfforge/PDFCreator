using System.Threading.Tasks;
using pdfforge.PDFCreator.UI.Presentation.Assistants.Update;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper
{
    public class DesignTimeUpdateLauncher : IUpdateLauncher
    {
        public Task LaunchUpdate(IApplicationVersion version)
        {
            return Task.FromResult((object) null);
        }
    }
}