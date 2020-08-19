using pdfforge.PDFCreator.Core.ServiceLocator;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Commands.UserGuide;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Welcome
{
    public class WelcomeViewModel : TranslatableViewModelBase<WelcomeWindowTranslation>, IWhitelisted
    {
        private readonly EditionHelper _editionHelper;
        private readonly IVersionHelper _versionHelper;
        private readonly ApplicationNameProvider _applicationNameProvider;
        private string EditionWithVersion => _applicationNameProvider?.EditionName + " " + _versionHelper?.FormatWithThreeDigits();

        public WelcomeViewModel(ICommandLocator commandLocator, ITranslationUpdater translationUpdater,
                                EditionHelper editionHelper, IVersionHelper versionHelper, ApplicationNameProvider applicationNameProvider)
            : base(translationUpdater)
        {
            _editionHelper = editionHelper;
            _versionHelper = versionHelper;
            _applicationNameProvider = applicationNameProvider;

            WhatsNewCommand = commandLocator.GetInitializedCommand<ShowUserGuideCommand, HelpTopic>(HelpTopic.WhatsNew);
            PrioritySupportCommand = commandLocator.GetCommand<IPrioritySupportUrlOpenCommand>();
        }

        protected override void OnTranslationChanged()
        {
            RaisePropertyChanged(nameof(WelcomeText));
        }

        public bool AllowPrioritySupport => !_editionHelper.IsFreeEdition;

        public ICommand WhatsNewCommand { get; }

        public ICommand PrioritySupportCommand { get; }

        public string WelcomeText => Translation?.GetWelcomeText(EditionWithVersion);
    }
}
