using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Settings.Workflow;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.WorkflowEditor;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    internal class DesignTimeWorkflowEditorViewModel : WorkflowEditorViewModel
    {
        public DesignTimeWorkflowEditorViewModel() : base(new DesignTimeCurrentSettingsProvider(), new DesignTimeTranslationUpdater(),
            new DesignTimeMetadataTabViewModel(), new List<IActionFacade>(), new InteractionRequest(), new DesignTimeTokenViewModelFactory(),
            new DesignTimeEventAggregator(), new DesignTimeCommandLocator())
        {
        }
    }
}
