using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.DirectConversion;
using pdfforge.PDFCreator.Core.JobInfoQueue;
using pdfforge.PDFCreator.Core.StartupInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Core.Startup.AppStarts
{
    public class MergeAppStart : MaybePipedStart
    {
        public IList<string> FilesForMerge { get; set; } = new List<string>();
        private string InfFile { get; set; }
        private readonly IJobInfoManager _jobInfoManager;
        private readonly IJobInfoQueue _jobInfoQueue;
        private readonly IFileConversionAssistant _fileConversionAssistant;
        private readonly IDirectConversionInfFileHelper _directConversionInfFileHelper;
        private readonly IDirectConversionHelper _directConversionHelper;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public MergeAppStart(IJobInfoManager jobInfoManager, IJobInfoQueue jobInfoQueue, IFileConversionAssistant fileConversionAssistant,
            IDirectConversionInfFileHelper directConversionInfFileHelper, IDirectConversionHelper directConversionHelper, IMaybePipedApplicationStarter maybePipedApplicationStarter)
            : base(maybePipedApplicationStarter)
        {
            _jobInfoManager = jobInfoManager;
            _jobInfoQueue = jobInfoQueue;
            _fileConversionAssistant = fileConversionAssistant;
            _directConversionInfFileHelper = directConversionInfFileHelper;
            _directConversionHelper = directConversionHelper;
        }

        public override async Task<ExitCode> Run()
        {
            if (!FilesForMergeCanConvertDirectly())
            {
                _logger.Warn("Following file(s) cannot be converted directly:\n{0}", string.Join(",\n", _droppedFiles.ToArray()));

                return ExitCode.PrintFileNotPrintable;
            }

            InfFile = _directConversionInfFileHelper.TransformToInfFileWithMerge(FilesForMerge, AppStartParameters);
            return await base.Run();
        }

        private readonly IList<string> _droppedFiles = new List<string>();

        private bool FilesForMergeCanConvertDirectly()
        {
            _droppedFiles.Clear();
            foreach (var file in FilesForMerge)
            {
                if (!_directConversionHelper.CanConvertDirectly(file))
                {
                    _droppedFiles.Add(file);
                }
            }

            return _droppedFiles.Count <= 0;
        }

        protected override string ComposePipeMessage()
        {
            return "NewJob|" + InfFile;
        }

        protected override bool StartApplication()
        {
            _logger.Debug("Adding new job");
            try
            {
                var jobInfo = _jobInfoManager.ReadFromInfFile(InfFile);
                _jobInfoQueue.Add(jobInfo);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, $"Could not read the file '{InfFile}'!");
                return false;
            }
        }
    }
}
