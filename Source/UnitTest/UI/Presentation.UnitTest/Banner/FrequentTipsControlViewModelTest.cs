using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Banner;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Commands.UserGuide;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities.Threading;
using System.Linq;
using System.Windows.Input;
using Translatable;

namespace Presentation.UnitTest.Banner
{
    [TestFixture]
    public class FrequentTipsControlViewModelTest
    {
        private FrequentTipsControlViewModel _frequentTipsControlViewModel;
        private FrequentTipsTranslation _translation = new FrequentTipsTranslation();
        private ICommandLocator _commandLocator;

        [SetUp]
        public void SetUp()
        {
            var translationUpdater = new TranslationUpdater(new TranslationFactory(null), new ThreadManager());
            _commandLocator = Substitute.For<ICommandLocator>();

            _frequentTipsControlViewModel = new FrequentTipsControlViewModel(translationUpdater, _commandLocator);
        }

        [Test]
        public void CheckForAllTipsInFrequentTips()
        {
            var frequentTipList = _frequentTipsControlViewModel.FrequentTipList;
            Assert.AreEqual(4, frequentTipList.Count,
                "Friendly reminder to write a unit test for the added Tip. Increase this expected count afterwards.");

            Assert.IsTrue(frequentTipList.Any(ft => ft.Equals(_frequentTipsControlViewModel.ComposeAutoSaveTip())), "AutoSaveTip not in FrequentTipList");
            Assert.IsTrue(frequentTipList.Any(ft => ft.Equals(_frequentTipsControlViewModel.ComposeF1HelpTip())), "F1HelpTip not in FrequentTipList");
            Assert.IsTrue(frequentTipList.Any(ft => ft.Equals(_frequentTipsControlViewModel.ComposePDFCreatorOnlineTip())), "PDFCreatorOnlineTip not in FrequentTipList");
            Assert.IsTrue(frequentTipList.Any(ft => ft.Equals(_frequentTipsControlViewModel.ComposeUserTokensTip())), "UserTokensTip not in FrequentTipList");
        }

        [Test]
        public void ComposeAutoSaveTip_Test()
        {
            var expectedCommand = Substitute.For<ICommand>();
            //Set condition for command locator to return expectedCommand
            _commandLocator.GetInitializedCommand<UrlOpenCommand, string>(Urls.Tip_AutoSaveUrl).Returns(expectedCommand);

            var frequentTip = _frequentTipsControlViewModel.ComposeAutoSaveTip();
            Assert.AreEqual(_translation.AutoSaveTitle, frequentTip.Title, "Title");
            Assert.AreEqual(_translation.AutoSaveText, frequentTip.Text, "Text");
            Assert.AreEqual(expectedCommand, frequentTip.Command, "Command");
        }

        [Test]
        public void ComposeOnlineToolsTip_Test()
        {
            var expectedCommand = Substitute.For<ICommand>();
            //Set condition for command locator to return expectedCommand
            _commandLocator.GetInitializedCommand<UrlOpenCommand, string>(Urls.PdfCreatorOnlineUrl).Returns(expectedCommand);

            var frequentTip = _frequentTipsControlViewModel.ComposePDFCreatorOnlineTip();
            Assert.AreEqual(_translation.PDFCreatorOnlineTitle, frequentTip.Title, "Title");
            Assert.AreEqual(_translation.PDFCreatorOnlineText, frequentTip.Text, "Text");
            Assert.AreEqual(expectedCommand, frequentTip.Command, "Command");
        }

        [Test]
        public void ComposeF1HelpTip_Test()
        {
            var expectedCommand = Substitute.For<ICommand>();
            //Set condition for command locator to return expectedCommand
            _commandLocator.GetInitializedCommand<ShowUserGuideCommand, HelpTopic>(HelpTopic.General).Returns(expectedCommand);

            var frequentTip = _frequentTipsControlViewModel.ComposeF1HelpTip();
            Assert.AreEqual(_translation.F1HelpTitle, frequentTip.Title, "Title");
            Assert.AreEqual(_translation.F1HelpText, frequentTip.Text, "Text");
            Assert.AreEqual(expectedCommand, frequentTip.Command, "Command");
        }

        [Test]
        public void ComposeUserTokensTip_Test()
        {
            var expectedCommand = Substitute.For<ICommand>();
            //Set condition for command locator to return expectedCommand
            _commandLocator.GetInitializedCommand<UrlOpenCommand, string>(Urls.Tip_UserTokensUrl).Returns(expectedCommand);

            var frequentTip = _frequentTipsControlViewModel.ComposeUserTokensTip();
            Assert.AreEqual(_translation.UserTokensTitle, frequentTip.Title, "Title");
            Assert.AreEqual(_translation.UserTokensText, frequentTip.Text, "Text");
            Assert.AreEqual(expectedCommand, frequentTip.Command, "Command");
        }
    }
}
