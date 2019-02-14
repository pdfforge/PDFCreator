using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.ProfileHasNotSupportedFeaturesExtension;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab
{
    public class ModifyMasterTabViewModel : ProfileNotSupportedFeaturesUserControlViewModel<ProfileModifyTranslation>, ITabViewModel
    {
        private string _title;

        public ModifyMasterTabViewModel(ITranslationUpdater translationUpdater, ISelectedProfileProvider selectedProfileProvider, IDispatcher dispatcher)
            : base(translationUpdater, selectedProfileProvider, dispatcher)
        {
        }

        protected override void OnTranslationChanged()
        {
            base.OnTranslationChanged();
            RaisePropertyChanged(Title);
        }

        public string Title
        {
            get { return Translation.Modify; }
            set { _title = value; }
        }

        public IconList Icon { get; set; } = IconList.ModifySettings;
        public bool HiddenByGPO => false;
        public bool BlockedByGPO => false;
        public override bool HasNotSupportedFeatures => CurrentProfile.HasNotSupportedModify();
    }
}
