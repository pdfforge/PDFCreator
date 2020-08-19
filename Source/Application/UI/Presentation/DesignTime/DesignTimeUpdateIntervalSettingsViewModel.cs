using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General;
using pdfforge.PDFCreator.Utilities;
using Prism.Events;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeUpdateIntervalSettingsViewModel : UpdateIntervalSettingsViewModel
    {
        public DesignTimeUpdateIntervalSettingsViewModel() : base(new DesignTimeUpdateHelper(), null, new ApplicationNameProvider(""), new DesignTimeCurrentSettingsProvider(), new GpoSettingsDefaults(), new DesignTimeTranslationUpdater(), new EventAggregator(), new InteractionRequest(), new DesignTimeCurrentSettings<UpdateInterval>(), new EditionHelper(false), new DesignTimeUpdateLauncher(), null)
        {
        }
    }
}
