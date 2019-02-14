using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using Prism.Commands;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants.Update
{
    public class RestartApplicationInteractionViewModel : OverlayViewModelBase<RestartApplicationInteraction, RestartApplicationInteractionTranslation>
    {
        public ICommand NowCommand { get; }
        public ICommand LaterCommand { get; }
        public ICommand CancelCommand { get; }

        public RestartApplicationInteractionViewModel(ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
            NowCommand = new DelegateCommand(() => FinishInteractionHandler(RestartApplicationInteractionResult.Now));
            LaterCommand = new DelegateCommand(() => FinishInteractionHandler(RestartApplicationInteractionResult.Later));
            CancelCommand = new DelegateCommand(() => FinishInteractionHandler(RestartApplicationInteractionResult.Cancel));
        }

        private void FinishInteractionHandler(RestartApplicationInteractionResult result)
        {
            Interaction.InteractionResult = result;
            FinishInteraction();
        }

        public override string Title => Translation.Title;
    }

    public class DesignTimeRestartApplicationViewModel : RestartApplicationInteractionViewModel
    {
        public DesignTimeRestartApplicationViewModel() : base(new DesignTimeTranslationUpdater())
        {
        }
    }
}
