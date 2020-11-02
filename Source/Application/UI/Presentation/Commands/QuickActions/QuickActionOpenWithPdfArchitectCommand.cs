using pdfforge.PDFCreator.Conversion.Actions.Actions.Interface;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.QuickActions
{
    public class QuickActionOpenWithPdfArchitectCommand : QuickActionCommandBase<FtpActionTranslation>
    {
        private readonly IDefaultViewerAction _action;

        public QuickActionOpenWithPdfArchitectCommand(ITranslationUpdater translationUpdater, IDefaultViewerAction action) : base(translationUpdater)
        {
            _action = action;
        }

        public override void Execute(object obj)
        {
            var files = GetPaths(obj);
            _action.OpenWithArchitect(files);
        }
    }
}
