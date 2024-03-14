using System.Collections.ObjectModel;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.WorkflowEditor;
using Prism.Events;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeProfilesViewModel : ProfilesViewModel
    {
        public DesignTimeProfilesViewModel()
            : base(new CurrentSettingsProvider(new DefaultSettingsProvider()),
                new DesignTimeTranslationUpdater(),
                new DesignTimeCommandLocator(),
                new DesignTimeCurrentSettings<ObservableCollection<ConversionProfile>>(),
                new GpoSettingsDefaults(),
                new Prism.Regions.RegionManager(),
                new WorkflowEditorSubViewProvider("save", "metadata", "outputformat"),
                new DesignTimeCommandBuilderProvider(),
                new DesignTimeDispatcher(), 
                new EventAggregator())
        {
        }
    }
}
