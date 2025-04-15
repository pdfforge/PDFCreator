using NLog;
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
        private readonly IVersionHelper _versionHelper;
        private readonly ApplicationNameProvider _applicationNameProvider;
        private readonly IJobInfoToProfileMapper _jobInfoToProfileMapper;

        public JobBuilder(IVersionHelper versionHelper, ApplicationNameProvider applicationNameProvider, IJobInfoToProfileMapper jobInfoToProfileMapper)
        {
            _versionHelper = versionHelper;
            _applicationNameProvider = applicationNameProvider;
            _jobInfoToProfileMapper = jobInfoToProfileMapper;
        }

        public abstract Job SkipPrintDialog(Job job);

        public Job BuildJobFromJobInfo(JobInfo jobInfo, PdfCreatorSettings settings)
        {
            _logger.Trace("Building Job from JobInfo");

            var preselectedProfile = _jobInfoToProfileMapper.GetPreselectedProfile(jobInfo, settings).Copy();

            _logger.Debug("Profile: {0} (GUID {1})", preselectedProfile.Name, preselectedProfile.Guid);
            var producer = _applicationNameProvider.ApplicationNameWithEdition + " " + _versionHelper.FormatWithThreeDigits();

            var currentSettings = new CurrentJobSettings(settings.ConversionProfiles, settings.ApplicationSettings.PrinterMappings, settings.ApplicationSettings.Accounts);

            var job = new Job(jobInfo, preselectedProfile, currentSettings, producer);

            SkipPrintDialog(job);

            return job;
        }
    }

    public class JobBuilderFree : JobBuilder
    {
        public JobBuilderFree(IVersionHelper versionHelper, ApplicationNameProvider applicationNameProvider, IJobInfoToProfileMapper jobInfoToProfileMapper)
            : base(versionHelper, applicationNameProvider, jobInfoToProfileMapper)
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
        public JobBuilderProfessional(IVersionHelper versionHelper, ApplicationNameProvider applicationNameProvider, IJobInfoToProfileMapper jobInfoToProfileMapper)
            : base(versionHelper, applicationNameProvider, jobInfoToProfileMapper)
        {
        }

        public override Job SkipPrintDialog(Job job)
        {
            return job;
        }
    }

    public class JobBuilderServer : JobBuilder
    {
        public JobBuilderServer(IVersionHelper versionHelper, ApplicationNameProvider applicationNameProvider, IJobInfoToProfileMapper jobInfoToProfileMapper)
            : base(versionHelper, applicationNameProvider, jobInfoToProfileMapper)
        {
        }

        public override Job SkipPrintDialog(Job job)
        {
            job.Profile.SkipPrintDialog = true;
            return job;
        }
    }
}
