using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.ServiceLocator;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Misc
{
    public class TokenHintPanelViewModel : TranslatableViewModelBase<TokenHintPanelTranslation>, IWhitelisted
    {
        private readonly ICurrentSettingsProvider _settings;
        private string _textWrapper = "";

        public TokenHintPanelViewModel(ITranslationUpdater translationUpdater, ITokenHelper tokenHelper, ICurrentSettingsProvider settings) : base(translationUpdater)
        {
            _settings = settings;
            TokenHelper = tokenHelper;

            if (_settings != null)
                _settings.SelectedProfileChanged += (sender, args) => RaisePropertyChanged(nameof(UserTokenProfile));
        }

        public string HintText
        {
            get
            {
                if (TokenHelper == null || Translation == null)
                    return "";
                return TokenHelper.ContainsUserToken(TextWrapper) ? Translation.UserTokenText : Translation.InsecureTokenText;
            }
        }

        protected override void OnTranslationChanged()
        {
            RaisePropertyChanged(HintText);
        }

        public string TextWrapper
        {
            get { return _textWrapper; }
            set
            {
                _textWrapper = value;
                RaisePropertyChanged(nameof(TextWrapper));
                RaisePropertyChanged(nameof(HintText));
            }
        }

        public ITokenHelper TokenHelper { get; }

        public void OnTextChanged(string text)
        {
            TextWrapper = text;
        }

        public ConversionProfile UserTokenProfile => _settings?.SelectedProfile;
    }

    public class DesignTimeTokenHintPanelViewModel : TokenHintPanelViewModel
    {
        public DesignTimeTokenHintPanelViewModel() : base(new DesignTimeTranslationUpdater(), null, null)
        {
        }
    }
}
