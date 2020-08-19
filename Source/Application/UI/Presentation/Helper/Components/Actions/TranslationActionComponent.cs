using pdfforge.PDFCreator.Conversion.Actions.Components;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.Components.Actions
{
    public interface ITranslationActionComponent : IActionComponent
    {
        string GetTranslation();

        ActionTranslationEnum Type { get; }
    }

    public class TranslationActionComponent<TTranslatable> : ITranslationActionComponent where TTranslatable : ITranslatable, new()
    {
        public ActionTranslationEnum Type { get; }
        private readonly Func<TTranslatable, string> _getTranslationFunction;

        public TTranslatable Translatable;

        public TranslationActionComponent(ITranslationUpdater translationUpdater, ActionTranslationEnum type, Func<TTranslatable, string> translationFunction)
        {
            _getTranslationFunction = translationFunction;
            Translatable = new TTranslatable();
            Type = type;
            translationUpdater.RegisterAndSetTranslation(x => Translatable = x.UpdateOrCreateTranslation(Translatable));
        }

        public string GetTranslation()
        {
            return _getTranslationFunction.Invoke(Translatable);
        }
    }

    public enum ActionTranslationEnum
    {
        Translation,
        InfoText
    }
}
