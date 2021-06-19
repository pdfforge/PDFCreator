using System.ComponentModel;

namespace pdfforge.PDFCreator.Conversion.Settings
{
    public interface IProfileSetting : INotifyPropertyChanged
    {
        bool Enabled { get; set; }
    }
}
