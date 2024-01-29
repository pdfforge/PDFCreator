using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Ghostscript.OutputDevices
{
    public class PrintingDeviceFactory
    {
        private readonly IPrinterWrapper _printer;
        private readonly IFile _file;
        private readonly IOsHelper _osHelper;
        private readonly ICommandLineUtil _commandLineUtil;

        private bool _displayUserNameInSpoolJobTitle = false;

        public void Init(bool displayUserNameInSpoolJobTitle)
        {
            _displayUserNameInSpoolJobTitle = displayUserNameInSpoolJobTitle;
        }

        public PrintingDeviceFactory(IPrinterWrapper printer, IFile file, IOsHelper osHelper, ICommandLineUtil commandLineUtil)
        {
            _printer = printer;
            _file = file;
            _osHelper = osHelper;
            _commandLineUtil = commandLineUtil;
        }

        public PrintingDevice Create(Job job)
        {
            return new PrintingDevice(job, _displayUserNameInSpoolJobTitle, _printer, _file, _osHelper, _commandLineUtil);
        }
    }

    /// <summary>
    ///     Extends OutputDevice for Printing with installed Windowsprinters
    /// </summary>
    public class PrintingDevice : OutputDevice
    {
        private readonly IPrinterWrapper _printer;
        private readonly bool _displayUserNameInSpoolJobTitle;

        public PrintingDevice(Job job, bool displayUserNameInSpoolJobTitle, IPrinterWrapper printer, IFile file, IOsHelper osHelper, ICommandLineUtil commandLineUtil)
            : base(job, ConversionMode.ImmediateConversion, file, osHelper, commandLineUtil)
        {
            _displayUserNameInSpoolJobTitle = displayUserNameInSpoolJobTitle;
            _printer = printer;
        }

        private string GetOutputFileParameter(ConversionProfile profile)
        {
            var printerName = "";
            switch (profile.Printing.SelectPrinter)
            {
                case SelectPrinter.DefaultPrinter:
                    //printer.PrinterName returns default printer
                    if (!_printer.IsValid)
                    {
                        var message = "The default printer (" + profile.Printing.PrinterName + ") is invalid!";
                        Logger.Error(message);
                        throw new ProcessingException(message, ErrorCode.Printing_InvalidDefaultPrinter);
                    }
                    printerName = _printer.PrinterName;
                    break;

                case SelectPrinter.SelectedPrinter:
                    _printer.PrinterName = Job.Profile.Printing.PrinterName;
                    //Hint: setting PrinterName, does not change the systems default
                    if (!_printer.IsValid)
                    {
                        var message = "The selected printer(" + profile.Printing.PrinterName + ") is invalid!";
                        Logger.Error(message);
                        throw new ProcessingException(message, ErrorCode.Printing_InvalidSelectedPrinter);
                    }
                    printerName = _printer.PrinterName;
                    break;

                case SelectPrinter.ShowDialog:
                default:
                    //leave printerName empty to get (only) %printer% for the parameter below which triggers the Ghostscript-Printing-Dialog
                    break;
            }
            
            return $"/OutputFile (%printer%{EncodeGhostscriptParametersOctal(printerName)})";
        }

        protected override void AddDeviceSpecificParameters(IList<string> parameters)
        {
            parameters.Add("-dPrinted");

            if (Job.Profile.Printing.FitToPage)
                parameters.Add("-dFitPage");

            parameters.Add("-c");

            var outputFile = GetOutputFileParameter(Job.Profile);

            var spoolJobTitle = _displayUserNameInSpoolJobTitle ? (Job.JobInfo.Metadata.PrintJobAuthor + "|") : "";
            spoolJobTitle += PathSafe.GetFileName(Job.OutputFiles[0]);
            var userSettings = $"/UserSettings << /DocumentName ({EncodeGhostscriptParametersOctal(spoolJobTitle)}) >>";
            
            parameters.Add($"<< {outputFile} {userSettings} /OutputDevice /mswinpr2 /NoCancel true >> setpagedevice ");

            AddDuplexParameter(parameters);
        }

        private void AddDuplexParameter(IList<string> parameters)
        {
            if (Job.Profile.Printing.Duplex == DuplexPrint.Disable)
                return;

            //No duplex settings for PrinterDialog
            if (Job.Profile.Printing.SelectPrinter == SelectPrinter.ShowDialog)
                return;

            if (!_printer.CanDuplex)
            {
                Logger.Warn($"The printer \"{_printer.PrinterName}\" does not support duplex.");
                return;
            }

            switch (Job.Profile.Printing.Duplex)
            {
                case DuplexPrint.LongEdge: //Book
                    parameters.Add("<< /Duplex true /Tumble false >> setpagedevice ");
                    break;
                case DuplexPrint.ShortEdge: //Calendar
                    parameters.Add("<< /Duplex true /Tumble true >> setpagedevice ");
                    break;
                case DuplexPrint.Disable:
                default:
                    //Nothing
                    break;
            }
        }

        protected override void AddPasswordParameter(IList<string> parameters)
        {
            if (Job.Profile.PdfSettings.Security.Enabled)
                parameters.Add($"{PasswordParameter}={Job.Passwords.PdfOwnerPassword}");
        }

        protected override void AddOutputFileParameter(IList<string> parameters)
        {
        }

        protected override string ComposeOutputFilename()
        {
            return "";
        }

        protected override void SetSourceFiles(IList<string> parameters, Job job)
        {
            if (job.Profile.OutputFormat.IsPdf())
                foreach (var file in Job.OutputFiles)
                    parameters.Add(PathHelper.GetShortPathName(file));
            else if (!string.IsNullOrEmpty(job.IntermediatePdfFile))
                SetSourceFilesFromIntermediateFiles(parameters);
            else
                SetSourceFilesFromSourceFileInfo(parameters);
        }
    }
}
