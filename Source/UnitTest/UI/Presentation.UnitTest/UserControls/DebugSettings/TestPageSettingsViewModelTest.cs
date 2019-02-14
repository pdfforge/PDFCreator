using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using Translatable;

namespace Presentation.UnitTest.UserControls.DebugSettings
{
    [TestFixture]
    public class TestPageSettingsViewModelTest
    {
        [SetUp]
        public void Setup()
        {
            _translationUpdater = Substitute.For<ITranslationUpdater>();
            _translationUpdater
                .When(x => x.RegisterAndSetTranslation(Arg.Any<ITranslatableViewModel<DebugSettingsTranslation>>()))
                .Do(x =>
                {
                    var viewModel = x.Arg<ITranslatableViewModel<DebugSettingsTranslation>>();
                    viewModel.Translation = new TranslationFactory().CreateTranslation<DebugSettingsTranslation>();
                });

            _settingsProvider = Substitute.For<ICurrentSettings<CreatorAppSettings>>();
            _settingsProvider.Settings.Returns(new CreatorAppSettings());
            _applicationSettingsProvider = Substitute.For<ICurrentSettings<ApplicationSettings>>();
            var applicationSettings = new ApplicationSettings();
            applicationSettings.LoggingLevel = LoggingLevel.Debug;
            _applicationSettingsProvider.Settings.Returns(applicationSettings);
            _gpoSettings = Substitute.For<IGpoSettings>();
            _testPageHelper = Substitute.For<ITestPageHelper>();
            _printHelper = Substitute.For<IPrinterHelper>();
        }

        private ITranslationUpdater _translationUpdater;
        private IGpoSettings _gpoSettings;
        private ITestPageHelper _testPageHelper;
        private IPrinterHelper _printHelper;
        private ICurrentSettings<CreatorAppSettings> _settingsProvider;
        private ICurrentSettings<ApplicationSettings> _applicationSettingsProvider;

        private TestPageSettingsViewModel BuildViewModel()
        {
            return new TestPageSettingsViewModel(_testPageHelper, _settingsProvider, _applicationSettingsProvider, _printHelper, _translationUpdater, _gpoSettings);
        }

        [Test]
        public void Check_Properties()
        {
            var viewModel = BuildViewModel();

            Assert.NotNull(viewModel.PrintPdfCreatorTestpageCommand);
            Assert.NotNull(viewModel.PrintWindowsTestpageCommand);
            Assert.NotNull(viewModel.Translation);
        }

        private TestPageSettingsViewModel SetupPdfCreatorTestPagePreperations()
        {
            var viewModel = BuildViewModel();

            return viewModel;
        }

        [Test]
        public void RequestTestPage_CreatesPage()
        {
            var viewModel = SetupPdfCreatorTestPagePreperations();

            var wasCalled = false;
            _testPageHelper
                .When(x => _testPageHelper.CreateTestPage())
                .Do(x => wasCalled = true);

            viewModel.PrintPdfCreatorTestpageCommand.Execute(null);

            Assert.IsTrue(wasCalled);
        }
    }
}
