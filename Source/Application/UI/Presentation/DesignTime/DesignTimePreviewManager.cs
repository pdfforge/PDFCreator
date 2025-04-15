using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Core.Workflow;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    internal class DesignTimePreviewManager : IPreviewManager
    {
        public List<Task<PreviewPages>> LaunchPreviewTasks(JobInfo jobInfo)
        {
            return new List<Task<PreviewPages>>();
        }

        IList<Task<PreviewPages>> IPreviewManager.LaunchPreviewTasks(JobInfo jobInfo)
        {
            return LaunchPreviewTasks(jobInfo);
        }

        public Task<IList<PreviewPage>> GetTotalPreviewPages(JobInfo jobInfo)
        {
            return Task.FromResult((IList<PreviewPage>)new List<PreviewPage>());
        }

        public void AbortAndCleanUpPreview(IList<SourceFileInfo> sourceFileInfos)
        {
        }

        public void AbortAndCleanUpPreview(string sfiFilename)
        {
        }
    }
}
