using pdfforge.PDFCreator.Conversion.Settings.Workflow;
using System;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.WorkflowEditor
{
    public class RemoveActionCommand : ICommand
    {
        private readonly ISelectedProfileProvider _selectedProfileProvider;

        public RemoveActionCommand(ISelectedProfileProvider selectedProfileProvider)
        {
            _selectedProfileProvider = selectedProfileProvider;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var facade = (IPresenterActionFacade)parameter;
            facade.IsEnabled = false;
            _selectedProfileProvider.SelectedProfile.ActionOrder.RemoveAll(x => x == facade.SettingsType.Name);
        }

#pragma warning disable CS0067

        public event EventHandler CanExecuteChanged;

#pragma warning restore CS0067
    }
}
