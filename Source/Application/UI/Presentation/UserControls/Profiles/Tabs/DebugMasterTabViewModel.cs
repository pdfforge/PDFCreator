using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs
{
    public class DebugMasterTabViewModel : TranslatableViewModelBase<ProfileModifyTranslation>, ITabViewModel
    {
        public DebugMasterTabViewModel(ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
        }

        public string Title { get; set; } = "DebugMasterTabTitle";
        public IconList Icon { get; set; } = IconList.LicenseSettings;
        public bool HiddenByGPO => false;
        public bool BlockedByGPO => false;
        public bool HasNotSupportedFeatures => false;
        public void MountView()
        {
        }

        public void UnmountView()
        {
        }
    }
}
