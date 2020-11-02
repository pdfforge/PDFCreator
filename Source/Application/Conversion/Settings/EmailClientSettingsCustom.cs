namespace pdfforge.PDFCreator.Conversion.Settings
{
    public partial class EmailClientSettings : IMailActionSettings
    {
        IMailActionSettings IMailActionSettings.Copy()
        {
            return Copy();
        }
    }
}

