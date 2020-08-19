using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Commands.UserGuide;
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
        private readonly EditionHelper _editionHelper;
        private ICommand _showLicenseCommand;
        public ApplicationNameProvider ApplicationNameProvider { get; }

        public AboutViewModel(
            IVersionHelper versionHelper,
            ITranslationUpdater translationUpdater,
            ICommandLocator commandLocator,
            ApplicationNameProvider applicationNameProvider,
            EditionHelper editionHelper)
            : base(translationUpdater)
        {
            _commandLocator = commandLocator;
            _editionHelper = editionHelper;
            ApplicationNameProvider = applicationNameProvider;
            VersionText = versionHelper.FormatWithBuildNumber();

            ShowManualCommand = _commandLocator.GetInitializedCommand<ShowUserGuideCommand, HelpTopic>(HelpTopic.General);
            _showLicenseCommand = _commandLocator.GetInitializedCommand<ShowUserGuideCommand, HelpTopic>(HelpTopic.License);

            PdfforgeWebsiteCommand = _commandLocator.GetInitializedCommand<UrlOpenCommand, string>(Urls.PdfforgeWebsiteUrl);
            PrioritySupportCommand = _commandLocator.GetCommand<IPrioritySupportUrlOpenCommand>();
        }

        public void SwitchLicenseCommand(HelpTopic topic)
        {
            ShowLicenseCommand = _commandLocator.GetInitializedCommand<ShowUserGuideCommand, HelpTopic>(topic);
        }

        public bool IsBusinessEdition => !_editionHelper.IsFreeEdition;

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
    }
}
