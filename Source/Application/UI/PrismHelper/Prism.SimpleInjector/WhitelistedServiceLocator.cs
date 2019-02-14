using pdfforge.PDFCreator.Core.ServiceLocator;
using SimpleInjector;

namespace pdfforge.PDFCreator.UI.PrismHelper.Prism.SimpleInjector
{
    public class WhitelistedServiceLocator : IWhitelistedServiceLocator
    {
        private readonly Container _container;

        public WhitelistedServiceLocator(Container container)
        {
            _container = container;
        }

        public T GetInstance<T>() where T : class, IWhitelisted
        {
            return _container.GetInstance<T>();
        }
    }
}
