using pdfforge.PDFCreator.Conversion.Actions.Components;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Workflow;
using pdfforge.PDFCreator.UI.Presentation.Helper.ActionHelper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.Components.Actions
{
    public class PresentationActionFacadeBuilder : IActionFacadeBuilder
    {
        private readonly List<IActionComponent> _components = new List<IActionComponent>();

        public ViewActionComponent AddView(string viewName)
        {
            var viewComponent = new ViewActionComponent(viewName);
            _components.Add(viewComponent);
            return viewComponent;
        }

        public DescriptionActionComponent AddDescription(IActionFacadeDescriptionHelper actionFacadeDescriptionHelper, SettingsActionComponent settingsActionComponent)
        {
            var descriptionComponent = new DescriptionActionComponent(actionFacadeDescriptionHelper, settingsActionComponent);
            _components.Add(descriptionComponent);
            return descriptionComponent;
        }

        public ActionComponent AddAction(Type actionType)
        {
            var action = new ActionComponent(actionType);
            _components.Add(action);
            return action;
        }

        public SettingsActionComponent AddSettings<TSettings>(ICurrentSettingsProvider currentSettingsProvider, Type settingType, Expression<Func<ConversionProfile, TSettings>> getSettingFunction) where TSettings : IProfileSetting
        {
            var settings = new SettingsActionComponent(currentSettingsProvider, settingType);
            settings.Init<TSettings>(getSettingFunction);
            _components.Add(settings);
            return settings;
        }

        public RestrictableActionComponent AddRestrictable(ActionRestrictionEnum restriction, SettingsActionComponent settingsActionComponent)
        {
            var restrictableActionComponent = new RestrictableActionComponent(restriction, settingsActionComponent);
            _components.Add(restrictableActionComponent);
            return restrictableActionComponent;
        }

        public TranslationActionComponent<TTranslation> AddTranslation<TTranslation>(ITranslationUpdater translationUpdater, ActionTranslationEnum type, Func<TTranslation, string> translationFunction)
        where TTranslation : ITranslatable, new()
        {
            var translationActionComponent = new TranslationActionComponent<TTranslation>(translationUpdater, type, translationFunction);
            _components.Add(translationActionComponent);
            return translationActionComponent;
        }

        public IActionFacade Build()
        {
            var componentBase = new PresenterActionFacade();
            _components.ForEach(x => componentBase.AddComponent(x));
            return componentBase;
        }
    }
}
