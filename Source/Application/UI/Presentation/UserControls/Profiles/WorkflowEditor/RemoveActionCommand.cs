using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Workflow;
using pdfforge.PDFCreator.Core.SettingsManagement;
using System;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.WorkflowEditor
{
    public class RemoveActionCommand : ICommand
    {
        private readonly ISelectedProfileProvider _selectedProfileProvider;
        private readonly IDefaultSettingsBuilder _defaultSettingsBuilder;

        public RemoveActionCommand(ISelectedProfileProvider selectedProfileProvider, IDefaultSettingsBuilder defaultSettingsBuilder)
        {
            _selectedProfileProvider = selectedProfileProvider;
            _defaultSettingsBuilder = defaultSettingsBuilder;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var facade = (IPresenterActionFacade)parameter;
            var settingsType = facade.SettingsType;

            var defaultProfile = _defaultSettingsBuilder.CreateDefaultProfile();
            var defaultSetting = facade.GetProfileSettingByConversionProfile(defaultProfile);
            var currentSetting = facade.GetProfileSettingByConversionProfile(_selectedProfileProvider.SelectedProfile);

            var replaceWithInfo = settingsType.GetMethod(nameof(ApplicationSettings.ReplaceWith));
            if (replaceWithInfo != null)
                replaceWithInfo.Invoke(currentSetting, new object[] { defaultSetting });

            // Important: Set enabled to false after resetting to defaults
            facade.IsEnabled = false;

            _selectedProfileProvider.SelectedProfile.ActionOrder.RemoveAll(x => x == facade.SettingsType.Name);
        }

#pragma warning disable CS0067

        public event EventHandler CanExecuteChanged;

#pragma warning restore CS0067
    }
}
