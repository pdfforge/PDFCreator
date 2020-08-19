using pdfforge.PDFCreator.Conversion.Actions.Components;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Workflow;
using pdfforge.PDFCreator.UI.Presentation.Helper.Components.Actions;
using System;
using System.Linq;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.ActionHelper
{
    public class PresenterActionFacade : ComponentBase, IPresenterActionFacade, IRestrictableAction
    {
        public string Translation => GetTranslationComponent(ActionTranslationEnum.Translation).GetTranslation();
        public string InfoText => GetTranslationComponent(ActionTranslationEnum.InfoText).GetTranslation();

        public IProfileSetting ProfileSetting
        {
            get => SettingsActionComponent.Setting;
            set => SettingsActionComponent.SetSettings(value);
        }

        public string Description => GetComponent<DescriptionActionComponent>().GetDescription();

        public bool IsEnabled
        {
            get => SettingsActionComponent != null && SettingsActionComponent.Enabled;
            set
            {
                if (SettingsActionComponent == null)
                    return;
                SettingsActionComponent.Enabled = value;
            }
        }

        private SettingsActionComponent SettingsActionComponent => GetComponent<SettingsActionComponent>();
        public Type SettingsType => SettingsActionComponent?.SettingsType;

        public bool IsRestricted
        {
            get
            {
                var restrictableActionComponent = GetComponent<RestrictableActionComponent>();
                if (restrictableActionComponent == null)
                    return false;
                return restrictableActionComponent.IsRestricted();
            }
        }

        public Type Action => GetComponent<ActionComponent>().ActionType;
        public string OverlayView => GetComponent<ViewActionComponent>().ViewName;

        private ITranslationActionComponent GetTranslationComponent(ActionTranslationEnum type)
        {
            return GetComponents<ITranslationActionComponent>().FirstOrDefault(x => x.Type == type);
        }

        public override string ToString()
        {
            // changed for Debug
            return $"Action Facade:{Action.Name}";
        }
    }

    public interface IRestrictableAction
    {
        bool IsRestricted { get; }
    }
}
