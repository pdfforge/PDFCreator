using NLog;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.Printing.Printing
{
    /// <summary>
    ///     The PrintFileHelperBase class provides reusable functionality for printing files
    /// </summary>
    public abstract class PrintFileHelperBase : IPrintFileHelper
    {
        protected readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IFileAssoc _fileAssoc;
        private PrintCommandGroup _printCommands = new PrintCommandGroup();
        private readonly IPrinterHelper _printerHelper;
        private readonly ISettingsProvider _settingsProvider;
        private readonly IDirectory _directory;
        private readonly IFile _file;

        protected PrintFileHelperBase(IPrinterHelper printerHelper, ISettingsProvider settingsProvider, IFileAssoc fileAssoc, IDirectory directory, IFile file)
        {
            _printerHelper = printerHelper;
            _settingsProvider = settingsProvider;
            _fileAssoc = fileAssoc;
            _directory = directory;
            _file = file;
        }

        public string PdfCreatorPrinter { get; set; }

        /// <summary>
        ///     Add a single file. The file is checked and dialogs are presented to the user, if there are problems.
        /// </summary>
        /// <param name="file">
        ///     A single file. If this is the path of a directory or an unprintable file, an error message will be
        ///     shown.
        /// </param>
        /// <returns>true, if all files are printable</returns>
        public bool AddFile(string file, bool silent)
        {
            return AddFiles(new[] { file }, silent);
        }

        /// <summary>
        ///     Add multiple files. The files are checked - if there are problems, abstract functions (that might present dialogs
        ///     to the user) get called.
        /// </summary>
        /// <param name="files">
        ///     A list of files. If this contains a directory or files are not printable, an error message will be
        ///     shown.
        /// </param>
        /// <param name="silent">If true, no message windows are shown</param>
        /// <returns>true, if all files are printable</returns>
        public bool AddFiles(IEnumerable<string> files, bool silent)
        {
            var printerName = GetPrinterName();
            foreach (var f in files)
            {
                _printCommands.Add(new PrintCommand(f, printerName, _fileAssoc, _printerHelper, _file, _settingsProvider.Settings.ApplicationSettings.ConversionTimeout));
            }

            if (!_printCommands.IsPrintable)
            {
                var sb = new StringBuilder("The following file(s) can not be printed:");
                var unprintable = _printCommands.UnprintableCommands;
                foreach (var file in unprintable)
                {
                    sb.AppendLine(file.Filename);
                }
                Logger.Error(sb.ToString);

                //all files are unprintable
                if (unprintable.Count >= files.Count())
                {
                    UnprintableFilesHint(unprintable, silent);
                    _printCommands.RemoveAllCommands();
                    return false;
                }

                var printRemainingPrintableFiles = ProceedWithRemainingPrintableFilesQuery(unprintable, silent);
                if (printRemainingPrintableFiles)
                {
                    _printCommands.RemoveUnprintableCommands();
                }
                else
                {
                    _printCommands.RemoveAllCommands();
                }
                return printRemainingPrintableFiles;
            }

            return true;
        }

        /// <summary>
        ///     Prints all files in the list.
        /// </summary>
        /// <returns>true, if all files could be printed</returns>
        public bool PrintAll(bool silent)
        {
            if (string.IsNullOrEmpty(PdfCreatorPrinter))
            {
                Logger.Error("No PDFCreator is installed.");
                return false;
            }

            var requiresDefaultPrinter = _printCommands.RequiresDefaultPrinter;
            var defaultPrinter = _printerHelper.GetDefaultPrinter();
            try
            {
                if (requiresDefaultPrinter && (defaultPrinter != PdfCreatorPrinter))
                {
                    Logger.Debug("Current default printer is " + defaultPrinter);
                    Logger.Info("PDFCreator must be set temporarily as default printer");
                    if (_settingsProvider.Settings.CreatorAppSettings.AskSwitchDefaultPrinter)
                    {
                        if (!SwitchDefaultPrinterQuery(silent))
                            return false;
                    }
                    if (!_printerHelper.SetDefaultPrinter(PdfCreatorPrinter))
                    {
                        Logger.Error("PDFCreator could not be set as default printer");
                        return false;
                    }

                    Logger.Debug("PDFCreator set as default printer");
                }

                return _printCommands.PrintAll(_settingsProvider.Settings.ApplicationSettings.ConversionTimeout);
            }
            finally
            {
                if (requiresDefaultPrinter)
                {
                    _printerHelper.SetDefaultPrinter(defaultPrinter);
                    Logger.Debug("Default printer set back to " + defaultPrinter);
                }
                _printCommands = new PrintCommandGroup();
            }
        }

        private string GetPrinterName()
        {
            if (string.IsNullOrWhiteSpace(PdfCreatorPrinter))
                PdfCreatorPrinter = _settingsProvider.Settings.CreatorAppSettings.PrimaryPrinter;

            return _printerHelper.GetApplicablePDFCreatorPrinter(PdfCreatorPrinter);
        }

        protected abstract void UnprintableFilesHint(IList<PrintCommand> unprintable, bool silent);

        protected abstract bool ProceedWithRemainingPrintableFilesQuery(IList<PrintCommand> unprintable, bool silent);

        protected abstract bool SwitchDefaultPrinterQuery(bool silent);
    }
}
