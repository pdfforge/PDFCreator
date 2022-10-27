using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.Font
{
    public interface IFontSelectorControlViewModelFactory
    {
        IFontSelectorControlViewModelBuilder BuilderWithSelectedProfile();
    }

    public class FontSelectorControlControlViewModelFactory : IFontSelectorControlViewModelFactory
    {
        private readonly IFontHelper _fontHelper;
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly IDispatcher _dispatcher;
        private readonly ISelectedProfileProvider _selectedProfileProvider;
        private readonly ITranslationUpdater _translationUpdater;

        public FontSelectorControlControlViewModelFactory(ISelectedProfileProvider selectedProfileProvider, IInteractionInvoker interactionInvoker, IFontHelper fontHelper, IDispatcher dispatcher, ITranslationUpdater translationUpdater)
        {
            _selectedProfileProvider = selectedProfileProvider;
            _interactionInvoker = interactionInvoker;
            _fontHelper = fontHelper;
            _dispatcher = dispatcher;
            _translationUpdater = translationUpdater;
        }

        public IFontSelectorControlViewModelBuilder BuilderWithSelectedProfile()
        {
            return new FontSelectorControlViewModelBuilder(_translationUpdater, _selectedProfileProvider, _interactionInvoker, _fontHelper, _dispatcher);
        }
    }
}
