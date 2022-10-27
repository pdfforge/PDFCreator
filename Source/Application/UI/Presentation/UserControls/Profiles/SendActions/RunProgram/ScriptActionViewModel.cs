using Optional;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement.DefaultSettings;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities.Tokens;
using System;
using System.ComponentModel;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.RunProgram
{
    public class ScriptActionViewModel : ActionViewModelBase<ScriptAction, ScriptTranslation>
    {
        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;
        private readonly IScriptActionHelper _scriptActionHelper;
        private readonly ITokenHelper _tokenHelper;
        private readonly ITokenViewModelFactory _tokenViewModelFactory;

        public ScriptActionViewModel(
            ITranslationUpdater translationUpdater,
            IOpenFileInteractionHelper openFileInteractionHelper,
            IScriptActionHelper scriptActionHelper,
            ITokenHelper tokenHelper,
            ITokenViewModelFactory tokenViewModelFactory,
            IDispatcher dispatcher,
            IActionLocator actionLocator,
            ErrorCodeInterpreter errorCodeInterpreter,
            ICurrentSettingsProvider currentSettingsProvider,
            IDefaultSettingsBuilder defaultSettingsBuilder,
            IActionOrderHelper actionOrderHelper)
            : base(actionLocator, errorCodeInterpreter, translationUpdater, currentSettingsProvider, dispatcher, defaultSettingsBuilder, actionOrderHelper)
        {
            _openFileInteractionHelper = openFileInteractionHelper;
            _scriptActionHelper = scriptActionHelper;
            _tokenHelper = tokenHelper;
            _tokenViewModelFactory = tokenViewModelFactory;

            if (_tokenHelper != null)
            {
                TokenReplacer = _tokenHelper.TokenReplacerWithPlaceHolders;
                var tokens = _tokenHelper.GetTokenListWithFormatting();

                ParameterTokenViewModel = _tokenViewModelFactory.BuilderWithSelectedProfile()
                    .WithSelector(p => p.Scripting.ParameterString)
                    .WithTokenList(tokens)
                    .WithTokenReplacerPreview(TokenReplacer)
                    .Build();

                ScriptFileTokenViewModel = _tokenViewModelFactory.BuilderWithSelectedProfile()
                    .WithSelector(p => p.Scripting.ScriptFile)
                    .WithTokenList(tokens)
                    .WithTokenReplacerPreview(TokenReplacer)
                    .WithButtonCommand(SelectScriptFileAction)
                    .Build();
            }
        }

        protected override string SettingsPreviewString
        {
            get
            {
                var scriptingSettings = CurrentProfile.Scripting;
                var desc = _scriptActionHelper.GetPreview(scriptingSettings.ScriptFile, scriptingSettings.ParameterString, TokenReplacer);

                return desc;
            }
        }

        public override void MountView()
        {
            ParameterTokenViewModel.MountView();
            ScriptFileTokenViewModel.MountView();

            ParameterTokenViewModel.TextChanged += TokenTextChanged;
            ScriptFileTokenViewModel.TextChanged += TokenTextChanged;
            TokenTextChanged(this, EventArgs.Empty);

            base.MountView();
        }

        public override void UnmountView()
        {
            ParameterTokenViewModel.UnmountView();
            ScriptFileTokenViewModel.UnmountView();

            ParameterTokenViewModel.TextChanged -= TokenTextChanged;
            ScriptFileTokenViewModel.TextChanged -= TokenTextChanged;

            base.UnmountView();
        }

        private Option<string> SelectScriptFileAction(string s1)
        {
            var title = Translation.SelectScriptTitle;
            var filter = Translation.ExecutableFiles
                         + @" (*.exe, *.bat, *.cmd)|*.exe;*.bat;*.cmd|"
                         + Translation.AllFiles
                         + @"(*.*)|*.*";

            var interactionResult = _openFileInteractionHelper.StartOpenFileInteraction(CurrentProfile.Scripting.ScriptFile, title, filter);
            interactionResult.MatchSome(s =>
            {
                ScriptFileTokenViewModel.Text = s;
                ScriptFileTokenViewModel.RaiseTextChanged();
            });

            return interactionResult;
        }

        public string PreviewScriptCall { get; set; }

        public TokenViewModel<ConversionProfile> ScriptFileTokenViewModel { get; set; }

        public TokenViewModel<ConversionProfile> ParameterTokenViewModel { get; set; }

        public TokenReplacer TokenReplacer { get; private set; }

        private void TokenTextChanged(object sender, EventArgs eventArgs)
        {
            PreviewScriptCall = _scriptActionHelper.GetPreview(ScriptFileTokenViewModel.Text, ParameterTokenViewModel.Text, TokenReplacer);
            RaisePropertyChanged(nameof(PreviewScriptCall));
        }

        protected override void OnCurrentProfileChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            base.OnCurrentProfileChanged(sender, propertyChangedEventArgs);

            ParameterTokenViewModel?.RaiseTextChanged();
            ScriptFileTokenViewModel?.RaiseTextChanged();
            TokenTextChanged(this, EventArgs.Empty);
        }
    }
}
