using NLog;
using pdfforge.Communication;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.DirectConversion;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Logging;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Startup;
using pdfforge.PDFCreator.ErrorReport;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.Utilities;
using Prism.Regions;
using SimpleInjector;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using Application = System.Windows.Forms.Application;

namespace pdfforge.PDFCreator.Editions.EditionBase
{
    public class ProgramBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static ErrorReportHelper _errorReportHelper;
        private static Container _container;

        public static void Main(string[] args, Func<Bootstrapper> getBootstrapperFunc)
        {
            var globalMutex = AcquireMutex();
            var exitCode = 0;

            try
            {
                CustomBindingResolver.WatchBindingResolution();

                _container = new Container();
                exitCode = StartApplication(args, getBootstrapperFunc);
            }
            finally
            {
                globalMutex.Release();
                _container?.Dispose();

                Logger.Debug("Ending PDFCreator");
                Environment.Exit(exitCode);
            }
        }

        private static int StartApplication(string[] args, Func<Bootstrapper> getBootstrapperFunc)
        {
            Application.EnableVisualStyles();

            Thread.CurrentThread.Name = "ProgramThread";

            InitializeLogging();
            Logger.Debug("Starting PDFCreator");

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            var application = new SimpleInjectorPrismApplication(_container);
            BootstrapContainerAndApplication(getBootstrapperFunc(), application);

            InitializeApplication(args, application);

            VerifyContainer();

            UpdateErrorReportHelper(_container);

            return application.Run();
        }

        private static void VerifyContainer()
        {
            if (Debugger.IsAttached)
                _container.Verify(VerificationOption.VerifyOnly);
        }

        private static void InitializeApplication(string[] args, SimpleInjectorPrismApplication application)
        {
            var resolver = new SimpleInjectorAppStartResolver(_container);
            var appStartFactory = new AppStartFactory(resolver, _container.GetInstance<IPathUtil>(), _container.GetInstance<IDirectConversionHelper>());
            var appStart = appStartFactory.CreateApplicationStart(args);
            var helpCommandHandler = _container.GetInstance<HelpCommandHandler>();
            var settingsManager = _container.GetInstance<ISettingsManager>();

            application.InitApplication(appStart, helpCommandHandler, settingsManager);
            application.DispatcherUnhandledException += Application_DispatcherUnhandledException;
        }

        private static void BootstrapContainerAndApplication(Bootstrapper bootstrapper, SimpleInjectorPrismApplication application)
        {
            bootstrapper.RegisterMainApplication(_container);
            bootstrapper.RegisterPrismNavigation(_container);
            SetupObsidian(bootstrapper);

            application.Initialize();

            bootstrapper.RegisterEditionDependentRegions(_container.GetInstance<IRegionManager>());
        }

        private static void SetupObsidian(Bootstrapper bootstrapper)
        {
            bootstrapper.RegisterObsidianInteractions();

            ViewRegistry.WindowFactory = new DelegateWindowFactory(content =>
            {
                var window = _container.GetInstance<InteractionHostWindow>();
                window.Content = content;

                return window;
            });
        }

        private static void InitializeLogging()
        {
            LoggingHelper.InitFileLogger("PDFCreator", LoggingLevel.Error);
            var inMemoryLogger = new InMemoryLogger(100);
            inMemoryLogger.Register();
            var assembly = typeof(ProgramBase).Assembly;
            var versionHelper = new VersionHelper(assembly);

            var errorHelper = new ErrorHelper("pdfcreator", "PDFCreator", versionHelper.ApplicationVersion, Urls.SentryDsnUrl);

            _errorReportHelper = new ErrorReportHelper(inMemoryLogger, new AssemblyHelper(assembly), errorHelper);
        }

        private static void UpdateErrorReportHelper(Container container)
        {
            var applicationNameProvider = container.GetInstance<ApplicationNameProvider>();
            var versionHelper = container.GetInstance<IVersionHelper>();
            _errorReportHelper.ErrorHelper = new ErrorHelper(applicationNameProvider.ProductIdentifier, applicationNameProvider.ApplicationNameWithEdition, versionHelper.ApplicationVersion, Urls.SentryDsnUrl);

            var languageProvider = container.GetInstance<IApplicationLanguageProvider>();

            void UpdateErrorHelperLanguage()
            {
                _errorReportHelper.ErrorHelper.CurrentUiLanguage = languageProvider.GetApplicationLanguage();
            }

            languageProvider.LanguageChanged += (sender, args) => UpdateErrorHelperLanguage();
            UpdateErrorHelperLanguage();
        }

        private static GlobalMutex AcquireMutex()
        {
            var globalMutex = new GlobalMutex("PDFCreator-137a7751-1070-4db4-a407-83c1625762c7");
            globalMutex.Acquire();

            return globalMutex;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            Logger.Fatal(ex, "Uncaught exception, IsTerminating: {0}", e.IsTerminating);
            _errorReportHelper.ShowErrorReportInNewProcess(ex);
        }

        private static void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Error(e.Exception, "Uncaught exception in WPF thread");

            e.Handled = true;

            _errorReportHelper.ShowErrorReport(e.Exception);

            System.Windows.Application.Current.Shutdown(1);
        }
    }
}
