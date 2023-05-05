using NLog;
using pdfforge.PDFCreator.Conversion.ConverterInterface;
using pdfforge.PDFCreator.Conversion.Ghostscript.OutputDevices;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Text;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Ghostscript.Conversion
{
    public class GhostscriptConverter : IConverter
    {
        private ConversionMode _firstConversionStepMode;

        private readonly StringBuilder _ghostscriptOutput = new StringBuilder();
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private GhostscriptVersion _ghostscriptVersion;
        private readonly IFile _file;
        private readonly IOsHelper _osHelper;
        private readonly ICommandLineUtil _commandLineUtil;

        public GhostscriptConverter(GhostscriptVersion ghostscriptVersion, IFile file, IOsHelper osHelper, ICommandLineUtil commandLineUtil)
        {
            _ghostscriptVersion = ghostscriptVersion;
            _file = file;
            _osHelper = osHelper;
            _commandLineUtil = commandLineUtil;
        }

        public string ConverterOutput => _ghostscriptOutput.ToString();

        private int NumberOfPages { get; set; }

        public void Init(bool outputFormatIsPdf, bool isProcessingRequired)
        {
            if (outputFormatIsPdf)
                _firstConversionStepMode = ConversionMode.PdfConversion;
            else if (isProcessingRequired)
                _firstConversionStepMode = ConversionMode.IntermediateConversion;
            else
                _firstConversionStepMode = ConversionMode.ImmediateConversion;
        }

        public void FirstConversionStep(Job job)
        {
            DoConversion(job, _firstConversionStepMode);
        }

        public void SecondConversionStep(Job job)
        {
            switch (_firstConversionStepMode)
            {
                case ConversionMode.PdfConversion:
                    MovePdfFileFromIntermediateToTempOutputFiles(job);
                    break;

                case ConversionMode.IntermediateConversion:
                    DoConversion(job, ConversionMode.IntermediateToTargetConversion);
                    break;

                case ConversionMode.IntermediateToTargetConversion:
                    throw new Exception($"{nameof(ConversionMode.IntermediateToTargetConversion)} is not valid as first step!");

                default:
                case ConversionMode.ImmediateConversion:
                    //no second conversion step required
                    break;
            }
        }

        private void MovePdfFileFromIntermediateToTempOutputFiles(Job job)
        {
            job.TempOutputFiles = new[] { job.IntermediatePdfFile };
        }

        public void CreateIntermediatePdf(Job job)
        {
            DoConversion(job, new PdfIntermediateDevice(job, _file, _osHelper, _commandLineUtil));
        }

        private void DoConversion(Job job, OutputDevice device)
        {
            var ghostScript = GetGhostscript();
            NumberOfPages = job.NumberOfPages;

            try
            {
                ghostScript.Output += Ghostscript_Output;
                job.OutputFiles.Clear();

                _logger.Debug("Starting Ghostscript Job");

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

                    throw new ProcessingException("Ghostscript execution failed: " + errorMessage, ErrorCode.Conversion_GhostscriptError);
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

        private void DoConversion(Job job, ConversionMode conversionMode)
        {
            var ghostScript = GetGhostscript();
            NumberOfPages = job.NumberOfPages;

            try
            {
                ghostScript.Output += Ghostscript_Output;
                job.OutputFiles.Clear();

                _logger.Debug("Starting Ghostscript Job");

                var device = GetOutputDevice(job, conversionMode);

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

                    throw new ProcessingException("Ghostscript execution failed: " + errorMessage, ErrorCode.Conversion_GhostscriptError);
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

        private OutputDevice GetOutputDevice(Job job, ConversionMode conversionMode)
        {
            OutputDevice device;
            if (conversionMode == ConversionMode.IntermediateConversion)
            {
                return new PdfIntermediateDevice(job, _file, _osHelper, _commandLineUtil);
            }

            switch (job.Profile.OutputFormat)
            {
                case OutputFormat.PdfA1B:
                case OutputFormat.PdfA2B:
                case OutputFormat.PdfA3B:
                case OutputFormat.PdfX:
                case OutputFormat.Pdf:
                    device = new PdfDevice(job, conversionMode, _file, _osHelper, _commandLineUtil);
                    break;

                case OutputFormat.Png:
                    device = new PngDevice(job, conversionMode, _file, _osHelper, _commandLineUtil);
                    break;

                case OutputFormat.Jpeg:
                    device = new JpegDevice(job, conversionMode, _file, _osHelper, _commandLineUtil);
                    break;

                case OutputFormat.Tif:
                    device = new TiffDevice(job, conversionMode, _file, _osHelper, _commandLineUtil);
                    break;

                case OutputFormat.Txt:
                    device = new TextDevice(job, conversionMode, _file, _osHelper, _commandLineUtil);
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

                    if (int.TryParse(page, out var pageNumber))
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
