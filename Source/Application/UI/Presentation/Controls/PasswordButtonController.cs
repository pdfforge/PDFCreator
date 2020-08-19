using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;

namespace pdfforge.PDFCreator.UI.Presentation.Controls
{
    public class PasswordButtonController
    {
        private readonly IPasswordButtonViewModel _buttonViewModel;
        public PrintJobPasswordButtonViewModel PrintJobPasswordButtonViewModel { get; }

        private string _password;

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                UpdateCanOkExecute();
            }
        }

        public PasswordButtonController(ITranslationUpdater translationUpdater, IPasswordButtonViewModel buttonViewModel, bool allowSkip, bool allowRemove)
        {
            _buttonViewModel = buttonViewModel;
            _password = "";

            PrintJobPasswordButtonViewModel = new PrintJobPasswordButtonViewModel(translationUpdater)
            {
                AllowSkip = allowSkip,
                AllowRemove = allowRemove,
                OkCommand = new DelegateCommand(OkExecute, OkCanExecute),
                SkipCommand = new DelegateCommand(SkipExecute),
                CancelCommand = new DelegateCommand(CancelExecute),
                RemoveCommand = new DelegateCommand(RemoveExecute)
            };

            _buttonViewModel.RaiseOkCanExecuteChanged = () => PrintJobPasswordButtonViewModel.OkCommand.RaiseCanExecuteChanged();
        }

        private void RemoveExecute(object obj)
        {
            _buttonViewModel.ClearPasswordFields();
            _buttonViewModel.RemoveHook();
            Finish();
        }

        public void UpdateCanOkExecute()
        {
            PrintJobPasswordButtonViewModel.OkCommand.RaiseCanExecuteChanged();
        }

        private void CancelExecute(object obj)
        {
            _buttonViewModel.ClearPasswordFields();
            Finish();
            _buttonViewModel.CancelHook();
        }

        private void Finish()
        {
            _buttonViewModel.SetPassword(Password);
            _buttonViewModel.FinishedHook();
        }

        private void SkipExecute(object obj)
        {
            _buttonViewModel.ClearPasswordFields();
            _buttonViewModel.SkipHook();
            Finish();
        }

        private void OkExecute(object obj)
        {
            _buttonViewModel.OkHook();
            Finish();
        }

        protected bool OkCanExecute(object obj)
        {
            return _buttonViewModel.CanExecuteHook();
        }
    }
}
