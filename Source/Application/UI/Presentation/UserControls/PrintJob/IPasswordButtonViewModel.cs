using pdfforge.PDFCreator.UI.Presentation.Controls;
using System;
using System.ComponentModel;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public interface IPasswordButtonViewModel : INotifyPropertyChanged
    {
        PasswordButtonController PasswordButtonController { get; }
        Action RaiseOkCanExecuteChanged { get; set; }
        string Password { get; set; }

        void ClearPasswordFields();

        void FinishedHook();

        void SetPassword(string password);

        void SkipHook();

        void CancelHook();

        void OkHook();

        void RemoveHook();

        bool CanExecuteHook();
    }
}
