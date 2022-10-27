using System.Windows.Input;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.SettingsManagement.DefaultSettings;
using pdfforge.PDFCreator.Core.SettingsManagementInterface;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings
{
    public abstract class RestoreSettingsViewModelBase : ADebugSettingsItemControlModel
    {
        private readonly IInteractionRequest _request;

        protected RestoreSettingsViewModelBase(IInteractionRequest request, ITranslationUpdater translationUpdater, IGpoSettings gpoSettings) :
            base(translationUpdater, gpoSettings)
        {
            _request = request;
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
                    RestoreDefaultSettings();
            });
        }

        protected abstract void RestoreDefaultSettings();
    }

    public class RestoreSettingsViewModel : RestoreSettingsViewModelBase
    {
        private readonly ISettingsProvider _settingsProvider;
        private readonly IDefaultSettingsBuilder _defaultSettingsBuilder;

        public RestoreSettingsViewModel(IInteractionRequest request, ITranslationUpdater translationUpdater, ISettingsProvider settingsProvider, IGpoSettings gpoSettings, IDefaultSettingsBuilder defaultSettingsBuilder) :
            base(request, translationUpdater, gpoSettings)
        {
            _settingsProvider = settingsProvider;
            _defaultSettingsBuilder = defaultSettingsBuilder;
        }

        protected override void RestoreDefaultSettings()
        {
            var defaultSettings = _defaultSettingsBuilder.CreateDefaultSettings(_settingsProvider.Settings);
            _settingsProvider.UpdateSettings((PdfCreatorSettings)defaultSettings);
        }
    }
}