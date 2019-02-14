using NSubstitute;
using NUnit.Framework;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.Routing;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Threading;
using Prism.Events;
using System.Windows.Input;
using Translatable;

namespace Presentation.UnitTest
{
    [TestFixture]
    public class MainShellViewModelTest
    {
        private IMacroCommand _navigateMainTabCommand;
        private IEventAggregator _eventAggregator;
        private IUpdateAssistant _updateAssistant;
        private ICurrentSettings<UsageStatistics> _usageStatisticsProvider;
        private IVersionHelper _versionHelper;

        [SetUp]
        public void Setup()
        {
            _versionHelper = Substitute.For<IVersionHelper>();
            _navigateMainTabCommand = Substitute.For<IMacroCommand>();

            var macroBuilder = Substitute.For<IMacroCommandBuilder>();
            macroBuilder.AddCommand<ICommand>().Returns(macroBuilder);
            macroBuilder.Build().Returns(_navigateMainTabCommand);

            var commandLocator = Substitute.For<ICommandLocator>();
            commandLocator.CreateMacroCommand().Returns(macroBuilder);

            _eventAggregator = new EventAggregator();
            _updateAssistant = Substitute.For<IUpdateAssistant>();

            _usageStatisticsProvider = Substitute.For<ICurrentSettings<UsageStatistics>>();

            ViewModel = new MainShellViewModel(new DragAndDropEventHandler(Substitute.For<IFileConversionAssistant>()), new TranslationUpdater(new TranslationFactory(),
                new ThreadManager()), new ApplicationNameProvider("Free"), new InteractionRequest(), new EventAggregator(), commandLocator, null, null, null,
                _updateAssistant, _eventAggregator, Substitute.For<IStartupActionHandler>(), _usageStatisticsProvider, _versionHelper);
        }

        private MainShellViewModel ViewModel { get; set; }

        [Test]
        public void DoNavigate_WithUri_CallsRegionManager()
        {
            var wasRun = false;
            _navigateMainTabCommand.When(navigateMainTabCommand => navigateMainTabCommand.Execute("MyView")).Do(info => wasRun = true);
            ViewModel.NavigateCommand.Execute("MyView");

            Assert.IsTrue(wasRun);
        }

        [Test]
        public void NoViewModelExists_CreateInstance_ViewModelExists()
        {
            Assert.NotNull(ViewModel);
        }
    }
}
