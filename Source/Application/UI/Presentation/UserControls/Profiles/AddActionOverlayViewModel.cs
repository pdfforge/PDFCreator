using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Workflow;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.WorkflowEditor;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public class AddActionOverlayViewModel : OverlayViewModelBase<AddActionOverlayInteraction, AddActionOverlayViewTranslation>, IMountable
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ISelectedProfileProvider _selectedProfileProvider;

        public bool ShowInfoText { get; set; }
        public string InfoText { get; set; }
        public ICommand OkCommand { get; }
        public ICommand InfoActionCommand { get; set; }
        public ICommand HideInfoActionCommand { get; set; }
        public ICommand TriggerAddActionCommand { get; set; }
        private ICommand AddActionCommand { get; set; }
        public ObservableCollection<IPresenterActionFacade> PreparationActions { get; set; }
        public ObservableCollection<IPresenterActionFacade> ModifyActions { get; set; }
        public ObservableCollection<IPresenterActionFacade> SendActions { get; set; }
        public IEnumerable<IActionFacade> ActionFacades { get; private set; }

        public AddActionOverlayViewModel(IEventAggregator eventAggregator, ISelectedProfileProvider selectedProfileProvider,
            IEnumerable<IActionFacade> actionFacades, ITranslationUpdater translationUpdater, ICommandLocator commandLocator)
            : base(translationUpdater)
        {
            _eventAggregator = eventAggregator;
            _selectedProfileProvider = selectedProfileProvider; 
            ActionFacades = actionFacades;

            InfoActionCommand = new DelegateCommand(ShowActionInfo);
            HideInfoActionCommand = new DelegateCommand(HideActionInfo);
            AddActionCommand = commandLocator.GetCommand<AddActionCommand>();
            TriggerAddActionCommand = new DelegateCommand(TriggerAddAction);

            GenerateCollectionViewsOfActions();

            OkCommand = new DelegateCommand((x) =>
            {
                Interaction.Success = true;
                FinishInteraction();
            });
        }

        private Dictionary<string, int> _settingIndex = SetupSettingsIndex();

        private static Dictionary<string, int> SetupSettingsIndex()
        {
            var index = 1;
            return new List<string>
            {
                // pre action
                nameof(UserTokens),
                nameof(ForwardToFurtherProfile),

                // modify action
                nameof(CoverPage),
                nameof(AttachmentPage),
                nameof(Stamping),
                nameof(BackgroundPage),
                nameof(Watermark),
                nameof(Security),
                nameof(Signature),

                // send action
                nameof(OpenViewer),
                nameof(EmailClientSettings),
                nameof(Printing),
                nameof(Scripting),
                nameof(Ftp),
                nameof(EmailSmtpSettings),
                nameof(HttpSettings),
                nameof(DropboxSettings)
            }.ToDictionary(x => x, x => index++);
        }

        private int GetSettingsIndex(string settingsName)
        {
            if (_settingIndex.TryGetValue(settingsName, out var index))
                return index;

            return int.MaxValue;
        }

        private void HideActionInfo(object obj)
        {
            ShowInfoText = false;
            RaisePropertyChanged(nameof(ShowInfoText));
        }

        private void ShowActionInfo(object obj)
        {
            if (obj is IPresenterActionFacade presenterFacade)
                InfoText = presenterFacade.InfoText;

            ShowInfoText = true;
            RaisePropertyChanged(nameof(InfoText));
            RaisePropertyChanged(nameof(ShowInfoText));
        }

        private void TriggerAddAction(object obj)
        {
            FinishInteraction();

            AddActionCommand.Execute(obj);
        }

        private void ProfileProviderOnSelectedProfileChanged(object sender, PropertyChangedEventArgs e)
        {
            _eventAggregator.GetEvent<ActionAddedToWorkflowEvent>().Publish();
        }

        private ObservableCollection<IPresenterActionFacade> GetObservableActionsList<TAction>(IEnumerable<IPresenterActionFacade> actions) where TAction : IAction
        {
            var presenterActionFacades = actions.Where(FilterActionFacadeByType<TAction>()).ToList();
            presenterActionFacades.Sort(((x, y) => GetSettingsIndex(x.SettingsType.Name) - GetSettingsIndex(y.SettingsType.Name)));
            return presenterActionFacades.ToObservableCollection();
        }

        private void GenerateCollectionViewsOfActions()
        {
            var actions = ActionFacades.OfType<IPresenterActionFacade>().ToList();

            PreparationActions = GetObservableActionsList<IPreConversionAction>(actions);
            ModifyActions = GetObservableActionsList<IConversionAction>(actions);
            SendActions = GetObservableActionsList<IPostConversionAction>(actions);

            RaisePropertyChanged(nameof(ModifyActions));
            RaisePropertyChanged(nameof(SendActions));
        }

        private Func<IPresenterActionFacade, bool> FilterActionFacadeByType<TType>() where TType : IAction
        {
            return x => x.Action.GetInterfaces().Contains(typeof(TType));
        }

        public override string Title => $"{Translation.AddAction}";

        public void MountView()
        {
            _selectedProfileProvider.SelectedProfileChanged += ProfileProviderOnSelectedProfileChanged;
        }

        public void UnmountView()
        {
            _selectedProfileProvider.SelectedProfileChanged -= ProfileProviderOnSelectedProfileChanged;
        }
    }
}
