using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Utilities
{
    public interface IShellExecuteHelper
    {
        ShellExecuteResult RunAsAdmin(string path, string arguments);
        Task<ShellExecuteResult> RunAsAdminAsync(string path, string arguments);
    }
}
