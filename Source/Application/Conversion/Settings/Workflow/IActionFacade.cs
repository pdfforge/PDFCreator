using System;

namespace pdfforge.PDFCreator.Conversion.Settings.Workflow
{
    public interface IActionFacadeWEG
    {
        Type Action { get; }

        Type SettingsType { get; }
    }
}
