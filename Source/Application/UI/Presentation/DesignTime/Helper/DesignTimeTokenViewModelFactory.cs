using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper
{
    public class DesignTimeTokenViewModelFactory : ITokenViewModelFactory
    {
        private readonly ITokenHelper _tokenHelper;

        public DesignTimeTokenViewModelFactory()
        {
            _tokenHelper = new TokenHelper(new DesignTimeTranslationUpdater());
        }

        public TokenViewModelBuilder<T> Builder<T>()
        {
            return new TokenViewModelBuilder<T>(_tokenHelper);
        }

        TokenViewModelBuilder<ConversionProfile> ITokenViewModelFactory.BuilderWithSelectedProfile()
        {
            return new SelectedProfileTokenViewModelBuilder(new DesignTimeCurrentSettingsProvider(), _tokenHelper);
        }
    }
}
