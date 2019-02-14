using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Core.DirectConversion;
using pdfforge.PDFCreator.Core.Workflow;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Core.Startup.AppStarts
{
    public class DirectConversionStart : MaybePipedStart
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IJobInfoManager _jobInfoManager;
        private readonly IDirectConversionInfFileHelper _directConversionInfFileHelper;
        private readonly IJobInfoQueue _jobInfoQueue;

        public DirectConversionStart(IJobInfoQueue jobInfoQueue,
            IMaybePipedApplicationStarter maybePipedApplicationStarter,
            IJobInfoManager jobInfoManager,
            IDirectConversionInfFileHelper directConversionInfFileHelper)
            : base(maybePipedApplicationStarter)
        {
            _jobInfoQueue = jobInfoQueue;
            _jobInfoManager = jobInfoManager;
            _directConversionInfFileHelper = directConversionInfFileHelper;
            DirectConversionFiles = new List<string>();
        }

        public List<string> DirectConversionFiles { get; set; }

        private List<string> _newInfFiles;

        private List<string> NewInfFiles
        {
            get
            {
                if (_newInfFiles != null)
                    return _newInfFiles;

                _newInfFiles = new List<string>();
                if (AppStartParameters.Merge)
                {
                    var infFile = _directConversionInfFileHelper.TransformToInfFileWithMerge(DirectConversionFiles, AppStartParameters);
                    if (!string.IsNullOrEmpty(infFile))
                        _newInfFiles.Add(infFile);
                }
                else
                {
                    foreach (var file in DirectConversionFiles)
                    {
                        var infFile = _directConversionInfFileHelper.TransformToInfFile(file, AppStartParameters);
                        if (!string.IsNullOrEmpty(infFile))
                            _newInfFiles.Add(infFile);
                    }
                }

                return _newInfFiles;
            }
        }

        protected override string ComposePipeMessage()
        {
            return "NewJob|" + string.Join("|", NewInfFiles);
        }

        protected override bool StartApplication()
        {
            if (NewInfFiles.Count <= 0)
                return false;

            foreach (var infFile in NewInfFiles)
            {
                _logger.Debug("Adding new job.");
                var jobInfo = _jobInfoManager.ReadFromInfFile(infFile);
                _jobInfoQueue.Add(jobInfo);
            }

            return true;
        }
    }
}
