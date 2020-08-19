using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.Conversion.Settings
{
    public interface IWatermarkSettings
    {
        bool Enabled { get; set; }
        string File { get; set; }
        int Opacity { get; set; }
        BackgroundRepetition Repetition { get; set; }
        bool FitToPage { get; set; }
    }
}
