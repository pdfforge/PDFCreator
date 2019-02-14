using System;

namespace pdfforge.PDFCreator.UI.Presentation
{
    public interface ICurrentSettings<TSetting>
    {
        TSetting Settings { get; set; }

        event EventHandler SettingsChanged;
    }
}
