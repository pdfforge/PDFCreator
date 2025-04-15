using ControlzEx.Standard;
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
            string[] assemblyNameCandidates = [
                "PDFCreator.dll",
                "PDFCreatorProfessional.dll",
                "PDFCreatorTerminalServer.dll",
                "PDFCreatorServer.dll"
            ];

            foreach (var assemblyNameCandidate in assemblyNameCandidates)
            {
                var assemblyPath = Path.Combine(applicationDir, assemblyNameCandidate);

                if (!File.Exists(assemblyPath) && Debugger.IsAttached)
                {
                    var editionDir = Path.GetFullPath(Path.Combine(applicationDir, @"..\..\..\..\..\..\Editions\PDFCreator\bin\Debug\net8.0-windows7.0\win-x64"));
                    assemblyPath = Path.Combine(editionDir, "PDFCreator.dll");
                }

                if (!File.Exists(assemblyPath))
                    continue;

                var assembly = Assembly.LoadFrom(assemblyPath);
                var bootstrapperType = assembly.GetTypes().Single(t => t.IsSubclassOf(typeof(Bootstrapper)) && !t.IsAbstract);
                return (Bootstrapper)Activator.CreateInstance(bootstrapperType);
            }
            throw new Exception("Could not find PDFCreator Assembly in folder " + applicationDir);
        }

        public static Container GetConfiguredContainer()
        {
            var container = new Container();
            container.Options.EnableAutoVerification = false;
            container.Options.ResolveUnregisteredConcreteTypes = true;
            var bootstrapper = CreateBootstrapper();
            bootstrapper.RegisterMainApplication(container);

            bootstrapper.InitializeServices(container);

            return container;
        }
    }
}
