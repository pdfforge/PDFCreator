using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.ProfileCommands
{
    public class ProfileAddCommand : ProfileCommandBase, ICommand
    {
        public ProfileAddCommand(
            IInteractionRequest interactionRequest,
            ICurrentSettings<ObservableCollection<ConversionProfile>> profilesProvider,
            ICurrentSettingsProvider currentSettingsProvider,
            ITranslationUpdater translationUpdater)
            : base(
                  interactionRequest,
                  currentSettingsProvider,
                  profilesProvider,
                  translationUpdater)
        {
        }

        public void Execute(Object obj)
        {
            var title = Translation.AddNewProfile;
            var questionText = Translation.EnterProfileName;

            var inputInteraction = new InputInteraction(title, questionText, ProfilenameIsValid);

            inputInteraction.InputText = Translation.NewProfile;

            InteractionRequest.Raise(inputInteraction, AddProfileCallback);
        }

        private void AddProfileCallback(InputInteraction interaction)
        {
            if (!interaction.Success)
                return;

            var name = interaction.InputText;

            var newProfile = CurrentSettingsProvider.SelectedProfile.Copy();
            newProfile.Guid = Guid.NewGuid().ToString();
            newProfile.Name = name;
            newProfile.Properties.Deletable = true;
            newProfile.Properties.Renamable = true;
            newProfile.Properties.IsShared = false;

            _profilesProvider.Settings.Add(newProfile);
            CurrentSettingsProvider.SelectedProfile = newProfile;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

#pragma warning disable CS0067

        public event EventHandler CanExecuteChanged;

#pragma warning restore CS0067
    }
}
