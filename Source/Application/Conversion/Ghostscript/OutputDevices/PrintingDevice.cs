using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Collections.Generic;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Ghostscript.OutputDevices
{
    /// <summary>
    ///     Extends OutputDevice for Printing with installed Windowsprinters
    /// </summary>
    public class PrintingDevice : OutputDevice
    {
        public const string PasswordParameter = "-sPDFPassword=";

        private readonly IPrinterWrapper _printer;

        public PrintingDevice(Job job) : base(job, ConversionMode.ImmediateConversion)
        {
            _printer = new PrinterWrapper();
        }

        public PrintingDevice(Job job, IPrinterWrapper printer, IFile file, IOsHelper osHelper, ICommandLineUtil commandLineUtil)
            : base(job, ConversionMode.ImmediateConversion, file, osHelper, commandLineUtil)
        {
            _printer = printer;
        }

        protected override void AddDeviceSpecificParameters(IList<string> parameters)
        {
            parameters.Add("-dPrinted");
            parameters.Add("-sDEVICE=mswinpr2");
            var printerName = "";
            //var _printer = new PrinterSettings();
            switch (Job.Profile.Printing.SelectPrinter)
            {
                case SelectPrinter.DefaultPrinter:
                    //printer.PrinterName returns default printer
                    if (!_printer.IsValid)
                    {
                        Logger.Error("The default printer (" + Job.Profile.Printing.PrinterName + ") is invalid!");
                        throw new Exception("100");
                    }
                    printerName = _printer.PrinterName;
                    break;

                case SelectPrinter.SelectedPrinter:
                    _printer.PrinterName = Job.Profile.Printing.PrinterName;
                    //Hint: setting PrinterName, does not change the systems default
                    if (!_printer.IsValid)
                    {
                        Logger.Error("The selected printer (" + Job.Profile.Printing.PrinterName + ") is invalid!");
                        throw new Exception("101");
                    }
                    printerName = _printer.PrinterName;
                    break;

                case SelectPrinter.ShowDialog:
                default:
                    //add nothing to trigger the Windows-Printing-Dialog
                    break;
            }
            if (!string.IsNullOrEmpty(printerName))
                parameters.Add($"-sOutputFile=%printer%{printerName}");
            parameters.Add("-c");
            parameters.Add("<< /NoCancel true >> setpagedevice ");

            //No duplex settings for PrinterDialog
            if (Job.Profile.Printing.SelectPrinter == SelectPrinter.ShowDialog)
                return;

            switch (Job.Profile.Printing.Duplex)
            {
                case DuplexPrint.LongEdge: //Book
                    if (_printer.CanDuplex)
                    {
                        parameters.Add("<< /Duplex true /Tumble false >> setpagedevice ");
                    }
                    break;

                case DuplexPrint.ShortEdge: //Calendar
                    if (_printer.CanDuplex)
                    {
                        parameters.Add("<< /Duplex true /Tumble true >> setpagedevice ");
                    }
                    break;

                case DuplexPrint.Disable:
                default:
                    //Nothing
                    break;
            }
        }

        protected override void AddOutputfileParameter(IList<string> parameters)
        {
            if (Job.Profile.PdfSettings.Security.Enabled)
                parameters.Add(PasswordParameter + Job.Passwords.PdfOwnerPassword);
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
