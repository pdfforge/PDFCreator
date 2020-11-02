using GongSolutions.Wpf.DragDrop;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.Workflow;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.WorkflowEditor.Commands;
using pdfforge.PDFCreator.Utilities;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.WorkflowEditor
{
    public class WorkflowEditorViewModel : ProfileUserControlViewModel<WorkflowEditorTranslation>, IMountable
    {
        private readonly IInteractionRequest _interactionRequest;
        private readonly IEventAggregator _eventAggregator;
        private readonly ICommandLocator _commandLocator;

        private IEnumerable<IActionFacade> ActionFacades { get; }
        public DelegateCommand RemoveActionCommand { get; set; }
        public DelegateCommand EditActionCommand { get; set; }
        public ICommand SetMetaDataCommand { get; set; }
        public ICommand SetSaveCommand { get; set; }
        public ICommand SetOutputFormatCommand { get; set; }
        public ICommand SetPrinterCommand { get; set; }
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
            IEnumerable<IActionFacade> actionFacades,
            IInteractionRequest interactionRequest,
            IEventAggregator eventAggregator,
            ICommandLocator commandLocator,
            IWorkflowEditorSubViewProvider viewProvider,
            ICommandBuilderProvider commandBuilderProvider,
            IDispatcher dispatcher
        ) : base(translationUpdater, selectedProfileProvider, dispatcher)
        {
            _interactionRequest = interactionRequest;
            _eventAggregator = eventAggregator;
            _commandLocator = commandLocator;

            ActionFacades = actionFacades;

            RemoveActionCommand = new DelegateCommand(ExecuteRemoveAction);
            EditActionCommand = new DelegateCommand(ExecuteEditAction);
            OpenAddActionOverviewCommand = new DelegateCommand(OpenAddActionOverview);

            var UpdateSettingsPreviewsCommand = new DelegateCommand(o => UpdateSettingsPreviews());

            SetMetaDataCommand = commandBuilderProvider.ProvideBuilder(_commandLocator)
                .AddInitializedCommand<WorkflowEditorCommand>(c => c.Initialize(viewProvider.MetaDataOverlay, t => t.MetaData))
                .AddCommand(UpdateSettingsPreviewsCommand)
                .Build();

            SetSaveCommand = commandBuilderProvider.ProvideBuilder(_commandLocator)
                .AddInitializedCommand<WorkflowEditorCommand>(c => c.Initialize(viewProvider.SaveOverlay, t => t.Save))
                .AddCommand(UpdateSettingsPreviewsCommand)
                .Build();

            SetOutputFormatCommand = commandBuilderProvider.ProvideBuilder(_commandLocator)
                .AddInitializedCommand<WorkflowEditorCommand>(c => c.Initialize(viewProvider.OutputFormatOverlay, t => t.OutputFormat))
                .AddCommand(UpdateSettingsPreviewsCommand)
                .Build();

            SetPrinterCommand = commandBuilderProvider.ProvideBuilder(_commandLocator)
                .AddInitializedCommand<WorkflowEditorCommand>(c => c.Initialize((viewProvider as ServerWorkflowEditorSubViewProvider)?.PrinterOverlay, t => t.Printer))
                .Build();

            PreparationDropTarget = new WorkflowEditorActionDropTargetHandler<IPreConversionAction>();
            ModifyDropTarget = new WorkflowEditorActionDropTargetHandler<IConversionAction>();
            ModifyDragSourceHandler = new WorkflowEditorActionDragSourceHandler(obj =>
            {
                var facade = (IActionFacade)obj;
                var isAssignableFrom = typeof(IFixedOrderAction).IsAssignableFrom(facade.SettingsType);
                return !isAssignableFrom;
            });

            SendDropTarget = new WorkflowEditorActionDropTargetHandler<IPostConversionAction>();

            selectedProfileProvider.SelectedProfileChanged += SelectedProfileOnPropertyChanged;

            eventAggregator.GetEvent<WorkflowSettingsChanged>().Subscribe(() =>
            {
                GenerateCollectionViewsOfActions();
                UpdateSettingsPreviews();
            });
        }

        #region Save

        public bool AutoSaveEnabled => CurrentProfile != null && CurrentProfile.AutoSave.Enabled;

        public string TargetFilename
        {
            get
            {
                if (CurrentProfile == null)
                    return "";

                var formatHelper = new OutputFormatHelper();
                return formatHelper.EnsureValidExtension(CurrentProfile.FileNameTemplate, CurrentProfile.OutputFormat);
            }
        }

        public string TargetDirectory
        {
            get
            {
                if (CurrentProfile == null)
                    return "";

                if (CurrentProfile.SaveFileTemporary)
                    return Translation.SaveOnlyTemporary;

                if (!string.IsNullOrEmpty(CurrentProfile.TargetDirectory))
                    return CurrentProfile.TargetDirectory;

                if (CurrentProfile.AutoSave.Enabled)
                    return Translation.MissingDirectory;

                return Translation.LastUsedDirectory;
            }
        }

        public bool HasMissingDirectory => TargetDirectory == Translation.MissingDirectory;

        public bool SkipPrintDialog => CurrentProfile != null && CurrentProfile.SkipPrintDialog;

        public bool ShowQuickActions => CurrentProfile != null && CurrentProfile.ShowQuickActions;

        public bool EnsureUniqueFilenames => CurrentProfile != null && CurrentProfile.AutoSave.EnsureUniqueFilenames;

        public bool ShowTrayNotification => CurrentProfile != null && CurrentProfile.ShowAllNotifications;

        #endregion Save

        #region OutputFormat

        public string OutputFormatString => CurrentProfile == null ? "" : CurrentProfile.OutputFormat.GetDescription();

        public string ResolutionCompressionLabel
        {
            get
            {
                if (CurrentProfile == null)
                    return "";

                if (!CurrentProfile.OutputFormat.IsPdf())
                    return Translation.ResolutionLabel;

                return Translation.CompressionLabel;
            }
        }

        public string Colors
        {
            get
            {
                if (CurrentProfile == null)
                    return "";

                try
                {
                    if (CurrentProfile.OutputFormat.IsPdf())
                    {
                        if (CurrentProfile.OutputFormat == OutputFormat.PdfX
                            && CurrentProfile.PdfSettings.ColorModel == ColorModel.Rgb)
                            return Translation.PdfColorValues[(int)ColorModel.Cmyk].Translation;

                        return Translation.PdfColorValues[(int)CurrentProfile.PdfSettings.ColorModel].Translation;
                    }

                    switch (CurrentProfile.OutputFormat)
                    {
                        case OutputFormat.Jpeg:
                            return Translation.JpegColorValues[(int)CurrentProfile.JpegSettings.Color].Translation;

                        case OutputFormat.Png:
                            return Translation.PngColorValues[(int)CurrentProfile.PngSettings.Color].Translation;

                        case OutputFormat.Tif:
                            return Translation.TiffColorValues[(int)CurrentProfile.TiffSettings.Color].Translation;

                        case OutputFormat.Txt:
                            return "./.";
                    }
                }
                catch { }

                return "";
            }
        }

        public string ResolutionCompression
        {
            get
            {
                if (CurrentProfile == null)
                    return "";

                if (CurrentProfile.OutputFormat.IsPdf())
                    return Translation.CompressionValues[(int)CurrentProfile.PdfSettings.CompressColorAndGray.Compression].Translation;

                switch (CurrentProfile.OutputFormat)
                {
                    case OutputFormat.Jpeg:
                        return CurrentProfile.JpegSettings.Dpi.ToString();

                    case OutputFormat.Png:
                        return CurrentProfile.PngSettings.Dpi.ToString();

                    case OutputFormat.Tif:
                        return CurrentProfile.TiffSettings.Dpi.ToString();

                    case OutputFormat.Txt:
                        return "./.";
                }

                return "";
            }
        }

        #endregion OutputFormat

        #region Metadata

        public bool ShowMetadata => CurrentProfile?.OutputFormat.IsPdf() ?? true;

        public string TitleTemplate => CurrentProfile == null ? "" : CurrentProfile.TitleTemplate;
        public string AuthorTemplate => CurrentProfile == null ? "" : CurrentProfile.AuthorTemplate;
        public string SubjectTemplate => CurrentProfile == null ? "" : CurrentProfile.SubjectTemplate;
        public string KeywordTemplate => CurrentProfile == null ? "" : CurrentProfile.KeywordTemplate;

        #endregion Metadata

        private void SelectedProfileOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_wasInit)
            {
                GenerateCollectionViewsOfActions();

                UpdateActionProperties();

                UpdateSettingsPreviews();
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
            var actionOrder = CurrentProfile.ActionOrder;
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

        public override void MountView()
        {
            GenerateCollectionViewsOfActions();

            _eventAggregator.GetEvent<ActionAddedToWorkflowEvent>().Subscribe(RefreshView);
            _wasInit = true;

            UpdateSettingsPreviews();
        }

        private async void OpenAddActionOverview(object obj)
        {
            await _interactionRequest.RaiseAsync(new AddActionOverlayInteraction());
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
            var workflowEditorOverlayInteraction = new WorkflowEditorOverlayInteraction(actionFacade.Translation, actionFacade.OverlayView, false, false);

            await _interactionRequest.RaiseAsync(workflowEditorOverlayInteraction);
            if (workflowEditorOverlayInteraction.Result != WorkflowEditorOverlayResult.Success)
                actionFacade.ProfileSetting = settingsCopy;

            GenerateCollectionViewsOfActions();
            UpdateSettingsPreviews();
        }

        private void UpdateActionProperties()
        {
            RaisePropertyChanged(nameof(PreparationActions));
            RaisePropertyChanged(nameof(ModifyActions));
            RaisePropertyChanged(nameof(SendActions));
        }

        private void UpdateSettingsPreviews()
        {
            RaisePropertyChanged(nameof(CurrentProfile));

            RaisePropertyChanged(nameof(AutoSaveEnabled));
            RaisePropertyChanged(nameof(TargetFilename));
            RaisePropertyChanged(nameof(TargetDirectory));
            RaisePropertyChanged(nameof(HasMissingDirectory));
            RaisePropertyChanged(nameof(SkipPrintDialog));
            RaisePropertyChanged(nameof(ShowQuickActions));
            RaisePropertyChanged(nameof(EnsureUniqueFilenames));
            RaisePropertyChanged(nameof(ShowTrayNotification));

            RaisePropertyChanged(nameof(OutputFormatString));
            RaisePropertyChanged(nameof(Colors));
            RaisePropertyChanged(nameof(ResolutionCompressionLabel));
            RaisePropertyChanged(nameof(ResolutionCompression));

            RaisePropertyChanged(nameof(ShowMetadata));
            RaisePropertyChanged(nameof(TitleTemplate));
            RaisePropertyChanged(nameof(AuthorTemplate));
            RaisePropertyChanged(nameof(SubjectTemplate));
            RaisePropertyChanged(nameof(KeywordTemplate));
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
