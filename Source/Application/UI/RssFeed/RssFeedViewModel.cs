using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Cache;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Commands.UserGuide;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.RssFeed
{
    public class RssFeedViewModel : TranslatableViewModelBase<MainShellTranslation>, IMountable
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly ICurrentSettings<Conversion.Settings.RssFeed> _rssFeedSettingsProvider;
        private readonly IGpoSettings _gpoSettings;
        private readonly IRssService _rssService;
        private readonly IFileCacheFactory _fileCacheFactory;
        private readonly ITempFolderProvider _tempFolderProvider;
        private readonly EditionHelper _editionHelper;
        private List<FeedItem> _feedItems;
        private readonly FileCache _fileCache;
        private const string CacheFilename = "rssfeed.json";

        public ICommand UrlOpenCommand { get; }
        public ICommand ShowRssFeedCommand { get; }
        public bool ShowReadMore { get; set; } = true;
        public bool RssFeedIsOpen { get; set; }

        public bool AllowPrioritySupport => !_editionHelper.IsFreeEdition;

        public ICommand WhatsNewCommand { get; }

        public ICommand PrioritySupportCommand { get; }

        private readonly string _editionWithVersion;
        public string WelcomeText => Translation?.GetWelcomeText(_editionWithVersion);

        public bool ShowWelcome { get; set; }

        private bool RssFeedIsEnabled => _rssFeedSettingsProvider.Settings.Enable;

        public bool DisableRssFeedViaGpo => _gpoSettings.DisableRssFeed;

        private DateTime _newestFeedEntry;

        private bool NewEntryInFeed => _newestFeedEntry > _rssFeedSettingsProvider.Settings.LatestRssUpdate;

        public RssFeedViewModel(ICommandLocator commandLocator, ICurrentSettings<Conversion.Settings.RssFeed> rssFeedSettingsProvider,
                                IGpoSettings gpoSettings, ITranslationUpdater translationUpdater,
                                IWelcomeSettingsHelper welcomeSettingsHelper, IRssService rssService,
                                IFileCacheFactory fileCacheFactory, ITempFolderProvider tempFolderProvider,
                                EditionHelper editionHelper, IVersionHelper versionHelper,
                                ApplicationNameProvider applicationNameProvider)
                                : base(translationUpdater)
        {
            _rssFeedSettingsProvider = rssFeedSettingsProvider;
            _gpoSettings = gpoSettings;
            _rssService = rssService;
            _fileCacheFactory = fileCacheFactory;
            _tempFolderProvider = tempFolderProvider;
            _editionHelper = editionHelper;

            _feedItems = new List<FeedItem>();
            _fileCache = GetFileCache();

            //Because the CheckIfRequiredAndSetCurrentVersion() sets the version in the registry
            ShowWelcome = welcomeSettingsHelper.CheckIfRequiredAndSetCurrentVersion();
            // ShowWelcome has to be set here in the constructor and not directly in the property
            RaisePropertyChanged(nameof(ShowWelcome));
            ShowWelcomeWindow();

            _editionWithVersion = applicationNameProvider?.EditionName + " " + versionHelper?.FormatWithThreeDigits();
            RaisePropertyChanged(WelcomeText);

            UrlOpenCommand = commandLocator.GetCommand<UrlOpenCommand>();
            ShowRssFeedCommand = new DelegateCommand(ShowRssFeedExecute);

            WhatsNewCommand = commandLocator.GetInitializedCommand<ShowUserGuideCommand, HelpTopic>(HelpTopic.WhatsNew);
            PrioritySupportCommand = commandLocator.GetCommand<IPrioritySupportUrlOpenCommand>();
        }

        protected override void OnTranslationChanged()
        {
            RaisePropertyChanged(nameof(WelcomeText));
        }

        private void ShowWelcomeWindow()
        {
            if (!ShowWelcome)
                return;

            RssFeedIsOpen = true;
            RaisePropertyChanged(nameof(RssFeedIsOpen));
        }

        private void ShowRssFeedExecute()
        {
            RssFeedIsOpen = !RssFeedIsOpen;
            RaisePropertyChanged(nameof(RssFeedIsOpen));
        }

        public List<FeedItem> FeedItems
        {
            get => _feedItems ?? (_feedItems = new List<FeedItem>());
            private set
            {
                _feedItems = value;
                RaisePropertyChanged(nameof(FeedItems));
            }
        }

        private bool RssEntriesAvailable()
        {
            if (FeedItems.Count > 0 && (RssFeedIsEnabled && !DisableRssFeedViaGpo))
                return true;

            FeedItems = new List<FeedItem>
            {
                new FeedItem()
                {
                    Title = RssFeedIsEnabled ? Translation.NoRssFeedAvailable : Translation.RssFeedDisabled,
                    Description = RssFeedIsEnabled ?   Translation.UnableToReadRssFeed :Translation.RssFeedDisabledDescription ,
                    PublishDate = DateTime.Now
                }
            };

            return false;
        }

        private void ManageRssFeedEntries()
        {
            _newestFeedEntry = FeedItems.First().PublishDate;
            if (NewEntryInFeed)
            {
                RssFeedIsOpen = true;
                RaisePropertyChanged(nameof(RssFeedIsOpen));
            }

            _rssFeedSettingsProvider.Settings.LatestRssUpdate = _newestFeedEntry;
        }

        private void EnableReadMoreLink(bool showReadMore)
        {
            ShowReadMore = showReadMore;
            RaisePropertyChanged(nameof(ShowReadMore));
        }

        public async void MountView()
        {
            EnableReadMoreLink(false);

            if (RssFeedIsEnabled && !DisableRssFeedViaGpo)
                await RetrieveFileFromCacheOrService();

            if (!RssEntriesAvailable())
                return;

            ManageRssFeedEntries();
            EnableReadMoreLink(true);
        }

        public void UnmountView()
        {
        }

        private FileCache GetFileCache()
        {
            var chacheDirectory = Path.Combine(_tempFolderProvider.TempFolder, "RSSFeed");
            return _fileCacheFactory.GetFileCache(chacheDirectory, TimeSpan.FromDays(1));
        }

        private async Task RetrieveFileFromCacheOrService()
        {
            try
            {
                if (_fileCache.FileAvailable(CacheFilename))
                {
                    // File was cached and can be used
                    FeedItems = await GetStreamFromFileAsync(CacheFilename);
                }
                else
                {
                    // File was not cached yet or is out-dated
                    FeedItems = await _rssService.FetchFeedAsync(Urls.RssFeedUrl);
                    var stream = await CreateStreamFromStringAsync(FeedItems);
                    await _fileCache.SaveFileAsync(CacheFilename, stream);
                }
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "An error occurred while fetching RSS feed.");
            }
        }

        private async Task<Stream> CreateStreamFromStringAsync(List<FeedItem> feedItems)
        {
            var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, feedItems);
            var writer = new StreamWriter(stream);
            await writer.FlushAsync();
            stream.Position = 0;
            return stream;
        }

        private async Task<List<FeedItem>> GetStreamFromFileAsync(string filename)
        {
            await using Stream stream = File.Open(_fileCache.GetCacheFilePath(filename), FileMode.Open);
            return await JsonSerializer.DeserializeAsync<List<FeedItem>>(stream);
        }
    }
}
