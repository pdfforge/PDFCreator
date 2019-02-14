using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send
{
    public class SendMasterTabViewModel : TranslatableViewModelBase<ProfileSendSubTabTranslation>, ITabViewModel
    {
        private string _title;

        public SendMasterTabViewModel(ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
        }

        protected override void OnTranslationChanged()
        {
            base.OnTranslationChanged();
            RaisePropertyChanged(Title);
        }

        public string Title
        {
            get { return Translation.Send; }
            set { _title = value; }
        }

        public IconList Icon { get; set; } = IconList.ShareSettings;
        public bool HiddenByGPO => false;
        public bool BlockedByGPO => false;
        public bool HasNotSupportedFeatures => false;
    }
}
