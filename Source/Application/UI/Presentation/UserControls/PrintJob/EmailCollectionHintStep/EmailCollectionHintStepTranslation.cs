using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.EmailCollectionHintStep;
public class EmailCollectionHintStepTranslation : ITranslatable
{
    public string EmailAddressLabel { get; private set; } = "Email address:";
    public string EmailPlaceholder { get; private set; } = "Email address";

    [Context(" ...receive helpful guides")]
    [TranslatorComment("Header for bullet points like 'receive helpful guides'")]
    public string BulletPointHeaderText { get; private set; } = "Enter your email to:";

    [Context("Enter your email to: ")]
    [TranslatorComment("Bullet point for the header 'Enter your email to:'")]
    public string BulletPointOneText { get; private set; } = "Get exclusive offers and expert tips";

    [Context("Enter your email to: ")]
    [TranslatorComment("Bullet point for the header 'Enter your email to:'")]
    public string BulletPointTwoText { get; private set; } = "Receive helpful guides and best practices";

    [Context("Enter your email to: ")]
    [TranslatorComment("Bullet point for the header 'Enter your email to:'")]
    public string BulletPointThreeText { get; private set; } = "Join 1,000,000+ users improving their PDF experience";

    [Context("Enter your email to: ")]
    [TranslatorComment("Bullet point for the header 'Enter your email to:'")]
    public string BulletPointFourText { get; private set; } = "Help ensure PDFCreator Free stays free for everyone";
    
    public string SkipButtonContent { get; private set; } = "Skip";
    public string SendButtonContent { get; private set; } = "Send";
    public string InvalidEmailErrorMessage { get; private set; } = "*Please provide a valid email address.";
    public string EmailCollectionTopText { get; private set; } = "Make the most of PDFCreator Free";
    public string AgreeToReceiveMarketingText { get; private set; } = "I agree to receive marketing and promotional communications.";
    public string FailedEmailSubmissionErrorTitle { get; private set; } = "Submission failed";
    public string FailedEmailSubmissionErrorMessage { get; private set; } = "Error submission failed (most likely due to a network issue), please try again.";

    [Context("... Privacy policy")]
    [TranslatorComment("Text before clickable 'Privacy policy'")]
    public string PrivacyPolicyText { get; private set; } = "The information you provide will be handled in accordance with our";
    
    public string PrivacyPolicyLinkText { get; private set; } = "Privacy policy";
}
