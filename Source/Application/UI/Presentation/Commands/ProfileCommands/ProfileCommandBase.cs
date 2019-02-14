using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System.Collections.ObjectModel;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.ProfileCommands
{
    public abstract class ProfileCommandBase : TranslatableViewModelBase<ProfileMangementTranslation>
    {
        protected readonly IInteractionRequest InteractionRequest;
        protected readonly ICurrentSettingsProvider CurrentSettingsProvider;
        protected readonly ICurrentSettings<ObservableCollection<ConversionProfile>> _profilesProvider;

        protected ProfileCommandBase(IInteractionRequest interactionRequest,
            ICurrentSettingsProvider currentSettingsProvider,
            ICurrentSettings<ObservableCollection<ConversionProfile>> profilesProvider,
            ITranslationUpdater translationUpdater)
            : base(translationUpdater)
        {
            InteractionRequest = interactionRequest;
            CurrentSettingsProvider = currentSettingsProvider;
            _profilesProvider = profilesProvider;
            translationUpdater.RegisterAndSetTranslation(this);
        }

        protected InputValidation ProfilenameIsValid(string profileName)
        {
            var invalidProfileMessage = Translation.InvalidProfileName;

            if (string.IsNullOrWhiteSpace(profileName))
                return new InputValidation(false, invalidProfileMessage);

            var profileNameDoesNotExist = CurrentSettingsProvider.GetProfileByName(profileName) == null;

            return new InputValidation(profileNameDoesNotExist,
                profileNameDoesNotExist ? null : invalidProfileMessage);
        }
    }
}
