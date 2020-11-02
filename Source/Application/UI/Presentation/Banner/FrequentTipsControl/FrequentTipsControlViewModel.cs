using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Commands.UserGuide;
using pdfforge.PDFCreator.UI.Presentation.DesignTime;
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
        private readonly bool _isInitialized;
        public string CurrentBannerTitle { get; set; }
        public string CurrentBannerText { get; set; }
        public ICommand CurrentBannerCommand { get; set; }

        public FrequentTipsControlViewModel(ITranslationUpdater translationUpdater, ICommandLocator commandLocator) : base(translationUpdater)
        {
            _commandLocator = commandLocator;
            _isInitialized = true;
            ComposeFrequentTipList();
            SetRandomTip();
        }

        private void ComposeFrequentTipList()
        {
            FrequentTipList = new List<FrequentTip>
            {
                ComposeUserGuideTip(Translation.F1HelpTitle, Translation.F1HelpText, HelpTopic.General),
                ComposeUrlTip(Translation.AutoSaveTitle,Translation.AutoSaveText, Urls.Tip_AutoSaveUrl),
                ComposeUrlTip(Translation.UserTokensTitle,Translation.UserTokensText, Urls.Tip_UserTokensUrl),
                ComposeUrlTip(Translation.PDFCreatorOnlineTitle,Translation.PDFCreatorOnlineText, Urls.PdfCreatorOnlineUrl ),
                ComposeUserGuideTip(Translation.TemporarySaveTitle, Translation.TemporarySaveText,HelpTopic.ProfileSave),
                ComposeUrlTip(Translation.WorkflowTitle,Translation.WorkflowText, Urls.Tip_WorkflowUrl ),
                ComposeUrlTip(Translation.DropBoxTitle, Translation.DropBoxText, Urls.Tip_DropBoxUrl ),
                ComposeUserGuideTip(Translation.ForwardToFurtherProfileTitle, Translation.ForwardToFurtherProfileText, HelpTopic.ForwardToProfile ),
            };
        }

        protected override void OnTranslationChanged()
        {
            if (!_isInitialized)
                return;

            ComposeFrequentTipList();
            SetRandomTip();
            RaisePropertyChanged(nameof(CurrentBannerTitle));
            RaisePropertyChanged(nameof(CurrentBannerText));
        }

        private void SetRandomTip()
        {
            if (FrequentTipList == null || FrequentTipList.Count <= 0)
                return;

            var random = new Random();
            var randomIndex = random.Next(FrequentTipList.Count);
            var currentFrequentBanner = FrequentTipList[randomIndex];

            CurrentBannerTitle = currentFrequentBanner.Title;
            CurrentBannerText = currentFrequentBanner.Text;
            CurrentBannerCommand = currentFrequentBanner.Command;
        }

        public FrequentTip ComposeUrlTip(string title, string body, string parameter)
        {
            return ComposeTip<UrlOpenCommand, string>(title, body, parameter);
        }

        public FrequentTip ComposeUserGuideTip(string title, string body, HelpTopic parameter)
        {
            return ComposeTip<ShowUserGuideCommand, HelpTopic>(title, body, parameter);
        }

        private FrequentTip ComposeTip<TCommand, TParameter>(string title, string body, TParameter parameter) where TCommand : class, IInitializedCommand<TParameter>
        {
            var composedTip = new FrequentTip();
            composedTip.Title = title;
            composedTip.Text = body;
            composedTip.Command = _commandLocator.GetInitializedCommand<TCommand, TParameter>(parameter);
            return composedTip;
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
