using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using System;
using pdfforge.PDFCreator.Core.SettingsManagement.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Background;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.ActionHelper
{
    public interface IPresenterActionFacade
    {
        Type ActionType { get; }

        Type SettingsType { get; }

        string OverlayViewName { get; }

        IActionViewModel ActionViewModel { get; }

        string Title { get; }

        string InfoText { get; }

        bool IsEnabled { get; }

        IProfileSetting GetCurrentSettingCopy();

        void ReplaceCurrentSetting(IProfileSetting profileSetting);

        void AddAction();

        void RemoveAction();
    }

    public class PresenterActionFacade<TActionUserControl> : IPresenterActionFacade
        where TActionUserControl : IActionUserControl
    {
        private readonly IActionOrderHelper _actionOrderHelper;

        public PresenterActionFacade(TActionUserControl actionUserControl, IActionOrderHelper actionOrderHelper)
        {
            _actionOrderHelper = actionOrderHelper;
            OverlayViewName = actionUserControl.GetType().Name;
            ActionViewModel = actionUserControl.ViewModel;
        }

        public string OverlayViewName { get; }

        public IActionViewModel ActionViewModel { get; }

        public Type ActionType => ActionViewModel.Action.GetType();

        public Type SettingsType => ActionViewModel.Action.SettingsType;

        public string Title => ActionViewModel.Title;

        public string InfoText => ActionViewModel.InfoText;

        public bool IsEnabled => ActionViewModel.IsEnabled;

        public void ReplaceCurrentSetting(IProfileSetting profileSetting)
        {
            ActionViewModel.ReplaceCurrentSetting(profileSetting);
        }

        public IProfileSetting GetCurrentSettingCopy()
        {
            return ActionViewModel.GetCurrentSettingCopy();
        }

        public void AddAction()
        {
            ActionViewModel.AddAction();
        }

        public void RemoveAction()
        {
            ActionViewModel.RemoveAction();
        }

        public override string ToString()
        {
            // changed for Debug
            return $"Action Facade:{ActionViewModel.Action.GetType().Name}";
        }
    }
}
