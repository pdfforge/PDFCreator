using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System;
using System.Collections.ObjectModel;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.ProfileCommands
{
    public class ProfileAddCommand : ProfileCommandBase, IWaitableCommand
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

        public void Execute(object parameter)
        {
            var title = Translation.AddNewProfile;
            var questionText = Translation.EnterProfileName;

            var inputInteraction = new InputInteraction(title, questionText, ProfilenameIsValid);

            inputInteraction.InputText = Translation.NewProfile;

            InteractionRequest.Raise(inputInteraction, AddProfileCallback);
        }

        private void AddProfileCallback(InputInteraction interaction)
        {
            if (interaction.Success)
            {
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

            var result = interaction.Success
                ? ResponseStatus.Success
                : ResponseStatus.Cancel;

            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(result));
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;

#pragma warning disable CS0067

        public event EventHandler CanExecuteChanged;

#pragma warning restore CS0067
    }
}
