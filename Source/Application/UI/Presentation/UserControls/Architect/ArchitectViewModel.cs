using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;
using System.Windows.Input;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Architect
{
    public class ArchitectViewModel : TranslatableViewModelBase<ArchitectViewTranslation>
    {
        private readonly IPdfArchitectCheck _pdfArchitectCheck;
        private readonly IProcessStarter _processStarter;
        private readonly IFile _file;
        private string _architectButtonCaption;

        public ArchitectViewModel(IPdfArchitectCheck pdfArchitectCheck, IProcessStarter processStarter, ITranslationUpdater translationUpdater, IFile file) : base(translationUpdater)
        {
            _pdfArchitectCheck = pdfArchitectCheck;
            _processStarter = processStarter;
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

            if (!string.IsNullOrWhiteSpace(applicationPath))
                _processStarter.Start(applicationPath);
        });

        public string DownloadPdfArchitectButtonText => IsPdfArchitectDownloaded ? Translation.InstallPdfArchitectButtonContent : Translation.DownloadPdfArchitectButtonContent;

        public ICommand LaunchWebsiteCommand => new DelegateCommand(o => _processStarter.Start(Urls.ArchitectWebsiteUrl));

        public ICommand DownloadPdfArchitectCommand => new DelegateCommand(o =>
        {
            var installerPath = _pdfArchitectCheck.GetInstallerPath();
            if (string.IsNullOrEmpty(installerPath) || !_file.Exists(installerPath))
            {
                _processStarter.Start(Urls.ArchitectDownloadUrl);
            }
            else
            {
                _processStarter.Start(installerPath);
            }
        });

        public bool IsPdfArchitectInstalled => _pdfArchitectCheck.IsInstalled();
        public bool IsPdfArchitectDownloaded => _pdfArchitectCheck.IsDownloaded();
    }
}
