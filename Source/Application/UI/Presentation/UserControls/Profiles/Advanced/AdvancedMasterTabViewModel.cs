using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Advanced
{
    public class AdvancedMasterTabViewModel : TranslatableViewModelBase<ProfileAdvancedTranslation>, ITabViewModel
    {
        private string _title;

        public AdvancedMasterTabViewModel(ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
        }

        protected override void OnTranslationChanged()
        {
            base.OnTranslationChanged();
            RaisePropertyChanged(Title);
        }

        public string Title
        {
            get { return Translation.Advanced; }
            set { _title = value; }
        }

        public IconList Icon { get; set; } = IconList.AdvancedSettings;
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
