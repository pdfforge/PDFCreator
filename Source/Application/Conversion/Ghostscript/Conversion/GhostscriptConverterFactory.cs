using pdfforge.PDFCreator.Conversion.ConverterInterface;
using pdfforge.PDFCreator.Utilities;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Ghostscript.Conversion
{
    public class GhostscriptConverterFactory : IPsConverterFactory
    {
        private readonly IFile _file;
        private readonly IOsHelper _osHelper;
        private readonly ICommandLineUtil _commandLineUtil;
        private readonly GhostscriptVersion _ghostscriptVersion;

        public GhostscriptConverterFactory(IGhostscriptDiscovery ghostscriptDiscovery, IFile file, IOsHelper osHelper, ICommandLineUtil commandLineUtil)
        {
            _file = file;
            _osHelper = osHelper;
            _commandLineUtil = commandLineUtil;
            _ghostscriptVersion = ghostscriptDiscovery.GetGhostscriptInstance();
        }

        public IConverter BuildPsConverter()
        {
            return new GhostscriptConverter(_ghostscriptVersion, _file, _osHelper, _commandLineUtil);
        }
    }
}
