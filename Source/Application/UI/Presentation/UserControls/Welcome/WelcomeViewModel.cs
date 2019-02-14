using pdfforge.PDFCreator.Core.ServiceLocator;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Commands.UserGuide;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Welcome
{
    public class WelcomeViewModel : TranslatableViewModelBase<WelcomeWindowTranslation>, IWhitelisted
    {
        private readonly IVersionHelper _versionHelper;
        private readonly ApplicationNameProvider _applicationNameProvider;
        private string _editionWithVersion => _applicationNameProvider?.EditionName + " " + _versionHelper?.FormatWithThreeDigits();

        public WelcomeViewModel(ICommandLocator commandLocator, ITranslationUpdater translationUpdater,
                                EditionHelper editionHelper, IVersionHelper versionHelper, ApplicationNameProvider applicationNameProvider)
            : base(translationUpdater)
        {
            _versionHelper = versionHelper;
            _applicationNameProvider = applicationNameProvider;
            AllowPrioritySupport = !editionHelper?.ShowOnlyForPlusAndBusiness ?? true;

            WhatsNewCommand = commandLocator.GetInitializedCommand<ShowUserGuideCommand, HelpTopic>(HelpTopic.WhatsNew);
            PrioritySupportCommand = commandLocator.GetCommand<PrioritySupportUrlOpenCommand>();
        }

        protected override void OnTranslationChanged()
        {
            RaisePropertyChanged(nameof(WelcomeText));
        }

        public Boolean AllowPrioritySupport { get; }

        public ICommand WhatsNewCommand { get; }

        public ICommand PrioritySupportCommand { get; }

        public string WelcomeText => Translation?.GetWelcomeText(_editionWithVersion);
    }
}
