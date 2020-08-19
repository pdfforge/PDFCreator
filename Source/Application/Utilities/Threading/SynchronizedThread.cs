using System;
using System.Threading;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Utilities.Threading
{
    /// <summary>
    ///     SynchronizedThread wraps a Thread object and adds some functionality to manage multiple threads. It allows to
    ///     subscribe to an event that is launched when the Thread work has finished.
    /// </summary>
    public sealed class SynchronizedThread : ISynchronizedThread
    {
        public Thread Thread { get; }
        private readonly ThreadStart _threadFunction;
        private readonly TaskCompletionSource<bool> _taskCompletionSource = new TaskCompletionSource<bool>();

        /// <summary>
        ///     Creates a new SynchronizedThread with the given function
        /// </summary>
        /// <param name="threadFunction">Thread delegate</param>
        public SynchronizedThread(ThreadStart threadFunction)
        {
            Thread = new Thread(RunThread);
            _threadFunction = threadFunction;
        }

        /// <summary>
        ///     OnThreadFinished is fired when the ThreadExecution has finished
        /// </summary>
        public event EventHandler<ThreadFinishedEventArgs> OnThreadFinished;

        /// <summary>
        ///     Gets or sets the name of the Thread
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets the current state of the thread
        /// </summary>
        public ThreadState ThreadState
        {
            get { return Thread.ThreadState; }
        }

        /// <summary>
        ///     Determines if the thread is alive
        /// </summary>
        public bool IsAlive
        {
            get { return Thread.IsAlive; }
        }

        /// <summary>
        ///     Sets the threading apartment state of the Thread. This must be called before starting the thread
        /// </summary>
        /// <param name="state">The new state</param>
        public void SetApartmentState(ApartmentState state)
        {
            Thread.SetApartmentState(state);
        }

        /// <summary>
        ///     Waits for the Thread to finish
        /// </summary>
        public void Join()
        {
            Thread.Join();
        }

        /// <summary>
        ///     Waits for the Thread to finish for the given amount of milliseconds
        /// </summary>
        /// <param name="millisecondsTimeout">Number of milliseconds to wait</param>
        public bool Join(int millisecondsTimeout)
        {
            return Thread.Join(millisecondsTimeout);
        }

        /// <summary>
        ///     Waits for the Thread to finish for the given TimeSpan
        /// </summary>
        /// <param name="timeout">TimeSpan to wait</param>
        public bool Join(TimeSpan timeout)
        {
            return Thread.Join(timeout);
        }

        public Task JoinAsync()
        {
            return _taskCompletionSource.Task;
        }

        /// <summary>
        ///     Starts thread execution
        /// </summary>
        public void Start()
        {
            if (!string.IsNullOrEmpty(Name))
                Thread.Name = Name;

            Thread.Start();
        }

        /// <summary>
        ///     Aborts the thread
        /// </summary>
        public void Abort()
        {
            Thread.Abort();
        }

        private void RunThread()
        {
            try
            {
                _threadFunction();
            }
            finally
            {
                OnThreadFinished?.Invoke(this, new ThreadFinishedEventArgs(this));
                _taskCompletionSource.SetResult(true);
            }
        }
    }

    /// <summary>
    ///     ThreadFinishedEventArgs holds arguments for the OnThreadFinished event
    /// </summary>
    public class ThreadFinishedEventArgs : EventArgs
    {
        /// <summary>
        ///     Create new EventArgs
        /// </summary>
        /// <param name="thread">SynchronizedThread that is concerned</param>
        public ThreadFinishedEventArgs(ISynchronizedThread thread)
        {
            SynchronizedThread = thread;
        }

        /// <summary>
        ///     Gets the SynchronizedThread object
        /// </summary>
        public ISynchronizedThread SynchronizedThread { get; private set; }
    }
}
