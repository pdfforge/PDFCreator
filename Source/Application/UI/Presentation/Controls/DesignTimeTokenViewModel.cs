using Optional;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using System;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.UI.Presentation.Controls
{
    internal class DesignTimeTokenViewModel : TokenViewModel<string>
    {
        public DesignTimeTokenViewModel() : base(s => s, "Text", new List<string>() { "some", "strings", "test" }, s => s + "_replaced", new List<Func<string, Option<string>>>())
        {
        }
    }
}
