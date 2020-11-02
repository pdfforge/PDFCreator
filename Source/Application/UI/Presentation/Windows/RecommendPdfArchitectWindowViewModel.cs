using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;
using pdfforge.PDFCreator.Utilities.Web;
using System.ComponentModel;
using System.Media;
using System.Windows.Input;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UI.Presentation.Windows
{
    public class RecommendPdfArchitectWindowViewModel : OverlayViewModelBase<RecommendPdfArchitectInteraction, RecommendPdfArchitectWindowTranslation>
    {
        private readonly IWebLinkLauncher _webLinkLauncher;
        private readonly IPdfArchitectCheck _pdfArchitectCheck;
        private readonly IProcessStarter _processStarter;
        private readonly IFile _file;
        private readonly ISoundPlayer _soundPlayer;

        // ReSharper disable once MemberCanBeProtected.Global
        public RecommendPdfArchitectWindowViewModel(ISoundPlayer soundPlayer, IWebLinkLauncher webLinkLauncher, ITranslationUpdater translationUpdater, IPdfArchitectCheck pdfArchitectCheck, IProcessStarter processStarter, IFile file)
            : base(translationUpdater)
        {
            _soundPlayer = soundPlayer;
            _webLinkLauncher = webLinkLauncher;
            _pdfArchitectCheck = pdfArchitectCheck;
            _processStarter = processStarter;
            _file = file;
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
            var installerPath = _pdfArchitectCheck.GetInstallerPath();
            if (!string.IsNullOrEmpty(installerPath) && _file.Exists(installerPath))
            {
                try
                {
                    _processStarter.Start(installerPath);
                }
                catch (Win32Exception e) when (e.NativeErrorCode == 1223)
                {
                    // ignore Win32Exception when UAC was not allowed
                }
            }
            else
            {
                _webLinkLauncher.Launch(Urls.ArchitectDownloadUrl);
            }

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
