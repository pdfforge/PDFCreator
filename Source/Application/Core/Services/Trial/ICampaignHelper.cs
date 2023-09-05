using Optional;
using pdfforge.LicenseValidator.Interface.Data;
using System.ComponentModel;

namespace pdfforge.PDFCreator.Core.Services.Trial
{
    public interface ICampaignHelper : INotifyPropertyChanged
    {
        void InitCampaign(Option<Activation, LicenseError> licenseActivation);

        string GetTrialExtendLink(string fallbackUrl);

        bool IsTrial { get; set; }
        string CurrentCampaign { get; set; }
        string LicenseKey { get; set; }
        int TrialRemainingDays { get; set; }
        string ExtendLicenseUrl { get; set; }
    }
}
