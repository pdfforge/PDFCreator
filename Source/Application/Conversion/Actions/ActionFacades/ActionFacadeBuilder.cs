using pdfforge.PDFCreator.Conversion.Actions.Components;
using pdfforge.PDFCreator.Conversion.Settings.Workflow;
using pdfforge.PDFCreator.UI.Presentation.Helper.Components.Actions;
using System;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Conversion.Actions.ActionFacades
{
    public class ActionFacadeBuilder : IActionFacadeBuilder
    {
        private readonly List<IActionComponent> _components = new List<IActionComponent>();

        public SettingsComponent AddSetting(Type type)
        {
            var setting = new SettingsComponent(type);
            _components.Add(setting);
            return setting;
        }

        public ActionComponent AddAction(Type type)
        {
            var action = new ActionComponent(type);
            _components.Add(action);
            return action;
        }

        public IActionFacade Build()
        {
            var actionFacade = new ActionFacade();
            _components.ForEach(x => actionFacade.AddComponent(x));
            return actionFacade;
        }
    }
}
