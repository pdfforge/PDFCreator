using System;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General
{
    public class GeneralSettingsViewModel : TranslatableViewModelBase<GeneralSettingsTranslation>, ITabViewModel
    {
        public string Title => Translation.GeneralTabTitle;
        public IconList Icon => IconList.GeneralSettings;
        public bool HiddenByGPO => false;
        public bool BlockedByGPO => false;
        public bool HasNotSupportedFeatures => false;

        public GeneralSettingsViewModel(ITranslationUpdater translationUpdater)
            : base(translationUpdater)
        {
            translationUpdater.RegisterAndSetTranslation(tf => RaisePropertyChanged(nameof(Title)));
        }
    }
}
