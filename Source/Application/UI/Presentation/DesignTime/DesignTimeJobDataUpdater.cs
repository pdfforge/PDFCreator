using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Workflow;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeJobDataUpdater : IJobDataUpdater
    {
        public void UpdateTokensAndMetadata(Job job)
        {
        }

        public Task UpdateTokensAndMetadataAsync(Job job)
        {
            return Task.CompletedTask;
        }
    }
}
