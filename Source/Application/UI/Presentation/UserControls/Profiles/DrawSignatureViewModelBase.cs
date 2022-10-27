using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System.Windows.Input;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public abstract class DrawSignatureViewModelBase<TInteraction, TTranslatable>
        : OverlayViewModelBase<TInteraction, TTranslatable>
        where TTranslatable : ITranslatable, new()
        where TInteraction : DrawSignatureInteraction
    {
        public override string Title => Interaction.Title;

        public DelegateCommand SaveCommand { get; }
        public DelegateCommand ResetCommand { get; }
        public ICommand CancelCommand { get; }

        protected DrawSignatureViewModelBase(ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
            SaveCommand = new DelegateCommand(o => SaveExecute(), o => SaveCanExecute());
            CancelCommand = new DelegateCommand(CancelExecute);
            ResetCommand = new DelegateCommand(o => ResetExecute(), o => ResetCanExecute()); ;
        }

        protected abstract void SaveExecute();

        protected abstract bool SaveCanExecute();

        private void CancelExecute(object obj)
        {
            Interaction.Success = false;
            FinishInteraction();
        }

        protected abstract void ResetExecute();

        protected abstract bool ResetCanExecute();
    }
}
