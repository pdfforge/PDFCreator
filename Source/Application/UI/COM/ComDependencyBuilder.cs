using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.ComImplementation;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services.Logging;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.UsageStatistics;
using pdfforge.PDFCreator.Editions.EditionBase;
using pdfforge.PDFCreator.Utilities;
using SimpleInjector;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace pdfforge.PDFCreator.UI.COM
{
    internal class ComDependencyBuilder
    {
        private static ComDependencies _comDependencies;

        public ComDependencies ComDependencies
        {
            get
            {
                if (_comDependencies != null)
                    return _comDependencies;
                return _comDependencies = BuildComDependencies();
            }
        }

        public Action<Container> ModifyRegistrations { get; set; } = container => { };

        public static void ResetDependencies()
        {
            _comDependencies = null;
        }

        private Bootstrapper CreateBootstrapper()
        {
            var assemblyHelper = new AssemblyHelper(GetType().Assembly);
            var applicationDir = assemblyHelper.GetAssemblyDirectory();
            var assemblyPath = Path.Combine(applicationDir, "PDFCreator.exe");

            var assembly = Assembly.LoadFrom(assemblyPath);
            var bootstrapperType = assembly.GetTypes().First(t => t.IsSubclassOf(typeof(Bootstrapper)) && !t.IsAbstract);
            return (Bootstrapper)Activator.CreateInstance(bootstrapperType);
        }

        private ComDependencies BuildComDependencies()
        {
            var bootstrapper = CreateBootstrapper();
            var container = new Container();
            bootstrapper.RegisterMainApplication(container);
            container.Register<PrintFileHelperComFactory>();
            container.Register<IComWorkflowFactory>(() => new ComWorkflowFactory(container));
            container.RegisterSingleton(() => new ThreadPool());
            container.Register<IPrintJobAdapterFactory, PrintJobAdapterFactory>();

            container.RegisterInitializer<PdfCreatorUsageStatisticsManager>(m => m.IsComMode = true);

            DoModifyRegistrations(container);

            var dependencies = container.GetInstance<ComDependencies>();

            LoggingHelper.InitFileLogger("PDFCreator", LoggingLevel.Error);

            var settingsManager = container.GetInstance<ISettingsManager>();
            settingsManager.LoadAllSettings();

            var translator = container.GetInstance<ITranslationHelper>();
            translator.InitTranslator("english");

            return dependencies;
        }

        private void DoModifyRegistrations(Container container)
        {
            container.Options.AllowOverridingRegistrations = true;

            container.Register<IMainWindowThreadLauncher, DummyMainWindowThreadLauncher>();

            ModifyRegistrations(container);

            container.Options.AllowOverridingRegistrations = false;
        }
    }
}
