using CommonServiceLocator;
using NUnit.Framework;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.ServiceLocator;
using pdfforge.PDFCreator.Core.Services.Logging;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Startup.StartConditions;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Editions.EditionBase;
using pdfforge.PDFCreator.Editions.PDFCreator;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.PrismHelper.Prism.SimpleInjector;
using Prism.Events;
using Prism.Regions;
using SimpleInjector;
using SimpleInjector.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Translatable;

namespace pdfforge.PDFCreator.UnitTest.Editions.PDFCreator
{
    [TestFixture]
    public class BootstrapperTest
    {
        // For some classes, we accept that they are registered as singleton but have transient dependencies.
        // There must be a reason for this, i.e. that we want a single instance of this class, but don't want it to share its state with other usages
        private readonly Type[] _lifestyleMismatchAcceptableClasses =
        {
            typeof(IJobInfoQueueManager),
            typeof(IFileConversionAssistant),
            typeof(IUpdateAssistant),
        };

        private static IList<Bootstrapper> AllBootstrappers()
        {
            return new Bootstrapper[]
            {
                new PDFCreatorBootstrapper()
            }.ToList();
        }

        private Container BuildContainer(Bootstrapper bootstrapper)
        {
            var container = new Container();
            bootstrapper.RegisterMainApplication(container);
            bootstrapper.RegisterPrismNavigation(container);
            //bootstrapper.RegisterObsidianInteractions();
            return container;
        }

        private Container BuildPrismContainer(Bootstrapper bootstrapper)
        {
            var container = BuildContainer(bootstrapper);
            //var prismApplication = new SimpleInjectorPrismApplication(container);
            //prismApplication.Initialize();

            container.RegisterSingleton<IEventAggregator, EventAggregator>();
            container.RegisterSingleton<IShellManager, ShellManager>();
            container.RegisterSingleton<IWhitelistedServiceLocator, WhitelistedServiceLocator>();
            container.RegisterSingleton<IRegionManager, RegionManager>();

            ServiceLocator.SetLocatorProvider(() => new SimpleInjectorServiceLocator(container));
            return container;
        }

        [Test]
        [TestCaseSource(nameof(AllBootstrappers))]
        public void AllBootstrappers_ContainDefaultStartupConditions(Bootstrapper bootstrapper)
        {
            var container = BuildContainer(bootstrapper);

            var conditions = container.GetAllInstances<IStartupCondition>();
            var types = conditions.Select(c => c.GetType());

            var defaultConditions = new[]
            {
                typeof(SpoolerRunningCondition),
                typeof(CheckSpoolFolderCondition),
                typeof(GhostscriptCondition),
                typeof(PrinterInstalledCondition)
            };

            CollectionAssert.IsSubsetOf(defaultConditions, types);
        }

        [Test]
        [TestCaseSource(nameof(AllBootstrappers))]
        public void AllBootstrappers_ProperlyRegisterLicenseCondition(Bootstrapper bootstrapper)
        {
            var container = BuildContainer(bootstrapper);

            var conditions = container.GetAllInstances<IStartupCondition>();
            var types = conditions.Select(c => c.GetType());

            CollectionAssert.DoesNotContain(types, typeof(LicenseCondition));
        }

        [Test]
        [TestCaseSource(nameof(AllBootstrappers))]
        public void AllBootstrappers_ProperlyRegisterTerminalServerCondition(Bootstrapper bootstrapper)
        {
            var container = BuildContainer(bootstrapper);

            var conditions = container.GetAllInstances<IStartupCondition>();
            var types = conditions.Select(c => c.GetType());

            CollectionAssert.Contains(types, typeof(TerminalServerNotAllowedCondition));
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        [TestCaseSource(nameof(AllBootstrappers))]
        public void ConfigurationOfContainer_CanBeValidatedWithoutWarnings(Bootstrapper bootstrapper)
        {
            var container = BuildPrismContainer(bootstrapper);

            Assert.DoesNotThrow(() => { container.Verify(VerificationOption.VerifyOnly); });

            var result = Analyzer.Analyze(container)
                .Where(x => x.Severity > DiagnosticSeverity.Information)
                .Where(x => (x.DiagnosticType != DiagnosticType.LifestyleMismatch) || !_lifestyleMismatchAcceptableClasses.Contains(x.ServiceType))
                .ToList();

            var message = "";
            foreach (var diagnosticResult in result)
                message += $"{diagnosticResult.Severity} | {diagnosticResult.DiagnosticType}: {diagnosticResult.Description} {Environment.NewLine}";

            Assert.IsFalse(result.Any(), message);
        }

        [Test]
        [TestCaseSource(nameof(AllBootstrappers))]
        public void ConfigurationOfContainer_DoesNotThrowExceptions(Bootstrapper bootstrapper)
        {
            var container = new Container();
            Assert.DoesNotThrow(() => bootstrapper.RegisterMainApplication(container));
        }

        [Test]
        [TestCaseSource(nameof(AllBootstrappers))]
        [Apartment(ApartmentState.STA)]
        public void ConfigurationOfContainer_WithInteractions_CanBeValidatedWithoutWarnings(Bootstrapper bootstrapper)
        {
            ViewRegistry.ClearInteractionMappings();

            // TODO this needs to be tested differently, maybe retrieve all registrations from ViewRegistry?
            //var windowResolver = new TestWindowResolver(container);
            //var windowRegistry = new WindowRegistry(windowResolver);

            var container = BuildPrismContainer(bootstrapper);

            LoggingHelper.InitConsoleLogger("PDFCreator-Test", LoggingLevel.Off);
            var settingsHelper = container.GetInstance<ISettingsManager>();
            settingsHelper.LoadAllSettings();

            container.Verify(VerificationOption.VerifyOnly);

            var result = Analyzer.Analyze(container)
                .Where(x => x.Severity > DiagnosticSeverity.Information)
                .Where(x => (x.DiagnosticType != DiagnosticType.LifestyleMismatch) || !_lifestyleMismatchAcceptableClasses.Contains(x.ServiceType))
                .ToList();

            var message = "";
            foreach (var diagnosticResult in result)
                message += $"{diagnosticResult.Severity} | {diagnosticResult.DiagnosticType}: {diagnosticResult.Description} {Environment.NewLine}";

            Assert.IsFalse(result.Any(), message);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void AllTypes_DoNotRequestITranslatableInConstructor()
        {
            var bootstrapper = new PDFCreatorBootstrapper();
            BuildPrismContainer(bootstrapper);

            var typesWithTranslatableConstructor = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(TypeHasTranslatableConstructor);

            CollectionAssert.IsEmpty(typesWithTranslatableConstructor);
        }

        private bool TypeHasTranslatableConstructor(Type type)
        {
            return type.GetConstructors()
                .Any(constructorInfo =>
                    constructorInfo.GetParameters().Any(a => typeof(ITranslatable).IsAssignableFrom(a.ParameterType)));
        }
    }
}
