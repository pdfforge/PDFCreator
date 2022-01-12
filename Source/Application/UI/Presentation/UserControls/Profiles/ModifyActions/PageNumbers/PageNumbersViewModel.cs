using System.Collections.Generic;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Actions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement.DefaultSettings;
using pdfforge.PDFCreator.Core.SettingsManagement.Helper;
using pdfforge.PDFCreator.UI.Presentation.Converter;
using pdfforge.PDFCreator.UI.Presentation.Helper.Font;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.PageNumbers
{
    public class PageNumbersViewModel : ActionViewModelBase<PageNumbersAction, PageNumbersTranslation>
    {
        private readonly ITokenHelper _tokenHelper;
        private readonly ITranslationUpdater _translationUpdater;
        private readonly ITokenViewModelFactory _tokenViewModelFactory;
        private TokenReplacer _tokenReplacer;
        private readonly IPositionToUnitConverterFactory _positionToUnitConverter;
        private IPositionToUnitConverter UnitConverter { get; set; }
        public TokenViewModel<ConversionProfile> PageNumbersUserControlTokenViewModel { get; set; }
        public FontSelectorControlViewModel PageNumbersFontSelectorControlViewModel { get; set; }
        public DelegateCommand ChangeUnitConverterCommand { get; private set; }

        public PageNumbersViewModel(ITranslationUpdater translationUpdater, IDispatcher dispatcher,
            IActionLocator actionLocator, ErrorCodeInterpreter errorCodeInterpreter, ITokenViewModelFactory tokenViewModelFactory,
            ICurrentSettingsProvider currentSettingsProvider, ITokenHelper tokenHelper,
            IDefaultSettingsBuilder defaultSettingsBuilder, IActionOrderHelper actionOrderHelper, IFontSelectorControlViewModelFactory fontSelectorControlViewModelFactory, IPositionToUnitConverterFactory positionToUnitConverter)
            : base(actionLocator, errorCodeInterpreter, translationUpdater, currentSettingsProvider, dispatcher, defaultSettingsBuilder, actionOrderHelper)
        {
            _tokenHelper = tokenHelper;
            _positionToUnitConverter = positionToUnitConverter;
            _translationUpdater = translationUpdater;
            _tokenViewModelFactory = tokenViewModelFactory;
            _translationUpdater.RegisterAndSetTranslation(tf =>
            {
                UpdateTokenViewModel();
            });
            SetFontViewModels(fontSelectorControlViewModelFactory);
            ChangeUnitConverterCommand = new DelegateCommand(ChangeUnitConverterExecute);
            UnitConverter = _positionToUnitConverter?.CreatePositionToUnitConverter(CurrentProfile?.PageNumbers?.UnitOfMeasurement ?? UnitOfMeasurement.Centimeter);
        }

        private bool wasInit;

        public PageNumberPosition CurrentPageNumberPosition
        {
            get
            {
                if (CurrentProfile == null) return PageNumberPosition.BottomRight;
                return CurrentProfile.PageNumbers.Position;
            }
            set
            {
                if (!value.IsCorner() && IsCorner)
                {
                    CurrentProfile.PageNumbers.HorizontalOffset = 0;
                    RaisePropertyChanged(nameof(CurrentProfile.PageNumbers.HorizontalOffset));
                }
                else if (value.IsCorner() && !IsCorner)
                {
                    CurrentProfile.PageNumbers.HorizontalOffset = new Conversion.Settings.PageNumbers().HorizontalOffset;
                    RaisePropertyChanged(nameof(CurrentProfile.PageNumbers.HorizontalOffset));
                }

                CurrentProfile.PageNumbers.Position = value;
                RaisePropertyChanged(nameof(IsCorner));
                RaisePropertyChanged(nameof(IsFromCenter));
                RaisePropertyChanged(nameof(IsFromLeft));
                RaisePropertyChanged(nameof(IsFromRight));
                RaisePropertyChanged(nameof(IsFromTop));
            }
        }

        public bool UseRomanNumerals
        {
            get => CurrentProfile?.PageNumbers?.UseRomanNumerals ?? false;
            set
            {
                CurrentProfile.PageNumbers.UseRomanNumerals = value;
                UpdateTokenViewModel();
                RaisePropertyChanged(nameof(PageNumbersUserControlTokenViewModel));
            }
        }

        public float HorizontalOffset
        {
            get
            {
                if (CurrentProfile?.PageNumbers?.HorizontalOffset == null)
                    return 0f;

                return UnitConverter.ConvertBack(CurrentProfile.PageNumbers.HorizontalOffset);
            }
            set => CurrentProfile.PageNumbers.HorizontalOffset = UnitConverter.ConvertToUnit(value);
        }

        public float VerticalOffset
        {
            get
            {
                if (CurrentProfile?.PageNumbers?.VerticalOffset == null)
                    return 0f;

                return UnitConverter.ConvertBack(CurrentProfile.PageNumbers.VerticalOffset);
            }
            set => CurrentProfile.PageNumbers.VerticalOffset = UnitConverter.ConvertToUnit(value);
        }

        protected override string SettingsPreviewString => CurrentProfile.PageNumbers.Format;
        public bool IsCorner => CurrentProfile != null && CurrentProfile.PageNumbers.Position.IsCorner();
        public bool IsFromLeft => CurrentProfile != null && CurrentProfile.PageNumbers.Position.IsLeft();
        public bool IsFromRight => CurrentProfile != null && CurrentProfile.PageNumbers.Position.IsRight();
        public bool IsFromCenter => CurrentProfile != null && CurrentProfile.PageNumbers.Position.IsCenter();
        public bool IsFromTop => CurrentProfile != null && CurrentProfile.PageNumbers.Position.IsTop();

        public override void MountView()
        {
            if (!wasInit)
            {
                wasInit = true;
            }

            PageNumbersUserControlTokenViewModel.MountView();
            PageNumbersFontSelectorControlViewModel.MountView();

            base.MountView();
        }

        public override void UnmountView()
        {
            base.UnmountView();
            PageNumbersUserControlTokenViewModel?.UnmountView();
            PageNumbersFontSelectorControlViewModel?.UnmountView();
        }

        private void UpdateTokenViewModel()
        {
            _tokenReplacer = _tokenHelper.TokenReplacerWithPlaceHolders;
            var tokens = _tokenHelper.GetTokenListForPageNumbers();
            SetTokenViewModels(_tokenViewModelFactory, tokens);
        }

        private void SetTokenViewModels(ITokenViewModelFactory tokenViewModelFactory, List<string> tokens)
        {
            TokenReplacer tokenReplacer;
            if (CurrentProfile != null && CurrentProfile.PageNumbers.UseRomanNumerals)
            {
                tokenReplacer = CreateRomanTokenReplacer(_tokenReplacer);
            }
            else
            {
                tokenReplacer = _tokenReplacer;
            }


            PageNumbersUserControlTokenViewModel = tokenViewModelFactory
                .BuilderWithSelectedProfile()
                .WithSelector(p => p.PageNumbers.Format)
                .WithTokenList(tokens)
                .WithTokenReplacerPreview(tokenReplacer)
                .Build();
        }

        private TokenReplacer CreateRomanTokenReplacer(TokenReplacer from)
        {
            const string pageNumberToken = "<PageNumber>";
            const string numberOfPagesToken = "<NumberOfPages>";
            var tokenReplacer = new TokenReplacer();
            foreach (var tokenName in from.GetTokenNames())
            {
                if (tokenName.Equals(pageNumberToken) || tokenName.Equals(numberOfPagesToken))
                {
                    tokenReplacer.AddStringToken(tokenName.Substring(1, tokenName.Length - 2), "I");
                }
                else
                {
                    tokenReplacer.AddToken(from.GetToken(tokenName.Substring(1, tokenName.Length - 2)));
                }
            }

            return tokenReplacer;
        }

        private void SetFontViewModels(IFontSelectorControlViewModelFactory tokenControlViewModelFactory)
        {
            PageNumbersFontSelectorControlViewModel = tokenControlViewModelFactory
                .BuilderWithSelectedProfile()
                .WithFontFileSelector(profile => profile.PageNumbers.FontFile)
                .WithFontNameSelector(profile => profile.PageNumbers.FontName)
                .WithFontColorSelector(profile => profile.PageNumbers.FontColor)
                .WithFontSizeSelector(profile => profile.PageNumbers.FontSize)
                .Build();
        }

        private void ChangeUnitConverterExecute(object obj)
        {
            // values must be saved in local variables before the converter is changed
            // so that we can maintain the real coordinates of the position
            var unit = (UnitOfMeasurement)obj;
            UnitConverter = _positionToUnitConverter.CreatePositionToUnitConverter(unit);

            RaisePropertyChanged(nameof(HorizontalOffset));
            RaisePropertyChanged(nameof(VerticalOffset));
        }
    }
}
