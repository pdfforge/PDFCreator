using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Commands.UserGuide;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.Banner
{
    public class FrequentTipsControlViewModel : TranslatableViewModelBase<FrequentTipsTranslation>
    {
        private readonly ICommandLocator _commandLocator;

        public List<FrequentTip> FrequentTipList;
        public string CurrentBannerTitle { get; set; }
        public string CurrentBannerText { get; set; }
        public ICommand CurrentBannerCommand { get; set; }

        public FrequentTipsControlViewModel(ITranslationUpdater translationUpdater, ICommandLocator commandLocator) : base(translationUpdater)
        {
            _commandLocator = commandLocator;
            ComposeFrequentTipList();
            SetRandomTip();
        }

        private void ComposeFrequentTipList()
        {
            FrequentTipList = new List<FrequentTip>
            {
                ComposeF1HelpTip(),
                ComposeAutoSaveTip(),
                ComposeUserTokensTip(),
                ComposePDFCreatorOnlineTip()
            };
        }

        protected override void OnTranslationChanged()
        {
            if (FrequentTipList != null)
                SetRandomTip();
            RaisePropertyChanged(nameof(CurrentBannerTitle));
            RaisePropertyChanged(nameof(CurrentBannerText));
        }

        private void SetRandomTip()
        {
            var random = new Random();
            var randomIndex = random.Next(FrequentTipList.Count);
            var currentFrequentBanner = FrequentTipList[randomIndex];

            CurrentBannerTitle = currentFrequentBanner.Title;
            CurrentBannerText = currentFrequentBanner.Text;
            CurrentBannerCommand = currentFrequentBanner.Command;
        }

        public FrequentTip ComposeF1HelpTip()
        {
            var frequentBannerElement = new FrequentTip();
            frequentBannerElement.Title = Translation.F1HelpTitle;
            frequentBannerElement.Text = Translation.F1HelpText;
            frequentBannerElement.Command = _commandLocator.GetInitializedCommand<ShowUserGuideCommand, HelpTopic>(HelpTopic.General);
            return frequentBannerElement;
        }

        public FrequentTip ComposeAutoSaveTip()
        {
            var frequentBannerElement = new FrequentTip();
            frequentBannerElement.Title = Translation.AutoSaveTitle;
            frequentBannerElement.Text = Translation.AutoSaveText;
            frequentBannerElement.Command = _commandLocator.GetInitializedCommand<UrlOpenCommand, string>(Urls.Tip_AutoSaveUrl);
            return frequentBannerElement;
        }

        public FrequentTip ComposeUserTokensTip()
        {
            var frequentBannerElement = new FrequentTip();
            frequentBannerElement.Title = Translation.UserTokensTitle;
            frequentBannerElement.Text = Translation.UserTokensText;
            frequentBannerElement.Command = _commandLocator.GetInitializedCommand<UrlOpenCommand, string>(Urls.Tip_UserTokensUrl);
            return frequentBannerElement;
        }

        public FrequentTip ComposePDFCreatorOnlineTip()
        {
            var frequentBannerElement = new FrequentTip();
            frequentBannerElement.Title = Translation.PDFCreatorOnlineTitle;
            frequentBannerElement.Text = Translation.PDFCreatorOnlineText;
            frequentBannerElement.Command = _commandLocator.GetInitializedCommand<UrlOpenCommand, string>(Urls.PdfCreatorOnlineUrl);
            return frequentBannerElement;
        }
    }

    public class DesignTimeFrequentTipsControlViewModel : FrequentTipsControlViewModel
    {
        public DesignTimeFrequentTipsControlViewModel() : base(new DesignTimeTranslationUpdater(), new DesignTimeCommandLocator())
        {
            CurrentBannerTitle = "Did you know that Hippos can fly?";
            CurrentBannerText = "Actually they can't but nice to have your attention. ";
            CurrentBannerText += CurrentBannerText;
            CurrentBannerText += CurrentBannerText;
            CurrentBannerText += CurrentBannerText;
        }
    }
}
