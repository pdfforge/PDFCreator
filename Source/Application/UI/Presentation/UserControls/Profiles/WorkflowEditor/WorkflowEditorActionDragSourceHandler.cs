using GongSolutions.Wpf.DragDrop;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.WorkflowEditor
{
    public class WorkflowEditorActionDragSourceHandler : DefaultDragHandler
    {
        private readonly Predicate<object> _typeFilter;

        public WorkflowEditorActionDragSourceHandler(Predicate<object> typeFilter)
        {
            _typeFilter = typeFilter;
        }

        public override bool CanStartDrag(IDragInfo dragInfo)
        {
            return _typeFilter.Invoke(dragInfo.SourceItem);
        }
    }
}
