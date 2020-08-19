using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction.DialogInteractions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Tokens;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab.Stamp
{
    public class StampUserControlViewModel : ProfileUserControlViewModel<DocumentTabTranslation>
    {
        private readonly IFontHelper _fontHelper;
        private readonly ITokenHelper _tokenHelper;
        private readonly ITranslationUpdater _translationUpdater;
        private readonly ITokenViewModelFactory _tokenViewModelFactory;
        private readonly IInteractionInvoker _interactionInvoker;
        private TokenReplacer _tokenReplacer;
        public TokenViewModel<ConversionProfile> StampUserControlTokenViewModel { get; set; }

        public StampUserControlViewModel(IInteractionInvoker interactionInvoker, IFontHelper fontHelper, ITokenHelper tokenHelper,
                        ITranslationUpdater translationUpdater, ISelectedProfileProvider profile, ITokenViewModelFactory tokenViewModelFactory, IDispatcher dispatcher)
            : base(translationUpdater, profile, dispatcher)
        {
            _interactionInvoker = interactionInvoker;
            _fontHelper = fontHelper;
            _tokenHelper = tokenHelper;
            _translationUpdater = translationUpdater;
            _tokenViewModelFactory = tokenViewModelFactory;
            _translationUpdater.RegisterAndSetTranslation(tf =>
            {
                _tokenReplacer = _tokenHelper.TokenReplacerWithPlaceHolders;
                var tokens = _tokenHelper.GetTokenListForStamp();
                SetTokenViewModels(_tokenViewModelFactory, tokens);
            });
        }

        private bool wasInit;

        public override void MountView()
        {
            if (CurrentProfile != null)
            {
                UpdateFontButtonText(CurrentProfile.Stamping);
            }

            if (!wasInit)
            {
                wasInit = true;
            }

            StampUserControlTokenViewModel.MountView();

            base.MountView();
        }

        public override void UnmountView()
        {
            base.UnmountView();
            StampUserControlTokenViewModel?.UnmountView();
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

        public string FontButtonText { get; set; }

        public DelegateCommand ChooseStampFont => new DelegateCommand(ChooseStampFontExecute);
        public DelegateCommand ChooseStampColor => new DelegateCommand(ChooseStampColorExecute);

        private void ChooseStampColorExecute(object obj)
        {
            var interaction = new ColorInteraction();
            interaction.Color = CurrentProfile.Stamping.Color;

            _interactionInvoker.Invoke(interaction);

            if (!interaction.Success)
                return;

            CurrentProfile.Stamping.Color = interaction.Color;
            RaisePropertyChanged(nameof(CurrentProfile));
        }

        private void ChooseStampFontExecute(object obj)
        {
            var interaction = new FontInteraction();
            interaction.Font = new Font(CurrentProfile.Stamping.FontName, CurrentProfile.Stamping.FontSize);

            _interactionInvoker.Invoke(interaction);

            if (!interaction.Success)
                return;

            var fontFilename = _fontHelper.GetFontFilename(interaction.Font);

            if (fontFilename == null)
            {
                DisplayFontError();
                return;
            }

            CurrentProfile.Stamping.FontName = interaction.Font.Name;
            CurrentProfile.Stamping.FontFile = fontFilename;
            CurrentProfile.Stamping.FontSize = interaction.Font.Size;

            UpdateFontButtonText(CurrentProfile.Stamping);
        }

        protected override void OnCurrentProfileChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            base.OnCurrentProfileChanged(sender, propertyChangedEventArgs);
            UpdateFontButtonText(CurrentProfile.Stamping);
        }

        private void UpdateFontButtonText(Stamping stamping)
        {
            var fontSize = stamping.FontSize.ToString();
            var fontstring = $"{stamping.FontName} {fontSize}pt";
            if (fontstring.Length > 25)
            {
                fontstring = stamping.FontName.Substring(0, 25 - 4 - fontSize.Length).TrimEnd();
                fontstring = $"{fontstring}. {fontSize}pt";
            }

            FontButtonText = fontstring;
            RaisePropertyChanged(nameof(FontButtonText));
        }

        private void DisplayFontError()
        {
            var message = Translation.FontFileNotSupported;

            var interaction = new MessageInteraction(message, "PDFCreator", MessageOptions.OK, MessageIcon.Warning);
            _interactionInvoker.Invoke(interaction);
        }
    }

    public class DesignTimeStampUserControlViewModel : StampUserControlViewModel
    {
        public DesignTimeStampUserControlViewModel() : base(null, null, null, new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider(), null, null)
        {
        }
    }
}
