using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System.Windows;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public class OutputFormatJpgViewModel : ProfileUserControlViewModel<OutputFormatTranslation>
    {
        public OutputFormatJpgViewModel(ITranslationUpdater translationUpdater, ISelectedProfileProvider selectedProfile, IDispatcher dispatcher) : base(translationUpdater, selectedProfile, dispatcher)
        {
            LostFocusCommand = new DelegateCommand<RoutedEventArgs>(OnLostFocus);
        }

        public DelegateCommand<RoutedEventArgs> LostFocusCommand { get; set; }

        private void OnLostFocus(RoutedEventArgs obj)
        {
            var quality = CurrentProfile.JpegSettings.Quality;
            if (quality < 1)
                CurrentProfile.JpegSettings.Quality = 1;
            if (quality > 100)
                CurrentProfile.JpegSettings.Quality = 100;

            RaisePropertyChanged(nameof(CurrentProfile));
        }
    }
}
