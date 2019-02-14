using NLog;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace pdfforge.PDFCreator.Core.Printing.Printing
{
    public enum PrintType
    {
        Print,
        PrintTo,
        Unprintable
    }

    /// <summary>
    ///     The PrintCommand class provides a way to print a file with PDFCreator
    /// </summary>
    public class PrintCommand
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IFileAssoc _fileAssoc;
        private readonly IPrinterHelper _printerHelper;

        private readonly int _timeout;
        private readonly List<SpecialShellCommand> _specialFileTypes;
        private readonly string[] _pictureFallbackCommandParams = { Environment.SystemDirectory + "\\shimgvw.dll,ImageView_PrintTo", "/pt", "%1", "%2", "%3", "%4" };

        /// <summary>
        ///     Create a new PrintCommand for the given file
        /// </summary>
        /// <param name="filename">The full path to the file that shall be printed</param>
        /// <param name="printer">The printer the command will print to</param>
        /// <param name="fileAssoc">The IFileAssoc implementation used to detect if the is printable</param>
        /// <param name="printerHelper">PrinterHelper to determine the DefaultPrinter</param>
        /// <param name="timeout">Timeout in seconds after which a print stops</param>
        public PrintCommand(string filename, string printer, IFileAssoc fileAssoc, IPrinterHelper printerHelper, int timeout)
        {
            _specialFileTypes = new List<SpecialShellCommand>
                {
                    new SpecialShellCommand("jpegfile", "printto", "rundll32.exe", _pictureFallbackCommandParams),
                    new SpecialShellCommand("pngfile", "printto", "rundll32.exe", _pictureFallbackCommandParams),
                    new SpecialShellCommand("TIFImage.Document", "printto", "rundll32.exe", _pictureFallbackCommandParams)
                };

            if (filename == null)
                throw new ArgumentNullException(nameof(filename));

            Filename = filename;
            Printer = printer;
            _fileAssoc = fileAssoc;
            _printerHelper = printerHelper;
            _timeout = timeout;

            RegisterSpecialFileTypesToFileAssoc();

            Logger.Trace($"Checking PrintCommand for '{filename}'");

            if (!File.Exists(filename))
            {
                Logger.Trace($"The file '{filename}' does not exist!");
                CommandType = PrintType.Unprintable;
                return;
            }

            var extension = Path.GetExtension(filename);
            if (string.IsNullOrEmpty(extension))
            {
                Logger.Trace("Unprintable: The file as no extension!");
                CommandType = PrintType.Unprintable;
                return;
            }

            if (!SupportsPrint() && !SupportsPrintTo())
            {
                Logger.Trace("Unprintable: The file does not support print or printto!");
                CommandType = PrintType.Unprintable;
            }
            else
            {
                CommandType = SupportsPrintTo() ? PrintType.PrintTo : PrintType.Print;
                Logger.Trace($"The file is printable: {CommandType}");
            }
        }

        public ProcessWrapperFactory ProcessWrapperFactory { get; set; } = new ProcessWrapperFactory();

        public PrintType CommandType { get; }

        public string Filename { get; }

        public bool Successful { get; private set; }

        public bool IsPrintable
        {
            get { return CommandType != PrintType.Unprintable; }
        }

        public bool RequiresDefaultPrinter
        {
            get { return CommandType != PrintType.PrintTo; }
        }

        public string Printer { get; }

        /// <summary>
        ///     Prints the file.
        /// </summary>
        /// <returns>true, if printing was successful</returns>
        public bool Print()
        {
            return Print(TimeSpan.FromSeconds(_timeout));
        }

        /// <summary>
        ///     Prints the file.
        /// </summary>
        /// <param name="processTimeout">The timespan to wait for the process to finish</param>
        /// <returns>true, if printing was successful</returns>
        public bool Print(TimeSpan processTimeout)
        {
            if (CommandType == PrintType.Unprintable)
                throw new InvalidOperationException("File is not printable");

            if (CommandType == PrintType.Print && Printer != _printerHelper.GetDefaultPrinter())
                throw new InvalidOperationException("The default printer needs to be set in order to print this file");

            var p = ProcessWrapperFactory.BuildProcessWrapper(new ProcessStartInfo());

            var verb = SupportsPrintTo()
                ? "printto"
                : "print";

            var command = GetCommand(verb);
            var arguments = SupportsPrintTo()
                ? command.GetReplacedCommandArgs(Filename, Printer)
                : command.GetReplacedCommandArgs(Filename);

            p.StartInfo.FileName = command.Command;
            p.StartInfo.Arguments = arguments;

            Logger.Debug($"Launching {verb} for \"{Filename}\": {command.Command} {arguments}");

            try
            {
                p.Start();
                p.WaitForExit(processTimeout);

                if (!p.HasExited)
                {
                    Logger.Warn("Process was not finishing after {0} seconds, killing it now...", processTimeout.TotalSeconds);
                    p.Kill();
                }
                else
                {
                    Successful = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception during printing"
                             + "\r\nType: " + ex.GetType()
                             + "\r\nMessage: " + ex.Message
                    );
                return false;
            }

            return Successful;
        }

        private void RegisterSpecialFileTypesToFileAssoc()
        {
            _fileAssoc.RegisterSpecialFileTypes(_specialFileTypes);
        }

        private bool SupportsPrint()
        {
            return _fileAssoc.HasPrint(Path.GetExtension(Filename));
        }

        private bool SupportsPrintTo()
        {
            return _fileAssoc.HasPrintTo(Path.GetExtension(Filename));
        }

        private ShellCommand GetCommand(string verb)
        {
            return _fileAssoc.GetShellCommand(Path.GetExtension(Filename), verb);
        }
    }
}
