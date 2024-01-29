namespace pdfforge.PDFCreator.Conversion.Settings
{
    public partial class EmailWebSettings : IMailActionSettings
    {
        IMailActionSettings IMailActionSettings.Copy()
        {
            return Copy();
        }
    }
}

