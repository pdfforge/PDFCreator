using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.SettingsManagement.GPO.Settings;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Commands.UserGuide;
using pdfforge.PDFCreator.UI.Presentation.Events;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls
{
    public class AboutViewModel : TranslatableViewModelBase<AboutViewTranslation>
    {
        private readonly ICommandLocator _commandLocator;
        private ICommand _showLicenseCommand;
        public ApplicationNameProvider ApplicationNameProvider { get; }
        private readonly IGpoSettings _gpoSettings;

        public AboutViewModel(
            IVersionHelper versionHelper,
            ITranslationUpdater translationUpdater,
            ICommandLocator commandLocator,
            ApplicationNameProvider applicationNameProvider,
            IGpoSettings gpoSettings)
            : base(translationUpdater)
        {
            _commandLocator = commandLocator;
            ApplicationNameProvider = applicationNameProvider;
            VersionText = versionHelper.FormatWithBuildNumber();

            ShowManualCommand = _commandLocator.GetInitializedCommand<ShowUserGuideCommand, HelpTopic>(HelpTopic.General);
            _showLicenseCommand = _commandLocator.GetInitializedCommand<ShowUserGuideCommand, HelpTopic>(HelpTopic.License);

            PdfforgeWebsiteCommand = _commandLocator.GetInitializedCommand<UrlOpenCommand, string>(Urls.PdfforgeWebsiteUrl);
            PrioritySupportCommand = _commandLocator.GetCommand<IPrioritySupportUrlOpenCommand>();

            FeedbackCommand = _commandLocator.GetCommand<FeedbackCommand>();
            _gpoSettings = gpoSettings;
        }

        public void SwitchLicenseCommand(HelpTopic topic)
        {
            ShowLicenseCommand = _commandLocator.GetInitializedCommand<ShowUserGuideCommand, HelpTopic>(topic);
        }

        public ICommand PrioritySupportCommand { get; }
        public string VersionText { get; }

        public ICommand ShowManualCommand { get; }

        public ICommand ShowLicenseCommand
        {
            get => _showLicenseCommand;
            set
            {
                _showLicenseCommand = value;
                RaisePropertyChanged(nameof(ShowLicenseCommand));
            }
        }

        public ICommand PdfforgeWebsiteCommand { get; }
        public ICommand FeedbackCommand { get; }

        public bool HideFeedbackForm => _gpoSettings.HideFeedbackForm;
    }
}
