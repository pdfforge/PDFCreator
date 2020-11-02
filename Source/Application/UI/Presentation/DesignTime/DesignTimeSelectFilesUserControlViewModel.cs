using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SelectFiles;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    internal class DesignTimeSelectFilesUserControlViewModel : SelectFilesUserControlViewModel
    {
        public DesignTimeSelectFilesUserControlViewModel() : base(new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider(), null, null,
            () => "",
            profile => new List<string> { "test1", "test2", "test3" }, new List<string> { "testToken1", "testToken2", "testToken3" }, "")
        {
        }
    }
}
