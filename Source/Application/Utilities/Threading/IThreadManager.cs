using System;
using System.Threading;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Utilities.Threading
{
    public interface IThreadManager
    {
        Action UpdateAfterShutdownAction { get; set; }
        TimeSpan HotStandbyDuration { get; set; }
        bool IsStandbyDisabled { get; set; }

        event EventHandler<ThreadFinishedEventArgs> CleanUpAfterThreadClosed;

        event EventHandler StandbyStarted;

        event EventHandler StandbyEnded;

        void StartSynchronizedThread(ISynchronizedThread thread);

        ISynchronizedThread StartSynchronizedThread(ThreadStart threadMethod, string threadName);

        ISynchronizedThread StartSynchronizedUiThread(ThreadStart threadMethod, string threadName);

        void Shutdown();

        Task WaitForThreads();

        void StopHotStandby();
    }
}
