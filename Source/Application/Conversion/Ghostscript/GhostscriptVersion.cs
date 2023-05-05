namespace pdfforge.PDFCreator.Conversion.Ghostscript
{
    /// <summary>
    ///     Specifies information on Ghostscript installations
    /// </summary>
    public class GhostscriptVersion
    {
        /// <summary>
        ///     Creates a new GhostscriptVersion object
        /// </summary>
        /// <param name="version">Ghostscript version string</param>
        /// <param name="exePath">Full path of the gsdll32.dll</param>
        /// <param name="libFolder">Full path of the Lib folder</param>
        public GhostscriptVersion(string version, string exePath, string[] libPaths)
        {
            Version = version;
            ExePath = exePath;
            LibPaths = libPaths;
        }

        public string Version { get; private set; }
        public string ExePath { get; private set; }
        public string[] LibPaths { get; private set; }
    }
}
