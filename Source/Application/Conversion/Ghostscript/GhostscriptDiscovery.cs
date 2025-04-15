﻿using pdfforge.PDFCreator.Utilities;
using System.Collections.Generic;
using System.Linq;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Ghostscript
{
    public interface IGhostscriptDiscovery
    {
        /// <summary>
        ///     Get the internal instance if it exists, otherwise the installed instance in the given version
        /// </summary>
        /// <returns>The best matching Ghostscript instance</returns>
        GhostscriptVersion GetGhostscriptInstance();
    }

    public class GhostscriptDiscovery : IGhostscriptDiscovery
    {
        private readonly IAssemblyHelper _assemblyHelper;
        private readonly IFileVersionInfoHelper _fileVersionInfoHelper;
        private readonly IFile _file;

        public GhostscriptDiscovery(IFile file, IAssemblyHelper assemblyHelper, IFileVersionInfoHelper fileVersionInfoHelper)
        {
            _file = file;
            _assemblyHelper = assemblyHelper;
            _fileVersionInfoHelper = fileVersionInfoHelper;
        }

        public List<string> PossibleGhostscriptPaths { get; set; } = ["Ghostscript", @"..\..\..\..\..\packages\Ghostscript", @"..\..\..\..\..\..\packages\Ghostscript", @"..\..\..\..\..\..\..\packages\Ghostscript", @"..\..\..\..\..\..\..\..\packages\Ghostscript", @"..\..\..\..\..\nuget-feed\Ghostscript", @"..\..\..\..\..\..\nuget-feed\Ghostscript", @"..\..\..\..\..\..\..\nuget-feed\Ghostscript", @"..\..\..\..\..\..\..\..\nuget-feed\Ghostscript"];

        /// <summary>
        ///     Get the internal instance if it exists, otherwise the installed instance in the given version
        /// </summary>
        /// <returns>The best matching Ghostscript instance</returns>
        public GhostscriptVersion GetGhostscriptInstance()
        {
            var applicationPath = _assemblyHelper.GetAssemblyDirectory();

            var paths = PossibleGhostscriptPaths
                .Select(path => PathSafe.Combine(applicationPath, path));

            foreach (var path in paths)
            {
                var exePath = PathSafe.Combine(path, @"Bin\gswin32c.exe");
                var libPaths = new[] { PathSafe.Combine(path, @"Bin"), PathSafe.Combine(path, @"Lib") };

                if (_file.Exists(exePath))
                {
                    var ghostscriptVersion = _fileVersionInfoHelper.GetFileVersion(exePath);

                    return new GhostscriptVersion(ghostscriptVersion, exePath, libPaths);
                }
            }

            return null;
        }
    }
}
