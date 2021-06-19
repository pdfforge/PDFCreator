using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.WorkflowEditor;
using pdfforge.PDFCreator.UI.Presentation.Wrapper;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeSaveOutputFormatMetadataViewModel : SaveOutputFormatMetadataViewModel
    {
        public DesignTimeSaveOutputFormatMetadataViewModel()
            : base(new DesignTimeCurrentSettingsProvider(),
                    new DesignTimeTranslationUpdater(),
                    new DesignTimeEventAggregator(),
                    new DesignTimeCommandLocator(),
                    new WorkflowEditorSubViewProvider("save", "metadata", "outputformat"),
                    new DesignTimeCommandBuilderProvider(),
                    new DispatcherWrapper(),
                    new DesignTimeEditionHelper(),
                    new DesignTimeActionManager())
        { }
    }
}
