using Sentry;

namespace pdfforge.PDFCreator.ErrorReport
{
    public class ErrorAssistant
    {
        public void ShowErrorWindow(SentryEvent report, ErrorHelper errorHelper)
        {
            var err = new ErrorReportWindow(report, errorHelper);
            err.ShowDialog();
        }
    }
}
