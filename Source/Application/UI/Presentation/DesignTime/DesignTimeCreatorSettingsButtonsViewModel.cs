using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    internal class DesignTimeCreatorSettingsButtonsViewModel : CreatorSettingsButtonsViewModel
    {
        public DesignTimeCreatorSettingsButtonsViewModel() :
            base(new GpoSettingsDefaults(),
                 new DesignTimeTranslationUpdater(),
                 new DesignTimeEventAggregator(),
                 new DesignTimeCommandLocator(),
                 new DesignTimeEditionHelper(),
                 new DesignTimeDispatcher(),
                 null)
        { }
    }
}
