using NLog;
using System;
using System.Threading;

namespace pdfforge.PDFCreator.Utilities.Threading
{
    internal class LocalMutex
    {
        private readonly string _mutexName;
        private Thread _mutexThread;
        private readonly AutoResetEvent _mutexAcquiredEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _releaseMutexEvent = new AutoResetEvent(false);
        private bool _wasAcquired;
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public LocalMutex(string mutexName)
        {
            _mutexName = mutexName;
        }

        public bool Acquire()
        {
            if (_mutexThread != null)
                throw new InvalidOperationException("The mutex already was acquired!");

            _mutexThread = new Thread(MutexThread);
            _mutexThread.Start();

            _mutexAcquiredEvent.WaitOne();

            return _wasAcquired;
        }

        private void MutexThread()
        {
            var mutex = new Mutex(false, _mutexName);

            try
            {
                try
                {
                    var timeout = TimeSpan.FromSeconds(1);

                    var wasClaimed = mutex.WaitOne(timeout);

                    if (!wasClaimed)
                    {
                        _logger.Info($"Could not claim local mutex {_mutexName} within {timeout.TotalSeconds}s");
                        return;
                    }
                }
                catch (AbandonedMutexException)
                {
                }

                _wasAcquired = true;
                _mutexAcquiredEvent.Set();

                _releaseMutexEvent.WaitOne();
            }
            finally
            {
                mutex.ReleaseMutex();
                _mutexThread = null;
            }
        }

        public void Release()
        {
            _releaseMutexEvent.Set();
        }
    }
}
