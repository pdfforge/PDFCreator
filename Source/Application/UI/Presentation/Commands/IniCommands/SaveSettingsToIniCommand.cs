using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using System;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.IniCommands
{
    public class SaveSettingsToIniCommand : ICommand
    {
        private readonly IIniSettingsAssistant _iniSettingsAssistant;
        private readonly ICurrentSettings<ApplicationSettings> _appSettings;

        public SaveSettingsToIniCommand(
            IIniSettingsAssistant iniSettingsAssistant,
            ICurrentSettings<ApplicationSettings> appSettings
            )
        {
            _iniSettingsAssistant = iniSettingsAssistant;
            _appSettings = appSettings;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var removePasswords = false;
            if (parameter is bool removePasswordsParam)
                removePasswords = removePasswordsParam;

            _iniSettingsAssistant.SaveIniSettings(removePasswords);
        }

#pragma warning disable 67

        public event EventHandler CanExecuteChanged;

#pragma warning restore 67
    }
}
