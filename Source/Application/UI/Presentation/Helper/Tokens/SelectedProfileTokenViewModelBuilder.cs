using Optional;
using pdfforge.PDFCreator.Conversion.Settings;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.Tokens
{
    public class SelectedProfileTokenViewModelBuilder : TokenViewModelBuilder<ConversionProfile>
    {
        private readonly ISelectedProfileProvider _selectedProfileProvider;

        public SelectedProfileTokenViewModelBuilder(ISelectedProfileProvider selectedProfileProvider, ITokenHelper tokenHelper)
            : base(tokenHelper, selectedProfileProvider.SelectedProfile)
        {
            _selectedProfileProvider = selectedProfileProvider;
        }

        protected override TokenViewModel<ConversionProfile> CreateTokenViewModelInstance(Expression<Func<ConversionProfile, string>> selector, ConversionProfile initialValue, IList<string> tokens, Func<string, string> generatePreview, IList<Func<string, Option<string>>> buttonCommandFunctions)
        {
            return new MountableTokenViewModel(_selectedProfileProvider, selector, initialValue, tokens, generatePreview, buttonCommandFunctions);
        }
    }
}
