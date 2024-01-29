using Optional;
using pdfforge.LicenseValidator.Interface.Data;
using pdfforge.PDFCreator.Utilities;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UrlHelper = pdfforge.Banners.UrlHelper;

namespace pdfforge.PDFCreator.Core.Services.Trial
{
    public class CampaignHelper : ICampaignHelper
    {
        private string _extendLicenseUrl;

        public string GetTrialExtendLink(string fallbackUrl)
        {
            var licenseExtendUrl = fallbackUrl;
            if (!string.IsNullOrEmpty(ExtendLicenseUrl))
                licenseExtendUrl = ExtendLicenseUrl;
            if (!string.IsNullOrEmpty(LicenseKey))
                licenseExtendUrl = UrlHelper.AddUrlParameters(licenseExtendUrl, "license_key", LicenseKey, true);
            return licenseExtendUrl;
        }

        public bool IsTrial { get; set; }
        public string CurrentCampaign { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string LicenseKey { get; set; }
        public int TrialRemainingDays { get; set; }

        public string ExtendLicenseUrl
        {
            get => _extendLicenseUrl;
            set
            {
                if (value == _extendLicenseUrl) return;
                _extendLicenseUrl = value;
                OnPropertyChanged();
            }
        }

        public void InitCampaign(Option<Activation, LicenseError> licenseActivation)
        {
            IsTrial = licenseActivation.Exists(a => a.IsTrial.Equals(true));
            CurrentCampaign = licenseActivation.Match(
                some: a => a.Campaign.ValueOr(""),
                none: e => "");
            LicenseKey = licenseActivation.Match(
                some: a => a.Key,
                none: e => "");
            ExpirationDate = licenseActivation.Map(a => a.LicenseExpires).ValueOr(DateTime.MinValue);
            TrialRemainingDays = (int)(ExpirationDate - DateTime.Now).TotalDays;
            if (TrialRemainingDays < 0)
                TrialRemainingDays = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}