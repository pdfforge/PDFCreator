using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SelectFiles
{
    public interface ISelectFilesUserControlViewModelFactory
    {
        ISelectFilesUserControlViewModelBuilder Builder();
    }

    public class SelectFilesUserControlViewModelFactory : ISelectFilesUserControlViewModelFactory
    {
        private readonly ITranslationUpdater _translationUpdater;
        private readonly ISelectedProfileProvider _selectedProfileProvider;
        private readonly IDispatcher _dispatcher;
        private readonly IInteractionRequest _interactionRequest;

        public SelectFilesUserControlViewModelFactory(ITranslationUpdater translationUpdater,
            ISelectedProfileProvider selectedProfileProvider,
            IDispatcher dispatcher,
            IInteractionRequest interactionRequest)
        {
            _translationUpdater = translationUpdater;
            _selectedProfileProvider = selectedProfileProvider;
            _dispatcher = dispatcher;
            _interactionRequest = interactionRequest;
        }

        public ISelectFilesUserControlViewModelBuilder Builder()
        {
            return new SelectFilesUserControlViewModelBuilder(_translationUpdater, _selectedProfileProvider, _dispatcher, _interactionRequest);
        }
    }
}
