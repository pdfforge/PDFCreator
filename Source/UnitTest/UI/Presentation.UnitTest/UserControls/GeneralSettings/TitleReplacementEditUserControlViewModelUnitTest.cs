using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Commands.UserGuide;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.TitleReplacementSettings;
using pdfforge.PDFCreator.Utilities.Threading;
using System.Windows.Input;
using Translatable;

namespace Presentation.UnitTest.UserControls.GeneralSettings
{
    [TestFixture]
    public class TitleReplacementEditUserControlViewModelUnitTest
    {
        [SetUp]
        public void Setup()
        {
            _replacementEditInteraction = new TitleReplacementEditInteraction(new TitleReplacement());
            _replacementEditInteraction.Replacement.Search = "SearchTest";
            _replacementEditInteraction.Replacement.Replace = "ReplacementTest";
            _replacementEditInteraction.Replacement.ReplacementType = ReplacementType.Replace;
            _finishedCalled = false;

            _translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());
            _commandLocator = Substitute.For<ICommandLocator>();

            _mockShowUserGuideCommand = Substitute.For<ICommand>();
            _commandLocator.GetInitializedCommand<ShowUserGuideCommand, HelpTopic>(HelpTopic.AppTitle).Returns(info => _mockShowUserGuideCommand);
        }

        [TearDown]
        public void TearDown()
        {
            _translationUpdater.Clear();
        }

        private ITranslationUpdater _translationUpdater;
        private ICommandLocator _commandLocator;
        private ICommand _mockShowUserGuideCommand;
        private TitleReplacementEditInteraction _replacementEditInteraction;
        private bool _finishedCalled;

        private TitleReplacementEditUserControlViewModel BuildViewModel()
        {
            var viewModel = new TitleReplacementEditUserControlViewModel(_translationUpdater, _commandLocator);
            viewModel.SetInteraction(_replacementEditInteraction);
            viewModel.FinishInteraction = () => { _finishedCalled = true; };
            return viewModel;
        }

        [TestCase(false, false, false, true, "Lore", "Ipsum", ReplacementType.RegEx)]
        [TestCase(false, false, true, false, "Lore", "Ipsum", ReplacementType.End)]
        [TestCase(false, true, false, false, "Lore", "Ipsum", ReplacementType.Start)]
        [TestCase(true, false, false, false, "Lore", "Ipsum", ReplacementType.Replace)]
        public void ChangeOuputFromInput_ExecuteOkCommand_ChangedInteractionValues(bool isAll, bool isStart, bool isEnd, bool isRegex, string search, string replace, ReplacementType replacementType)
        {
            var viewModel = BuildViewModel();
            viewModel.IsReplaceByRegex = isRegex;
            viewModel.IsRemoveAll = isAll;
            viewModel.IsRemoveAtBeginning = isStart;
            viewModel.IsRemoveAtEnd = isEnd;
            viewModel.SearchForInput = search;
            viewModel.ReplaceWithInput = replace;

            viewModel.OkCommand.Execute(null);

            Assert.IsTrue(viewModel.Interaction.Success);
            Assert.AreEqual(viewModel.Interaction.Replacement.ReplacementType, replacementType);
            Assert.AreEqual(viewModel.Interaction.Replacement.Search, search);
            Assert.AreEqual(viewModel.Interaction.Replacement.Replace, replace);
        }

        [TestCase(false, false, false, true, "Lore", "Ipsum", ReplacementType.RegEx)]
        [TestCase(false, false, true, false, "Lore", "Ipsum", ReplacementType.End)]
        [TestCase(false, true, false, false, "Lore", "Ipsum", ReplacementType.Start)]
        [TestCase(true, false, false, false, "Lore", "Ipsum", ReplacementType.Replace)]
        public void HaveOldInteractionValue_ChangeInteractionObject_ChangedValue(bool isAll, bool isStart, bool isEnd, bool isRegex, string search, string replace, ReplacementType replacementType)
        {
            var viewModel = BuildViewModel();

            var titleReplacement = viewModel.Interaction.Replacement.Copy();

            titleReplacement.ReplacementType = replacementType;
            titleReplacement.Search = search;
            titleReplacement.Replace = replace;

            var newInteraction = new TitleReplacementEditInteraction(titleReplacement);
            viewModel.SetInteraction(newInteraction);
            Assert.AreEqual(viewModel.IsRemoveAll, isAll);
            Assert.AreEqual(viewModel.IsRemoveAtBeginning, isStart);
            Assert.AreEqual(viewModel.IsRemoveAtEnd, isEnd);
            Assert.AreEqual(viewModel.IsReplaceByRegex, isRegex);
            Assert.AreEqual(viewModel.SearchForInput, search);
            Assert.AreEqual(viewModel.ReplaceWithInput, replace);
        }

        [Test]
        public void CreateViewModel_SetIsRegexTrue()
        {
            var viewModel = BuildViewModel();
            viewModel.IsReplaceByRegex = true;
            Assert.IsTrue(viewModel.IsReplaceByRegex);
        }

        [Test]
        public void CreateViewModel_SetIsRemoveAtBeginningTrue_RemoveAtBeginningIsTrue()
        {
            var viewModel = BuildViewModel();
            viewModel.IsRemoveAtBeginning = true;
            Assert.IsTrue(viewModel.IsRemoveAtBeginning);
        }

        [Test]
        public void CreateViewModel_SetIsRemoveAtEndTrue_RemoveAtEndIsTrue()
        {
            var viewModel = BuildViewModel();
            viewModel.IsRemoveAtEnd = true;
            Assert.IsTrue(viewModel.IsRemoveAtEnd);
        }

        [Test]
        public void DesignTimeViewModelTest()
        {
            var designTime = new DesignTimeTitleReplacementEditUserControlViewModel();
            Assert.NotNull(designTime);
        }

        [Test]
        public void ExecuteCancelCommand_FinishInteractionIsCalled()
        {
            _finishedCalled = false;
            var viewModel = BuildViewModel();
            viewModel.CancelCommand.Execute(null);
            Assert.IsTrue(_finishedCalled);
        }

        [Test]
        public void Nothing_RunConstructor_PropertiesAreSet()
        {
            var viewModel = BuildViewModel();
            Assert.IsNotNull(viewModel.Translation);
            Assert.AreSame(viewModel.ShowUserGuideCommand, _mockShowUserGuideCommand);
            Assert.NotNull(viewModel.OkCommand);
            Assert.NotNull(viewModel.CancelCommand);
            Assert.AreEqual(viewModel.SearchForInput, viewModel.Interaction.Replacement.Search);
            Assert.AreEqual(viewModel.ReplaceWithInput, viewModel.Interaction.Replacement.Replace);
            Assert.NotNull(viewModel.CancelCommand);
            Assert.AreEqual(viewModel.Title, viewModel.Translation.EditTextReplacementTitle);

            Assert.IsTrue(viewModel.IsRemoveAll);
            Assert.IsFalse(viewModel.IsInvalid);
            Assert.IsFalse(viewModel.IsRemoveAtBeginning);
            Assert.IsFalse(viewModel.IsRemoveAtEnd);
            Assert.IsFalse(viewModel.IsReplaceByRegex);
        }

        [Test]
        public void SetIsRegexTrue_ChangeInputToBrokenRegex_IsInvalidIsTrue()
        {
            var viewModel = BuildViewModel();
            viewModel.IsRemoveAll = false;
            viewModel.IsReplaceByRegex = true;

            viewModel.SearchForInput = "(";
            Assert.IsTrue(viewModel.IsInvalid);
        }
    }
}
