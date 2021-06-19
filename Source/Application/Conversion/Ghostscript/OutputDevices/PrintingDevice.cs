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
        public const string PasswordParameter = "-sPDFPassword=";
        private readonly IPrinterWrapper _printer;
        private readonly bool _displayUserNameInSpoolJobTitle;

        public PrintingDevice(Job job, bool displayUserNameInSpoolJobTitle, IPrinterWrapper printer, IFile file, IOsHelper osHelper, ICommandLineUtil commandLineUtil)
            : base(job, ConversionMode.ImmediateConversion, file, osHelper, commandLineUtil)
        {
            _displayUserNameInSpoolJobTitle = displayUserNameInSpoolJobTitle;
            _printer = printer;
        }

        protected override void AddDeviceSpecificParameters(IList<string> parameters)
        {
            parameters.Add("-dPrinted");

            if (Job.Profile.Printing.FitToPage)
                parameters.Add("-dFitPage");

            var printerName = "";
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

            parameters.Add("-c");

            var printerParameter = "";
            if (!string.IsNullOrEmpty(printerName))
                printerParameter = $"/OutputFile ({EncodeGhostscriptParametersOctal("%printer%" + printerName)})";

            var spoolJobTitle = _displayUserNameInSpoolJobTitle ? (Job.JobInfo.Metadata.PrintJobAuthor + "|") : "";
            spoolJobTitle += PathSafe.GetFileName(Job.OutputFiles[0]);

            parameters.Add($"mark {printerParameter} /UserSettings << /DocumentName ({EncodeGhostscriptParametersOctal(spoolJobTitle)}) >> (mswinpr2) finddevice putdeviceprops setdevice");
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
