using NLog;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.JobInfoQueue;
using pdfforge.PDFCreator.Core.SettingsManagement;
using System;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Conversion.Actions
{
    public class ForwardToFurtherProfileAction : IPreConversionAction, ICheckable, IBusinessFeatureAction
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ISettingsProvider _settingsProvider;
        private readonly IJobInfoDuplicator _jobInfoDuplicator;
        private readonly IJobInfoQueue _jobInfoQueue;

        public ForwardToFurtherProfileAction(ISettingsProvider settingsProvider, IJobInfoDuplicator jobInfoDuplicator,
            IJobInfoQueue jobInfoQueue)
        {
            _settingsProvider = settingsProvider;
            _jobInfoDuplicator = jobInfoDuplicator;
            _jobInfoQueue = jobInfoQueue;
        }

        public ActionResult ProcessJob(Job job)
        {
            if (!job.Profile.ForwardToFurtherProfile.Enabled)
                return new ActionResult();

            try
            {
                _logger.Trace("Launched " + nameof(ForwardToFurtherProfileAction));
                var forwardProfileGuid = job.Profile.ForwardToFurtherProfile.ProfileGuid;
                _logger.Trace("Duplicate JobInfo");
                var forwardJobInfo = _jobInfoDuplicator.Duplicate(job.JobInfo, forwardProfileGuid);
                _logger.Debug("Forward duplicate JobInfo to profile with ID: " + forwardProfileGuid);
                _jobInfoQueue.AddFirst(forwardJobInfo);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception during forward to further porfile action: ");
                return new ActionResult(ErrorCode.ForwardToFurtherProfile_GeneralError);
            }

            return new ActionResult();
        }

        public bool IsEnabled(ConversionProfile profile)
        {
            return profile.ForwardToFurtherProfile.Enabled;
        }

        public void ApplyPreSpecifiedTokens(Job job)
        {
            // nothing to do here
        }

        public ActionResult Check(ConversionProfile profile, Accounts accounts, CheckLevel checkLevel)
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

            var forwardProfile = _settingsProvider.Settings.GetProfileByGuid(forwardGuid);
            if (forwardProfile == null)
            {
                _logger.Error($"The forward to further profile action forwards to an unknown profile (Guid: {forwardGuid}.");
                return new ActionResult(ErrorCode.ForwardToFurtherProfile_UnknownProfile);
            }

            if (HasCircularDependency(profile))
                return new ActionResult(ErrorCode.ForwardToFurtherProfile_CircularDependency);

            return new ActionResult();
        }

        private bool HasCircularDependency(ConversionProfile profile, IList<string> referencedProfiles = null)
        {
            if (referencedProfiles == null)
                referencedProfiles = new List<string>();

            if (referencedProfiles.Contains(profile.Guid))
            {
                _logger.Error("The forward to further profile action forwards causes a circular dependency " +
                              "between profiles with the Guids:\r\n" +
                              string.Join(" -> ", referencedProfiles));
                return true;
            }

            if (!profile.ForwardToFurtherProfile.Enabled)
                return false;

            referencedProfiles.Add(profile.Guid);

            var forwardProfile = _settingsProvider.Settings.GetProfileByGuid(profile.ForwardToFurtherProfile.ProfileGuid);
            if (forwardProfile == null)
            {
                // There is no circular dependency. The missing profile will be displayed in the check for the forwarded profile.
                return false;
            }

            return HasCircularDependency(forwardProfile, referencedProfiles);
        }
    }
}
