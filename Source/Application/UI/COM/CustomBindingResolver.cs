using NLog;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace pdfforge.PDFCreator.UI.COM
{
    internal static class CustomBindingResolver
    {
        private static string _assemblyDir;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static string[] IgnoredAssemblies = new[]
        {
            "PdfToolsProcessing.resources"
        };

        public static void WatchBindingResolution()
        {
            _assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += CurrentDomain_ResolveAssembly;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_ResolveAssembly;
        }

        private static Assembly CurrentDomain_ResolveAssembly(object sender, ResolveEventArgs e)
        {
            Logger.Trace("Trying to resolve assembly: " + e.Name);
            var requestAssemblyName = e.Name.Substring(0, e.Name.IndexOf(','));

            if (IgnoredAssemblies.Contains(requestAssemblyName))
            {
                Logger.Trace("The assembly will be skipped: " + requestAssemblyName);
                return null;
            }

            var assemblyPath = Path.Combine(_assemblyDir, requestAssemblyName + ".dll");

            if (!File.Exists(assemblyPath))
            {
                Logger.Error("The assembly file does not exist: " + assemblyPath);
                return null;
            }

            try
            {
                Logger.Trace("Loading assembly: " + assemblyPath);
                var assembly = Assembly.LoadFile(assemblyPath);
                Logger.Trace($"Loaded: {assembly.GetName().Name} (requested {e.Name})");

                if (requestAssemblyName != assembly.GetName().Name)
                {
                    Logger.Error("Could not find assembly: " + e.Name);
                    return null;
                }

                return assembly;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Error while loading assembly: {assemblyPath} (does the assembly file name match the assembly name?");
                return null;
            }
        }
    }
}
