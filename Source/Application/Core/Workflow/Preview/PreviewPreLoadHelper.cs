using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.SettingsManagementInterface;

namespace pdfforge.PDFCreator.Core.Workflow
{
    public interface IPreviewPreLoadHelper
    {
        void PreLoadPreview(JobInfo jobInfo);
    }

    public class PreviewPreLoadHelper(IPreviewManager previewManager, IJobInfoToProfileMapper infoToProfileMapper, ISettingsProvider settingsProvider) : IPreviewPreLoadHelper
    {
        public void PreLoadPreview(JobInfo jobInfo)
        {
            var profile = infoToProfileMapper.GetPreselectedProfile(jobInfo, settingsProvider.Settings);
            //Generate Preview in background threads
            if (profile.Preview.Enabled
                && !profile.AutoSave.Enabled 
                && !profile.UserTokens.Enabled // For user tokens the preview needs to be re-created after the user tokens are removed.
                && !profile.SkipPrintDialog)
            {
                previewManager.LaunchPreviewTasks(jobInfo);
            }
        }
    }

    public class DisabledPreviewPreloadHelper() : IPreviewPreLoadHelper
    {
        public void PreLoadPreview(JobInfo jobInfo)
        {
            // Do nothing
        }
    }
}
