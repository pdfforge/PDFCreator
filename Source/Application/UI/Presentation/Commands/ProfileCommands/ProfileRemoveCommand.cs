using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.ProfileCommands
{
    public class ProfileRemoveCommand : ProfileCommandBase, ICommand
    {
        private readonly ICurrentSettings<ObservableCollection<PrinterMapping>> _printerMappingProvider;
        private readonly IDispatcher _dispatcher;

        public ProfileRemoveCommand(
            IInteractionRequest interactionRequest,
            ICurrentSettings<ObservableCollection<ConversionProfile>> profileProvider,
            ICurrentSettings<ObservableCollection<PrinterMapping>> printerMappingProvider,
            ICurrentSettingsProvider currentSettingsProvider,
            ITranslationUpdater translationUpdater,
            IDispatcher dispatcher)
            : base(interactionRequest, currentSettingsProvider, profileProvider, translationUpdater)
        {
            _printerMappingProvider = printerMappingProvider;
            _dispatcher = dispatcher;

            CurrentSettingsProvider.SelectedProfileChanged += CurrentSettingsProviderOnSelectedProfileChanged;
        }

        private void CurrentSettingsProviderOnSelectedProfileChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            _dispatcher.BeginInvoke(() => CanExecuteChanged?.Invoke(this, EventArgs.Empty));
        }

        private List<PrinterMapping> _usedPrintersMappings;

        public void Execute(object parameter)
        {
            var currentId = CurrentSettingsProvider.SelectedProfile.Guid;
            var printerMappings = _printerMappingProvider.Settings;
            _usedPrintersMappings = printerMappings.Where(pm => pm.ProfileGuid.Equals(currentId)).ToList();

            var title = Translation.RemoveProfile;

            var sb = new StringBuilder();
            sb.AppendLine(CurrentSettingsProvider.SelectedProfile.Name);
            sb.AppendLine(Translation.RemoveProfileForSure);
            if (_usedPrintersMappings.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine(Translation.GetProfileIsMappedToMessage(_usedPrintersMappings.Count));
                foreach (var pm in _usedPrintersMappings)
                {
                    sb.AppendLine(pm.PrinterName);
                }
                sb.AppendLine();
                sb.AppendLine(Translation.GetPrinterWillBeMappedToMessage(_usedPrintersMappings.Count));
            }
            var message = sb.ToString();
            var icon = _usedPrintersMappings.Count > 0 ? MessageIcon.Warning : MessageIcon.Question;
            var interaction = new MessageInteraction(message, title, MessageOptions.YesNo, icon);
            InteractionRequest.Raise(interaction, RemoveProfileCallback);
        }

        private void RemoveProfileCallback(MessageInteraction interaction)
        {
            if (interaction.Response != MessageResponse.Yes)
                return;

            _profilesProvider.Settings.Remove(CurrentSettingsProvider.SelectedProfile);

            foreach (var pm in _usedPrintersMappings)
            {
                pm.ProfileGuid = ProfileGuids.DEFAULT_PROFILE_GUID;
            }
        }

        public bool CanExecute(object parameter)
        {
            var currentProfile = CurrentSettingsProvider.SelectedProfile;
            return currentProfile != null && currentProfile.Properties.Deletable;
        }

        public event EventHandler CanExecuteChanged;
    }
}
