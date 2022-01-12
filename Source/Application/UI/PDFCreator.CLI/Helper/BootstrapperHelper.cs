using pdfforge.PDFCreator.Editions.EditionBase;
using pdfforge.PDFCreator.Utilities;
using SimpleInjector;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace pdfforge.PDFCreator.UI.CLI.Helper
{
    public static class BootstrapperHelper
    {
        private static Bootstrapper CreateBootstrapper()
        {
            var assemblyHelper = new AssemblyHelper(Assembly.GetExecutingAssembly());
            var applicationDir = assemblyHelper.GetAssemblyDirectory();
            var assemblyPath = Path.Combine(applicationDir, "PDFCreator.exe");

            if (!File.Exists(assemblyPath) && Debugger.IsAttached)
            {
                var editionDir = Path.GetFullPath(Path.Combine(applicationDir, @"..\..\..\..\Editions\PDFCreator\bin\Debug"));
                assemblyPath = Path.Combine(editionDir, "PDFCreator.exe");
            }

            if (!File.Exists(assemblyPath))
                throw new Exception("Could not find PDFCreator in path " + assemblyPath);

            var assembly = Assembly.LoadFrom(assemblyPath);
            var bootstrapperType = assembly.GetTypes().Single(t => t.IsSubclassOf(typeof(Bootstrapper)) && !t.IsAbstract);
            return (Bootstrapper)Activator.CreateInstance(bootstrapperType);
        }

        public static Container GetConfiguredContainer()
        {
            var container = new Container();
            var bootstrapper = CreateBootstrapper();
            bootstrapper.RegisterMainApplication(container);

            bootstrapper.InitializeServices(container);

            return container;
        }
    }
}
