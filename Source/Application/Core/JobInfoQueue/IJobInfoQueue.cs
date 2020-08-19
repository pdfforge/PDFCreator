using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using System;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Core.JobInfoQueue
{
    public interface IJobInfoQueue
    {
        /// <summary>
        ///     The List of jobs currently waiting
        /// </summary>
        IList<JobInfo> JobInfos { get; }

        /// <summary>
        ///     Get the number ob items in the JobInfo Queue
        /// </summary>
        int Count { get; }

        /// <summary>
        ///     Get the next pending job. If this is null, the queue is empty
        /// </summary>
        JobInfo NextJob { get; }

        /// <summary>
        ///     Determines if the Queue is emtpy
        /// </summary>
        /// <returns>true, if the Queue is empty</returns>
        bool IsEmpty { get; }

        event EventHandler<NewJobInfoEventArgs> OnNewJobInfo;

        /// <summary>
        ///     Appends an item to the end of the JobInfo Queue
        /// </summary>
        /// <param name="jobInfo">The JobInfo to add</param>
        void Add(JobInfo jobInfo);

        /// <summary>
        ///     Adds an item to the start of the JobInfo Queue
        /// </summary>
        /// <param name="jobInfo">The JobInfo to add</param>
        void AddFirst(JobInfo jobInfo);

        /// <summary>
        ///     Appends several items to the end of the JobInfo Queue
        /// </summary>
        /// <param name="jobInfos">The JobInfos to add</param>
        void Add(IEnumerable<JobInfo> jobInfos);

        /// <summary>
        ///     Removes a JobInfo from the Queue
        /// </summary>
        /// <param name="jobInfo">The JobInfo to remove</param>
        /// <returns>true, if successful</returns>
        bool Remove(JobInfo jobInfo);

        /// <summary>
        ///     Removes a JobInfo from the Queue
        /// </summary>
        /// <param name="jobInfo">The JobInfo to remove</param>
        /// <param name="deleteFiles">If true, the inf and source files will be deleted</param>
        /// <returns>true, if successful</returns>
        bool Remove(JobInfo jobInfo, bool deleteFiles);

        void Clear();
    }
}
