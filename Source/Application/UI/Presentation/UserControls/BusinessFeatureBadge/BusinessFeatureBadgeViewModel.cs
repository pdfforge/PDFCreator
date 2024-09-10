using pdfforge.PDFCreator.Core.ServiceLocator;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System.Windows.Input;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Commands;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls
{
    public enum RequiredEdition
    {
        AllLicensed
    }

    public class BusinessFeatureBadgeViewModel : TranslatableViewModelBase<BusinessFeatureTranslation>, IWhitelisted
    {
        private RequiredEdition _edition;

        public BusinessFeatureBadgeViewModel(EditionHelper editionHelper, ICommandLocator commandLocator, ITranslationUpdater translationUpdater)
            : base(translationUpdater)
        {
            ShowEditionWebsiteCommand = commandLocator.GetInitializedCommand<UrlOpenCommand, string>(Urls.BusinessHintLink);
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

        public ICommand ShowEditionWebsiteCommand { get; }
    }
}
