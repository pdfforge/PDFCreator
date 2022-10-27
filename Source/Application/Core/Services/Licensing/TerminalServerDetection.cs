using pdfforge.PDFCreator.Utilities.WindowsApi;

namespace pdfforge.PDFCreator.Setup.Shared.Helper
{
    public enum TerminalServerMode
    {
        NoTerminalServer = 0,
        IsTerminalServer = 1,
        IsWindowsEnterpriseMultiSession = 2
    }

    public interface ITerminalServerDetection
    {
        bool IsTerminalServer();

        bool IsWindowsEnterpriseMultiSession();

        TerminalServerMode GetTerminalServerMode();
    }

    public class TerminalServerDetection : ITerminalServerDetection
    {
        private readonly IKernel32Wrapper _kernel32Wrapper;

        public TerminalServerDetection(IKernel32Wrapper kernel32Wrapper)
        {
            _kernel32Wrapper = kernel32Wrapper;
        }

        public TerminalServerMode GetTerminalServerMode()
        {
            if (!IsTerminalServer())
                return TerminalServerMode.NoTerminalServer;

            if (IsWindowsEnterpriseMultiSession())
                return TerminalServerMode.IsWindowsEnterpriseMultiSession;

            return TerminalServerMode.IsTerminalServer;
        }

        public bool IsTerminalServer()
        {
            var lWinVer = _kernel32Wrapper.GetVersionEx();

            var isTerminal = (lWinVer.wSuiteMask & SuiteMask.VER_SUITE_TERMINAL) == SuiteMask.VER_SUITE_TERMINAL;
            var isSingleUserTs = (lWinVer.wSuiteMask & SuiteMask.VER_SUITE_SINGLEUSERTS) == SuiteMask.VER_SUITE_SINGLEUSERTS;

            return isTerminal && !isSingleUserTs;
        }

        public bool IsWindowsEnterpriseMultiSession()
        {
            if (IsTerminalServer())
            {
                var productType = _kernel32Wrapper.GetProductInfo();
                if (productType == ProductType.PRODUCT_SERVERRDSH)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
