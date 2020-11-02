using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public class SmtpJobStepPasswordViewModel : JobStepPasswordViewModelBase<MailTranslation>
    {
        public string SmtpAccountInfo { get; set; }

        public SmtpJobStepPasswordViewModel(ITranslationUpdater translationUpdater) : base(translationUpdater, nameof(JobPasswords.SmtpPassword))
        {
        }

        protected override void ExecuteWorkflow()
        {
            SmtpAccountInfo = Job.Accounts.GetSmtpAccount(Job.Profile)?.AccountInfo;
            RaisePropertyChanged(nameof(SmtpAccountInfo));
        }

        public override void SkipHook()
        {
            Job.Profile.EmailSmtpSettings.Enabled = false;
        }

        protected override string GetCancelErrorMessage()
        {
            return "User cancelled in the SmtpPasswordView";
        }
    }
}
