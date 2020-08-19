using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Core.Communication;
using pdfforge.PDFCreator.Core.DirectConversion;
using pdfforge.PDFCreator.Core.JobInfoQueue;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.Startup.StartConditions;
using System.Runtime.InteropServices;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.ComImplementation
{
    public class PdfCreatorAdapter
    {
        private readonly IDirectConversionInfFileHelper _directConversionInfFileHelper;
        private readonly IFile _file;
        private readonly IJobInfoManager _jobInfoManager;
        private readonly IDirectConversionHelper _directConversionHelper;
        private readonly IJobInfoQueue _jobInfoQueue;
        private readonly IPipeServerManager _pipeServerManager;
        private readonly PrintFileHelperComFactory _printFileHelperComFactory;
        private readonly ISpoolFolderAccess _spoolFolderAccess;

        public PdfCreatorAdapter(
            IFile file,
            PrintFileHelperComFactory printFileHelperComFactory,
            IJobInfoQueue jobInfoQueue,
            ISpoolFolderAccess spoolFolderAccess,
            IJobInfoManager jobInfoManager,
            IDirectConversionHelper directConversionHelper,
            IDirectConversionInfFileHelper directConversionInfFileHelper,
            IPrinterHelper printerHelper,
            IPipeServerManager pipeServerManager)
        {
            PrinterHelper = printerHelper;
            _file = file;
            _printFileHelperComFactory = printFileHelperComFactory;
            _jobInfoQueue = jobInfoQueue;
            _spoolFolderAccess = spoolFolderAccess;
            _jobInfoManager = jobInfoManager;
            _directConversionHelper = directConversionHelper;
            _directConversionInfFileHelper = directConversionInfFileHelper;
            _pipeServerManager = pipeServerManager;
        }

        public IPrinterHelper PrinterHelper { get; private set; }

        public bool IsInstanceRunning => _pipeServerManager.IsServerRunning();

        public void PrintFile(string path)
        {
            PrintFileSwitchingPrinters(path, false);
        }

        public void PrintFileSwitchingPrinters(string path, bool allowDefaultPrinterSwitch)
        {
            PathCheck(path);

            var printFileHelper = _printFileHelperComFactory.CreatePrintFileHelperCom();

            printFileHelper.AddFile(path);
            printFileHelper.AllowDefaultPrinterSwitch = allowDefaultPrinterSwitch;
            printFileHelper.PrintAll();
        }

        public void AddFileToQueue(string path)
        {
            PathCheck(path);

            if (!_directConversionHelper.CanConvertDirectly(path))
                throw new COMException("Only .ps and .pdf files can be directly added to the queue.");

            if (!_spoolFolderAccess.CanAccess())
                throw new COMException("Accessing the spool folder failed.");

            var infFile = _directConversionInfFileHelper.TransformToInfFile(path, new AppStartParameters());

            _jobInfoQueue.Add(_jobInfoManager.ReadFromInfFile(infFile));
        }

        private void PathCheck(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new COMException("The specified path must not be empty or uninitiliazed.");

            if (!_file.Exists(path))
                throw new COMException("File with such a path doesn't exist. Please check if the specified path is correct.");
        }
    }
}
