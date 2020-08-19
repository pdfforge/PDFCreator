using System;
using System.Windows.Input;
using pdfforge.LicenseValidator.Interface;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;
using pdfforge.PDFCreator.Utilities.Threading;
using pdfforge.PDFCreator.Utilities.Web;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.License
{
    public class OfflineActivationUserControlViewModel : OverlayViewModelBase<OfflineActivationInteraction, OfflineActivationViewModelTranslation>
    {
        //private readonly ILicenseChecker _licenseChecker;
        private readonly LicenseKeySyntaxChecker _licenseKeySyntaxChecker = new LicenseKeySyntaxChecker();

        private readonly IWebLinkLauncher _webLinkLauncher;
        private readonly IOfflineActivator _offlineActivator;

        public OfflineActivationUserControlViewModel(IWebLinkLauncher webLinkLauncher, IOfflineActivator offlineActivator, ITranslationUpdater translationUpdater):base(translationUpdater)
        {
            _webLinkLauncher = webLinkLauncher;
            _offlineActivator = offlineActivator;

            CancelCommand = new DelegateCommand(ExecuteCancelCommand);

            OkCommand = new DelegateCommand(OkCommandExecute, OkCommandCanExecute);
            OpenOfflineActivationUrlCommand = new DelegateCommand(OpenOfflineActivationUrlCommandExecute);
        }

        public DelegateCommand OkCommand { get; }
        public DelegateCommand CancelCommand { get; }
        public DelegateCommand OpenOfflineActivationUrlCommand { get; }

        public string OfflineActivationUrl => Urls.OfflineActivationUrl;

        public string LicenseServerAnswer
        {
            get { return Interaction?.LicenseServerAnswer; }
            set
            {
                Interaction.LicenseServerAnswer = value;
                RaisePropertyChanged(nameof(LicenseServerAnswer));
                OkCommand.RaiseCanExecuteChanged();
            }
        }

        public string LicenseKey
        {
            get { return Interaction?.LicenseKey; }
            set
            {
                Interaction.LicenseKey = value;
                RaisePropertyChanged(nameof(LicenseKey));
                RaisePropertyChanged(nameof(LicenseKeyIsValid));
                RaisePropertyChanged(nameof(OfflineActivationString));
            }
        }

        public bool LicenseKeyIsValid
            => _licenseKeySyntaxChecker.ValidateLicenseKey(LicenseKey) == ValidationResult.Valid;

        public string OfflineActivationString
        {
            get
            {
                if (LicenseKeyIsValid)
                    return _offlineActivator.BuildOfflineActivationString(LicenseKey.Trim());

                return Translation.InvalidLicenseKeySyntax;
            }
        }

        public override string Title => Translation.Title;

        private void OkCommandExecute(object obj)
        {
            Interaction.Success = true;
            FinishInteraction();
        }

        private bool OkCommandCanExecute(object obj)
        {
            return !string.IsNullOrWhiteSpace(LicenseServerAnswer);
        }

        private void OpenOfflineActivationUrlCommandExecute(object obj)
        {
            _webLinkLauncher.Launch(OfflineActivationUrl);
        }

        private void ExecuteCancelCommand(object o)
        {
            FinishInteraction();
        }

        protected override void HandleInteractionObjectChanged()
        {
            LicenseKey = Interaction.LicenseKey;
        }
    }

    public class DesignTimeOfflineActivationUserControlViewModel : OfflineActivationUserControlViewModel
    {
        public DesignTimeOfflineActivationUserControlViewModel() : base(null, null, new TranslationUpdater(new TranslationFactory(null), new ThreadManager()))
        {
        }
    }
}
