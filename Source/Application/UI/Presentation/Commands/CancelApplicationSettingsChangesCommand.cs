using System;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class CancelApplicationSettingsChangesCommand : ICommand
    {
        private readonly ICurrentSettingsProvider _currentSettingsProvider;

        public CancelApplicationSettingsChangesCommand(ICurrentSettingsProvider currentSettingsProvider)
        {
            _currentSettingsProvider = currentSettingsProvider;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _currentSettingsProvider.Reset(false);
        }

#pragma warning disable CS0067

        public event EventHandler CanExecuteChanged;

#pragma warning restore CS0067
    }
}
