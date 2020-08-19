using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities.Web;
using System.Media;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.Windows
{
    public class RecommendPdfArchitectWindowViewModel : OverlayViewModelBase<RecommendPdfArchitectInteraction, RecommendPdfArchitectWindowTranslation>
    {
        private readonly IWebLinkLauncher _webLinkLauncher;
        private readonly ISoundPlayer _soundPlayer;

        // ReSharper disable once MemberCanBeProtected.Global
        public RecommendPdfArchitectWindowViewModel(ISoundPlayer soundPlayer, IWebLinkLauncher webLinkLauncher, ITranslationUpdater translationUpdater)
            : base(translationUpdater)
        {
            _soundPlayer = soundPlayer;
            _webLinkLauncher = webLinkLauncher;
            InfoCommand = new DelegateCommand(ExecuteInfo);
            DownloadCommand = new DelegateCommand(ExecuteDownload);
        }

        public ICommand InfoCommand { get; }

        public ICommand DownloadCommand { get; }

        private void ExecuteInfo(object o)
        {
            _webLinkLauncher.Launch(Urls.ArchitectWebsiteUrl);
            FinishInteraction();
        }

        private void ExecuteDownload(object o)
        {
            _webLinkLauncher.Launch(Urls.ArchitectDownloadUrl);
            FinishInteraction();
        }

        protected override void HandleInteractionObjectChanged()
        {
            if (Interaction.ShowViewerWarning)
            {
                _soundPlayer.Play(SystemSounds.Hand);
            }
            RaisePropertyChanged(nameof(RecommendedText));
            RaisePropertyChanged(nameof(ErrorText));
        }

        public string RecommendedText
        {
            get
            {
                if (Interaction == null)
                    return "";
                return Interaction.IsUpdate ? Translation.RecommendTextUpdate : Translation.RecommendTextInstall;
            }
        }

        public string ErrorText
        {
            get
            {
                if (Interaction == null)
                    return "";
                return Interaction.IsUpdate ? Translation.ErrorTextUpdate : Translation.ErrorTextInstall;
            }
        }

        public override string Title => "PDFCreator";
    }
}
