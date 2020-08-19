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

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/workflow-editor")] WorkflowEditor,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/modify")] Stamp,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/metadata")] ProfileMetadata,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/save")] ProfileSave,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/modify")] Cover,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/modify")] Background,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/modify")] Watermark,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/save")] OpenViewer,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/modify")] Attachment,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/send/print-document")] PrintDocument,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/send/dropbox")] Dropbox,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/send/open-email-client")] OpenEmailClient,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/send/send-email-over-smtp")] SendEmailOverSmtp,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/advanced-features/run-script")] RunScript,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/send/upload-with-ftp")] UploadWithFtp,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/send/upload-with-http")] UploadWithHttp,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/advanced-features/user-tokens")] UserTokens,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/convert")] JpegOutput,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/convert")] PngOutput,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/convert")] TiffOutput,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/convert")] TextOutput,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/convert")] PdfOutput,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/convert")] Convert,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/security")] PdfSecurity,

        [HelpTopic("pdfcreator/pdfcreator-settings/profile-settings/security")] PdfSignature,

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
