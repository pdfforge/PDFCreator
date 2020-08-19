using pdfforge.PDFCreator.Core.StartupInterface;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Core.Startup.AppStarts
{
    public abstract class AppStartBase : IAppStart
    {
        private readonly ICheckAllStartupConditions _checkAllStartupConditions;

        protected AppStartBase(ICheckAllStartupConditions checkAllStartupConditions)
        {
            _checkAllStartupConditions = checkAllStartupConditions;
        }

        public abstract Task<ExitCode> Run();

        public bool SkipStartupConditionCheck { get; set; }

        public void CheckApplicationConditions()
        {
            _checkAllStartupConditions.CheckAll();
        }
    }
}
