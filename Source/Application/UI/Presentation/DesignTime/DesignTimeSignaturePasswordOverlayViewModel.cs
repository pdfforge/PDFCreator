using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Signature;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeSignaturePasswordOverlayViewModel : SignaturePasswordOverlayViewModel
    {
        public DesignTimeSignaturePasswordOverlayViewModel() : base(new DesignTimeTranslationUpdater(), new DesignTimeSignaturePasswordCheck())
        {
        }
    }
}
