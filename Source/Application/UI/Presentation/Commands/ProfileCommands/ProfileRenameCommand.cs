using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using pdfforge.PDFCreator.UI.Presentation.Events;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.ProfileCommands
{
    public class ProfileRenameCommand : ProfileCommandBase, ICommand
    {
        private readonly IDispatcher _dispatcher;
        private readonly IEventAggregator _eventAggregator;
        public ProfileRenameCommand(
            IInteractionRequest interactionRequest,
            ICurrentSettings<ObservableCollection<ConversionProfile>> profilesProvider,
            ICurrentSettingsProvider currentSettingsProvider,
            ITranslationUpdater translationUpdater,
            IDispatcher dispatcher,IEventAggregator eventAggregator)
            : base(interactionRequest, currentSettingsProvider, profilesProvider, translationUpdater)
        {
            _dispatcher = dispatcher;
            _eventAggregator = eventAggregator;
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

            var inputInteraction = new InputInteraction(title, questionText, ProfileNameIsValidOrUnchanged);
            inputInteraction.InputText = CurrentSettingsProvider.SelectedProfile.Name;

            InteractionRequest.Raise(inputInteraction, RenameProfileCallback);
        }

        private InputValidation ProfileNameIsValidOrUnchanged(string profileName)
        {
            if (CurrentSettingsProvider.SelectedProfile.Name == profileName)
                return new InputValidation(true);

            return ProfileNameIsValid(profileName);
        }

        private void RenameProfileCallback(InputInteraction interaction)
        {
            if (!interaction.Success)
                return;

            var newname = interaction.InputText;
            CurrentSettingsProvider.SelectedProfile.Name = newname;
            
            _eventAggregator.GetEvent<ProfileRenamedEvent>().Publish();
        }

        public bool CanExecute(object parameter)
        {
            var currentProfile = CurrentSettingsProvider.SelectedProfile;
            return currentProfile != null && currentProfile.Properties.Renamable;
        }

        public event EventHandler CanExecuteChanged;
    }
}
