using System;

namespace pdfforge.PDFCreator.Core.UsageStatistics
{
    public interface IServiceEventsHandler
    {
        void HandleServiceStopped(TimeSpan serviceUptime);
    }
}
