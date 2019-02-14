using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Core.DirectConversion;
using pdfforge.PDFCreator.Core.Workflow;

namespace pdfforge.PDFCreator.Core.Startup.AppStarts
{
    public class NewDirectConversionStart : MaybePipedStart
    {
        private readonly IJobInfoManager _jobInfoManager;
        private readonly IJobInfoQueue _jobInfoQueue;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private string _newInfFile;

        protected NewDirectConversionStart(IJobInfoQueue jobInfoQueue,
            IMaybePipedApplicationStarter maybePipedApplicationStarter, IJobInfoManager jobInfoManager, IDirectConversion directConversion)
            : base(maybePipedApplicationStarter)
        {
            _jobInfoQueue = jobInfoQueue;
            _jobInfoManager = jobInfoManager;
            DirectConversion = directConversion;
        }

        private IDirectConversion DirectConversion { get; }

        public string NewDirectConversionFile { get; internal set; }

        public string PrinterName { get; internal set; }

        public string OutputFileParameter { get; set; }

        public string ProfileParameter { get; set; }

        private string NewInfFile
        {
            get
            {
                if (!string.IsNullOrEmpty(_newInfFile))
                    return _newInfFile;

                _newInfFile = DirectConversion.TransformToInfFile(NewDirectConversionFile, PrinterName, ProfileParameter, OutputFileParameter);

                if (string.IsNullOrEmpty(_newInfFile))
                    _newInfFile = "";

                return _newInfFile;
            }
        }

        protected override string ComposePipeMessage()
        {
            return "NewJob|" + NewInfFile;
        }

        protected override bool StartApplication()
        {
            if (string.IsNullOrEmpty(NewInfFile))
                return false;

            _logger.Debug("Adding new job.");
            var jobInfo = _jobInfoManager.ReadFromInfFile(NewInfFile);
            _jobInfoQueue.Add(jobInfo);

            return true;
        }
    }
}
