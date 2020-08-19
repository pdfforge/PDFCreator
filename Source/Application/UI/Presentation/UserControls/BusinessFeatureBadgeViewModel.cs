using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.ServiceLocator;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities.Web;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls
{
    public enum RequiredEdition
    {
        AllLicensed
    }

    public class BusinessFeatureBadgeViewModel : TranslatableViewModelBase<BusinessFeatureTranslation>, IWhitelisted
    {
        private readonly IWebLinkLauncher _webLinkLauncher;
        private RequiredEdition _edition;

        public BusinessFeatureBadgeViewModel(EditionHelper editionHelper, IWebLinkLauncher webLinkLauncher, ITranslationUpdater translationUpdater)
            : base(translationUpdater)
        {
            _webLinkLauncher = webLinkLauncher;
            ShowBusinessHint = editionHelper.IsFreeEdition;
            RaisePropertyChanged(nameof(ShowBusinessHint));
        }

        public RequiredEdition Edition
        {
            get { return _edition; }
            set
            {
                SetProperty(ref _edition, value);
                RaisePropertyChanged(nameof(FeatureText));
                RaisePropertyChanged(nameof(ToolTip));
            }
        }

        public string FeatureText => Translation.BusinessFeature.ToUpper();

        public string ToolTip => Translation.ProfessionalRequiredHint;

        public bool ShowBusinessHint { get; }

        public ICommand ShowEditionWebsiteCommand => new DelegateCommand(ShowEditionWebsiteExecute);

        private void ShowEditionWebsiteExecute(object o)
        {
            var url = Urls.BusinessHintLink;
            _webLinkLauncher.Launch(url);
        }
    }
}
