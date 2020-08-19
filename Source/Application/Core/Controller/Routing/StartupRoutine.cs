using System.Collections.Generic;
using System.Linq;

namespace pdfforge.PDFCreator.Core.Controller.Routing
{
    public class StartupRoutine : IStartupRoutine
    {
        private List<IStartupAction> _startupActions = new List<IStartupAction>();

        public StartupRoutine()
        {
        }

        public StartupRoutine(params IStartupAction[] startupActions)
        {
            _startupActions = startupActions.ToList();
        }

        public StartupRoutine(IEnumerable<IStartupAction> startupActions)
        {
            _startupActions = startupActions.ToList();
        }

        public void AddAction(IStartupAction action)
        {
            _startupActions.Add(action);
        }

        public void OverrideRoutine(StartupRoutine routine)
        {
            _startupActions = new List<IStartupAction>();
            for (int i = 0; i < routine._startupActions.Count; i++)
            {
                _startupActions.Add(routine._startupActions[i]);
            }
        }

        public List<TType> GetActionByType<TType>() where TType : class, IStartupAction
        {
            var actionByType = new List<TType>();
            foreach (var startupRoutine in _startupActions)
            {
                if (startupRoutine is TType)
                {
                    actionByType.Add(startupRoutine as TType);
                }
            }
            return actionByType;
        }

        public IEnumerable<IStartupAction> GetAllActions()
        {
            return _startupActions;
        }
    }

    public interface IStartupRoutine
    {
        void AddAction(IStartupAction action);

        void OverrideRoutine(StartupRoutine routine);

        List<TType> GetActionByType<TType>() where TType : class, IStartupAction;

        IEnumerable<IStartupAction> GetAllActions();
    }
}
