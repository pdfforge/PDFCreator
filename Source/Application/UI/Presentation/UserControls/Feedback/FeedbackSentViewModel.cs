using System.Windows.Input;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Interactions.Feedback;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Feedback
{
    public class FeedbackSentViewModel : OverlayViewModelBase<FeedbackSentInteraction, FeedbackSentTranslation>
    {
        public FeedbackSentViewModel(ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
            OkCommand = new DelegateCommand(OkCommandExecute);
        }

        public override string Title => Translation.WindowTitle;
        public string TitleText => GetTitleText();

        private string GetTitleText()
        {
            if (Interaction == null)
                return "";

            switch (Interaction.SelectedFeedbackType)
            {
                case FeedbackType.FeatureSuggestion:
                    return Translation.PositiveTitleText;
                case FeedbackType.PositiveFeedback:
                case FeedbackType.ReportIssue:
                case FeedbackType.None:
                default:
                    return Translation.TitleText;
            }
        }

        public string BodyText => GetBodyText();

        private string GetBodyText()
        {
            if (Interaction == null) 
                return "";

            switch (Interaction.SelectedFeedbackType)
            {
                case FeedbackType.PositiveFeedback:
                    return Translation.PositiveBodyText;
                case FeedbackType.ReportIssue:
                    return Translation.NegativeBodyText;
                case FeedbackType.FeatureSuggestion:
                    return Translation.FeatureBodyText;
                case FeedbackType.None:
                default:
                    return Translation.BodyText;
            }
        }

        public bool ShowTrustPilotMessage => Interaction.SelectedFeedbackType == FeedbackType.PositiveFeedback;

        protected override void HandleInteractionObjectChanged()
        {
            base.HandleInteractionObjectChanged();
            RaisePropertyChanged(nameof(TitleText));
            RaisePropertyChanged(nameof(BodyText));
        }

        public string TrustPilotLink => "https://google.com";
        public ICommand OkCommand { get; set; }

        private void OkCommandExecute(object o)
        {
            FinishInteraction();
        }
    }
}
