using System;
using System.IO;
using System.Windows;

namespace pdfforge.PDFCreator.ErrorReport
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (!TryParseArgs(e.Args, out var errorFile, out var sentryUrl))
                Environment.Exit(-2); ;

            ShowReportWindow(errorFile, sentryUrl);
            Environment.Exit(0);
        }

        private bool TryParseArgs(string[] args, out string errorFile, out string sentryUrl)
        {
            errorFile = null;
            sentryUrl = null;

            if (args.Length != 2)
                return false;

            errorFile = args[0];

            if (!File.Exists(errorFile))
                return false;

            sentryUrl = args[1];

            if (!sentryUrl.StartsWith("https://"))
                return false;

            return true;
        }

        private void ShowReportWindow(string errorFile, string sentryUrl)
        {
            try
            {
                var errorHelper = new ErrorHelper("pdfcreator", "PDFCreator", new Version(), sentryUrl);
                var report = errorHelper.LoadReport(errorFile);

                var err = new ErrorReportWindow(report, errorHelper);
                err.ShowDialog();

                File.Delete(errorFile);
            }
            catch (Exception)
            {
                Environment.Exit(-1);
            }
        }
    }
}
