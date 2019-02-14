using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.Tokens
{
    public class SelectedProfileTokenViewModelBuilder : TokenViewModelBuilder<ConversionProfile>
    {
        private readonly ISelectedProfileProvider _selectedProfileProvider;

        public SelectedProfileTokenViewModelBuilder(ISelectedProfileProvider selectedProfileProvider, ITokenHelper tokenHelper)
            : base(tokenHelper, selectedProfileProvider.SelectedProfile)
        {
            _selectedProfileProvider = selectedProfileProvider;

            ViewModelDecorators.Add(AddSelectedProfileChangedAction);
        }

        private void AddSelectedProfileChangedAction(TokenViewModel<ConversionProfile> viewModel)
        {
            _selectedProfileProvider.SettingsChanged += (sender, args) => viewModel.CurrentValue = _selectedProfileProvider.SelectedProfile;
            _selectedProfileProvider.SelectedProfileChanged += (sender, args) => viewModel.CurrentValue = _selectedProfileProvider.SelectedProfile;
        }
    }
}
