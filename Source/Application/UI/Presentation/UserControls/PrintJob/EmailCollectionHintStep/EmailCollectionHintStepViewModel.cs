using System;
using System.Threading.Tasks;
using System.Windows.Input;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Utilities.Messages;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.EmailCollection;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.UI.Presentation.Workflow;
using pdfforge.PDFCreator.Utilities;
using DelegateCommand = Prism.Commands.DelegateCommand;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.EmailCollectionHintStep;
public class EmailCollectionHintStepViewModel : TranslatableViewModelBase<EmailCollectionHintStepTranslation>, IWorkflowViewModel
{
    public ICommand SkipEmailStepCommand { get; set; }
    public EventHandler StepFinished;
    private readonly IConditionalHintManager _conditionalHintManager;
    private readonly IInteractionRequest _interactionRequest;
    private TaskCompletionSource<object> _taskCompletionSource = new();

    private string _email = "";
    private bool _isEmailInvalid;
    private bool _marketingConsent;
    private bool _isSending;

    public EmailCollectionHintStepViewModel(ITranslationUpdater translationUpdater, ICommandLocator commandLocator, IConditionalHintManager conditionalHintManager, IInteractionRequest interactionRequest) : base(translationUpdater)
    {
        _conditionalHintManager = conditionalHintManager;
        _interactionRequest = interactionRequest;
        SkipEmailStepCommand = new DelegateCommand(CancelExecute);

        SendEmailInformationCommand = new DelegateCommand(SendEmailInformationCommandExecute, SendEmailInformationCommandCanExecute);
        SendEmailInformationCommand.ObservesProperty(() => EmailAddress);
        OpenPrivacyPolicyCommand = commandLocator.GetInitializedCommand<UrlOpenCommand, string>(Urls.PrivacyPolicyUrl);
    }

    public DelegateCommand SendEmailInformationCommand { get; }
    public ICommand OpenPrivacyPolicyCommand { get; }

    public string EmailAddress
    {
        get => _email;
        set
        {
            if (value == _email) return;
            _email = value;
            IsEmailInvalid = false;
            RaisePropertyChanged();
        }
    }

    public static string PrivacyPolicyUrl => Urls.PrivacyPolicyUrl;

    public bool IsEmailInvalid
    {
        get => _isEmailInvalid;
        set
        {
            _isEmailInvalid = value;
            RaisePropertyChanged();
        }
    }

    public bool MarketingConsent
    {
        get => _marketingConsent;
        set
        {
            _marketingConsent = value;
            RaisePropertyChanged();
        }
    }

    public bool IsSending
    {
        get => _isSending;
        private set
        {
            _isSending = value;
            RaisePropertyChanged();
        }
    }

    public Task ExecuteWorkflowStep(Job job)
    {
        return _taskCompletionSource.Task;
    }
    private void InvokeStepFinished()
    {
        StepFinished?.Invoke(this, EventArgs.Empty);
        _taskCompletionSource.SetResult(null);
    }

    private async void SendEmailInformationCommandExecute()
    {
        IsSending = true;

        if (EmailValidator.IsValidEmail(_email))
        {
            if (await _conditionalHintManager.SendEmailInformation(EmailAddress, _marketingConsent))
            {
                IsSending = false;
                InvokeStepFinished();
            }
            else
            {
                IsSending = false;
                var messageInteraction = new MessageInteraction(Translation.FailedEmailSubmissionErrorMessage, Translation.FailedEmailSubmissionErrorTitle, MessageOptions.Ok, MessageIcon.Exclamation);
                _interactionRequest.Raise(messageInteraction);
            }
        }
        else
        {
            IsSending = false;
            IsEmailInvalid = true;
        }
    }

    private bool SendEmailInformationCommandCanExecute()
    {
        return !string.IsNullOrWhiteSpace(EmailAddress);
    }

    private void CancelExecute()
    {
        InvokeStepFinished();
    }
}
