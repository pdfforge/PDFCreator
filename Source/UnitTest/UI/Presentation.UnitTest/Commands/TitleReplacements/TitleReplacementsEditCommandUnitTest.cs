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
    public class TitleReplacementsEditCommandUnitTest
    {
        private ICurrentSettings<ObservableCollection<TitleReplacement>> _settingsProvider;
        private UnitTestInteractionRequest _interactionRequest;
        private TitleReplacementEditCommand _command;
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

            _command = new TitleReplacementEditCommand(_interactionRequest, _settingsProvider);
        }

        [Test]
        public void TestProperites()
        {
            Assert.NotNull(_interactionRequest);
            Assert.NotNull(_settingsProvider);
        }

        [TestCase(5)]
        [TestCase("sldkjf")]
        [TestCase(null)]
        public void TestCanExecute_FalseAnythingButTitleReplacement(object obj)
        {
            Assert.IsFalse(_command.CanExecute(obj));
        }

        [Test]
        public void TestCanExecute_TrueForTitleReplacement()
        {
            Assert.IsTrue(_command.CanExecute(new TitleReplacement()));
        }

        [Test]
        public void Execute_RunSuccessfulInteractionWithTitleReplacement()
        {
            _interactionRequest.RegisterInteractionHandler<TitleReplacementEditInteraction>(i =>
            {
                i.Success = true; //User cancels
            });

            var titleReplacement = new TitleReplacement();
            var titleReplacementSearch = "searchText";
            titleReplacement.Search = titleReplacementSearch;
            var titleReplacementReplace = "replaceText";
            titleReplacement.Replace = titleReplacementReplace;
            titleReplacement.ReplacementType = ReplacementType.RegEx;
            _command.Execute(titleReplacement);

            var titleReplacementEditInteraction = _interactionRequest.AssertWasRaised<TitleReplacementEditInteraction>();
            Assert.AreEqual(titleReplacementSearch, titleReplacementEditInteraction.Replacement.Search);
            Assert.AreEqual(titleReplacementReplace, titleReplacementEditInteraction.Replacement.Replace);
            Assert.AreEqual(ReplacementType.RegEx, titleReplacementEditInteraction.Replacement.ReplacementType);
        }

        [Test]
        public void Execute_RunFailedInteractionWithTitleReplacement()
        {
            _interactionRequest.RegisterInteractionHandler<TitleReplacementEditInteraction>(i =>
            {
                i.Success = false; //User cancels
            });

            var titleReplacement = new TitleReplacement();
            var titleReplacementSearch = "searchText";
            titleReplacement.Search = titleReplacementSearch;
            var titleReplacementReplace = "replaceText";
            titleReplacement.Replace = titleReplacementReplace;
            titleReplacement.ReplacementType = ReplacementType.RegEx;

            _command.Execute(titleReplacement);

            var titleReplacementEditInteraction = _interactionRequest.AssertWasRaised<TitleReplacementEditInteraction>();
            Assert.AreSame(titleReplacementEditInteraction.Replacement, titleReplacement);
        }
    }
}
