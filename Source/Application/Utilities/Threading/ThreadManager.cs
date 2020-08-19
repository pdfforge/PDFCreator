using NLog;
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
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentQueue<ISynchronizedThread> _threads = new ConcurrentQueue<ISynchronizedThread>();

        private bool _isShuttingDown;

        public Action UpdateAfterShutdownAction { get; set; }

#pragma warning disable CS0067

        public event EventHandler<ThreadFinishedEventArgs> CleanUpAfterThreadClosed;

#pragma warning restore CS0067

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
            }

            _logger.Debug("All synchronized threads have ended");
        }

        public void Shutdown()
        {
            _logger.Debug("Shutting down the application");
            _isShuttingDown = true;

            foreach (var thread in _threads.ToArray())
            {
                if (string.IsNullOrEmpty(thread.Name))
                    _logger.Debug("Aborting thread");
                else
                    _logger.Debug("Aborting thread " + thread.Name);

                thread.Abort();
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
