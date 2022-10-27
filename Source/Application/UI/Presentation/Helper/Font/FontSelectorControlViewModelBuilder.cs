using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq.Expressions;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.Font
{
    public interface IFontSelectorControlViewModelBuilder
    {
        IFontSelectorControlViewModelBuilder WithFontNameSelector(Expression<Func<ConversionProfile, string>> selector);

        IFontSelectorControlViewModelBuilder WithFontFileSelector(Expression<Func<ConversionProfile, string>> selector);

        IFontSelectorControlViewModelBuilder WithFontSizeSelector(Expression<Func<ConversionProfile, float>> selector);

        IFontSelectorControlViewModelBuilder WithFontColorSelector(Expression<Func<ConversionProfile, Color>> selector);

        FontSelectorControlViewModel Build();
    }

    public class FontSelectorControlViewModelBuilder : IFontSelectorControlViewModelBuilder
    {
        private readonly IFontHelper _fontHelper;
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly IDispatcher _dispatcher;
        private readonly ISelectedProfileProvider _selectedProfileProvider;
        private readonly ITranslationUpdater _translationUpdater;

        private Expression<Func<ConversionProfile, string>> _fontNameSelector;
        private Expression<Func<ConversionProfile, string>> _fontFileSelector;
        private Expression<Func<ConversionProfile, float>> _fontSizeSelector;
        private Expression<Func<ConversionProfile, Color>> _fontColorSelector;

        protected IList<Action<FontSelectorControlViewModel>> ViewModelDecorators { get; } = new List<Action<FontSelectorControlViewModel>>();

        public FontSelectorControlViewModelBuilder(ITranslationUpdater translationUpdater, ISelectedProfileProvider selectedProfileProvider, IInteractionInvoker interactionInvoker, IFontHelper fontHelper, IDispatcher dispatcher)
        {
            _translationUpdater = translationUpdater;
            _selectedProfileProvider = selectedProfileProvider;
            _interactionInvoker = interactionInvoker;
            _fontHelper = fontHelper;
            _dispatcher = dispatcher;
        }

        public IFontSelectorControlViewModelBuilder WithFontNameSelector(Expression<Func<ConversionProfile, string>> selector)
        {
            _fontNameSelector = selector;
            return this;
        }

        public IFontSelectorControlViewModelBuilder WithFontFileSelector(Expression<Func<ConversionProfile, string>> selector)
        {
            _fontFileSelector = selector;
            return this;
        }

        public IFontSelectorControlViewModelBuilder WithFontSizeSelector(Expression<Func<ConversionProfile, float>> selector)
        {
            _fontSizeSelector = selector;
            return this;
        }

        public IFontSelectorControlViewModelBuilder WithFontColorSelector(Expression<Func<ConversionProfile, Color>> selector)
        {
            _fontColorSelector = selector;
            return this;
        }

        private void ValidateRequiredFields()
        {
            if (_fontNameSelector == null)
                throw new InvalidOperationException("The font name selector must be set!");
            if (_fontFileSelector == null)
                throw new InvalidOperationException("The font file selector must be set!");
            if (_fontSizeSelector == null)
                throw new InvalidOperationException("The font size selector must be set!");
            if (_fontColorSelector == null)
                throw new InvalidOperationException("The font color selector must be set!");
        }

        public FontSelectorControlViewModel Build()
        {
            ValidateRequiredFields();

            var viewModel = CreateTokenViewModelInstance(_fontNameSelector, _fontFileSelector, _fontSizeSelector, _fontColorSelector);

            foreach (var viewModelDecorator in ViewModelDecorators)
            {
                viewModelDecorator(viewModel);
            }

            return viewModel;
        }

        private FontSelectorControlViewModel CreateTokenViewModelInstance(Expression<Func<ConversionProfile, string>> fontNameSelector, Expression<Func<ConversionProfile, string>> fontFileSelector, Expression<Func<ConversionProfile, float>> fontSizeSelector, Expression<Func<ConversionProfile, Color>> fontColorSelector)
        {
            return new FontSelectorControlViewModel(_translationUpdater, _selectedProfileProvider, _dispatcher, _fontHelper, _interactionInvoker, fontNameSelector, fontFileSelector, fontSizeSelector, fontColorSelector);
        }
    }
}
