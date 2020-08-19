using Optional;
using pdfforge.PDFCreator.Utilities.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.Tokens
{
    public class TokenViewModelBuilder<T>
    {
        private readonly ITokenHelper _tokenHelper;
        private Expression<Func<T, string>> _selector;
        private IList<string> _tokenList = new List<string>();
        private Func<string, string> _previewFunc;
        private IList<Func<string, Option<string>>> _buttonCommandFunctions = new List<Func<string, Option<string>>>();
        private T _initialValue;

        protected IList<Action<TokenViewModel<T>>> ViewModelDecorators { get; } = new List<Action<TokenViewModel<T>>>();

        public TokenViewModelBuilder(ITokenHelper tokenHelper)
        {
            _tokenHelper = tokenHelper;
            _initialValue = default(T);
        }

        public TokenViewModelBuilder(ITokenHelper tokenHelper, T initialValue)
        {
            _tokenHelper = tokenHelper;
            _initialValue = initialValue;
        }

        public TokenViewModelBuilder<T> WithSelector(Expression<Func<T, string>> selector)
        {
            _selector = selector;
            return this;
        }

        public TokenViewModelBuilder<T> WithInitialValue(T initialValue)
        {
            _initialValue = initialValue;
            return this;
        }

        public TokenViewModelBuilder<T> WithTokenList(IList<string> tokens)
        {
            _tokenList = tokens;
            return this;
        }

        public TokenViewModelBuilder<T> WithTokenReplacerPreview(TokenReplacer tokenReplacer)
        {
            _previewFunc = tokenReplacer.ReplaceTokens;
            return this;
        }

        public TokenViewModelBuilder<T> WithDefaultTokenReplacerPreview(Func<ITokenHelper, IList<string>> tokenListSelector)
        {
            var tokenReplacer = _tokenHelper.TokenReplacerWithPlaceHolders;
            return WithTokenList(tokenListSelector(_tokenHelper)).WithTokenReplacerPreview(tokenReplacer);
        }

        public TokenViewModelBuilder<T> WithDefaultTokenReplacerPreview()
        {
            var tokenReplacer = _tokenHelper.TokenReplacerWithPlaceHolders;
            return WithTokenReplacerPreview(tokenReplacer);
        }

        public TokenViewModelBuilder<T> WithTokenCustomPreview(Func<string, string> previewFunc)
        {
            _previewFunc = previewFunc;
            return this;
        }

        public TokenViewModelBuilder<T> WithButtonCommand(Func<string, Option<string>> buttonCommand)
        {
            if (_buttonCommandFunctions.Any())
                throw new InvalidOperationException("ButtonCommand is already set!");
            _buttonCommandFunctions.Add(buttonCommand);

            return this;
        }

        public TokenViewModelBuilder<T> WithSecondaryButtonCommand(Func<string, Option<string>> secondaryButtonCommand)
        {
            if (_buttonCommandFunctions.Count == 0)
                throw new InvalidOperationException("Set WithButtonCommand first.");
            _buttonCommandFunctions.Add(secondaryButtonCommand);

            return this;
        }

        private void ValidateRequiredFields()
        {
            if (_selector == null)
                throw new InvalidOperationException("The selector must be set!");

            if (_previewFunc == null)
                throw new InvalidOperationException("The preview function must be set!");
        }

        public TokenViewModel<T> Build()
        {
            ValidateRequiredFields();

            var viewModel = CreateTokenViewModelInstance(_selector, _initialValue, _tokenList, _previewFunc, _buttonCommandFunctions);

            foreach (var viewModelDecorator in ViewModelDecorators)
            {
                viewModelDecorator(viewModel);
            }

            return viewModel;
        }

        protected virtual TokenViewModel<T> CreateTokenViewModelInstance(Expression<Func<T, string>> selector, T initialValue, IList<string> tokens, Func<string, string> generatePreview, IList<Func<string, Option<string>>> buttonCommandFunctions)
        {
            return new TokenViewModel<T>(_selector, _initialValue, _tokenList, _previewFunc, _buttonCommandFunctions);
        }
    }
}
