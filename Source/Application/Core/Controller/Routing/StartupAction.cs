using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Core.Controller.Routing
{
    public abstract class StartupAction : IStartupAction
    {
        public abstract Task Execute();
    }
}
