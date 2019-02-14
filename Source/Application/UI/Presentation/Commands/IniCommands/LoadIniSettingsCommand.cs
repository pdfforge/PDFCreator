using pdfforge.PDFCreator.UI.Presentation.Assistants;
using System;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.IniCommands
{
    public class LoadIniSettingsCommand : ICommand
    {
        private readonly IIniSettingsAssistant _iniSettingsAssistant;

        public LoadIniSettingsCommand(
            IIniSettingsAssistant iniSettingsAssistant)
        {
            _iniSettingsAssistant = iniSettingsAssistant;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _iniSettingsAssistant.LoadIniSettings();
        }

#pragma warning disable 67

        public event EventHandler CanExecuteChanged;

#pragma warning restore 67
    }
}
