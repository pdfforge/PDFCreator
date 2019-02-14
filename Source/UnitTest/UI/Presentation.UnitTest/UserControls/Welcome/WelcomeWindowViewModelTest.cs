using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Commands.UserGuide;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Welcome;
using pdfforge.PDFCreator.Utilities;
using System.Windows.Input;

namespace Presentation.UnitTest.UserControls.Welcome
{
    [TestFixture]
    public class WelcomeWindowViewModelTest
    {
        private WelcomeViewModel _welcomeViewModel;
        private ICommand _whatsNewCommand;
        private ICommand _prioritySupportCommand;
        private IVersionHelper _versionHelper;

        [SetUp]
        public void Setup()
        {
            var commandLocator = Substitute.For<ICommandLocator>();
            _whatsNewCommand = Substitute.For<ICommand>();
            commandLocator.GetInitializedCommand<ShowUserGuideCommand, HelpTopic>(HelpTopic.WhatsNew).Returns(_whatsNewCommand);
            _prioritySupportCommand = Substitute.For<ICommand>();
            _versionHelper = Substitute.For<IVersionHelper>();
            commandLocator.GetCommand<PrioritySupportUrlOpenCommand>().Returns(_prioritySupportCommand);

            _welcomeViewModel = new WelcomeViewModel(commandLocator, new DesignTimeTranslationUpdater(),
                new EditionHelper(false, false), _versionHelper, new DesignTimeApplicationNameProvider());
        }

        [Test]
        public void Commands_GetInitialized()
        {
            Assert.AreSame(_whatsNewCommand, _welcomeViewModel.WhatsNewCommand, "WhatsNewCommand");
            Assert.AreSame(_prioritySupportCommand, _welcomeViewModel.PrioritySupportCommand, "PrioritySupportCommand");
        }
    }
}
