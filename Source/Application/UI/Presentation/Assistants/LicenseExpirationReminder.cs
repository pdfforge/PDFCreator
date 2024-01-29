using Optional;
using pdfforge.LicenseValidator.Interface;
using pdfforge.LicenseValidator.Interface.Data;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services.Trial;
using pdfforge.PDFCreator.UI.Presentation.DesignTime;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants
{
    public class LicenseExpirationReminder : ILicenseExpirationReminder
    {
        private readonly ICurrentSettings<ApplicationSettings> _settingsProvider;
        private readonly IGpoSettings _gpoSettings;
        private readonly Option<Activation, LicenseError> _activation;
        private readonly ICampaignHelper _campaignHelper;

        public LicenseExpirationReminder(ILicenseChecker licenseChecker, ICurrentSettings<ApplicationSettings> settingsProvider, IGpoSettings gpoSettings, ICampaignHelper campaignHelper)
        {
            _activation = licenseChecker.GetSavedActivation();
            _settingsProvider = settingsProvider;
            _gpoSettings = gpoSettings;
            _campaignHelper = campaignHelper;
        }

        public void SetReminderForLicenseExpiration()
        {
            _activation.MatchSome(a =>
            {
                _settingsProvider.Settings.LicenseExpirationReminder =
                     a.LicenseExpires <= DateTime.Now.AddDays(ReminderPeriod.SecondReminderPeriod) ? DateTime.Now.AddDays(1) : a.LicenseExpires.AddDays(-ReminderPeriod.SecondReminderPeriod);
            });
        }

        private bool CheckIfLicenseIsAboutToExpire(Activation activation)
        {
            return DateTime.Now.AddDays(ReminderPeriod.FirstReminderPeriod) >= activation.LicenseExpires.Date &&
                   DateTime.Now >= _settingsProvider.Settings.LicenseExpirationReminder;
        }

        public bool IsExpirationReminderDue()
        {
            if (_gpoSettings.DisableLicenseExpirationReminder || _gpoSettings.HideLicenseTab || _campaignHelper.IsTrial)
                return false;

            return _activation.Exists(CheckIfLicenseIsAboutToExpire);
        }

        public int DaysTillLicenseExpires => CalculateDaysTillLicenseExpiration();

        public string LicenseKey => _activation.Map(a => a.GetNormalizedKey()).ValueOr("");

        private int CalculateDaysTillLicenseExpiration()
        {
            return _activation.Map(a =>
            {
                var remainingTime = a.LicenseExpires - DateTime.Now;
                return (int)remainingTime.TotalDays;
            }).ValueOr(0);
        }

        private struct ReminderPeriod
        {
            public static double FirstReminderPeriod => 30;
            public static double SecondReminderPeriod => 7;
        }
    }

    public interface ILicenseExpirationReminder
    {
        string LicenseKey { get; }
        int DaysTillLicenseExpires { get; }

        bool IsExpirationReminderDue();

        void SetReminderForLicenseExpiration();
    }

    internal class DesignTimeLicenseExpirationReminder : LicenseExpirationReminder
    {
        public DesignTimeLicenseExpirationReminder() : base(new DesignTimeLicenseChecker(), new DesignTimeCurrentSettings<ApplicationSettings>(), new GpoSettingsDefaults(), new CampaignHelper())
        {
        }
    }
}
