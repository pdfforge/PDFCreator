using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Core.Controller.Routing
{
    public interface IStartupAction
    {
        Task Execute();
    }
}
