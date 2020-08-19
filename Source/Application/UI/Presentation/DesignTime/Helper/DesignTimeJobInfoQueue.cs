using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Core.JobInfoQueue;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper
{
    public class DesignTimeJobInfoQueue : IJobInfoQueue
    {
        public DesignTimeJobInfoQueue()
        {
            JobInfos.Add(new JobInfo
            {
                PrintDateTime = DateTime.Now,
                Metadata = new Metadata { PrintJobName = "Print Job 1" },
                SourceFiles = new ObservableCollection<SourceFileInfo>
                {
                    new SourceFileInfo()
                    {
                        DocumentTitle = "Print Job 1",
                        TotalPages = 4,
                        JobCounter = 1
                    }
                }
            });
            JobInfos.Add(new JobInfo
            {
                PrintDateTime = DateTime.Now,
                Metadata = new Metadata { PrintJobName = "Print Job 2" },
                SourceFiles = new ObservableCollection<SourceFileInfo>
                {
                    new SourceFileInfo()
                    {
                        DocumentTitle = "Print Job 2",
                        TotalPages = 2,
                        JobCounter = 2
                    }
                }
            });
        }

        public IList<JobInfo> JobInfos { get; } = new List<JobInfo>();
        public int Count { get; } = 42;
        public JobInfo NextJob { get; }
        public bool IsEmpty { get; } = true;

        public bool Remove(JobInfo jobInfo, bool deleteFiles)
        {
            return true;
        }

        public void Clear()
        {
        }

        public void Add(IEnumerable<JobInfo> jobInfos)
        {
        }

        public bool Remove(JobInfo jobInfo)
        {
            return true;
        }

        public void Add(JobInfo jobInfo)
        {
        }

        public void AddFirst(JobInfo jobInfo)
        {
        }

#pragma warning disable CS0067

        public event EventHandler<NewJobInfoEventArgs> OnNewJobInfo;

#pragma warning restore CS0067
    }
}
