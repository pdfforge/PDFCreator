using System;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeJobStepPasswordViewModel : JobStepPasswordViewModelBase<PasswordButtonControlTranslation>
    {
        public DesignTimeJobStepPasswordViewModel() : base(new DesignTimeTranslationUpdater())
        {
        }

        protected override void DisableAction()
        {
            throw new NotImplementedException();
        }

        protected override void InitializeWorkflowStep()
        {
            throw new NotImplementedException();
        }

        protected override void StorePasswordsInJobPasswords()
        {
            throw new NotImplementedException();
        }

        protected override bool ContinueCanExecute(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
