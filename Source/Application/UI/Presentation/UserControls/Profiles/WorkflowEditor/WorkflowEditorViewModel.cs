using GongSolutions.Wpf.DragDrop;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.Workflow;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.ActionHelper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.WorkflowEditor
{
    public class WorkflowEditorViewModel : ProfileUserControlViewModel<WorkflowEditorTranslation>, IMountable
    {
        public bool IsServer { get; private set; }

        private readonly IInteractionRequest _interactionRequest;
        private readonly IEventAggregator _eventAggregator;
        private readonly ICommandLocator _commandLocator;

        private IEnumerable<IPresenterActionFacade> ActionFacades { get; }
        public DelegateCommand RemoveActionCommand { get; set; }
        public DelegateCommand EditActionCommand { get; set; }

        public ObservableCollection<IPresenterActionFacade> PreparationActions { get; set; }
        public ObservableCollection<IPresenterActionFacade> ModifyActions { get; set; }
        public ObservableCollection<IPresenterActionFacade> SendActions { get; set; }

        public DelegateCommand OpenAddActionOverviewCommand { get; set; }

        public IDropTarget PreparationDropTarget { get; private set; }
        public IDropTarget ModifyDropTarget { get; private set; }
        public IDropTarget SendDropTarget { get; private set; }
        public IDragSource ModifyDragSourceHandler { get; }

        private bool _wasInit = false;

        public bool HasPreConversion => PreparationActions != null && PreparationActions.Count > 0;

        public WorkflowEditorViewModel(ISelectedProfileProvider selectedProfileProvider,
            ITranslationUpdater translationUpdater,
            IEnumerable<IPresenterActionFacade> actionFacades,
            IInteractionRequest interactionRequest,
            IEventAggregator eventAggregator,
            ICommandLocator commandLocator,
            IWorkflowEditorSubViewProvider viewProvider,
            ICommandBuilderProvider commandBuilderProvider,
            IDispatcher dispatcher,
            EditionHelper editionHelper
        ) : base(translationUpdater, selectedProfileProvider, dispatcher)
        {
            IsServer = editionHelper.IsServer;

            _interactionRequest = interactionRequest;
            _eventAggregator = eventAggregator;
            _commandLocator = commandLocator;

            ActionFacades = actionFacades;

            RemoveActionCommand = new DelegateCommand(ExecuteRemoveAction);
            EditActionCommand = new DelegateCommand(ExecuteEditAction);
            OpenAddActionOverviewCommand = new DelegateCommand(OpenAddActionOverview);

            PreparationDropTarget = new WorkflowEditorActionDropTargetHandler<IPreConversionAction>();
            ModifyDropTarget = new WorkflowEditorActionDropTargetHandler<IConversionAction>();
            ModifyDragSourceHandler = new WorkflowEditorActionDragSourceHandler(obj =>
            {
                var facade = (IPresenterActionFacade)obj;
                var isAssignableFrom = typeof(IFixedOrderAction).IsAssignableFrom(facade.SettingsType);
                return !isAssignableFrom;
            });

            SendDropTarget = new WorkflowEditorActionDropTargetHandler<IPostConversionAction>();

            selectedProfileProvider.SelectedProfileChanged += SelectedProfileOnPropertyChanged;

            eventAggregator.GetEvent<WorkflowSettingsChanged>().Subscribe(GenerateCollectionViewsOfActions);
        }

        private void SelectedProfileOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_wasInit)
            {
                GenerateCollectionViewsOfActions();

                UpdateActionProperties();
            }
        }

        private void GenerateCollectionViewsOfActions()
        {
            if (PreparationActions != null)
                PreparationActions.CollectionChanged -= OnActionCollectionChanged;

            if (ModifyActions != null)
                ModifyActions.CollectionChanged -= OnActionCollectionChanged;

            if (SendActions != null)
                SendActions.CollectionChanged -= OnActionCollectionChanged;

            var actions = GenerateCollection();
            PreparationActions = actions.Where(FilterActionFacadeByType<IPreConversionAction>()).ToObservableCollection();
            ModifyActions = actions.Where(FilterActionFacadeByType<IConversionAction>()).ToObservableCollection();
            SendActions = actions.Where(FilterActionFacadeByType<IPostConversionAction>()).ToObservableCollection();

            PreparationActions.CollectionChanged += OnActionCollectionChanged;
            ModifyActions.CollectionChanged += OnActionCollectionChanged;
            SendActions.CollectionChanged += OnActionCollectionChanged;

            UpdateActionProperties();
            RaisePropertyChanged(nameof(HasPreConversion));
        }

        private void OnActionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateOrder();
        }

        private void UpdateOrder()
        {
            CurrentProfile.ActionOrder.Clear();
            var newOrder = CurrentProfile.ActionOrder;

            foreach (IPresenterActionFacade action in PreparationActions)
            {
                newOrder.Add(action.SettingsType.Name);
            }

            foreach (IPresenterActionFacade action in ModifyActions)
            {
                newOrder.Add(action.SettingsType.Name);
            }

            foreach (IPresenterActionFacade action in SendActions)
            {
                newOrder.Add(action.SettingsType.Name);
            }

            RaisePropertyChanged(nameof(HasPreConversion));
        }

        private List<IPresenterActionFacade> GenerateCollection()
        {
            var actionOrder = CurrentProfile.ActionOrder;
            return actionOrder
                .Select(GetActionFacadeByTypeName)
                .Where(x => x != null)
                .ToList();
        }

        private IPresenterActionFacade GetActionFacadeByTypeName(string x)
        {
            return ActionFacades.FirstOrDefault(y => y.SettingsType.Name == x);
        }

        private Func<IPresenterActionFacade, bool> FilterActionFacadeByType<TType>() where TType : IAction
        {
            return x => x.ActionType.GetInterfaces().Contains(typeof(TType)) && x.IsEnabled;
        }

        public override void MountView()
        {
            GenerateCollectionViewsOfActions();

            _eventAggregator.GetEvent<ActionAddedToWorkflowEvent>().Subscribe(RefreshView);
            _wasInit = true;
        }

        private async void OpenAddActionOverview(object obj)
        {
            await _interactionRequest.RaiseAsync(new AddActionOverlayInteraction());
        }

        private void RefreshView()
        {
            GenerateCollectionViewsOfActions();
        }

        private async void ExecuteEditAction(object obj)
        {
            var actionFacade = (IPresenterActionFacade)obj;
            var settingsCopy = actionFacade.GetCurrentSettingCopy();
            var workflowEditorOverlayInteraction = new WorkflowEditorOverlayInteraction(actionFacade.Title, actionFacade.OverlayViewName, false, false);

            await _interactionRequest.RaiseAsync(workflowEditorOverlayInteraction);
            if (workflowEditorOverlayInteraction.Result != WorkflowEditorOverlayResult.Success)
                actionFacade.ReplaceCurrentSetting(settingsCopy);

            GenerateCollectionViewsOfActions();
        }

        private void UpdateActionProperties()
        {
            RaisePropertyChanged(nameof(PreparationActions));
            RaisePropertyChanged(nameof(ModifyActions));
            RaisePropertyChanged(nameof(SendActions));
        }

        private void ExecuteRemoveAction(object actionFacade)
        {
            _commandLocator.GetCommand<RemoveActionCommand>().Execute(actionFacade);

            GenerateCollectionViewsOfActions();
        }

        public override void UnmountView()
        {
            _eventAggregator.GetEvent<ActionAddedToWorkflowEvent>().Unsubscribe(RefreshView);
        }
    }
}
