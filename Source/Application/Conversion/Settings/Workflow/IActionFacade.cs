using System;

namespace pdfforge.PDFCreator.Conversion.Settings.Workflow
{
    public interface IActionFacade
    {
        Type Action { get; }

        Type SettingsType { get; }
    }


    public interface IActionFacade<TSettings> : IActionFacade
    {
        TSettings Settings { get; }
    }

    public interface IPresenterActionFacade : IActionFacade
    {
        string Translation { get; }
        string OverlayView { get; }
        string Description { get; }
        bool IsEnabled { get; set; }

        string InfoText { get; }
        IProfileSetting ProfileSetting { get; set; }
        IProfileSetting GetProfileSettingByConversionProfile(ConversionProfile profile);
    }
}
