using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.ProfileHasNotSupportedFeaturesExtension;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SecureTab
{
    public class SecureMasterTabViewModel : ProfileNotSupportedFeaturesUserControlViewModel<ProfileSecureTranslation>, ITabViewModel
    {
        public SecureMasterTabViewModel(ITranslationUpdater translationUpdater, ISelectedProfileProvider selectedProfileProvider, IDispatcher dispatcher)
            : base(translationUpdater, selectedProfileProvider, dispatcher)
        {
        }

        protected override void OnTranslationChanged()
        {
            base.OnTranslationChanged();
            RaisePropertyChanged(Title);
        }

        public string Title => Translation.Secure;

        public IconList Icon { get; set; } = IconList.SecureSettings;
        public bool HiddenByGPO => false;
        public bool BlockedByGPO => false;
        public override bool HasNotSupportedFeatures => CurrentProfile.HasNotSupportedSecure();
    }
}
