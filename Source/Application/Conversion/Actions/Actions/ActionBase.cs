using NLog;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using System;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions
{
    public abstract class ActionBase<TSetting> : IAction where TSetting : class, IProfileSetting
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly Func<ConversionProfile, TSetting> _settingsGetter;

        protected ActionBase(Func<ConversionProfile, TSetting> settingsGetter)
        {
            _settingsGetter = settingsGetter;
        }

        public Type SettingsType => typeof(TSetting);

        public bool IsEnabled(ConversionProfile profile)
        {
            return _settingsGetter(profile).Enabled;
        }

        public void SetIsEnabled(ConversionProfile profile, bool value)
        {
            _settingsGetter(profile).Enabled = value;
        }

        public IProfileSetting GetProfileSetting(ConversionProfile profile)
        {
            return _settingsGetter(profile);
        }

        protected abstract ActionResult DoProcessJob(Job job);

        public ActionResult ProcessJob(Job job)
        {
            _logger.Info("Launched action " + GetType().Name);

            ApplyPreSpecifiedTokens(job);

            try
            {
                return DoProcessJob(job);
            }
            finally
            {
                _logger.Info("Finished action " + GetType().Name);
            }
        }

        public abstract void ApplyPreSpecifiedTokens(Job job);

        public abstract bool IsRestricted(ConversionProfile profile);

        public void ApplyRestrictions(Job job)
        {
            if (!IsEnabled(job.Profile))
                return;

            if (IsRestricted(job.Profile))
            {
                _logger.Warn("{0} is restricted for {1} and gets disabled", GetType().Name, job.Profile.OutputFormat);
                SetIsEnabled(job.Profile, false);
            }
            else
                ApplyActionSpecificRestrictions(job);
        }

        protected abstract void ApplyActionSpecificRestrictions(Job job);

        public abstract ActionResult Check(ConversionProfile profile, CurrentCheckSettings settings, CheckLevel checkLevel);
    }
}
