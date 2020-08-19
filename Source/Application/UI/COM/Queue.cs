using NLog;
using pdfforge.PDFCreator.Core.ComImplementation;
using pdfforge.PDFCreator.Core.JobInfoQueue;
using System;
using System.Runtime.InteropServices;

namespace pdfforge.PDFCreator.UI.COM
{
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [Guid("3803F46C-F5AA-4B86-8B9C-6EFFAC9CDCFA")]
    public interface IQueue
    {
        void Initialize();

        bool WaitForJob(int timeOut);

        bool WaitForJobs(int jobCount, int timeOut);

        int Count { get; }
        PrintJob NextJob { get; }

        PrintJob GetJobByIndex(int jobIndex);

        void MergeJobs(PrintJob job1, PrintJob job2);

        void MergeAllJobs();

        void Clear();

        void DeleteJob(int index);

        void ReleaseCom();
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("66A9CAB1-404A-4918-8DE2-29C26B9B271E")]
    [ProgId("PDFCreator.JobQueue")]
    public class Queue : IQueue
    {
        private readonly IJobInfoQueue _jobInfoQueue;
        private readonly IPrintJobAdapterFactory _printJobAdapterFactory;
        private readonly QueueAdapter _queueAdapter;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public Queue()
        {
            try
            {
                var builder = new ComDependencyBuilder();
                var dependencies = builder.ComDependencies;

                _queueAdapter = dependencies.QueueAdapter;
                _printJobAdapterFactory = _queueAdapter.PrintJobAdapterFactory;
                _jobInfoQueue = _queueAdapter.JobInfoQueue;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
        }

        /// <summary>
        ///     Initializes the essential components like JobInfoQueue for the COM object
        /// </summary>
        public void Initialize()
        {
            try
            {
                _queueAdapter.Initialize();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
        }

        /// <summary>
        ///     Waits for exactly one job to enter the queue
        /// </summary>
        /// <param name="timeOut">Duration which the queue should wait for a job</param>
        /// <returns>False, if the duration was exceeded. Otherwise it returns true</returns>
        public bool WaitForJob(int timeOut)
        {
            return _queueAdapter.WaitForJob(timeOut);
        }

        /// <summary>
        ///     Waits for n jobs to enter the queue
        /// </summary>
        /// <param name="jobCount">Number of jobs to wait for</param>
        /// <param name="timeOut">Duration which the queue should wait for the n jobs</param>
        /// <returns>False, if the duration was exceeded. Otherwise it returns true</returns>
        public bool WaitForJobs(int jobCount, int timeOut)
        {
            return _queueAdapter.WaitForJobs(jobCount, timeOut);
        }

        /// <summary>
        ///     Returns the number of jobs in the queue
        /// </summary>
        public int Count => _queueAdapter.Count;

        /// <summary>
        ///     Returns the next job in the queue as a ComJob
        /// </summary>
        public PrintJob NextJob => new PrintJob(_queueAdapter.NextJob, _jobInfoQueue, _printJobAdapterFactory);

        /// <summary>
        ///     Creates the job from the queue by index
        /// </summary>
        /// <param name="jobIndex">Index of the jobinfo in the queue</param>
        /// <returns>The corresponding ComJob</returns>
        public PrintJob GetJobByIndex(int jobIndex)
        {
            return new PrintJob(_queueAdapter.JobById(jobIndex), _jobInfoQueue, _printJobAdapterFactory);
        }

        /// <summary>
        ///     Merges two ComJobs
        /// </summary>
        /// <param name="job1">The first job to merge</param>
        /// <param name="job2">The second job to merge</param>
        public void MergeJobs(PrintJob job1, PrintJob job2)
        {
            _queueAdapter.MergeJobs(job1.JobInfo, job2.JobInfo);
        }

        /// <summary>
        ///     Merges all jobs in the queue
        /// </summary>
        public void MergeAllJobs()
        {
            _queueAdapter.MergeAllJobs();
        }

        /// <summary>
        ///     Remove all elements from the Queue
        /// </summary>
        public void Clear()
        {
            _queueAdapter.Clear();
        }

        /// <summary>
        ///     Deletes a chosen print job.
        /// </summary>
        /// <param name="index">Determines the print job to be removed by its position in the queue.</param>
        public void DeleteJob(int index)
        {
            _queueAdapter.DeleteJob(index);
        }

        /// <summary>
        ///     Shuts down the used instance
        /// </summary>
        public void ReleaseCom()
        {
            _queueAdapter.ReleaseCom();
        }
    }
}
