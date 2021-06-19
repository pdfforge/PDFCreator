using pdfforge.PDFCreator.Conversion.ActionsInterface;
using SimpleInjector;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls
{
    public interface IActionLocator
    {
        IAction GetAction<T>() where T : class, IAction;
    }

    public class ActionLocator : IActionLocator
    {
        private readonly Container _container;

        public ActionLocator(Container container)
        {
            _container = container;
        }

        public IAction GetAction<T>() where T : class, IAction
        {
            var instance = _container.GetInstance<T>();
            return instance;
        }
    }
}
