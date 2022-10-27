using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement.DefaultSettings;
using pdfforge.PDFCreator.UI.Presentation.Helper.Font;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities.Tokens;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Stamp
{
    public class StampActionViewModel : ActionViewModelBase<StampAction, DocumentTabTranslation>
    {
        private readonly ITokenHelper _tokenHelper;
        private readonly ITranslationUpdater _translationUpdater;
        private readonly ITokenViewModelFactory _tokenViewModelFactory;
        private TokenReplacer _tokenReplacer;
        public TokenViewModel<ConversionProfile> StampUserControlTokenViewModel { get; set; }
        public FontSelectorControlViewModel StampFontSelectorControlViewModel { get; set; }

        public StampActionViewModel(ITranslationUpdater translationUpdater, IDispatcher dispatcher,
            IActionLocator actionLocator, ErrorCodeInterpreter errorCodeInterpreter, ITokenViewModelFactory tokenViewModelFactory,
            ICurrentSettingsProvider currentSettingsProvider, ITokenHelper tokenHelper,
            IDefaultSettingsBuilder defaultSettingsBuilder, IActionOrderHelper actionOrderHelper, IFontSelectorControlViewModelFactory fontSelectorControlViewModelFactory)
            : base(actionLocator, errorCodeInterpreter, translationUpdater, currentSettingsProvider, dispatcher, defaultSettingsBuilder, actionOrderHelper)
        {
            _tokenHelper = tokenHelper;
            _translationUpdater = translationUpdater;
            _tokenViewModelFactory = tokenViewModelFactory;
            _translationUpdater.RegisterAndSetTranslation(tf =>
            {
                _tokenReplacer = _tokenHelper.TokenReplacerWithPlaceHolders;
                var tokens = _tokenHelper.GetTokenListForStamp();
                SetTokenViewModels(_tokenViewModelFactory, tokens);
            });
            SetFontViewModels(fontSelectorControlViewModelFactory);
        }

        private bool wasInit;

        protected override string SettingsPreviewString => CurrentProfile.Stamping.StampText;

        public override void MountView()
        {
            if (!wasInit)
            {
                wasInit = true;
            }

            StampUserControlTokenViewModel.MountView();
            StampFontSelectorControlViewModel.MountView();

            base.MountView();
        }

        public override void UnmountView()
        {
            base.UnmountView();
            StampUserControlTokenViewModel?.UnmountView();
            StampFontSelectorControlViewModel?.UnmountView();
        }

        private void SetTokenViewModels(ITokenViewModelFactory tokenViewModelFactory, List<string> tokens)
        {
            StampUserControlTokenViewModel = tokenViewModelFactory
                .BuilderWithSelectedProfile()
                .WithSelector(p => p.Stamping.StampText)
                .WithTokenList(tokens)
                .WithDefaultTokenReplacerPreview()
                .Build();
        }

        private void SetFontViewModels(IFontSelectorControlViewModelFactory tokenControlViewModelFactory)
        {
            StampFontSelectorControlViewModel = tokenControlViewModelFactory
                .BuilderWithSelectedProfile()
                .WithFontFileSelector(profile => profile.Stamping.FontFile)
                .WithFontNameSelector(profile => profile.Stamping.FontName)
                .WithFontColorSelector(profile => profile.Stamping.Color)
                .WithFontSizeSelector(profile => profile.Stamping.FontSize)
                .Build();
        }
    }
}
