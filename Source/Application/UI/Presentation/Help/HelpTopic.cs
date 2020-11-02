using pdfforge.PDFCreator.Utilities.UserGuide;

namespace pdfforge.PDFCreator.UI.Presentation.Help
{
    public enum HelpTopic
    {
        // Dummy help topic to represent an unset topic
        [HelpTopic("pdfcreator/index")] Unset,

        [HelpTopic("pdfcreator/index")] General,

        [HelpTopic("pdfcreator/introduction/whats-new")] WhatsNew,

        [HelpTopic("pdfcreator/using-pdfcreator/create-a-pdf")] CreatingPdf,

        [HelpTopic("pdfcreator/pdfcreator-settings/application-settings/index")] AppSettings,

        [HelpTopic("pdfcreator/pdfcreator-settings/accounts")] Accounts,

        [HelpTopic("pdfcreator/pdfcreator-settings/home")] Home,

        // Application settings
        [HelpTopic("pdfcreator/pdfcreator-settings/application-settings/general")] AppGeneral,

        [HelpTopic("pdfcreator/pdfcreator-settings/printers")] AppPrinters,

        [HelpTopic("pdfcreator/pdfcreator-settings/application-settings/title")] AppTitle,

        [HelpTopic("pdfcreator/pdfcreator-settings/application-settings/defaultviewer")] AppViewer,

        [HelpTopic("pdfcreator/pdfcreator-settings/application-settings/debug")] AppDebug,

        [HelpTopic("pdfcreator/pdfcreator-settings/application-settings/license")] AppLicense,

        // Profile settings
        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/index")] ProfileSettings,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/actions/index")] WorkflowEditor,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/actions/modify/stamp")] Stamp,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/metadata")] ProfileMetadata,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/save")] ProfileSave,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/actions/modify/cover")] Cover,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/actions/modify/background")] Background,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/actions/modify/watermark")] Watermark,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/actions/send/open-file")] OpenViewer,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/actions/modify/attachment")] Attachment,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/actions/send/print")] PrintDocument,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/actions/send/dropbox")] Dropbox,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/actions/send/e-mail")] OpenEmailClient,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/actions/send/smtp")] SendEmailOverSmtp,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/actions/send/run-program")] RunScript,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/actions/preparation/forwardtoprofile")] ForwardToProfile,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/actions/send/ftp")] UploadWithFtp,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/actions/send/http")] UploadWithHttp,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/actions/preparation/user-tokens")] UserTokens,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/output-format")] JpegOutput,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/output-format")] PngOutput,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/output-format")] TiffOutput,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/output-format")] TextOutput,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/output-format")] PdfOutput,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/output-format")] OutputFormat,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/actions/modify/encryption")] PdfSecurity,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/actions/modify/signature")] PdfSignature,

        //Using-PdfCreator
        [HelpTopic("pdfcreator/using-pdfcreator/index")] ConvertDocuments,

        [HelpTopic("pdfcreator/using-pdfcreator/tokens")] Tokens,

        [HelpTopic("pdfcreator/using-pdfcreator/quick-action")] QuickAction,

        //Faq
        [HelpTopic("pdfcreator/frequently-asked-questions/pdf-tools")] PdfTools,

        //Server
        [HelpTopic("pdfcreator-server/index")] Server,

        [HelpTopic("pdfcreator-server/pdfcreator-server-settings/queues")] ServerQueueSettings,

        [HelpTopic("pdfcreator-server/pdfcreator-server-settings/cs-script")] ServerCsScript,

        [HelpTopic("pdfcreator-server/pdfcreator-server-settings/errors")] ServerErrorReportList,

        [HelpTopic("pdfcreator-server/pdfcreator-server-settings/application-settings/general")] ServerGeneralSettingsTab,
        [HelpTopic("pdfcreator-server/performance-test")] ServerPerformanceTest,

        [HelpTopic("pdfcreator/license/index")] License,

        [HelpTopic("pdfcreator-server/license/index")] ServerLicense
    }
}
