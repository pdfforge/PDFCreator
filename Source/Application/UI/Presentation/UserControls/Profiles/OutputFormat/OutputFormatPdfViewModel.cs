using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System.ComponentModel;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public class OutputFormatPdfViewModel : ProfileUserControlViewModel<OutputFormatTranslation>
    {
        private readonly EditionHelper _editionHelper;

        public bool SupportsPdfAValidation => !_editionHelper.IsFreeEdition;

        public OutputFormatPdfViewModel(ITranslationUpdater translationUpdater, ISelectedProfileProvider selectedProfile,
                                    EditionHelper editionHelper, IDispatcher dispatcher) : base(translationUpdater, selectedProfile, dispatcher)
        {
            _editionHelper = editionHelper;
            CurrentProfileChanged += (sender, args) =>
            {
                RaiseIsPdfOutputChanged();
                CurrentProfile.PropertyChanged += RaiseIsPdfOutputChanged;
                CurrentProfile.PdfSettings.PropertyChanged += RaiseIsPdfOutputChanged;
            };

            if (CurrentProfile != null)
            {
                CurrentProfile.PropertyChanged += RaiseIsPdfOutputChanged;
                CurrentProfile.PdfSettings.PropertyChanged += RaiseIsPdfOutputChanged;
            }
        }

        private void RaiseIsPdfOutputChanged(object sender = null, PropertyChangedEventArgs args = null)
        {
            RaisePropertyChanged(nameof(IsPdfOutput));
            RaisePropertyChanged(nameof(IsPdfAOutput));
            RaisePropertyChanged(nameof(HasNotSupportedColorModel));
        }

        public bool IsPdfOutput => CurrentProfile.OutputFormat.IsPdf();
        public bool IsPdfAOutput => CurrentProfile.OutputFormat.IsPdfA();
        public bool HasNotSupportedColorModel => (CurrentProfile.OutputFormat == OutputFormat.PdfX) && (CurrentProfile.PdfSettings.ColorModel == ColorModel.Rgb);
    }
}
