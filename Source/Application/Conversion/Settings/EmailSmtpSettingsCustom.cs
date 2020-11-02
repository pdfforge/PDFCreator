namespace pdfforge.PDFCreator.Conversion.Settings
{
    public partial class EmailSmtpSettings : IMailActionSettings
    {
        IMailActionSettings IMailActionSettings.Copy()
        {
            return Copy();
        }
    }
}

