using pdfforge.PDFCreator.Conversion.Actions.Components;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.Components.Actions
{
    public class DescriptionActionComponent : IActionComponent
    {
        private readonly IActionFacadeDescriptionHelper _actionFacadeDescriptionHelper;
        private readonly SettingsActionComponent _settingsActionComponent;

        public DescriptionActionComponent(IActionFacadeDescriptionHelper actionFacadeDescriptionHelper, SettingsActionComponent settingsActionComponent)
        {
            _actionFacadeDescriptionHelper = actionFacadeDescriptionHelper;
            _settingsActionComponent = settingsActionComponent;
        }

        public string GetDescription()
        {
            return _actionFacadeDescriptionHelper.GetDescription(_settingsActionComponent.Setting);
        }
    }
}
