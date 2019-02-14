using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    public class MailSignatureHelperFreeVersion : IMailSignatureHelper
    {
        protected MailSignatureHelperTranslation Translation;

        public MailSignatureHelperFreeVersion(ITranslationUpdater translationUpdater)
        {
            translationUpdater.RegisterAndSetTranslation(tf => Translation = tf.UpdateOrCreateTranslation(Translation));
        }

        public virtual string ComposeMailSignature()
        {
            return Environment.NewLine
                + "______________________________"
                + Environment.NewLine
                + Translation.MailSignatureFreeVersion
                + Environment.NewLine
                + "www.pdfforge.org";
        }
    }
}
