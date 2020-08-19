using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using Prism.Regions;
using System;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public class WorkflowEditorOverlayViewModel : OverlayViewModelBase<WorkflowEditorOverlayInteraction, WorkflowEditorOverlayViewTranslation>
    {
        private readonly IRegionManager _regionManager;
        public ICommand OkCommand { get; }

        public Action UpdateHelpTopic;

        public WorkflowEditorOverlayViewModel(IRegionManager regionManager, ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
            _regionManager = regionManager;
            OkCommand = new DelegateCommand((x) =>
            {
                Interaction.Success = true;
                FinishInteraction();
            });
        }

        protected override void HandleInteractionObjectChanged()
        {
            base.HandleInteractionObjectChanged();
            _regionManager.RequestNavigate(RegionNames.ProfileWorkflowEditorOverlayRegion, Interaction.View);
            UpdateHelpTopic?.Invoke();
        }

        public override string Title => $"{Translation.ModifySettings} {Interaction.Title}";
    }

    public class DesignTimeWorkflowEditorOverlayViewModel : WorkflowEditorOverlayViewModel
    {
        public DesignTimeWorkflowEditorOverlayViewModel(IRegionManager regionManager, ITranslationUpdater translationUpdater) : base(regionManager, translationUpdater)
        {
        }
    }
}
