using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using System;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public interface ISaveChangedSettingsCommand : ICommand
    {
    }

    public class SaveChangedSettingsCommand : ISaveChangedSettingsCommand
    {
        private readonly ICurrentSettingsProvider _currentSettingsProvider;
        private readonly ISettingsManager _settingsManager;
        private readonly ISettingsChanged _settingsChanged;

        public SaveChangedSettingsCommand(ICurrentSettingsProvider currentSettingsProvider,
            ISettingsManager settingsManager, ISettingsChanged settingsChanged)
        {
            _currentSettingsProvider = currentSettingsProvider;
            _settingsManager = settingsManager;
            _settingsChanged = settingsChanged;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (_settingsChanged.HaveChanged())
            {
                _currentSettingsProvider.StoreCurrentSettings();
                _settingsManager.SaveCurrentSettings();
            }
        }

#pragma warning disable CS0067

        public event EventHandler CanExecuteChanged;

#pragma warning restore CS0067
    }
}
