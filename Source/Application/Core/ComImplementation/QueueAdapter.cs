using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Communication;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Workflow;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace pdfforge.PDFCreator.Core.ComImplementation
{
    public class QueueAdapter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IJobInfoManager _jobInfoManager;

        private readonly AutoResetEvent _newJobResetEvent = new AutoResetEvent(false);
        private readonly IPipeServerManager _pipeServerManager;
        private readonly ComStartupConditionChecker _startupConditionChecker;
        private readonly ISettingsProvider _settingsProvider;

        private readonly ThreadPool _threadPool;

        private bool _isComActive;

        public QueueAdapter(ThreadPool threadPool, IJobInfoQueue jobInfoQueue, ISettingsProvider settingsProvider, IJobInfoManager jobInfoManager, IPipeServerManager pipeServerManager, IPrintJobAdapterFactory printJobAdapterFactory, ComStartupConditionChecker startupConditionChecker)
        {
            _threadPool = threadPool;
            JobInfoQueue = jobInfoQueue;
            PrintJobAdapterFactory = printJobAdapterFactory;
            _settingsProvider = settingsProvider;
            _jobInfoManager = jobInfoManager;
            _pipeServerManager = pipeServerManager;
            _startupConditionChecker = startupConditionChecker;
        }

        public IJobInfoQueue JobInfoQueue { get; }
        public IPrintJobAdapterFactory PrintJobAdapterFactory { get; set; }

        public int Count => JobInfoQueue.Count;

        private bool IsServerInstanceRunning => _pipeServerManager.IsServerRunning();

        public Job NextJob
        {
            get { return JobById(0); }
        }

        public void Initialize()
        {
            Logger.Trace("COM: Starting initialization process");

            _isComActive = true;

            var startupCheck = _startupConditionChecker.CheckStartupConditions();
            if (!startupCheck.Item1)
                throw new InvalidOperationException("Can't initialize the COM interface! " + startupCheck.Item2);

            if (IsServerInstanceRunning)
                throw new InvalidOperationException("Access forbidden. An instance of PDFCreator is currently running.");

            JobInfoQueue.OnNewJobInfo += (sender, eventArgs) => OnNewJob();

            Logger.Trace("COM: Starting pipe server thread");
            _pipeServerManager.StartServer();
        }

        public void MergeJobs(JobInfo job1, JobInfo job2)
        {
            Logger.Trace("COM: Merging two ComJobs.");
            _jobInfoManager.Merge(job1, job2);
            JobInfoQueue.Remove(job2);
        }

        public void MergeAllJobs()
        {
            if (JobInfoQueue.Count == 0)
                throw new COMException("The queue must not be empty.");

            Logger.Trace("COM: Merging all ComJobs.");
            while (JobInfoQueue.Count > 1)
            {
                var firstJob = JobInfoQueue.JobInfos[0];
                var nextJob = JobInfoQueue.JobInfos[1];

                _jobInfoManager.Merge(firstJob, nextJob);
                JobInfoQueue.Remove(nextJob);
            }
        }

        public void Clear()
        {
            while (JobInfoQueue.JobInfos.Count > 0)
            {
                JobInfoQueue.Remove(JobInfoQueue.JobInfos[0], true);
            }
        }

        public void DeleteJob(int index)
        {
            if (index < 0 || index >= Count)
                throw new COMException("The given index was out of range.");

            JobInfoQueue.Remove(JobInfoQueue.JobInfos[index], true);
        }

        public bool WaitForJob(int timeOut)
        {
            return WaitForJobs(1, TimeSpan.FromSeconds(timeOut));
        }

        public bool WaitForJobs(int jobCount, int timeOut)
        {
            var ts = TimeSpan.FromSeconds(timeOut);
            return WaitForJobs(jobCount, ts);
        }

        private bool WaitForJobs(int jobCount, TimeSpan timeOut)
        {
            if (!_isComActive)
                throw new COMException("No COM instance was found. Initialize the object first.");

            Logger.Trace("Waiting for {0} jobs for {1} seconds", jobCount, timeOut);

            var maxTime = DateTime.Now + timeOut;

            while (JobInfoQueue.Count < jobCount)
            {
                if (DateTime.Now > maxTime)
                    return false;

                _newJobResetEvent.WaitOne(timeOut);
            }
            return true;
        }

        private void OnNewJob()
        {
            _newJobResetEvent.Set();
        }

        public void ReleaseCom()
        {
            if (_isComActive)
            {
                if (!_threadPool.Join(TimeSpan.FromSeconds(4)))
                    throw new COMException("One of the thread jobs didn't finish within the time out.");

                Logger.Trace("COM: Cleaning up COM resources.");
                if (IsServerInstanceRunning)
                    _pipeServerManager.Shutdown();

                _isComActive = false;

                Logger.Trace("COM: Emptying queue.");
                Clear();
            }
            else
            {
                throw new COMException("No COM Instance was found.");
            }
        }

        /// <summary>
        ///     Creates the job from the queue by index
        /// </summary>
        /// <param name="id">Index of the jobinfo in the queue</param>
        /// <returns>The corresponding ComJob</returns>
        public Job JobById(int id)
        {
            try
            {
                var currentJobInfo = JobInfoQueue.JobInfos[id];

                return new Job(currentJobInfo, _settingsProvider.GetDefaultProfile(), _settingsProvider.Settings.ApplicationSettings.Accounts);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new COMException("Invalid index. Please check the index parameter.");
            }
        }
    }
}
