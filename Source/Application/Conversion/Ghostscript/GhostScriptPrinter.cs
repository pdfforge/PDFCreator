using NLog;
using pdfforge.PDFCreator.Conversion.Ghostscript.OutputDevices;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using System;

namespace pdfforge.PDFCreator.Conversion.Ghostscript
{
    public class GhostScriptPrinter : IJobPrinter
    {
        private readonly IGhostscriptDiscovery _ghostscriptDiscovery;
        private readonly PrintingDeviceFactory _printingDeviceFactory;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public GhostScriptPrinter(IGhostscriptDiscovery ghostscriptDiscovery, PrintingDeviceFactory printingDeviceFactory)
        {
            _ghostscriptDiscovery = ghostscriptDiscovery;
            _printingDeviceFactory = printingDeviceFactory;
        }

        public void Print(Job job)
        {
            var gsVersion = _ghostscriptDiscovery.GetGhostscriptInstance();

            if (gsVersion == null)
            {
                _logger.Error("No valid Ghostscript version found.");
                throw new InvalidOperationException("No valid Ghostscript version found.");
            }
            _logger.Debug("Ghostscript Version: " + gsVersion.Version + " loaded from " + gsVersion.ExePath);
            var ghostscript = new GhostScript(gsVersion);
            ghostscript.Timeout = TimeSpan.FromMinutes(10);

            OutputDevice printingDevice = _printingDeviceFactory.Create(job);
            ghostscript.Run(printingDevice, job.JobTempFolder);
        }
    }
}
