using GongSolutions.Wpf.DragDrop;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.ProfileHasNotSupportedFeaturesExtension;
using pdfforge.PDFCreator.Conversion.Settings.Workflow;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using static System.String;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.WorkflowEditor
{
    public class WorkflowEditorViewModel : TranslatableViewModelBase<WorkflowEditorTranslation>, IMountable
    {
        private readonly ISelectedProfileProvider _selectedProfileProvider;
        private readonly ITranslationUpdater _translationUpdater;
        private readonly IInteractionRequest _interactionRequest;
        private readonly ITokenViewModelFactory _tokenViewModelFactory;
        private readonly IEventAggregator _eventAggregator;
        private readonly ICommandLocator _commandLocator;
        private readonly MetadataViewModel _metadataViewModel;

        private IEnumerable<IActionFacade> ActionFacades { get; }
        public DelegateCommand RemoveActionCommand { get; set; }
        public DelegateCommand EditActionCommand { get; set; }
        public DelegateCommand SetMetaDataCommand { get; set; }
        public DelegateCommand SetSaveCommand { get; set; }
        public DelegateCommand SetOutputFormatCommand { get; set; }
        public DelegateCommand SetPrinterCommand { get; set; }
        public ObservableCollection<IPresenterActionFacade> PreparationActions { get; set; }
        public ObservableCollection<IPresenterActionFacade> ModifyActions { get; set; }
        public ObservableCollection<IPresenterActionFacade> SendActions { get; set; }
        public IWorkflowEditorSubViewProvider ViewProvider;

        public OutputFormat OutputFormatDescription => _selectedProfileProvider.SelectedProfile?.OutputFormat ?? OutputFormat.Pdf;

        public string MetadataDescription => _selectedProfileProvider.SelectedProfile?.AuthorTemplate;

        public string TargetDirectory => GetTargetDirectoryText();
        public string TargetFilename => GetTargetFilenameText();
        public string AutoSaveText => GetAutoSaveText();

        public bool HasNotSupportedMetadataFeature => _selectedProfileProvider.SelectedProfile?.HasNotSupportedMetadata() ?? false;
        public bool HasNotSupportedConvertFeature => _selectedProfileProvider.SelectedProfile?.HasNotSupportedConvert() ?? false;

        public string TitlePreview => Translation.GetFormattedTitlePreview(_metadataViewModel.TitleTokenViewModel.Text);
        public string AuthorPreview => Translation.GetFormattedAuthorPreview(_metadataViewModel.AuthorTokenViewModel.Text);
        public string SubjectPreview => Translation.GetFormattedSubjectPreview(_metadataViewModel.SubjectTokenViewModel.Text);
        public string KeywordsPreview => Translation.GetFormattedKeywordsPreview(_metadataViewModel.KeywordsTokenViewModel.Text);

        public DelegateCommand OpenAddActionOverviewCommand { get; set; }

        public IDropTarget PreparationDropTarget { get; private set; }
        public IDropTarget ModifyDropTarget { get; private set; }
        public IDropTarget SendDropTarget { get; private set; }
        public IDragSource ModifyDragSourceHandler { get; }

        private bool _wasInit = false;

        public bool HasPreConversion => PreparationActions != null && PreparationActions.Count > 0;

        public WorkflowEditorViewModel(ISelectedProfileProvider selectedProfileProvider,
            ITranslationUpdater translationUpdater, MetadataViewModel metadataViewModel,
            IEnumerable<IActionFacade> actionFacades, IInteractionRequest interactionRequest,
             ITokenViewModelFactory tokenViewModelFactory,
            IEventAggregator eventAggregator, ICommandLocator commandLocator) : base(translationUpdater)
        {
            _selectedProfileProvider = selectedProfileProvider;
            _translationUpdater = translationUpdater;
            _interactionRequest = interactionRequest;
            _tokenViewModelFactory = tokenViewModelFactory;
            _eventAggregator = eventAggregator;
            _commandLocator = commandLocator;

            ActionFacades = actionFacades;
            _metadataViewModel = metadataViewModel;

            RemoveActionCommand = new DelegateCommand(ExecuteRemoveAction);
            EditActionCommand = new DelegateCommand(ExecuteEditAction);
            OpenAddActionOverviewCommand = new DelegateCommand(OpenAddActionOverview);

            SetMetaDataCommand = new DelegateCommand(async _ => await OpenEditDialog(ViewProvider.MetaDataOverlay, Translation.MetadataTab));
            SetSaveCommand = new DelegateCommand(async _ => await OpenEditDialog(ViewProvider.SaveOverlay, Translation.Save));
            SetOutputFormatCommand = new DelegateCommand(async _ => await OpenEditDialog(ViewProvider.OutputFormatOverlay, Translation.OutputFormat));
            SetPrinterCommand = new DelegateCommand(async _ => await OpenEditDialog((ViewProvider as ServerWorkflowEditorSubViewProvider)?.PrinterOverlay, Translation.OutputFormat));

            PreparationDropTarget = new WorkflowEditorActionDropTargetHandler<IPreConversionAction>();
            ModifyDropTarget = new WorkflowEditorActionDropTargetHandler<IConversionAction>();
            ModifyDragSourceHandler = new WorkflowEditorActionDragSourceHandler(obj =>
            {
                var facade = (IActionFacade)obj;
                var isAssignableFrom = typeof(IFixedOrderAction).IsAssignableFrom(facade.SettingsType);
                return !isAssignableFrom;
            });
            SendDropTarget = new WorkflowEditorActionDropTargetHandler<IPostConversionAction>();

            _translationUpdater.RegisterAndSetTranslation(tf => _metadataViewModel.SetTokenViewModels(tokenViewModelFactory));

            if (_selectedProfileProvider != null)
            {
                _selectedProfileProvider.SelectedProfileChanged += SelectedProfileOnPropertyChanged;
            }
        }

        private string GetTargetDirectoryText()
        {
            if (!IsNullOrEmpty(_selectedProfileProvider.SelectedProfile?.TargetDirectory))
                return _selectedProfileProvider.SelectedProfile?.TargetDirectory;

            if (_selectedProfileProvider.SelectedProfile != null && _selectedProfileProvider.SelectedProfile.AutoSave.Enabled)
                return Translation.MissingDirectory;

            return Translation.LastUsedDirectory;
        }

        private string GetTargetFilenameText()
        {
            var profile = _selectedProfileProvider.SelectedProfile;

            if (profile == null)
                return "";

            var formatHelper = new OutputFormatHelper();

            return formatHelper.EnsureValidExtension(profile.FileNameTemplate, profile.OutputFormat);
        }

        private string GetAutoSaveText()
        {
            var profile = _selectedProfileProvider.SelectedProfile;

            if (profile == null)
                return "";

            return profile.AutoSave.Enabled
                ? Translation.AutoSaveEnabled
                : Translation.AutoSaveDisabled;
        }

        private void SelectedProfileOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_wasInit)
            {
                GenerateCollectionViewsOfActions();

                UpdateActionProperties();

                UpdateConfigurationProperties();
            }
        }

        private async Task OpenEditDialog(string targetView, string title)
        {
            var settingsCopy = _selectedProfileProvider.SelectedProfile.Copy();
            var workflowEditorOverlayInteraction = new WorkflowEditorOverlayInteraction(false, title, targetView);

            await _interactionRequest.RaiseAsync(workflowEditorOverlayInteraction);
            if (!workflowEditorOverlayInteraction.Success && !settingsCopy.Equals(_selectedProfileProvider.SelectedProfile))
            {
                _selectedProfileProvider.SelectedProfile.ReplaceWith(settingsCopy);
            }

            GenerateCollectionViewsOfActions();
            UpdateConfigurationProperties();
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
            _selectedProfileProvider.SelectedProfile.ActionOrder.Clear();
            var newOrder = _selectedProfileProvider.SelectedProfile.ActionOrder;

            foreach (IPresenterActionFacade modifyAction in PreparationActions)
            {
                newOrder.Add(modifyAction.SettingsType.Name);
            }

            foreach (IPresenterActionFacade modifyAction in ModifyActions)
            {
                newOrder.Add(modifyAction.SettingsType.Name);
            }

            foreach (IPresenterActionFacade modifyAction in SendActions)
            {
                newOrder.Add(modifyAction.SettingsType.Name);
            }

            RaisePropertyChanged(nameof(HasPreConversion));
        }

        private List<IPresenterActionFacade> GenerateCollection()
        {
            var actionOrder = _selectedProfileProvider.SelectedProfile.ActionOrder;
            return actionOrder
                .Select(GetActionFacadeByTypeName)
                .Where(x => x != null)
                .ToList();
        }

        private IPresenterActionFacade GetActionFacadeByTypeName(string x)
        {
            var actions = ActionFacades.OfType<IPresenterActionFacade>();
            return actions.FirstOrDefault(y => y.SettingsType.Name == x);
        }

        private Func<IPresenterActionFacade, bool> FilterActionFacadeByType<TType>() where TType : IAction
        {
            return x => x.Action.GetInterfaces().Contains(typeof(TType)) && x.IsEnabled;
        }

        public void MountView()
        {
            GenerateCollectionViewsOfActions();

            _eventAggregator.GetEvent<ActionAddedToWorkflowEvent>().Subscribe(RefreshView);
            _wasInit = true;

            UpdateConfigurationProperties();
        }

        private async void OpenAddActionOverview(object obj)
        {
            await _interactionRequest.RaiseAsync(new AddActionOverlayInteraction(false));
        }

        private void RefreshView()
        {
            GenerateCollectionViewsOfActions();
        }

        private IProfileSetting CopySetting(IProfileSetting setting)
        {
            var copyMethod = setting?.GetType().GetMethod(nameof(ConversionProfile.Copy));
            return (IProfileSetting)copyMethod?.Invoke(setting, null);
        }

        private async void ExecuteEditAction(object obj)
        {
            var actionFacade = (IPresenterActionFacade)obj;
            var settingsCopy = CopySetting(actionFacade.ProfileSetting);
            var workflowEditorOverlayInteraction = new WorkflowEditorOverlayInteraction(false, actionFacade.Translation, actionFacade.OverlayView);

            await _interactionRequest.RaiseAsync(workflowEditorOverlayInteraction);
            if (!workflowEditorOverlayInteraction.Success)
                actionFacade.ProfileSetting = settingsCopy;

            GenerateCollectionViewsOfActions();
            UpdateConfigurationProperties();
        }

        private void UpdateActionProperties()
        {
            RaisePropertyChanged(nameof(PreparationActions));
            RaisePropertyChanged(nameof(ModifyActions));
            RaisePropertyChanged(nameof(SendActions));
        }

        private void UpdateConfigurationProperties()
        {
            _metadataViewModel.SetTokenViewModels(_tokenViewModelFactory);

            RaisePropertyChanged(nameof(OutputFormatDescription));
            RaisePropertyChanged(nameof(TargetDirectory));
            RaisePropertyChanged(nameof(TargetFilename));
            RaisePropertyChanged(nameof(AutoSaveText));
            RaisePropertyChanged(nameof(MetadataDescription));

            RaisePropertyChanged(nameof(TitlePreview));
            RaisePropertyChanged(nameof(AuthorPreview));
            RaisePropertyChanged(nameof(SubjectPreview));
            RaisePropertyChanged(nameof(KeywordsPreview));

            RaisePropertyChanged(nameof(HasNotSupportedConvertFeature));
            RaisePropertyChanged(nameof(HasNotSupportedMetadataFeature));
        }

        private void ExecuteRemoveAction(object actionFacade)
        {
            _commandLocator.GetCommand<RemoveActionCommand>().Execute(actionFacade);

            GenerateCollectionViewsOfActions();
        }

        public void UnmountView()
        {
            _eventAggregator.GetEvent<ActionAddedToWorkflowEvent>().Unsubscribe(RefreshView);
        }
    }
}
