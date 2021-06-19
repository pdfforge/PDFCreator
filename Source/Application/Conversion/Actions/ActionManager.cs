using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using System.Collections.Generic;
using System.Linq;

namespace pdfforge.PDFCreator.Conversion.Actions
{
    public class ActionManager : IActionManager
    {
        private readonly IEnumerable<IAction> _allActions;

        public ActionManager(IEnumerable<IAction> allActions)
        {
            _allActions = allActions.ToList();
        }

        public IEnumerable<T> GetEnabledActionsInCurrentOrder<T>(Job job) where T : IAction
        {
            return GetEnabledActionsInCurrentOrder<T>(job.Profile);
        }

        public IEnumerable<TAction> GetEnabledActionsInCurrentOrder<TAction>(ConversionProfile profile) where TAction : IAction
        {
            //Filter the action types in advance, since Pre- and PostConversionScriptAction share the settings type
            var actionsOfType = _allActions.OfType<TAction>();
            var orderedWorkflowList = profile.ActionOrder
                .Where(s => actionsOfType.Any(x => x.SettingsType.Name == s))
                .Select(s => actionsOfType.First(x => x.SettingsType.Name == s));
            return orderedWorkflowList.Where(x => x.IsEnabled(profile));
        }

        public bool HasSendActions(ConversionProfile profile)
        {
            return _allActions
                .OfType<IPostConversionAction>()
                .Where(a => !(a is DefaultViewerAction))
                .Any(a => a.IsEnabled(profile));
        }
    }
}
