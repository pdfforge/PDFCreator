using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Actions.Queries;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;
using pdfforge.PDFCreator.Utilities.Web;
using System;
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
        private readonly ICurrentSettings<ApplicationSettings> _currentApplicationSettings;
        private readonly ISoundPlayer _soundPlayer;

        // ReSharper disable once MemberCanBeProtected.Global
        public RecommendPdfArchitectWindowViewModel(ISoundPlayer soundPlayer, IWebLinkLauncher webLinkLauncher,
            ITranslationUpdater translationUpdater, IPdfArchitectCheck pdfArchitectCheck, IProcessStarter processStarter,
            IFile file, ICurrentSettings<ApplicationSettings> currentApplicationSettings)
            : base(translationUpdater)
        {
            _soundPlayer = soundPlayer;
            _webLinkLauncher = webLinkLauncher;
            _pdfArchitectCheck = pdfArchitectCheck;
            _processStarter = processStarter;
            _file = file;
            _currentApplicationSettings = currentApplicationSettings;
            InfoCommand = new DelegateCommand(ExecuteInfo);
            DownloadCommand = new DelegateCommand(ExecuteDownload);
        }

        public override string Title => "PDFCreator";

        public ICommand InfoCommand { get; }

        public ICommand DownloadCommand { get; }

        public bool DoNotRecommendArchitect
        {
            get { return _currentApplicationSettings.Settings.DontRecommendArchitect; }
            set
            {
                _currentApplicationSettings.Settings.DontRecommendArchitect = value;
                RaisePropertyChanged(nameof(DoNotRecommendArchitect));
            }
        }

        public string RecommendText { get; private set; }
        public bool OfferDoNotShowAgain { get; private set; }

        protected override void HandleInteractionObjectChanged()
        {
            _soundPlayer.Play(SystemSounds.Question);

            var recommendPurpose = Interaction?.RecommendPurpose
                                   ?? PdfArchitectRecommendPurpose.NoPdfViewer;

            switch (recommendPurpose)
            {
                case PdfArchitectRecommendPurpose.UpdateRequired:
                    RecommendText = Translation.ArchitectVersionDoesNotSupportThisFeature;
                    RecommendText += Environment.NewLine;
                    RecommendText += Translation.WeRecommendNewArchitectVersion;
                    OfferDoNotShowAgain = false;
                    break;

                case PdfArchitectRecommendPurpose.NotInstalled:
                    RecommendText += Translation.WeRecommendPdfArchitect;
                    OfferDoNotShowAgain = true;
                    break;

                case PdfArchitectRecommendPurpose.NoPdfViewer:
                default:
                    RecommendText = Translation.NoApplicationAssociatedWithPdfFiles;
                    RecommendText += Environment.NewLine;
                    RecommendText += Translation.WeRecommendPdfArchitect;
                    OfferDoNotShowAgain = false;
                    break;
            }

            RaisePropertyChanged(nameof(RecommendText));
            RaisePropertyChanged(nameof(OfferDoNotShowAgain));
        }

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
    }
}
