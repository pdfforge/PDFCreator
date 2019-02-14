using MahApps.Metro.Controls;
using System.Text;
using System.Windows;
using Tartaros;

namespace pdfforge.PDFCreator.ErrorReport
{
    public partial class ErrorReportWindow : MetroWindow
    {
        private readonly Report _report;
        private readonly TartarosClient _tartarosClient;

        public string EmailAddress { get; set; }

        public ErrorReportWindow()
        {
            InitializeComponent();
        }

        public ErrorReportWindow(Report errorReport, TartarosClient tartarosClient)
            : this()
        {
            _report = errorReport;

            _tartarosClient = tartarosClient;
            ErrorDescriptionText.Text = ComposeErrorText(errorReport);
        }

        private string ComposeErrorText(Report report)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Error Report for PDFCreator " + report.Version);

            sb.AppendLine();
            sb.AppendLine("Exception:");
            sb.AppendLine(report.ErrorType);
            sb.AppendLine(report.ErrorMessage);
            sb.AppendLine(string.Join("\r\n", report.StackTrace));

            sb.AppendLine();
            sb.AppendLine("Log:");
            sb.AppendLine(string.Join("\r\n", report.Log));

            sb.AppendLine();
            sb.AppendLine("Environment:");
            sb.AppendLine(report.WindowsVersion);

            sb.AppendLine();
            sb.AppendLine("Platform:");
            sb.AppendLine(report.Platform);

            sb.AppendLine();
            sb.AppendLine("Environment variables:");
            foreach (var additionalEntry in report.EnvironmentVariables)
            {
                sb.AppendLine($"{additionalEntry.Key}={additionalEntry.Value}");
            }

            sb.AppendLine();
            sb.AppendLine("Additional Data:");
            foreach (var additionalEntry in report.AdditionalEntries)
            {
                sb.AppendLine($"{additionalEntry.Key}={additionalEntry.Value}");
            }

            return sb.ToString();
        }

        private void ReportButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(EmailAddress))
                _report.AdditionalEntries["Email"] = EmailAddress;

            var success = _tartarosClient.SendErrorReport(_report);

            if (success)
            {
                MessageBox.Show("Thank you, the error has been reported successfully.", "Error reported");
                Close();
            }
        }
    }
}
