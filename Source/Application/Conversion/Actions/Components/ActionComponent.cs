using pdfforge.PDFCreator.Conversion.Actions.Components;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.Components.Actions
{
    public class ActionComponent : IActionComponent
    {
        public Type ActionType { get; }

        public ActionComponent(Type actionType)
        {
            ActionType = actionType;
        }
    }
}
