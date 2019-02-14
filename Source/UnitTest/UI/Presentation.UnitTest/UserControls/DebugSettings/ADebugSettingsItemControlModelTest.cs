using NSubstitute;
using NUnit.Framework;
using pdfforge.DataStorage;
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
    public class ADebugSettingsItemControlModelTest
    {
        private IInteractionRequest _invoker;
        private ISettingsManager _settingsManager;
        private ITranslationUpdater _translationUpdater;
        private IGpoSettings _gpoSettings;
        private ISettingsProvider _simpleSettingsProvider;
        private ICurrentSettingsProvider _currentSettingsProvider;
        private IDefaultSettingsBuilder _defaultSettingsBuilder;

        [SetUp]
        public void Setup()
        {
            _invoker = Substitute.For<IInteractionRequest>();

            _gpoSettings = Substitute.For<IGpoSettings>();

            var settings = new PdfCreatorSettings();
            _simpleSettingsProvider = Substitute.For<ISettingsProvider>();
            _simpleSettingsProvider.Settings.Returns(settings);

            _settingsManager = Substitute.For<ISettingsManager>();
            _settingsManager.GetSettingsProvider().Returns(_simpleSettingsProvider);

            _currentSettingsProvider = Substitute.For<ICurrentSettingsProvider>();

            _defaultSettingsBuilder = Substitute.For<IDefaultSettingsBuilder>();
            _defaultSettingsBuilder.CreateDefaultSettings(Arg.Any<ISettings>()).Returns(new PdfCreatorSettings());
            _translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());
        }

        [TearDown]
        public void TearDown()
        {
            _translationUpdater.Clear();
        }

        public ADebugSettingsItemControlModel BuildModel()
        {
            return new RestoreSettingsViewModel(_invoker, _translationUpdater, _simpleSettingsProvider, _gpoSettings, _defaultSettingsBuilder);
        }

        [Test]
        public void GetProperties_AllPropertiesAreSet()
        {
            var viewModel = BuildModel();

            Assert.NotNull(viewModel.GpoSettings);
            Assert.NotNull(viewModel.Translation);
        }

        private void HandleMessageInteraction(Action<MessageInteraction> action)
        {
            _invoker
                .When(x => x.Raise(Arg.Any<MessageInteraction>(), Arg.Any<Action<MessageInteraction>>()))
                .Do(x =>
                {
                    var interaction = x.Arg<MessageInteraction>();
                    action(interaction);
                    interaction.Response = MessageResponse.Yes;

                    x.Arg<Action<MessageInteraction>>().Invoke(interaction);
                });
        }

        [Test]
        public void CreateNewSettings_SetNewSettings_SettingsAreSetAndEventIsCalled()
        {
            var settings = new PdfCreatorSettings();

            settings.CreatorAppSettings.PrimaryPrinter = "primaryPrinter";
            _simpleSettingsProvider.Settings.Returns(settings);

            HandleMessageInteraction(interaction => interaction.Response = MessageResponse.Yes);

            var viewModel = BuildModel();

            (viewModel as RestoreSettingsViewModel).RestoreDefaultSettingsCommand.Execute(null);

            Received.InOrder(() =>
            {
                _simpleSettingsProvider.UpdateSettings(Arg.Any<PdfCreatorSettings>());
            });
        }
    }
}
