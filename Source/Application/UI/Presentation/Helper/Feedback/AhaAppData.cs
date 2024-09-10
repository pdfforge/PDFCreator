namespace pdfforge.PDFCreator.UI.Presentation.Helper.Feedback
{
    public class AhaAppData
    {
        public AhaAppData(string authKey)
        {
            AuthKey = authKey;
        }
        public string AuthKey { get; private set; }
    }
}
