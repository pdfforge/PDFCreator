using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities.UserGuide;
using System.Windows.Input;
using pdfforge.PDFCreator.Core.SettingsManagement.DefaultSettings;
using pdfforge.PDFCreator.Core.SettingsManagement.Helper;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.PreparationActions.UserToken
{
    public class UserTokenUserControlViewModel : ActionViewModelBase<UserTokensAction, UserTokenTranslation>
    {
        public UserTokenUserControlViewModel(
            ITranslationUpdater translationUpdater,
            IUserGuideLauncher userGuideLauncher,
            IDispatcher dispatcher,
            IActionLocator actionLocator,
            ErrorCodeInterpreter errorCodeInterpreter,
            ICurrentSettingsProvider currentSettingsProvider,
            IDefaultSettingsBuilder defaultSettingsBuilder,
            IActionOrderHelper actionOrderHelper)
            : base(actionLocator, errorCodeInterpreter, translationUpdater, currentSettingsProvider, dispatcher, defaultSettingsBuilder, actionOrderHelper)
        {
            OpenUserGuideCommand = new DelegateCommand(o => userGuideLauncher?.ShowHelpTopic(HelpTopic.UserTokens));
        }

        public ICommand OpenUserGuideCommand { get; private set; }

        protected override string SettingsPreviewString
        {
            get { return TranslationAttribute.GetValue(CurrentProfile.UserTokens.Seperator); }
        }
    }
}
