using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Core.Services
{
    public static class StaticTaskHelper
    {
        public static Task CompletedTask { get; } = Task.FromResult(false);
    }
}
