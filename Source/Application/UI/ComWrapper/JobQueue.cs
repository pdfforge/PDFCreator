using System;

namespace pdfforge.PDFCreator.UI.ComWrapper
{
	public class JobQueue
	{
		private readonly dynamic _jobQueue;
		
		public JobQueue()
		{
			Type jobQueueType = Type.GetTypeFromProgID("PDFCreator.JobQueue");
			_jobQueue = Activator.CreateInstance(jobQueueType);
		}
		internal JobQueue(dynamic jobQueue)
		{
			_jobQueue = jobQueue;
		}
		public int Count
		{
			 get { return _jobQueue.Count; }
		}
		
		public PrintJob NextJob
		{
			 get { return new PrintJob(_jobQueue.NextJob); }
		}
		
		public void Initialize()
		{
			_jobQueue.Initialize();
		}
		
		public bool WaitForJob(int timeOut)
		{
			return _jobQueue.WaitForJob(timeOut);
		}
		
		public bool WaitForJobs(int jobCount, int timeOut)
		{
			return _jobQueue.WaitForJobs(jobCount, timeOut);
		}
		
		public PrintJob GetJobByIndex(int jobIndex)
		{
			return new PrintJob(_jobQueue.GetJobByIndex(jobIndex));
		}
		
		public void MergeJobs(PrintJob job1, PrintJob job2)
		{
			_jobQueue.MergeJobs(job1, job2);
		}
		
		public void MergeAllJobs()
		{
			_jobQueue.MergeAllJobs();
		}
		
		public void Clear()
		{
			_jobQueue.Clear();
		}
		
		public void DeleteJob(int index)
		{
			_jobQueue.DeleteJob(index);
		}
		
		public void ReleaseCom()
		{
			_jobQueue.ReleaseCom();
		}
		
	}
}
