using NLog;
using pdfforge.PDFCreator.Conversion.ConverterInterface;
using pdfforge.PDFCreator.Conversion.Ghostscript.OutputDevices;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System;
using System.Text;

namespace pdfforge.PDFCreator.Conversion.Ghostscript.Conversion
{
    public class GhostscriptConverter : IConverter
    {
        private readonly StringBuilder _ghostscriptOutput = new StringBuilder();
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private GhostscriptVersion _ghostscriptVersion;

        public GhostscriptConverter(GhostscriptVersion ghostscriptVersion)
        {
            _ghostscriptVersion = ghostscriptVersion;
        }

        public string ConverterOutput => _ghostscriptOutput.ToString();

        private int NumberOfPages { get; set; }

        public void DoConversion(Job job)
        {
            var ghostScript = GetGhostscript();
            NumberOfPages = job.NumberOfPages;

            try
            {
                ghostScript.Output += Ghostscript_Output;
                job.OutputFiles.Clear();

                _logger.Debug("Starting Ghostscript Job");

                var device = GetCorrectOutputDevice(job);

                _logger.Trace("Output format is: {0}", job.Profile.OutputFormat);

                ghostScript.Output += Ghostscript_Logging;
                var success = ghostScript.Run(device, job.JobTempFolder);
                ghostScript.Output -= Ghostscript_Logging;

                _logger.Trace("Finished Ghostscript execution");

                if (!success)
                {
                    var errorMessage = ExtractGhostscriptErrors(ConverterOutput);
                    _logger.Error("Ghostscript execution failed: " + errorMessage);
                    if (errorMessage.Contains("Redistilling encrypted PDF is not permitted"))
                    {
                        throw new ProcessingException("Ghostscript execution failed: " + errorMessage, ErrorCode.Conversion_Ghostscript_PasswordProtectedPDFError);
                    }
                    {
                        throw new ProcessingException("Ghostscript execution failed: " + errorMessage, ErrorCode.Conversion_GhostscriptError);
                    }
                }

                _logger.Trace("Ghostscript Job was successful");
            }
            catch (ProcessingException ex)
            {
                _logger.Error("There was a Ghostscript error while converting the Job {0}: {1}", job.JobInfo.InfFile, ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error("There was an unexpected error while converting the Job {0}: {1}", job.JobInfo.InfFile, ex);
                throw new ProcessingException("Ghostscript execution failed", ErrorCode.Conversion_GhostscriptError);
            }
            finally
            {
                ghostScript.Output -= Ghostscript_Output;
            }
        }

        public event EventHandler<ConversionProgressChangedEventArgs> OnReportProgress;

        private OutputDevice GetCorrectOutputDevice(Job job)
        {
            OutputDevice device;
            switch (job.Profile.OutputFormat)
            {
                case OutputFormat.PdfA1B:
                case OutputFormat.PdfA2B:
                case OutputFormat.PdfA3B:
                case OutputFormat.PdfX:
                case OutputFormat.Pdf:
                    device = new PdfDevice(job);
                    break;

                case OutputFormat.Png:
                    device = new PngDevice(job);
                    break;

                case OutputFormat.Jpeg:
                    device = new JpegDevice(job);
                    break;

                case OutputFormat.Tif:
                    device = new TiffDevice(job);
                    break;

                case OutputFormat.Txt:
                    device = new TextDevice(job);
                    break;

                default:
                    throw new Exception("Illegal OutputFormat specified");
            }
            return device;
        }

        private GhostScript GetGhostscript()
        {
            _logger.Debug("Ghostscript Version: " + _ghostscriptVersion.Version + " loaded from " + _ghostscriptVersion.ExePath);
            return new GhostScript(_ghostscriptVersion);
        }

        public string ExtractGhostscriptErrors(string ghostscriptOutput)
        {
            var lines = ghostscriptOutput.Split('\n');

            var sb = new StringBuilder();

            foreach (var line in lines)
            {
                if (line.StartsWith("GPL Ghostscript"))
                    continue;

                if (line.StartsWith("Copyright (C)"))
                    continue;

                if (line.StartsWith("This software comes with NO WARRANTY"))
                    continue;

                if (line.StartsWith("Loading"))
                    continue;

                if (line.StartsWith("%%"))
                    continue;

                sb.AppendLine(line);
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Process Ghostscript output to provide logging
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event Arguments</param>
        private void Ghostscript_Logging(object sender, OutputEventArgs e)
        {
            _ghostscriptOutput.Append(e.Output);
        }

        /// <summary>
        ///     Process Ghostscript output to detect the progress
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event Arguments</param>
        private void Ghostscript_Output(object sender, OutputEventArgs e)
        {
            var output = e.Output;

            const string pageMarker = "[Page: ";
            if (output.Contains("[LastPage]"))
            {
                ReportProgress(100);
            }
            else if (output.Contains(pageMarker))
            {
                var start = output.LastIndexOf(pageMarker, StringComparison.Ordinal);
                var end = output.IndexOf("]", start, StringComparison.Ordinal);
                if ((start >= 0) && (end > start))
                {
                    start += pageMarker.Length;
                    var page = output.Substring(start, end - start);
                    int pageNumber;

                    if (int.TryParse(page, out pageNumber))
                    {
                        if (pageNumber <= NumberOfPages)
                            ReportProgress(pageNumber * 100 / NumberOfPages);
                    }
                }
            }
        }

        private void ReportProgress(int progress)
        {
            var eventArgs = new ConversionProgressChangedEventArgs(progress);
            OnReportProgress?.Invoke(this, eventArgs);
        }
    }
}
