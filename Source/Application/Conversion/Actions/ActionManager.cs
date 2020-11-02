using NLog;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Workflow;
using System.Collections.Generic;
using System.Linq;

namespace pdfforge.PDFCreator.Conversion.Actions
{
    public class ActionManager : IActionManager
    {
        private readonly IEnumerable<IAction> _allActions;
        private readonly IList<IActionFacade> _actionFacades;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ActionManager(IEnumerable<IAction> allActions, IEnumerable<IActionFacade> actionFacades)
        {
            _allActions = allActions.ToList();
            _actionFacades = actionFacades.ToList();
        }

        public IEnumerable<T> GetActions<T>(ConversionProfile profile) where T : IAction
        {
            var list = CreateActionListWithOrder(profile)
                   .AddRemainingActions(_allActions);

            return list.FilterForEnabledInProfile<T>(profile);
        }

        private List<IAction> CreateActionListWithOrder(ConversionProfile profile)
        {
            var orderedWorkflowList = profile.ActionOrder.Select(s => _actionFacades.FirstOrDefault(x => x.SettingsType.Name == s));
            return orderedWorkflowList.Select(actionFacade => _allActions.FirstOrDefault(action => actionFacade != null && action.GetType() == actionFacade.Action)).ToList();
        }

        public IEnumerable<T> GetActions<T>(Job job) where T : IAction
        {
            return GetActions<T>(job.Profile);
        }
    }

    internal static class ActionListExtension
    {
        public static List<IAction> AddRemainingActions(this List<IAction> list, IEnumerable<IAction> allActions)
        {
            var returnList = list.ToList();
            foreach (var action in allActions)
            {
                if (!returnList.Contains(action))
                    returnList.Add(action);
            }

            return returnList;
        }

        public static IEnumerable<TActionType> FilterForEnabledInProfile<TActionType>(this List<IAction> list, ConversionProfile profile) where TActionType : IAction
        {
            return list.OfType<TActionType>().Where(x => x.IsEnabled(profile));
        }
    }
}
