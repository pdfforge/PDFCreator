using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.Core.Workflow
{
    public interface IJobInfoToProfileMapper 
    {
        ConversionProfile GetPreselectedProfile(JobInfo jobInfo, PdfCreatorSettings settings);
    }

    public class JobInfoToProfileMapper : IJobInfoToProfileMapper
    {
        public JobInfoToProfileMapper() 
        { }

        /// <summary>
        ///     Determines the preselected profile for the printer that was used while creating the job
        /// </summary>
        /// <param name="jobInfo">The jobinfo used for the decision</param>
        /// <param name="settings">The settings used for the decision</param>
        /// <returns>The profile that is associated with the printer or the default profile</returns>
        public ConversionProfile GetPreselectedProfile(JobInfo jobInfo, PdfCreatorSettings settings)
        {
            // Check for a printer via Parameter
            var profile = settings.GetProfileByMappedPrinter(jobInfo.PrinterParameter);

            // Check for a profile via Parameter
            if (profile == null)
                profile = settings.GetProfileByGuidOrName(jobInfo.ProfileParameter);

            // Check for a printer via Driver
            if (profile == null)
                profile = settings.GetProfileByMappedPrinter(jobInfo.PrinterName);

            // try profile from primary printer
            if (profile == null)
                profile = settings.GetProfileByMappedPrinter(settings.CreatorAppSettings.PrimaryPrinter);

            // try default profile
            if (profile == null)
                profile = settings.GetProfileByGuid(ProfileGuids.DEFAULT_PROFILE_GUID);

            // last resort: first profile from the list
            if (profile == null)
                profile = settings.ConversionProfiles[0];

            return profile;
        }
    }
}
