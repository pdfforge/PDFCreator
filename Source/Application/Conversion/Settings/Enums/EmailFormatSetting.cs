using Translatable;

namespace pdfforge.PDFCreator.Conversion.Settings.Enums
{
    [Translatable]
    public enum EmailFormatSetting
    {
        [Translation("Auto detect (the client's default setting)")]
        Auto,
        [Translation("HTML (only for Microsoft Outlook)")]
        Html,
        [Translation("Text (only for Microsoft Outlook)")]
        Text
    }

    public static class EmailFormatExtension
    {
        public static bool IsHtml(this EmailFormatSetting formatSetting)
        {
            return formatSetting == EmailFormatSetting.Html;
        }
    }
}
