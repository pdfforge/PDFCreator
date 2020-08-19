using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.Core.Workflow
{
    public interface IJobBuilder
    {
        Job BuildJobFromJobInfo(JobInfo jobInfo, PdfCreatorSettings settings);
    }

    public abstract class JobBuilder : IJobBuilder
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IMailSignatureHelper _mailSignatureHelper;
        private readonly IVersionHelper _versionHelper;
        private readonly ApplicationNameProvider _applicationNameProvider;

        public JobBuilder(IMailSignatureHelper mailSignatureHelper, IVersionHelper versionHelper, ApplicationNameProvider applicationNameProvider)
        {
            _mailSignatureHelper = mailSignatureHelper;
            _versionHelper = versionHelper;
            _applicationNameProvider = applicationNameProvider;
        }

        public abstract Job SkipPrintDialog(Job job);

        public Job BuildJobFromJobInfo(JobInfo jobInfo, PdfCreatorSettings settings)
        {
            _logger.Trace("Building Job from JobInfo");

            var preselectedProfile = PreselectedProfile(jobInfo, settings).Copy();

            _logger.Debug("Profile: {0} (GUID {1})", preselectedProfile.Name, preselectedProfile.Guid);
            var producer = _applicationNameProvider.ApplicationNameWithEdition + " " + _versionHelper.FormatWithThreeDigits();
            var job = new Job(jobInfo, preselectedProfile, settings.ApplicationSettings.Accounts, producer);

            SkipPrintDialog(job);

            return job;
        }

        /// <summary>
        ///     Determines the preselected profile for the printer that was used while creating the job
        /// </summary>
        /// <param name="jobInfo">The jobinfo used for the decision</param>
        /// <param name="settings">The settings used for the decision</param>
        /// <returns>The profile that is associated with the printer or the default profile</returns>
        private ConversionProfile PreselectedProfile(JobInfo jobInfo, PdfCreatorSettings settings)
        {
            // Check for a printer via Parameter
            var profile = settings.GetProfileByMappedPrinter(jobInfo.PrinterParameter);

            // Check for a profile via Parameter
            if (profile == null)
                profile = settings.GetProfileByNameOrGuid(jobInfo.ProfileParameter);

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

    public class JobBuilderFree : JobBuilder
    {
        public JobBuilderFree(IMailSignatureHelper mailSignatureHelper, IVersionHelper versionHelper, ApplicationNameProvider applicationNameProvider)
            : base(mailSignatureHelper, versionHelper, applicationNameProvider)
        {
        }

        public override Job SkipPrintDialog(Job job)
        {
            job.Profile.SkipPrintDialog = false;
            return job;
        }
    }

    public class JobBuilderProfessional : JobBuilder
    {
        public JobBuilderProfessional(IMailSignatureHelper mailSignatureHelper, IVersionHelper versionHelper, ApplicationNameProvider applicationNameProvider)
            : base(mailSignatureHelper, versionHelper, applicationNameProvider)
        {
        }

        public override Job SkipPrintDialog(Job job)
        {
            return job;
        }
    }

    public class JobBuilderServer : JobBuilder
    {
        public JobBuilderServer(IMailSignatureHelper mailSignatureHelper, IVersionHelper versionHelper, ApplicationNameProvider applicationNameProvider)
            : base(mailSignatureHelper, versionHelper, applicationNameProvider)
        {
        }

        public override Job SkipPrintDialog(Job job)
        {
            job.Profile.SkipPrintDialog = true;
            return job;
        }
    }
}
