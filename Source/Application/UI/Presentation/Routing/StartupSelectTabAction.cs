using pdfforge.PDFCreator.Core.Controller.Routing;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.Routing
{
    public class StartupSelectTabAction : StartupAction
    {
        public readonly string TabRegion;
        public readonly string TabName;

        public StartupSelectTabAction(string tabRegion, string tabName)
        {
            TabRegion = tabRegion;
            TabName = tabName;
        }

        public override Task Execute()
        {
            return Task.FromResult(false);
        }
    }
}
