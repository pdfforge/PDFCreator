using pdfforge.PDFCreator.Conversion.Actions;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeActionManager : ActionManager
    {
        public DesignTimeActionManager() : base(new List<IAction>())
        { }
    }
}
