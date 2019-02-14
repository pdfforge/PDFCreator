using NSubstitute;
using NUnit.Framework;
using pdfforge.DataStorage.Storage;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings;
using pdfforge.PDFCreator.Utilities.Threading;
using System;
using Translatable;

namespace Presentation.UnitTest.UserControls.DebugSettings
{
    [TestFixture]
    public class RestoreSettingsViewModelTest
    {
        private IInteractionRequest _invoker;
        private ISettingsManager _settingsManager;
        private ITranslationUpdater _translationUpdater;
        private ICurrentSettingsProvider _currentSettingsProvider;
        private IGpoSettings _gpoSettings;
        private ISettingsProvider _settingsProvider;
        private IDefaultSettingsBuilder _defaultSettingsBuilder;

        [SetUp]
        public void Setup()
        {
            _invoker = Substitute.For<IInteractionRequest>();

            IStorage storage = Substitute.For<IStorage>();
            var pdfCreatorSettings = new PdfCreatorSettings();

            _currentSettingsProvider = Substitute.For<ICurrentSettingsProvider>();

            _gpoSettings = Substitute.For<IGpoSettings>();

            _settingsProvider = Substitute.For<ISettingsProvider>();
            _settingsProvider.Settings.Returns(pdfCreatorSettings);
            _settingsManager = Substitute.For<ISettingsManager>();
            _settingsManager.GetSettingsProvider().Returns(_settingsProvider);

            _defaultSettingsBuilder = Substitute.For<IDefaultSettingsBuilder>();

            _translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());
        }

        [TearDown]
        public void TearDown()
        {
            _translationUpdater.Clear();
        }

        public RestoreSettingsViewModel BuildModel()
        {
            return new RestoreSettingsViewModel(_invoker, _translationUpdater, _settingsProvider, _gpoSettings, _defaultSettingsBuilder);
        }

        [Test]
        public void GetProperties_AllPropertiesAreSet()
        {
            var viewModel = BuildModel();

            Assert.NotNull(viewModel.RestoreDefaultSettingsCommand);
        }

        private void HandleMessageInteraction(Action<MessageInteraction> action)
        {
            _invoker
                .When(x => x.Raise(Arg.Any<MessageInteraction>()))
                .Do(x => action(x.Arg<MessageInteraction>()));
        }

        [Test]
        public void RestoreDefaultSettings_FailInteraction_DoNotChangeSettings()
        {
            IStorage storage = Substitute.For<IStorage>();
            var settings = new PdfCreatorSettings() { CreatorAppSettings = { PrimaryPrinter = "primaryPrinter" } };

            _settingsProvider.Settings.Returns(settings);

            HandleMessageInteraction(interaction => interaction.Response = MessageResponse.No);

            var viewModel = BuildModel();

            var wasSettingsLoadedCalled = false;

            //viewModel.SettingsLoaded += (sender, args) => wasSettingsLoadedCalled = true;

            viewModel.RestoreDefaultSettingsCommand.Execute(null);

            Assert.IsFalse(wasSettingsLoadedCalled);
        }
    }
}
