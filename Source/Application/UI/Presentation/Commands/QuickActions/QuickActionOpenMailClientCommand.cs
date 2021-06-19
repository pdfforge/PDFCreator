using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Core.SettingsManagement.DefaultSettings;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.QuickActions
{
    public class QuickActionOpenMailClientCommand : QuickActionCommandBase<FtpActionTranslation>
    {
        private readonly IEMailClientAction _action;
        private readonly IDefaultSettingsBuilder _defaultSettingsBuilder;

        public QuickActionOpenMailClientCommand(ITranslationUpdater translationUpdater, IEMailClientAction action, IDefaultSettingsBuilder defaultSettingsBuilder) : base(translationUpdater)
        {
            _action = action;
            _defaultSettingsBuilder = defaultSettingsBuilder;
        }

        public override void Execute(object obj)
        {
            var files = GetPaths(obj);
            var defaultProfile = _defaultSettingsBuilder.CreateDefaultProfile();
            _action.OpenEmptyClient(files, defaultProfile.EmailClientSettings);
        }
    }
}
