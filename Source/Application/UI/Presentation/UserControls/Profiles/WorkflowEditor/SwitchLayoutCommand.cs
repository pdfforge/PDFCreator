using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Events;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.WorkflowEditor
{
    public class SwitchLayoutCommand : TranslatableAsyncCommandBase<SwitchLayoutTranslation>
    {
        private readonly IRegionManager _regionManager;
        private readonly IInteractionRequest _interactionRequest;
        private readonly ICurrentSettings<ObservableCollection<ConversionProfile>> _profiles;
        private readonly IActionOrderHelper _actionOrderHelper;
        private readonly ICurrentSettingsProvider _currentSettingsProvider;
        private readonly IEventAggregator _eventAggregator;
        private bool _isExecuting;

        public SwitchLayoutCommand(IRegionManager regionManager, IInteractionRequest interactionRequest, ICurrentSettings<ObservableCollection<ConversionProfile>> profiles, IActionOrderHelper actionOrderHelper, ITranslationUpdater translationUpdater, ICurrentSettingsProvider currentSettingsProvider, IEventAggregator eventAggregator) : base(translationUpdater)
        {
            _regionManager = regionManager;
            _interactionRequest = interactionRequest;
            _profiles = profiles;
            _actionOrderHelper = actionOrderHelper;
            _currentSettingsProvider = currentSettingsProvider;
            _eventAggregator = eventAggregator;
        }

        public override bool CanExecute(object parameter)
        {
            return !_isExecuting;
        }

        public override async Task ExecuteAsync(object parameter)
        {
            _isExecuting = true;
            try
            {
                if (_currentSettingsProvider.SelectedProfile.EnableWorkflowEditor)
                {
                    var messageText = Translation.WarningLayoutSwitchCopy + Environment.NewLine + Translation.WantToContinue;
                    var messageInteraction = new MessageInteraction(messageText, Translation.WarningLayoutSwitchTitle, MessageOptions.YesNo, MessageIcon.Warning);
                    var interaction = await _interactionRequest.RaiseAsync(messageInteraction);
                    if (interaction.Response != MessageResponse.Yes)
                        return;
                }
                //switch state
                _currentSettingsProvider.SelectedProfile.EnableWorkflowEditor = !_currentSettingsProvider.SelectedProfile.EnableWorkflowEditor;

                foreach (var conversionProfile in _profiles.Settings)
                {
                    _actionOrderHelper.ForceDefaultOrder(conversionProfile);
                }

                if (_currentSettingsProvider.SelectedProfile.EnableWorkflowEditor)
                {
                    _regionManager.RequestNavigate(RegionNames.ProfileLayoutRegion, nameof(WorkflowEditorView));
                }
                else
                {
                    _regionManager.RequestNavigate(RegionNames.ProfileLayoutRegion, nameof(TabBasedProfileLayoutView));
                }
                _eventAggregator.GetEvent<SwitchWorkflowLayoutEvent>().Publish();
            }
            finally
            {
                _isExecuting = false;
            }
        }
    }
}
