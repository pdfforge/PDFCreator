using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Core.StartupInterface
{
    public interface IAppStart
    {
        /// <summary>
        /// Check all applicable startup conditions. If a condition fails, a StartupConditionFailedException will be thrown
        /// </summary>
        void CheckApplicationConditions();

        /// <summary>
        /// Run the application start. The exit code will be used as exit code of the application.
        /// </summary>
        /// <returns></returns>
        Task<ExitCode> Run();
    }
}
