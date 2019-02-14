using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace pdfforge.PDFCreator.UI.RssFeed
{
    public class RssFeedViewModel : TranslatableViewModelBase<MainShellTranslation>, IMountable
    {
        private readonly ICurrentSettings<Conversion.Settings.RssFeed> _rssFeedSettingsProvider;
        private readonly IGpoSettings _gpoSettings;
        private readonly IRssService _rssService;
        private List<FeedItem> _feedItems;

        public ICommand UrlOpenCommand { get; }
        public ICommand ShowRssFeedCommand { get; }
        private static object RedBrushColor => new BrushConverter().ConvertFromString("#c5091d");
        public Brush TitleBarColor { get; set; } = Brushes.Gray;
        public bool ShowReadMore { get; set; } = true;
        public IconList RssFeedIcon { get; set; } = IconList.RssFeedIcon;
        public bool RssFeedIsOpen { get; set; }
        public bool ShowWelcome { get; set; }
        private bool RssFeedIsEnabled => _rssFeedSettingsProvider.Settings.Enable;
        public bool DisableRssFeedViaGpo => !_gpoSettings.DisableRssFeed;

        private DateTime _newestFeedEntry;
        private bool NewEntryInFeed => _newestFeedEntry > _rssFeedSettingsProvider.Settings.LatestRssUpdate;

        public RssFeedViewModel(ICommandLocator commandLocator, ICurrentSettings<Conversion.Settings.RssFeed> rssFeedSettingsProvider,
                                IGpoSettings gpoSettings, ITranslationUpdater translationUpdater, IWelcomeSettingsHelper welcomeSettingsHelper, IRssService rssService)
                                : base(translationUpdater)
        {
            _rssFeedSettingsProvider = rssFeedSettingsProvider;
            _gpoSettings = gpoSettings;
            _rssService = rssService;

            _feedItems = new List<FeedItem>();

            ShowWelcome = welcomeSettingsHelper.CheckIfRequiredAndSetCurrentVersion();  // Because the CheckIfRequiredAndSetCurrentVersion() sets the version in the registry,
            RaisePropertyChanged(nameof(ShowWelcome));                       // ShowWelcome has to be set here in the ctor and not directly in the property

            UrlOpenCommand = commandLocator.GetCommand<UrlOpenCommand>();

            ShowRssFeedCommand = new DelegateCommand(ShowRssFeed);

            ShowWelcomeWindow();
        }

        private void ShowWelcomeWindow()
        {
            if (!ShowWelcome)
                return;

            RssFeedIsOpen = true;
            RaisePropertyChanged(nameof(RssFeedIsOpen));
            ChangeTitleBarColor();
            ChangeRssFeedIcon();
        }

        private void ShowRssFeed()
        {
            RssFeedIsOpen = !RssFeedIsOpen;
            ChangeRssFeedIcon();
            RaisePropertyChanged(nameof(RssFeedIsOpen));
        }

        private void ChangeRssFeedIcon()
        {
            RssFeedIcon = RssFeedIsOpen ? IconList.RssFeedArrowIcon : IconList.RssFeedIcon;
            RaisePropertyChanged(nameof(RssFeedIcon));
        }

        private void ChangeTitleBarColor()
        {
            TitleBarColor = (Brush)RedBrushColor;
            RaisePropertyChanged(nameof(TitleBarColor));
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
            if (FeedItems.Count > 0 && RssFeedIsEnabled)
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
                ChangeTitleBarColor();
                ChangeRssFeedIcon();
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

            if (RssFeedIsEnabled)
            {
                await FetchRssFeed();
            }

            if (!RssEntriesAvailable())
                return;

            ManageRssFeedEntries();
            EnableReadMoreLink(true);
        }

        private async Task FetchRssFeed()
        {
            FeedItems = await _rssService.FetchFeedAsync(Urls.RssFeedUrl);
        }

        public void UnmountView()
        {
        }
    }
}
