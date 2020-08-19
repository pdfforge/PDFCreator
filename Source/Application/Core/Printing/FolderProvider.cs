using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Core.Printing.Port;
using pdfforge.PDFCreator.Core.SettingsManagement;
using System;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.Printing
{
    public class FolderProvider : ITempFolderProvider, ISpoolerProvider, IAppDataProvider
    {
        private const string PrinterPortName = "pdfcmon";
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPath _path;
        private readonly IPrinterPortReader _printerPortReader;

        public FolderProvider(IPrinterPortReader printerPortReader, IPath path)
        {
            _printerPortReader = printerPortReader;
            _path = path;

            var tempFolderBase = GetTempFolderBase();
            var localAppDataFolderBase = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var roamingAppDataFolderBase = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            TempFolder = PathSafe.Combine(tempFolderBase, "Temp");
            _logger.Debug("Temp folder is '{0}'", TempFolder);

            SpoolFolder = PathSafe.Combine(tempFolderBase, "Spool");
            _logger.Debug("Spool folder is '{0}'", SpoolFolder);

            LocalAppDataFolder = PathSafe.Combine(localAppDataFolderBase, "pdfforge", "PDFCreator");
            _logger.Debug("LocalAppData folder is '{0}'", LocalAppDataFolder);

            RoamingAppDataFolder = PathSafe.Combine(roamingAppDataFolderBase, "pdfforge", "PDFCreator");
            _logger.Debug("RoamingAppData folder is '{0}'", RoamingAppDataFolder);
        }

        public string SpoolFolder { get; }

        public string TempFolder { get; }

        public string LocalAppDataFolder { get; }

        public string RoamingAppDataFolder { get; }

        private string GetTempFolderBase()
        {
            var tempFolderName = ReadTempFolderName();
            return PathSafe.Combine(_path.GetTempPath(), tempFolderName);
        }

        private string ReadTempFolderName()
        {
            var printerPort = _printerPortReader.ReadPrinterPort(PrinterPortName);

            if (printerPort == null)
                return "PDFCreator";

            return printerPort.TempFolderName;
        }
    }
}
