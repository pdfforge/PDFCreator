using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.ServiceLocator;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities.Process;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls
{
    public enum RequiredEdition
    {
        AllLicensed
    }

    public class BusinessFeatureControlViewModel : TranslatableViewModelBase<BusinessFeatureControlTranslation>, IWhitelisted
    {
        private readonly IProcessStarter _processStarter;
        private RequiredEdition _edition;

        public BusinessFeatureControlViewModel(EditionHelper editionHelper, IProcessStarter processStarter, ITranslationUpdater translationUpdater)
            : base(translationUpdater)
        {
            _processStarter = processStarter;
            ShowBusinessHint = editionHelper.ShowOnlyForPlusAndBusiness;
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

        public string ToolTip => Translation.BusinessRequiredHint;

        public bool ShowBusinessHint { get; }

        public ICommand ShowEditionWebsiteCommand => new DelegateCommand(ShowEditionWebsiteExecute);

        private void ShowEditionWebsiteExecute(object o)
        {
            var url = Urls.BusinessHintLink;
            _processStarter.Start(url);
        }
    }
}
