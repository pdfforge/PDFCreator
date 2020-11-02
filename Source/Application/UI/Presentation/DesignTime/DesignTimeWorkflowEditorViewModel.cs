using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Settings.Workflow;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.WorkflowEditor;
using pdfforge.PDFCreator.UI.Presentation.Wrapper;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    internal class DesignTimeWorkflowEditorViewModel : WorkflowEditorViewModel
    {
        public DesignTimeWorkflowEditorViewModel()
            : base(
                new DesignTimeCurrentSettingsProvider(),
                new DesignTimeTranslationUpdater(),
                new List<IActionFacade>(),
                new InteractionRequest(),
                new DesignTimeEventAggregator(),
                new DesignTimeCommandLocator(),
                new WorkflowEditorSubViewProvider("save", "metadata", "outputformat"),
                new DesignTimeCommandBuilderProvider(),
                new DispatcherWrapper())
        { }
    }
}
