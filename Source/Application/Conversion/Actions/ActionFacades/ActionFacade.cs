using pdfforge.PDFCreator.Conversion.Actions.Components;
using pdfforge.PDFCreator.Conversion.Settings.Workflow;
using pdfforge.PDFCreator.UI.Presentation.Helper.Components.Actions;
using System;

namespace pdfforge.PDFCreator.Conversion.Actions.ActionFacades
{
    public class ActionFacade : ComponentBase, IActionFacade
    {
        private SettingsComponent SettingsActionComponent => GetComponent<SettingsComponent>();
        public Type SettingsType => SettingsActionComponent?.SettingsType;
        public Type Action => GetComponent<ActionComponent>().ActionType;
    }
}
