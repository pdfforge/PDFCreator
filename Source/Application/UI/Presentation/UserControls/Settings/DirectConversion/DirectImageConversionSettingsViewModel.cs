using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DirectConversion
{
    public class DirectImageConversionSettingsViewModel : TranslatableViewModelBase<DirectConversionTranslation>,  IMountable
    {
        public DirectImageConversionSettingsViewModel(ITranslationUpdater translationUpdater) 
            : base(translationUpdater)
        {
        }

        public void MountView()
        {

        }
        public void UnmountView()
        {
        }

    }
}
