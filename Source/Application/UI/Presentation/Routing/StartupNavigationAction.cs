using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.Routing
{
    public class StartupNavigationAction : IMainShellStartupAction
    {
        public string Region;
        public string Target;

        public Task Execute()
        {
            return Task.FromResult(false);
        }
    }
}
