using System;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeCurrentSettings<TSettings> : ICurrentSettings<TSettings> where TSettings : new()
    {
        public TSettings Settings { get; set; }

#pragma warning disable 67

        public event EventHandler SettingsChanged;

#pragma warning restore 67

        public DesignTimeCurrentSettings()
        {
            Settings = new TSettings();
        }
    }
}
