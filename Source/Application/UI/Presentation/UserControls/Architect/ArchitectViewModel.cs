using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;
using pdfforge.PDFCreator.Utilities.Web;
using System.ComponentModel;
using System.Windows.Input;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Architect
{
    public class ArchitectViewModel : TranslatableViewModelBase<ArchitectViewTranslation>
    {
        private readonly IPdfArchitectCheck _pdfArchitectCheck;
        private readonly IProcessStarter _processStarter;
        private readonly IWebLinkLauncher _webLinkLauncher;
        private readonly IFile _file;

        public ArchitectViewModel(IPdfArchitectCheck pdfArchitectCheck, IProcessStarter processStarter, IWebLinkLauncher webLinkLauncher, ITranslationUpdater translationUpdater, IFile file) : base(translationUpdater)
        {
            _pdfArchitectCheck = pdfArchitectCheck;
            _processStarter = processStarter;
            _webLinkLauncher = webLinkLauncher;
            _file = file;
        }

        protected override void OnTranslationChanged()
        {
            base.OnTranslationChanged();
            RaisePropertyChanged(nameof(DownloadPdfArchitectButtonText));
        }

        public ICommand LaunchPdfArchitectCommand => new DelegateCommand(o =>
        {
            var applicationPath = _pdfArchitectCheck.GetInstallationPath();
            try
            {
                if (!string.IsNullOrWhiteSpace(applicationPath))
                    _processStarter.Start(applicationPath);
            }
            catch
            {
                //ignore
            }
        });

        public string DownloadPdfArchitectButtonText => IsPdfArchitectDownloaded ? Translation.InstallPdfArchitectButtonContent : Translation.DownloadPdfArchitectButtonContent;

        public ICommand LaunchWebsiteCommand => new DelegateCommand(o => _webLinkLauncher.Launch(Urls.ArchitectWebsiteUrl));

        public ICommand DownloadPdfArchitectCommand => new DelegateCommand(o =>
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
                return;
            }

            _webLinkLauncher.Launch(Urls.ArchitectDownloadUrl);
        });

        public bool IsPdfArchitectInstalled => _pdfArchitectCheck.IsInstalled();
        public bool IsPdfArchitectDownloaded => _pdfArchitectCheck.IsDownloaded();
    }
}
