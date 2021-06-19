using MahApps.Metro.Controls;
using Sentry;
using System.Windows;

namespace pdfforge.PDFCreator.ErrorReport
{
    public partial class ErrorReportWindow : MetroWindow
    {
        private readonly SentryEvent _report;
        private readonly SentryClient _sentryClient;

        public string EmailAddress { get; set; }

        public ErrorReportWindow()
        {
            InitializeComponent();
        }

        public ErrorReportWindow(SentryEvent errorReport, ErrorHelper errorHelper)
            : this()
        {
            _report = errorReport;

            _sentryClient = errorHelper.BuildSentryClient();
            ErrorDescriptionText.Text = errorHelper.ComposeErrorText(errorReport);
        }

        private void ReportButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(EmailAddress))
                _report.User.Email = EmailAddress;

            if (_report.Tags.ContainsKey(SentryTagNames.MachineId))
                _report.User.Id = _report.Tags[SentryTagNames.MachineId];

            try
            {
                _sentryClient.CaptureEvent(_report);

                MessageBox.Show("Thank you, the error has been reported successfully.", "Error reported");
                Close();
            }
            catch
            {
                MessageBox.Show("There was an error while sending the error report.", "Error");
            }
        }
    }
}
