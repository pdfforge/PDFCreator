using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Commands.TitleReplacements;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.TitleReplacementSettings;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using System.Collections.ObjectModel;

namespace Presentation.UnitTest.Commands.TitleReplacements
{
    [TestFixture]
    public class TitleReplacementsAddCommandUnitTest
    {
        private ICurrentSettings<ObservableCollection<TitleReplacement>> _settingsProvider;
        private UnitTestInteractionRequest _interactionRequest;
        private TitleReplacementAddCommand _command;
        private PdfCreatorSettings _settings;
        private ApplicationSettings _applicationSettings;
        private ObservableCollection<TitleReplacement> _titleReplacements;

        [SetUp]
        public void Setup()
        {
            _settings = new PdfCreatorSettings();
            _applicationSettings = new ApplicationSettings();
            _settings.ApplicationSettings = _applicationSettings;
            _titleReplacements = new ObservableCollection<TitleReplacement>();
            _applicationSettings.TitleReplacement = _titleReplacements;
            _settingsProvider = Substitute.For<ICurrentSettings<ObservableCollection<TitleReplacement>>>();
            _settingsProvider.Settings.Returns(_titleReplacements);

            _interactionRequest = new UnitTestInteractionRequest();

            _command = new TitleReplacementAddCommand(_interactionRequest, _settingsProvider);
        }

        [Test]
        public void TestProperites()
        {
            Assert.NotNull(_interactionRequest);
            Assert.NotNull(_settingsProvider);
        }

        [Test]
        public void TestCanExecute_IsAlwaysTrue()
        {
            Assert.IsTrue(_command.CanExecute(null));
        }

        [Test]
        public void Execute_RunSuccessfulInteractionWithTitleReplacement()
        {
            _interactionRequest.RegisterInteractionHandler<TitleReplacementEditInteraction>(i =>
            {
                i.Success = true; //User cancels
            });

            _command.Execute(null);

            var titleReplacementEditInteraction = _interactionRequest.AssertWasRaised<TitleReplacementEditInteraction>();
            Assert.IsTrue(string.IsNullOrEmpty(titleReplacementEditInteraction.Replacement.Search));
            Assert.IsTrue(string.IsNullOrEmpty(titleReplacementEditInteraction.Replacement.Replace));
            Assert.AreEqual(ReplacementType.Replace, titleReplacementEditInteraction.Replacement.ReplacementType);
            Assert.Greater(_settingsProvider.Settings.Count, 0);
        }

        [Test]
        public void Execute_RunFailedInteractionWithTitleReplacement()
        {
            _interactionRequest.RegisterInteractionHandler<TitleReplacementEditInteraction>(i =>
            {
                i.Success = false; //User cancels
            });

            _command.Execute(null);

            var titleReplacementEditInteraction = _interactionRequest.AssertWasRaised<TitleReplacementEditInteraction>();
            Assert.IsTrue(string.IsNullOrEmpty(titleReplacementEditInteraction.Replacement.Search));
            Assert.IsTrue(string.IsNullOrEmpty(titleReplacementEditInteraction.Replacement.Replace));
            Assert.AreEqual(ReplacementType.Replace, titleReplacementEditInteraction.Replacement.ReplacementType);
            Assert.AreEqual(_settingsProvider.Settings.Count, 0);
        }
    }
}
