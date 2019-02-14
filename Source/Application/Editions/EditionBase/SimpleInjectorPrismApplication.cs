using CommonServiceLocator;
using NLog;
using pdfforge.PDFCreator.Core.ServiceLocator;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.PrismHelper.Prism.SimpleInjector;
using Prism;
using Prism.Ioc;
using Prism.Regions;
using SimpleInjector;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace pdfforge.PDFCreator.Editions.EditionBase
{
    public class SimpleInjectorPrismApplication : PrismApplicationBase
    {
        private readonly Container _container;

        private IAppStart _appStart;
        private ISettingsManager _settingsManager;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public SimpleInjectorPrismApplication(Container container)
        {
            _container = container;
        }

        public void InitApplication(IAppStart appStart, HelpCommandHandler helpCommandHandler, ISettingsManager settingsManager)
        {
            _appStart = appStart;
            _settingsManager = settingsManager;
            helpCommandHandler.RegisterHelpCommandBinding();
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            RegisterXamlCulture();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                if (!_appStart.SkipStartupConditionCheck)
                {
                    // Load settings to have the proper translation available
                    _settingsManager.LoadAllSettings();

                    _appStart.CheckApplicationConditions();
                }
            }
            catch (StartupConditionFailedException ex)
            {
                _logger.Error($"Error while starting the application: {ex.Message} ({ex.ExitCode})");
                Shutdown(ex.ExitCode);
                return;
            }

            var exitCode = _appStart.Run();
            Shutdown((int)exitCode);
        }

        protected override IContainerExtension CreateContainerExtension()
        {
            return new SimpleInjectorContainerExtension(_container);
        }

        protected override void RegisterRequiredTypes(IContainerRegistry containerRegistry)
        {
            base.RegisterRequiredTypes(containerRegistry);

            _container.RegisterSingleton<IServiceLocator, SimpleInjectorServiceLocator>();

            var whitelistedServiceLocator = new WhitelistedServiceLocator(_container);
            RestrictedServiceLocator.Current = whitelistedServiceLocator;
            _container.RegisterSingleton<IWhitelistedServiceLocator>(() => whitelistedServiceLocator);

            _container.RegisterSingleton<IShellManager, ShellManager>();

            _container.RegisterSingleton<IRegionNavigationContentLoader, RegionNavigationContentLoader>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        private void RegisterXamlCulture()
        {
            // Ensure the current culture passed into bindings is the OS culture.
            // By default, WPF uses en-US as the culture, regardless of the system settings.
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }

        protected override Window CreateShell()
        {
            // null to prevent Prism from creating an unused window
            return null;
        }
    }
}
