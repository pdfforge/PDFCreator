using pdfforge.Communication;
using System;
using System.Diagnostics;

namespace pdfforge.PDFCreator.Editions.EditionBase
{
    internal static class PdfCreatorQuickStartHelper
    {
        public static bool TryActivateRunningPDFCreatorInstance(string[] args)
        {
            // We only try this is no arguments were passed to PDFCreator
            if (args.Length != 0)
                return false;

            try
            {
                var pipeName = "PDFCreator-" + Process.GetCurrentProcess().SessionId;
                var pipeServer = new PipeServer(pipeName, pipeName);
                var pipe = new PipeClient(pipeName);

                if (!pipeServer.IsServerRunning())
                    return false;

                return pipe.SendMessage("ShowMain|", 500);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
