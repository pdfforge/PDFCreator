using pdfforge.Communication;
using pdfforge.PDFCreator.Utilities.Threading;
using System.Diagnostics;
using System.Threading;

namespace pdfforge.PDFCreator.Editions.EditionBase
{
#if DEBUG

    public static class DebugStandbyHelper
    {
        public static bool IsStandbyRunning()
        {
            var mutex = new Mutex(false, "Global\\" + ThreadManager.StandbyMutexName);

            try
            {
                var acquired = mutex.WaitOne(0);
                if (acquired)
                    mutex.ReleaseMutex();

                return !acquired;
            }
            catch (AbandonedMutexException)
            {
                return false;
            }
        }

        public static void TerminateStandby()
        {
            var pipeName = "PDFCreator-" + Process.GetCurrentProcess().SessionId;
            var pipeClient = new PipeClient(pipeName);

            pipeClient.SendMessage("StopHotStandby|", 500);
        }
    }

#endif
}
