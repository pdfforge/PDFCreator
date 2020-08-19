using pdfforge.PDFCreator.Conversion.Actions.Actions.Interface;
using pdfforge.PDFCreator.Conversion.Actions.Queries;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.QuickActions
{
    public class QuickActionOpenWithPdfArchitectCommand : QuickActionCommandBase<FtpActionTranslation>
    {
        private readonly IDefaultViewerAction _action;
        private readonly IPdfArchitectCheck _architectCheck;
        private readonly IRecommendArchitect _recommendArchitect;

        public QuickActionOpenWithPdfArchitectCommand(ITranslationUpdater translationUpdater, IDefaultViewerAction action, IPdfArchitectCheck architectCheck, IRecommendArchitect recommendArchitect) : base(translationUpdater)
        {
            _action = action;
            _architectCheck = architectCheck;
            _recommendArchitect = recommendArchitect;
        }

        public override void Execute(object obj)
        {
            var files = GetPaths(obj);

            if (_architectCheck.IsInstalled())
            {
                foreach (var file in files)
                {
                    _action.OpenWithArchitect(file);
                }
            }
            else
            {
                _recommendArchitect.Show();
            }
        }
    }
}
