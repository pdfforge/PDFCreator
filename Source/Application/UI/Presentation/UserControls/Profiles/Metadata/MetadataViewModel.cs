using System.ComponentModel;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public class MetadataViewModel : ProfileUserControlViewModel<MetadataTabTranslation>, IStatusHintViewModel
    {
        private readonly IProfileChecker _profileChecker;
        private readonly ErrorCodeInterpreter _errorCodeInterpreter;
        public bool HideStatusInOverlay => false;
        public string StatusText { get; private set; }
        public bool HasWarning { get; private set; }

        public TokenViewModel<ConversionProfile> TitleTokenViewModel { get; private set; }
        public TokenViewModel<ConversionProfile> AuthorTokenViewModel { get; private set; }
        public TokenViewModel<ConversionProfile> SubjectTokenViewModel { get; private set; }
        public TokenViewModel<ConversionProfile> KeywordsTokenViewModel { get; private set; }

        public MetadataViewModel(ITranslationUpdater translationUpdater, ITokenViewModelFactory tokenViewModelFactory,
            ISelectedProfileProvider selectedProfileProvider, IDispatcher dispatcher,
            IProfileChecker profileChecker, ErrorCodeInterpreter errorCodeInterpreter) 
            : base(translationUpdater, selectedProfileProvider, dispatcher)
        {
            _profileChecker = profileChecker;
            _errorCodeInterpreter = errorCodeInterpreter;
            translationUpdater?.RegisterAndSetTranslation(tf => SetTokenViewModels(tokenViewModelFactory));
        }

        public void SetTokenViewModels(ITokenViewModelFactory tokenViewModelFactory)
        {
            var builder = tokenViewModelFactory
                .BuilderWithSelectedProfile()
                .WithDefaultTokenReplacerPreview(th => th.GetTokenListForMetadata());

            TitleTokenViewModel = builder.WithSelector(p => p.TitleTemplate).Build();
            AuthorTokenViewModel = builder.WithSelector(p => p.AuthorTemplate).Build();
            SubjectTokenViewModel = builder.WithSelector(p => p.SubjectTemplate).Build();
            KeywordsTokenViewModel = builder.WithSelector(p => p.KeywordTemplate).Build();

            UpdateMetadata();
        }

        private void UpdateMetadata()
        {
            RaisePropertyChanged(nameof(TitleTokenViewModel));
            RaisePropertyChanged(nameof(AuthorTokenViewModel));
            RaisePropertyChanged(nameof(SubjectTokenViewModel));
            RaisePropertyChanged(nameof(KeywordsTokenViewModel));
        }

        public override void MountView()
        {
            CurrentProfile.PropertyChanged += UpdateStatus;
            UpdateStatus(null, null);

            TitleTokenViewModel.MountView();
            AuthorTokenViewModel.MountView();
            SubjectTokenViewModel.MountView();
            KeywordsTokenViewModel.MountView();
            
            base.MountView();
        }

        public override void UnmountView()
        {
            base.UnmountView();
            
            TitleTokenViewModel.UnmountView();
            AuthorTokenViewModel.UnmountView();
            SubjectTokenViewModel.UnmountView();
            KeywordsTokenViewModel.UnmountView();

            CurrentProfile.PropertyChanged -= UpdateStatus;
        }

        private void UpdateStatus(object sender, PropertyChangedEventArgs e)
        {
            var result = _profileChecker.CheckMetadata(CurrentProfile);

            if (!CurrentProfile.FormatSupportsMetadata)
            {
                HasWarning = true;
                StatusText = Translation.NotSupportedMetadata;
            }
            else if (!result)
            {
                HasWarning = true;
                StatusText = _errorCodeInterpreter.GetFirstErrorText(result, false);
            }
            else
            {
                HasWarning = false;
                StatusText = "";
            }

            RaisePropertyChanged(nameof(StatusText));
            RaisePropertyChanged(nameof(HasWarning));
        }
    }
}
