using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.TitleReplacementSettings;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.TitleReplacements
{
    public class TitleReplacementEditCommand : ICommand
    {
        private readonly IInteractionRequest _interactionRequest;
        private readonly ICurrentSettings<ObservableCollection<TitleReplacement>> _settingsProvider;

        public TitleReplacementEditCommand(IInteractionRequest interactionRequest, ICurrentSettings<ObservableCollection<TitleReplacement>> settingsProvider)
        {
            _interactionRequest = interactionRequest;
            _settingsProvider = settingsProvider;
        }

        public bool CanExecute(object parameter)
        {
            return parameter is TitleReplacement;
        }

        public void Execute(object parameter)
        {
            var editTitleReplacementInteraction = new TitleReplacementEditInteraction(parameter as TitleReplacement);
            _interactionRequest.Raise(editTitleReplacementInteraction, EditInteractionCallback);
        }

        private void EditInteractionCallback(TitleReplacementEditInteraction obj)
        {
            if (obj.Success)
            {
                //todo find a better solution to trigger PropertyChanged of the ObservableCollection
                var titleReplacementList = _settingsProvider.Settings;
                titleReplacementList.Remove(obj.Replacement);
                titleReplacementList.Add(obj.Replacement);
            }
        }

#pragma warning disable CS0067

        public event EventHandler CanExecuteChanged;

#pragma warning restore CS0067
    }
}
