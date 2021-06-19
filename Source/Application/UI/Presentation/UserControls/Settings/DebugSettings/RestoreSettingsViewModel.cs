using System.Windows.Input;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.SettingsManagement.DefaultSettings;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings
{
    public class RestoreSettingsViewModel : ADebugSettingsItemControlModel
    {
        private readonly IInteractionRequest _request;
        private readonly ISettingsProvider _settingsProvider;
        private readonly IDefaultSettingsBuilder _defaultSettingsBuilder;

        public RestoreSettingsViewModel(IInteractionRequest request, ITranslationUpdater translationUpdater, ISettingsProvider settingsProvider, IGpoSettings gpoSettings, IDefaultSettingsBuilder defaultSettingsBuilder) :
            base( translationUpdater, gpoSettings)
        {
            _request = request;
            _settingsProvider = settingsProvider;
            _defaultSettingsBuilder = defaultSettingsBuilder;
            RestoreDefaultSettingsCommand = new DelegateCommand(RestoreDefaultSettingsExecute);
        }

        public ICommand RestoreDefaultSettingsCommand { get; }

        private void RestoreDefaultSettingsExecute(object obj)
        {
            var title = Translation.RestoreDefaultSettingsTitle;
            var message = Translation.RestoreDefaultSettingsMessage;
            var messageInteraction = new MessageInteraction(message, title, MessageOptions.YesNo, MessageIcon.Question);
            _request.Raise(messageInteraction, interaction =>
            {
                if (messageInteraction.Response == MessageResponse.Yes)
                {
                    var profileBuilder = _defaultSettingsBuilder;
                    var defaultSettings = profileBuilder.CreateDefaultSettings(_settingsProvider.Settings);
                    _settingsProvider.UpdateSettings((PdfCreatorSettings)defaultSettings);
                }
            });
        }
    }
}