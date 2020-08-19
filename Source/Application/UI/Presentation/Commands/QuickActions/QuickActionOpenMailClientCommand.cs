using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.QuickActions
{
    public class QuickActionOpenMailClientCommand : QuickActionCommandBase<FtpActionTranslation>
    {
        private readonly IEMailClientAction _action;
        private readonly IMailSignatureHelper _mailSignatureHelper;

        public QuickActionOpenMailClientCommand(ITranslationUpdater translationUpdater, IEMailClientAction action, IMailSignatureHelper mailSignatureHelper) : base(translationUpdater)
        {
            _action = action;
            _mailSignatureHelper = mailSignatureHelper;
        }

        public override void Execute(object obj)
        {
            List<string> files = GetPaths(obj);
            var signature = _mailSignatureHelper.ComposeMailSignature();
            _action.OpenEmptyClient(files, signature);
        }
    }
}
