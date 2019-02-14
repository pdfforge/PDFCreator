using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.ProfileHasNotSupportedFeaturesExtension;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System.ComponentModel;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs.ConvertTabs
{
    public class ConvertPdfViewModel : ProfileUserControlViewModel<ConvertPdfTranslation>
    {
        private readonly EditionHelper _editionHelper;

        public bool AllowForPlusAndBusiness
        {
            get { return _editionHelper.ShowOnlyForPlusAndBusiness; }
        }

        public ConvertPdfViewModel(ITranslationUpdater translationUpdater, ISelectedProfileProvider selectedProfile,
                                    EditionHelper editionHelper, IDispatcher dispatcher) : base(translationUpdater, selectedProfile, dispatcher)
        {
            _editionHelper = editionHelper;
            CurrentProfileChanged += (sender, args) =>
            {
                RaiseIsPdfOutputChanged();
                CurrentProfile.PropertyChanged += RaiseIsPdfOutputChanged;
                CurrentProfile.SetRaiseConditionsForNotSupportedFeatureSections(RaiseIsPdfOutputChanged);
            };

            if (CurrentProfile == null)
                return;

            CurrentProfile.PropertyChanged += RaiseIsPdfOutputChanged;
            CurrentProfile.SetRaiseConditionsForNotSupportedFeatureSections(RaiseIsPdfOutputChanged);
        }

        private void RaiseIsPdfOutputChanged(object sender = null, PropertyChangedEventArgs args = null)
        {
            if (args == null || args.PropertyName == nameof(CurrentProfile.OutputFormat))
            {
                RaisePropertyChanged(nameof(IsPdfOutput));
                RaisePropertyChanged(nameof(IsPdfAOutput));
            }

            RaisePropertyChanged(nameof(HasNotSupportedColorModel));
        }

        public bool IsPdfOutput => CurrentProfile.OutputFormat.IsPdf();
        public bool IsPdfAOutput => CurrentProfile.OutputFormat.IsPdfA();
        public bool HasNotSupportedColorModel => CurrentProfile.HasNotSupportedColorModel();
    }
}
