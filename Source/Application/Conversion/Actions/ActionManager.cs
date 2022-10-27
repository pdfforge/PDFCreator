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
            var enabledActionsOfType = _allActions.OfType<TAction>().Where(x => x.IsEnabled(profile)).ToList();

            //Copy the action order to allow changes in Pre/PostConversionScriptAction 
            var actionOrderCopy = new List<string>(profile.ActionOrder);
            
            //Attach enabled actions that are not in the action order list
            foreach (var action in enabledActionsOfType)
            {
                if (!profile.ActionOrder.Contains(action.SettingsType.Name))
                    actionOrderCopy.Add(action.SettingsType.Name);
            }
            
            var orderedWorkflowList = actionOrderCopy
                .Where(s => enabledActionsOfType.Any(x => x.SettingsType.Name == s))
                .Select(s => enabledActionsOfType.First(x => x.SettingsType.Name == s));
            return orderedWorkflowList;
        }

        public bool HasSendActions(ConversionProfile profile)
        {
            return _allActions
                .OfType<IPostConversionAction>()
                .Where(a => !(a is OpenFileAction))
                .Any(a => a.IsEnabled(profile));
        }
    }
}
