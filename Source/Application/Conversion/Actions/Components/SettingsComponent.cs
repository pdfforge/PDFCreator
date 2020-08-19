using System;

namespace pdfforge.PDFCreator.Conversion.Actions.Components
{
    public class SettingsComponent : IActionComponent
    {
        public SettingsComponent(Type settingsType)
        {
            SettingsType = settingsType;
        }

        public Type SettingsType { get; }
    }
}
