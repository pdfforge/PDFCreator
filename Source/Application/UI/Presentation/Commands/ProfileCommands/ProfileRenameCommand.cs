using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.ProfileCommands
{
    public class ProfileRenameCommand : ProfileCommandBase, ICommand
    {
        private readonly IDispatcher _dispatcher;

        public ProfileRenameCommand(
            IInteractionRequest interactionRequest,
            ICurrentSettings<ObservableCollection<ConversionProfile>> profilesProvider,
            ICurrentSettingsProvider currentSettingsProvider,
            ITranslationUpdater translationUpdater,
            IDispatcher dispatcher)
            : base(interactionRequest, currentSettingsProvider, profilesProvider, translationUpdater)
        {
            _dispatcher = dispatcher;
            CurrentSettingsProvider.SelectedProfileChanged += CurrentSettingsProviderOnSelectedProfileChanged;
        }

        private void CurrentSettingsProviderOnSelectedProfileChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            _dispatcher?.BeginInvoke(() => CanExecuteChanged?.Invoke(this, new EventArgs()));
        }

        public void Execute(object parameter)
        {
            var title = Translation.RenameProfile;
            var questionText = Translation.EnterNewProfileName;

            var inputInteraction = new InputInteraction(title, questionText, ProfilenameIsValid);
            inputInteraction.InputText = CurrentSettingsProvider.SelectedProfile.Name;

            InteractionRequest.Raise(inputInteraction, RenameProfileCallback);
        }

        private void RenameProfileCallback(InputInteraction interaction)
        {
            if (!interaction.Success)
                return;

            var newname = interaction.InputText;
            CurrentSettingsProvider.SelectedProfile.Name = newname;
        }

        public bool CanExecute(object parameter)
        {
            var currentProfile = CurrentSettingsProvider.SelectedProfile;
            return currentProfile != null && currentProfile.Properties.Renamable;
        }

        public event EventHandler CanExecuteChanged;
    }
}
