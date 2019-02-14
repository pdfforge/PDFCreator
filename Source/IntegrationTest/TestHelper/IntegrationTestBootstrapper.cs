using pdfforge.PDFCreator.Conversion.Ghostscript;
using pdfforge.PDFCreator.Editions.EditionBase;
using pdfforge.PDFCreator.Editions.PDFCreator;
using SimpleInjector;

namespace PDFCreator.TestUtilities
{
    public class IntegrationTestBootstrapper
    {
        private readonly Bootstrapper _bootstrapper = new PDFCreatorBootstrapper();

        public Container ConfigureContainer()
        {
            var container = new Container();
            _bootstrapper.RegisterMainApplication(container);

            AddIntegrationTestRegistrations(container);
            OverrideRegistrations(container);

            return container;
        }

        protected virtual void AddIntegrationTestRegistrations(Container container)
        {
        }

        private void OverrideRegistrations(Container container)
        {
            container.Options.AllowOverridingRegistrations = true;

            container.Register<IGhostscriptDiscovery, PaketGhostscriptDiscovery>();

            container.Options.AllowOverridingRegistrations = false;
        }
    }
}
