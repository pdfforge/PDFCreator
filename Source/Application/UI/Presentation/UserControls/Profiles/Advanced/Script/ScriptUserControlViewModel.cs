using Optional;
using pdfforge.PDFCreator.Conversion.Actions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities.Threading;
using pdfforge.PDFCreator.Utilities.Tokens;
using System;
using System.ComponentModel;
using SystemInterface.IO;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Advanced.Script
{
    public class ScriptUserControlViewModel : ProfileUserControlViewModel<ScriptTranslation>
    {
        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;
        private readonly IScriptActionHelper _scriptActionHelper;

        public ScriptUserControlViewModel(
            ITranslationUpdater translationUpdater,
            ISelectedProfileProvider provider,
            IOpenFileInteractionHelper openFileInteractionHelper,
            IScriptActionHelper scriptActionHelper,
            ITokenHelper tokenHelper,
            ITokenViewModelFactory tokenViewModelFactory,
            IDispatcher dispatcher)
            : base(translationUpdater, provider, dispatcher)
        {
            _openFileInteractionHelper = openFileInteractionHelper;
            _scriptActionHelper = scriptActionHelper;

            if (tokenHelper != null)
            {
                TokenReplacer = tokenHelper.TokenReplacerWithPlaceHolders;
                var tokens = tokenHelper.GetTokenListWithFormatting();

                ParameterTokenViewModel = tokenViewModelFactory.BuilderWithSelectedProfile()
                    .WithSelector(p => p.Scripting.ParameterString)
                    .WithTokenList(tokens)
                    .WithTokenReplacerPreview(TokenReplacer)
                    .Build();

                ScriptFileTokenViewModel = tokenViewModelFactory.BuilderWithSelectedProfile()
                    .WithSelector(p => p.Scripting.ScriptFile)
                    .WithTokenList(tokens)
                    .WithTokenReplacerPreview(TokenReplacer)
                    .WithButtonCommand(SelectScriptFileAction)
                    .Build();

                ParameterTokenViewModel.TextChanged += TokenTextChanged;
                ScriptFileTokenViewModel.TextChanged += TokenTextChanged;
                TokenTextChanged(this, EventArgs.Empty);
            }
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

        public TokenReplacer TokenReplacer { get; }

        private void TokenTextChanged(object sender, EventArgs eventArgs)
        {
            PreviewScriptCall = ComposeSampleCommand(ScriptFileTokenViewModel.Text, ParameterTokenViewModel.Text);
            RaisePropertyChanged(nameof(PreviewScriptCall));
        }

        private string ComposeSampleCommand(string scriptPath, string additionalParams)
        {
            if (string.IsNullOrEmpty(scriptPath) || (scriptPath.Trim().Length == 0))
                return "";

            var scriptCall = PathSafe.GetFileName(_scriptActionHelper.ComposeScriptPath(scriptPath, TokenReplacer));

            if (!string.IsNullOrEmpty(additionalParams))
            {
                scriptCall += " " + _scriptActionHelper.ComposeScriptParameters(additionalParams, new[] { @"C:\File1.pdf", @"C:\File2.pdf" }, TokenReplacer);
            }
            else
            {
                scriptCall += @" C:\File1.pdf C:\File2.pdf";
            }

            return scriptCall;
        }

        protected override void OnCurrentProfileChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            base.OnCurrentProfileChanged(sender, propertyChangedEventArgs);

            ParameterTokenViewModel.RaiseTextChanged();
            ScriptFileTokenViewModel.RaiseTextChanged();
            TokenTextChanged(this, EventArgs.Empty);
        }
    }

    public class DesignTimeScriptUserControlViewModel : ScriptUserControlViewModel
    {
        public DesignTimeScriptUserControlViewModel() : base(new TranslationUpdater(new TranslationFactory(), new ThreadManager()), new DesignTimeCurrentSettingsProvider(), null, null, null, new DesignTimeTokenViewModelFactory(), null)
        {
        }
    }
}
