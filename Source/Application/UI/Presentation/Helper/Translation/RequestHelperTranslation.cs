using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
public class RequestHelperTranslation : ITranslatable
{
    public string TrialRequestFailedNetworkIssueMessage { get; protected set; } = "Trial request failed due to a network issue, please try again.";
    public string TrialRequestFailedEmailAlreadyUsedMessage { get; protected set; } = "Trial request failed because the email address is already being used for a trial.";
    public string TrialRequestFailedTitle { get; protected set; } = "Trial request failed";
    public string TrialRequestSuccessfulMessage { get; protected set; } = "Trial request sent. You will receive an email with your trial license key momentarily.";
    public string TrialRequestSuccessfulTitle { get; protected set; } = "Request successful";
}
