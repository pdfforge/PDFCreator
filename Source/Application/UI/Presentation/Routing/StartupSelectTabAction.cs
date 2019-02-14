using pdfforge.PDFCreator.Core.Controller.Routing;

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

        public override void Execute()
        {
        }
    }
}
