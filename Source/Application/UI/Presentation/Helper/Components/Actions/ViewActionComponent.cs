using pdfforge.PDFCreator.Conversion.Actions.Components;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.Components.Actions
{
    public class ViewActionComponent : IActionComponent
    {
        public string ViewName { get; private set; }

        public ViewActionComponent(string viewName)
        {
            ViewName = viewName;
        }
    }
}
