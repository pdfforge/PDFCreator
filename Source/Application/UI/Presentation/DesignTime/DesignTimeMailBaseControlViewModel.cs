using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.MailBase;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    internal class DesignTimeMailBaseControlViewModel : MailBaseControlViewModel<MailBaseTabTranslation>
    {
        public DesignTimeMailBaseControlViewModel() : base(new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider(), null, null)
        {
            MailActionSettings = new EmailClientSettings();
            MailActionSettings.AdditionalAttachments = new List<string>
            {
                "test1","test2","test3"
            };
        }

        protected override IMailActionSettings MailActionSettings { get; } = null;
    }
}
