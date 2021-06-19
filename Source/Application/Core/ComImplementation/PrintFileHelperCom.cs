using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.Printing.Printing;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Utilities;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.ComImplementation
{
    public class PrintFileHelperComFactory
    {
        private readonly IFileAssoc _fileAssoc;
        private readonly IPrinterHelper _printerHelper;
        private readonly ISettingsProvider _settingsProvider;
        private readonly IDirectory _directory;
        private readonly IFile _file;

        public PrintFileHelperComFactory(IPrinterHelper printerHelper, ISettingsProvider settingsProvider,
            IFileAssoc fileAssoc, IDirectory directory, IFile file)
        {
            _printerHelper = printerHelper;
            _settingsProvider = settingsProvider;
            _fileAssoc = fileAssoc;
            _directory = directory;
            _file = file;
        }

        public PrintFileHelperCom CreatePrintFileHelperCom()
        {
            return new PrintFileHelperCom(_printerHelper, _settingsProvider, _fileAssoc, _directory, _file);
        }
    }

    public class PrintFileHelperCom : PrintFileHelperBase
    {
        internal PrintFileHelperCom(IPrinterHelper printerHelper, ISettingsProvider settingsProvider, IFileAssoc fileAssoc, IDirectory directory, IFile file) : base(printerHelper, settingsProvider, fileAssoc, directory, file)
        {
        }

        public bool AllowDefaultPrinterSwitch { get; set; }

        /// silent can be ignored here
        protected override void UnprintableFilesHint(IList<PrintCommand> unprintable, bool silent)
        {
            var unprintableFilesNames = new StringBuilder("The following files cannot be printed: \n");

            foreach (var unprintableFile in unprintable)
            {
                unprintableFilesNames.AppendLine(unprintableFile.Filename);
            }

            throw new COMException(unprintableFilesNames.ToString());
        }

        /// silent can be ignored here
        protected override bool ProceedWithRemainingPrintableFilesQuery(IList<PrintCommand> unprintable, bool silent)
        {
            var unprintableFilesNames = new StringBuilder("The following file cannot be printed: \n");

            foreach (var unprintableFile in unprintable)
            {
                unprintableFilesNames.AppendLine(unprintableFile.Filename);
            }

            throw new COMException(unprintableFilesNames.ToString());
        }

        /// <summary>
        /// silent can be ignored here
        /// </summary>
        /// <param name="silent"></param>
        protected override bool SwitchDefaultPrinterQuery(bool silent)
        {
            //Depending on what the COM user chose to do, we set the
            //default printer or not
            return AllowDefaultPrinterSwitch;
        }
    }
}
