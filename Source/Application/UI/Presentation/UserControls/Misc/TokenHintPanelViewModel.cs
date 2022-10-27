using pdfforge.PDFCreator.Core.ServiceLocator;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System.ComponentModel;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Misc
{
    public class TokenHintPanelViewModel : TranslatableViewModelBase<TokenHintPanelTranslation>, IWhitelisted, IMountable
    {
        private readonly ICurrentSettingsProvider _settings;

        private ITokenHelper _tokenHelper;

        public TokenHintPanelViewModel(ITranslationUpdater translationUpdater, ITokenHelper tokenHelper, ICurrentSettingsProvider settings) : base(translationUpdater)
        {
            _settings = settings;
            _tokenHelper = tokenHelper;
        }

        private string _textWithToken = "";
        public TokenWarningCheckResult TokenWarningCheckResult => _settings?.SelectedProfile != null ? _tokenHelper.TokenWarningCheck(_textWithToken, _settings.SelectedProfile) : TokenWarningCheckResult.NoWarning;

        public void OnTextChanged(string textWithToken)
        {
            _textWithToken = textWithToken;
            RaisePropertyChanged(nameof(TokenWarningCheckResult));
        }

        private void OnSelectedProfileChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(TokenWarningCheckResult));
        }

        public void MountView()
        {
            if (_settings != null)
                _settings.SelectedProfileChanged += OnSelectedProfileChanged;
        }

        public void UnmountView()
        {
            _settings.SelectedProfileChanged -= OnSelectedProfileChanged;
        }
    }

    public class DesignTimeTokenHintPanelViewModel : TokenHintPanelViewModel
    {
        public DesignTimeTokenHintPanelViewModel() : base(new DesignTimeTranslationUpdater(), null, null)
        {
        }
    }
}
