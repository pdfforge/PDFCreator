using SimpleInjector;

namespace pdfforge.PDFCreator.Editions.EditionBase.Prism.SimpleInjector
{
    public class PrismViewRegistration
    {
        private readonly Container _container;
        private readonly Bootstrapper _bootstrapper;

        public PrismViewRegistration(Container container, Bootstrapper bootstrapper)
        {
            _container = container;
            _bootstrapper = bootstrapper;
        }
    }
}
