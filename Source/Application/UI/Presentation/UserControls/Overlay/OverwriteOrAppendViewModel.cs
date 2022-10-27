using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Overlay
{
    public class OverwriteOrAppendViewModel : InteractionAwareViewModelBase<OverwriteOrAppendInteraction>, ITranslatableViewModel<OverwriteOrAppendTranslation>
    {
        public override string Title => Translation?.Title;
        public OverwriteOrAppendTranslation Translation { get; set; }
        public ICommand MergeCommand { get; }
        public ICommand OverwriteCommand { get; }
        public ICommand CancelCommand { get; }

        public OverwriteOrAppendViewModel(ITranslationUpdater translationUpdater)
        {
            translationUpdater.RegisterAndSetTranslation(this);

            MergeCommand = new DelegateCommand(o =>
            {
                Interaction.Chosen = ExistingFileBehaviour.Merge;
                Interaction.Cancel = false;
                FinishInteraction.Invoke();
            });

            OverwriteCommand = new DelegateCommand(o =>
            {
                Interaction.Chosen = ExistingFileBehaviour.Overwrite;
                Interaction.Cancel = false;
                FinishInteraction.Invoke();
            });

            CancelCommand = new DelegateCommand(o =>
            {
                Interaction.Cancel = true;
                FinishInteraction.Invoke();
            });
        }
    }

    public class DesignTimeOverwriteOrAppendViewModel : OverwriteOrAppendViewModel
    {
        public DesignTimeOverwriteOrAppendViewModel() : base(new DesignTimeTranslationUpdater())
        {
        }
    }
}
