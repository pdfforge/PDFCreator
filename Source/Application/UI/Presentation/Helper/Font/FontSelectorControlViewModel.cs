using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction.DialogInteractions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Stamp;
using pdfforge.PDFCreator.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq.Expressions;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.Font
{
    public class FontSelectorControlViewModel : ProfileUserControlViewModel<DocumentTabTranslation>
    {
        private readonly IFontHelper _fontHelper;
        private readonly IInteractionInvoker _interactionInvoker;

        private readonly Func<ConversionProfile, string> _fontNameGetter;
        private readonly Action<ConversionProfile, string> _fontNameSetter;
        private readonly Func<ConversionProfile, string> _fontFileGetter;
        private readonly Action<ConversionProfile, string> _fontFileSetter;
        private readonly Func<ConversionProfile, float> _fontSizeGetter;
        private readonly Action<ConversionProfile, float> _fontSizeSetter;
        private readonly Func<ConversionProfile, Color> _fontColorGetter;
        private readonly Action<ConversionProfile, Color> _fontColorSetter;

        public FontSelectorControlViewModel(ITranslationUpdater translationUpdater, ISelectedProfileProvider selectedProfileProvider,
            IDispatcher dispatcher, IFontHelper fontHelper, IInteractionInvoker interactionInvoker,
            Expression<Func<ConversionProfile, string>> fontNameSelector,
            Expression<Func<ConversionProfile, string>> fontFileSelector,
            Expression<Func<ConversionProfile, float>> fontSizeSelector,
            Expression<Func<ConversionProfile, Color>> fontColorSelector)
            : base(translationUpdater, selectedProfileProvider, dispatcher)
        {
            _interactionInvoker = interactionInvoker;
            _fontHelper = fontHelper;
            _fontNameGetter = fontNameSelector.Compile();
            _fontFileGetter = fontFileSelector.Compile();
            _fontSizeGetter = fontSizeSelector.Compile();
            _fontColorGetter = fontColorSelector.Compile();
            _fontNameSetter = CompileSetter(fontNameSelector);
            _fontFileSetter = CompileSetter(fontFileSelector);
            _fontSizeSetter = CompileSetter(fontSizeSelector);
            _fontColorSetter = CompileSetter(fontColorSelector);
        }

        private Action<ConversionProfile, T> CompileSetter<T>(Expression<Func<ConversionProfile, T>> selector)
        {
            var newValue = Expression.Parameter(selector.Body.Type);
            var setter = Expression.Lambda<Action<ConversionProfile, T>>(
                Expression.Assign(selector.Body, newValue),
                selector.Parameters[0], newValue).Compile();
            return setter;
        }

        private bool wasInit;

        public override void MountView()
        {
            if (CurrentProfile != null)
            {
                UpdateFontButtonText(CurrentProfile);
            }

            if (!wasInit)
            {
                wasInit = true;
            }

            base.MountView();
        }

        public string FontButtonText { get; set; }

        public Color FontColor => _fontColorGetter(CurrentProfile);

        public DelegateCommand ChooseStampFont => new DelegateCommand(ChooseStampFontExecute);
        public DelegateCommand ChooseStampColor => new DelegateCommand(ChooseStampColorExecute);

        private void ChooseStampColorExecute(object obj)
        {
            var interaction = new ColorInteraction();
            interaction.Color = _fontColorGetter(CurrentProfile);

            _interactionInvoker.Invoke(interaction);

            if (!interaction.Success)
                return;

            _fontColorSetter(CurrentProfile, interaction.Color);
            RaisePropertyChanged(nameof(FontColor));
        }

        private void ChooseStampFontExecute(object obj)
        {
            var interaction = new FontInteraction();
            interaction.Font = new System.Drawing.Font(_fontNameGetter(CurrentProfile), _fontSizeGetter(CurrentProfile));

            _interactionInvoker.Invoke(interaction);

            if (!interaction.Success)
                return;

            var fontFilename = _fontHelper.GetFontFilename(interaction.Font);

            if (fontFilename == null)
            {
                DisplayFontError();
                return;
            }

            _fontNameSetter(CurrentProfile, interaction.Font.Name);
            _fontFileSetter(CurrentProfile, fontFilename);
            _fontSizeSetter(CurrentProfile, interaction.Font.Size);

            UpdateFontButtonText(CurrentProfile);
        }

        protected override void OnCurrentProfileChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            base.OnCurrentProfileChanged(sender, propertyChangedEventArgs);
            UpdateFontButtonText(CurrentProfile);
        }

        private void UpdateFontButtonText(ConversionProfile profile)
        {
            var fontSize = _fontSizeGetter(profile).ToString();
            var fontstring = $"{_fontNameGetter(profile)} {fontSize}pt";
            if (fontstring.Length > 25)
            {
                fontstring = _fontNameGetter(profile).Substring(0, 25 - 4 - fontSize.Length).TrimEnd();
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
}
