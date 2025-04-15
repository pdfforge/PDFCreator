using System;
using System.IO;
using System.Reflection;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Utilities
{
    public interface IAssemblyHelper
    {
        string GetAssemblyDirectory();
    }

    public class AssemblyHelper : IAssemblyHelper
    {
        private readonly Assembly _assembly;

        public AssemblyHelper(Assembly assembly)
        {
            _assembly = assembly;
        }

        public string GetAssemblyDirectory()
        {
            var assemblyPath = GetAssemblyPath(_assembly);
            return PathSafe.GetDirectoryName(assemblyPath);
        }


        private string GetAssemblyPath(Assembly assembly)
        {
            var assemblyPath = assembly.Location;

            if (string.IsNullOrEmpty(assemblyPath))
                assemblyPath = assembly.Location;

            if (assemblyPath.StartsWith(@"file:///", StringComparison.OrdinalIgnoreCase))
                assemblyPath = assemblyPath.Substring(8);

            return assemblyPath;
        }
    }
}
