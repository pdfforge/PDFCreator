using NLog;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Helper;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using System;

namespace pdfforge.PDFCreator.Conversion.Actions
{
    public abstract class ForwardToFurtherProfileActionBase : ActionBase<ForwardToFurtherProfile>, IPreConversionAction, IBusinessFeatureAction
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected abstract IJobInfoDuplicator JobInfoDuplicator { get; }

        protected ForwardToFurtherProfileActionBase() : base(p => p.ForwardToFurtherProfile)
        {
        }

        protected abstract void Enqueue(JobInfo jobInfo);

        protected override ActionResult DoProcessJob(Job job)
        {
            if (!job.Profile.ForwardToFurtherProfile.Enabled)
                return new ActionResult();

            try
            {
                _logger.Trace("Launched ForwardToFurtherProfileAction");
                var forwardProfileGuid = job.Profile.ForwardToFurtherProfile.ProfileGuid;
                _logger.Trace("Duplicate JobInfo");
                var forwardJobInfo = JobInfoDuplicator.Duplicate(job.JobInfo, forwardProfileGuid);
                _logger.Debug("Forward duplicate JobInfo to profile with ID: " + forwardProfileGuid);

                Enqueue(forwardJobInfo);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception during forward to further porfile action: ");
                return new ActionResult(ErrorCode.ForwardToFurtherProfile_GeneralError);
            }

            return new ActionResult();
        }

        public override void ApplyPreSpecifiedTokens(Job job)
        {
            // nothing to do here
        }

        public override bool IsRestricted(ConversionProfile profile)
        {
            return false;
        }

        protected override void ApplyActionSpecificRestrictions(Job job)
        { }

        public override ActionResult Check(ConversionProfile profile, CurrentCheckSettings settings, CheckLevel checkLevel)
        {
            if (!profile.ForwardToFurtherProfile.Enabled)
                return new ActionResult();

            _logger.Debug("Check ForwardToFurtherProfileAction");

            var forwardGuid = profile.ForwardToFurtherProfile.ProfileGuid;
            if (forwardGuid.Equals(profile.Guid))
            {
                _logger.Error("The forward to further profile action forwards to itself.");
                return new ActionResult(ErrorCode.ForwardToFurtherProfile_ForwardToItself);
            }

            var forwardProfile = SettingsHelper.GetProfileByGuid(settings.Profiles, forwardGuid);
            if (forwardProfile == null)
            {
                _logger.Error($"The forward to further profile action forwards to an unknown profile (Guid: {forwardGuid}.");
                return new ActionResult(ErrorCode.ForwardToFurtherProfile_UnknownProfile);
            }

            var (hasCircularDependency, dependencyRoute) = CyclicDependenciesHelper.HasCyclicDependency(profile, settings.PrinterMappings, settings.Profiles);

            if (hasCircularDependency)
            {
                _logger.Error("The forward to further profile action forwards causes a circular dependency " +
                              "between profiles with the Guids:\r\n" +
                              string.Join(" -> ", dependencyRoute));

                return new ActionResult(ErrorCode.ForwardToFurtherProfile_CircularDependency);
            }

            return new ActionResult();
        }
    }
}
