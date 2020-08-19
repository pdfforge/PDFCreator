using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.ViewModelBases
{
    public abstract class TranslatableAsyncCommandBase<TTranslatable> : AsyncCommandBase where TTranslatable : ITranslatable, new()
    {
        protected TTranslatable Translation;

        public TranslatableAsyncCommandBase(ITranslationUpdater translationUpdater)
        {
            translationUpdater.RegisterAndSetTranslation(SetTranslationAction);
        }

        private void SetTranslationAction(ITranslationFactory tf)
        {
            Translation = tf.UpdateOrCreateTranslation(Translation);
        }
    }
}
