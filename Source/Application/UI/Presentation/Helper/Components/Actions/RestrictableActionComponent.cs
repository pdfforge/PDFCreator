using pdfforge.PDFCreator.Conversion.Actions.Components;
using pdfforge.PDFCreator.Conversion.Settings.ProfileHasNotSupportedFeaturesExtension;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.Components.Actions
{
    public class RestrictableActionComponent : IActionComponent
    {
        private readonly SettingsActionComponent _settingsActionComponent;
        public ActionRestrictionEnum Restriction { get; }

        public RestrictableActionComponent(ActionRestrictionEnum restriction, SettingsActionComponent settingsActionComponent)
        {
            Restriction = restriction;
            _settingsActionComponent = settingsActionComponent;
        }

        public bool IsRestricted()
        {
            var conversionProfile = _settingsActionComponent.CurrentSetting;
            if (conversionProfile == null)
                return false;

            switch (Restriction)
            {
                case ActionRestrictionEnum.Conversion:
                    return conversionProfile.HasNotSupportedConvert();

                case ActionRestrictionEnum.Metadata:
                    return conversionProfile.HasNotSupportedMetadata();

                case ActionRestrictionEnum.Security:
                    return conversionProfile.HasNotSupportedSecure();

                case ActionRestrictionEnum.Signature:
                    return conversionProfile.HasNotSupportedSignature();

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum ActionRestrictionEnum
    {
        Conversion,
        Metadata,
        Security,
        Signature,
        None
    }
}
