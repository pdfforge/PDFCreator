using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using Prism.Regions;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public class WorkflowEditorOverlayViewModel : OverlayViewModelBase<WorkflowEditorOverlayInteraction, WorkflowEditorOverlayViewTranslation>
    {
        private readonly IRegionManager _regionManager;
        public DelegateCommand OkCommand { get; }
        public DelegateCommand CloseCommand { get; }
        public DelegateCommand BackCommand { get; }

        public Action UpdateHelpTopic;

        public WorkflowEditorOverlayViewModel(IRegionManager regionManager, ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
            _regionManager = regionManager;
            OkCommand = new DelegateCommand((x) =>
            {
                Interaction.Result = WorkflowEditorOverlayResult.Success;
                FinishInteraction();
            }, x => Interaction?.IsDisabled == false);

            CloseCommand = new DelegateCommand(x =>
            {
                Interaction.Result = WorkflowEditorOverlayResult.Close;
                FinishInteraction();
            }, x => Interaction?.IsDisabled == true);

            BackCommand = new DelegateCommand(x =>
            {
                Interaction.Result = WorkflowEditorOverlayResult.Back;
                FinishInteraction();
            });
        }

        protected override void HandleInteractionObjectChanged()
        {
            base.HandleInteractionObjectChanged();
            _regionManager.RequestNavigate(RegionNames.ProfileWorkflowEditorOverlayRegion, Interaction.View);
            UpdateHelpTopic?.Invoke();
            OkCommand.RaiseCanExecuteChanged();
            CloseCommand.RaiseCanExecuteChanged();
        }

        public override string Title => $"{Translation.ModifySettings} {Interaction.Title}";
    }
}
