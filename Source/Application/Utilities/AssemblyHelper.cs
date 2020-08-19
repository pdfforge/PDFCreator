using System;
using System.IO;
using System.Reflection;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Utilities
{
    public interface IAssemblyHelper
    {
        string GetAssemblyDirectory();

        DateTime GetLinkerTime();
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

        public DateTime GetLinkerTime()
        {
            var filePath = _assembly.Location;
            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;

            var buffer = new byte[2048];

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                stream.Read(buffer, 0, 2048);

            var offset = BitConverter.ToInt32(buffer, c_PeHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(buffer, offset + c_LinkerTimestampOffset);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var linkTimeUtc = epoch.AddSeconds(secondsSince1970);

            var tz = TimeZoneInfo.Local;
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, tz);

            return localTime;
        }

        private string GetAssemblyPath(Assembly assembly)
        {
            var assemblyPath = assembly.CodeBase;

            if (string.IsNullOrEmpty(assemblyPath))
                assemblyPath = assembly.Location;

            if (assemblyPath.StartsWith(@"file:///", StringComparison.OrdinalIgnoreCase))
                assemblyPath = assemblyPath.Substring(8);

            return assemblyPath;
        }
    }
}
