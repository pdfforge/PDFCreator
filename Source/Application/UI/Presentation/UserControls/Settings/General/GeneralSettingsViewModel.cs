using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General
{
    public class GeneralSettingsViewModel : TranslatableViewModelBase<GeneralSettingsTranslation>, IMountable
    {
        public GeneralSettingsViewModel(ITranslationUpdater translationUpdater)
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
