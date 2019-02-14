using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper;
using pdfforge.PDFCreator.UI.RssFeed;
using Presentation.UnitTest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RssFeed.UnitTest
{
    [TestFixture]
    public class RssFeedViewModelUnitTest
    {
        private ICommandLocator _commandLocator;
        private ICurrentSettings<pdfforge.PDFCreator.Conversion.Settings.RssFeed> _rssFeedSettingsProvider;
        private IGpoSettings _gpoSettings;
        private IWelcomeSettingsHelper _welcomeSettingsHelper;
        private IRssService _rssService;

        [SetUp]
        public void Setup()
        {
            _commandLocator = Substitute.For<ICommandLocator>();
            _gpoSettings = Substitute.For<IGpoSettings>();
            _welcomeSettingsHelper = Substitute.For<IWelcomeSettingsHelper>();
            _rssService = Substitute.For<IRssService>();

            var settings = new PdfCreatorSettings();
            settings.ApplicationSettings.RssFeed = new pdfforge.PDFCreator.Conversion.Settings.RssFeed { Enable = true };

            _rssFeedSettingsProvider = Substitute.For<ICurrentSettings<pdfforge.PDFCreator.Conversion.Settings.RssFeed>>();
            _rssFeedSettingsProvider.Settings.Returns(settings.ApplicationSettings.RssFeed);
        }

        private RssFeedViewModel BuildViewModel()
        {
            return new RssFeedViewModel(_commandLocator, _rssFeedSettingsProvider, _gpoSettings, new UnitTestTranslationUpdater(),
                                        _welcomeSettingsHelper, _rssService);
        }

        [Test]
        public void RssFeedViewModel_WelcomeSettingsHelperReturnsTrue_RssFeedIsOpen_TitleBarIsRed_IconIsArrow()
        {
            _welcomeSettingsHelper.CheckIfRequiredAndSetCurrentVersion().Returns(true);

            var vm = BuildViewModel();

            var tbc = vm.TitleBarColor.GetCurrentValueAsFrozen().ToString();
            var expectedColor = "#ffc5091d"; // the nuance of red we use for the title bar

            Assert.IsTrue(vm.RssFeedIsOpen);
            Assert.AreEqual(tbc, expectedColor.ToUpper());
            Assert.IsTrue(vm.RssFeedIcon == IconList.RssFeedArrowIcon);
        }

        [Test]
        public void RssFeedViewModel_WelcomeSettingsHelperReturnsFalse_RssFeedIsClosed_TitleBarIsGray_IconIsRssFeedIcon()
        {
            _welcomeSettingsHelper.CheckIfRequiredAndSetCurrentVersion().Returns(false);

            var vm = BuildViewModel();

            var tbc = vm.TitleBarColor.GetCurrentValueAsFrozen().ToString();
            var expectedColor = Brushes.Gray.ToString(); // the nuance of red we use for the title bar

            Assert.IsFalse(vm.RssFeedIsOpen);
            Assert.AreEqual(tbc, expectedColor.ToUpper());
            Assert.IsTrue(vm.RssFeedIcon == IconList.RssFeedIcon);
        }

        [Test]
        public void RssFeedViewModel_RssServiceReceived()
        {
            var task = new Task<List<FeedItem>>(x => new List<FeedItem>(), -1);

            _rssService.FetchFeedAsync(Arg.Any<string>()).Returns(task);

            var vm = BuildViewModel();
            vm.MountView();

            task.RunSynchronously();

            Assert.IsFalse(vm.ShowReadMore);
            Assert.AreEqual(vm.FeedItems.Count, 1);
        }

        [Test]
        public void RssFeedViewModel_TaskAlreadyRun_ShowReadMoreIsTrue()
        {
            var task = new Task<List<FeedItem>>(x =>
            {
                var feedItems = new List<FeedItem> { new FeedItem() };
                return feedItems;
            }, -1);

            task.RunSynchronously();

            _rssService.FetchFeedAsync(Arg.Any<string>()).Returns(task);

            var vm = BuildViewModel();
            vm.MountView();

            Assert.IsTrue(vm.ShowReadMore);
            Assert.AreEqual(vm.FeedItems.Count, 1);
        }

        [Test]
        public void RssFeedViewModel_LatestRssUpdateIsOlderThanEntryPublishDate_FeedIsOpenWithArrowIconAndRedTitleBar()
        {
            var task = new Task<List<FeedItem>>(x =>
            {
                var feedItems = new List<FeedItem>
                {
                    new FeedItem()
                    {
                        Description = "desc",
                        Link = "link",
                        PublishDate = DateTime.MaxValue, // PublishDate is younger than LatestRssUpdate
                        Title = "title"
                    }
                };
                return feedItems;
            }, -1);

            task.RunSynchronously();

            _rssFeedSettingsProvider.Settings.LatestRssUpdate = DateTime.MinValue;

            _rssService.FetchFeedAsync(Arg.Any<string>()).Returns(task);

            var vm = BuildViewModel();

            vm.MountView();

            var tbc = vm.TitleBarColor.GetCurrentValueAsFrozen().ToString();
            var expectedColor = "#ffc5091d"; // the nuance of red we use for the title bar

            Assert.IsTrue(vm.RssFeedIsOpen);
            Assert.IsTrue(vm.ShowReadMore);
            Assert.AreEqual(tbc, expectedColor.ToUpper());
            Assert.IsTrue(vm.RssFeedIcon == IconList.RssFeedArrowIcon);
            Assert.AreEqual(vm.FeedItems.Count, 1);
        }

        [Test]
        public void ShowRssFeedCommand_CanExecute_IsAlwaysTrue()
        {
            var vm = BuildViewModel();
            Assert.IsTrue(vm.ShowRssFeedCommand.CanExecute(null));
        }

        [Test]
        public void ShowRssFeedCommand_ExecuteOnce_IconIsArrow_FeedIsOpen()
        {
            var vm = BuildViewModel();

            vm.ShowRssFeedCommand.Execute(null);

            Assert.IsTrue(vm.RssFeedIsOpen);
            Assert.IsTrue(vm.RssFeedIcon == IconList.RssFeedArrowIcon);
        }

        [Test]
        public void ShowRssFeedCommand_ExecuteTwice_IconIsRssFeed_FeedIsClosed()
        {
            var vm = BuildViewModel();

            vm.ShowRssFeedCommand.Execute(null);
            vm.ShowRssFeedCommand.Execute(null);

            Assert.IsFalse(vm.RssFeedIsOpen);
            Assert.IsTrue(vm.RssFeedIcon == IconList.RssFeedIcon);
        }
    }
}
