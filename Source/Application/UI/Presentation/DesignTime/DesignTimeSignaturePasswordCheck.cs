using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeSignaturePasswordCheck : ISignaturePasswordCheck
    {
        public bool IsValidPassword(string certificateFile, string certificatePassword)
        {
            return true;
        }
    }
}
