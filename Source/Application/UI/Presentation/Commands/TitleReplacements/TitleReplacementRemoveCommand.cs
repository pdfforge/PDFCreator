using pdfforge.PDFCreator.Conversion.Settings;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.TitleReplacements
{
    public class TitleReplacementRemoveCommand : ICommand
    {
        private readonly ICurrentSettings<ObservableCollection<TitleReplacement>> _settingsProvider;

        public TitleReplacementRemoveCommand(ICurrentSettings<ObservableCollection<TitleReplacement>> settingsProvider)
        {
            _settingsProvider = settingsProvider;
        }

        public bool CanExecute(object parameter)
        {
            return parameter is TitleReplacement;
        }

        public void Execute(object parameter)
        {
            _settingsProvider.Settings.Remove(parameter as TitleReplacement);
        }

#pragma warning disable CS0067

        public event EventHandler CanExecuteChanged;

#pragma warning restore CS0067
    }
}
