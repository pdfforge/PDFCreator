using pdfforge.PDFCreator.Conversion.Ghostscript;
using pdfforge.PDFCreator.Utilities;
using System.Collections.Generic;
using System.IO;
using SystemInterface.IO;

namespace PDFCreator.TestUtilities
{
    public class PaketGhostscriptDiscovery : IGhostscriptDiscovery
    {
        private readonly IAssemblyHelper _assemblyHelper;

        public PaketGhostscriptDiscovery(IAssemblyHelper assemblyHelper)
        {
            _assemblyHelper = assemblyHelper;
        }

        private IEnumerable<string> GetPathCandidates()
        {
            var path = _assemblyHelper.GetAssemblyDirectory();

            while (path.Length > 3)
            {
                yield return path;

                path = Path.GetDirectoryName(path) ?? "";
            }
        }

        public GhostscriptVersion GetGhostscriptInstance()
        {
            foreach (var path in GetPathCandidates())
            {
                var testPath = PathSafe.Combine(path, @"packages\Ghostscript");

                var exePath = PathSafe.Combine(testPath, @"Bin\gswin32c.exe");
                var libPath = PathSafe.Combine(testPath, @"Bin") + ';' + PathSafe.Combine(testPath, @"Lib") + ';' +
                              PathSafe.Combine(testPath, @"Fonts");

                if (File.Exists(exePath))
                {
                    return new GhostscriptVersion("<internal>", exePath, libPath);
                }
            }

            return null;
        }
    }
}
