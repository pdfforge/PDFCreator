using System;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings
{
    public abstract class ADebugSettingsItemControlModel : TranslatableViewModelBase<DebugSettingsTranslation>
    {

        protected ADebugSettingsItemControlModel(ITranslationUpdater translationUpdater, IGpoSettings gpoSettings) : base(translationUpdater)
        {
            GpoSettings = gpoSettings;
        }

        public IGpoSettings GpoSettings { get; private set; }
    }
    
}