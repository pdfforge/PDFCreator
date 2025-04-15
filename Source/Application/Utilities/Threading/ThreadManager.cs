using Microsoft.Win32;
using NLog;
using pdfforge.Communication;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Utilities.Threading
{
    /// <summary>
    ///     The ThreadManager class handles and watches all applications threads. If all registered threads are finished, the
    ///     application will exit.
    /// </summary>
    public class ThreadManager : IThreadManager
    {
        public const string StandbyMutexName = "PDFCreator-Standby-137a7751-1070-4db4-a407-83c1625762c7";

        public TimeSpan HotStandbyDuration { get; set; } = TimeSpan.Zero;
        public bool IsStandbyDisabled { get; set; }

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentQueue<ISynchronizedThread> _threads = new ConcurrentQueue<ISynchronizedThread>();

        private bool _isShuttingDown;

        private TaskCompletionSource<bool> _stopHotStandbyCompletionSource = null;

        public Action UpdateAfterShutdownAction { get; set; }

#pragma warning disable CS0067

        public event EventHandler<ThreadFinishedEventArgs> CleanUpAfterThreadClosed;

#pragma warning restore CS0067

        public event EventHandler StandbyStarted;

        public event EventHandler StandbyEnded;

        /// <summary>
        ///     Adds and starts a synchronized thread to the thread list. The application will wait for all of these to end before
        ///     it terminates
        /// </summary>
        /// <param name="thread">A thread that needs to be synchronized. This thread will not be automatically started</param>
        public void StartSynchronizedThread(ISynchronizedThread thread)
        {
            if (_isShuttingDown)
            {
                _logger.Warn("Tried to start thread while shutdown already started!");
                return;
            }

            _logger.Debug("Adding thread " + thread.Name);

            _threads.Enqueue(thread);

            _stopHotStandbyCompletionSource?.TrySetResult(false);

            if (thread.ThreadState == ThreadState.Unstarted)
                thread.Start();
        }

        public ISynchronizedThread StartSynchronizedThread(ThreadStart threadMethod, string threadName)
        {
            return StartSynchronizedThread(threadMethod, threadName, ApartmentState.MTA);
        }

        public ISynchronizedThread StartSynchronizedUiThread(ThreadStart threadMethod, string threadName)
        {
            return StartSynchronizedThread(threadMethod, threadName, ApartmentState.STA);
        }

        /// <summary>
        ///     Wait for all Threads and exit the application afterwards
        /// </summary>
        public async Task WaitForThreads()
        {
            _logger.Debug("Waiting for all synchronized threads to end");

            while (!_threads.IsEmpty)
            {
                _logger.Debug(_threads.Count + " Threads remaining");

                if (_threads.TryDequeue(out var thread))
                {
                    await thread.JoinAsync();
                }

                if (_threads.IsEmpty)
                {
                    await WaitForStandbyDuration();
                }
            }

            _logger.Debug("All synchronized threads have ended");
        }

        private async Task WaitForStandbyDuration()
        {
            var standbyGlobalMutex = new GlobalMutex(StandbyMutexName);
            var standbyLocalMutex = new LocalMutex(StandbyMutexName);

            var systemShutdownHandler = new SessionEndingEventHandler((sender, args) => _stopHotStandbyCompletionSource?.TrySetResult(true));
            SystemEvents.SessionEnding += systemShutdownHandler;

            _stopHotStandbyCompletionSource = new TaskCompletionSource<bool>();
            standbyGlobalMutex.Acquire();
            standbyLocalMutex.Acquire();

            StandbyStarted?.Invoke(this, EventArgs.Empty);

            try
            {
                // Task.Delay does not support a Timespan with more than int.MaxValue milliseconds
                var standbyDuration = HotStandbyDuration > TimeSpan.FromMilliseconds(int.MaxValue)
                    ? TimeSpan.FromMilliseconds(-1)
                    : HotStandbyDuration;

                if (IsStandbyDisabled || UpdateAfterShutdownAction != null)
                    standbyDuration = TimeSpan.Zero;

                await Task.WhenAny(_stopHotStandbyCompletionSource.Task, Task.Delay(standbyDuration));
            }
            finally
            {
                standbyGlobalMutex.Release();
                standbyLocalMutex.Release();
                _stopHotStandbyCompletionSource = null;
                SystemEvents.SessionEnding -= systemShutdownHandler;
                StandbyEnded?.Invoke(this, EventArgs.Empty);
            }
        }

        public void StopHotStandby()
        {
            _stopHotStandbyCompletionSource?.TrySetResult(true);
        }

        public void Shutdown()
        {
            _logger.Debug("Shutting down the application");
            _isShuttingDown = true;

            foreach (var thread in _threads.ToArray())
            {
                if (string.IsNullOrEmpty(thread.Name))
                    _logger.Debug("Interrupting thread");
                else
                    _logger.Debug("Interrupt thread " + thread.Name);

                thread.Interrupt();
            }

            _logger.Debug("Exiting...");

            if (UpdateAfterShutdownAction != null)
            {
                _logger.Debug("Starting application update...");
                UpdateAfterShutdownAction();
            }
        }

        private ISynchronizedThread StartSynchronizedThread(ThreadStart threadMethod, string threadName, ApartmentState state)
        {
            _logger.Debug($"Starting {threadName} thread");

            var t = new SynchronizedThread(threadMethod);
            t.Name = threadName;
            t.SetApartmentState(state);

            StartSynchronizedThread(t);
            return t;
        }
    }
}
