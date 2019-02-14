using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Core.Workflow;

namespace pdfforge.PDFCreator.Core.DirectConversion
{
    public interface IDirectConversion
    {
        void ConvertDirectly(string file);

        bool CanConvertDirectly(string file);
    }

    public class DirectConversion : IDirectConversion
    {
        internal static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IDirectConversionHelper _directConversionHelper;
        private readonly IDirectConversionInfFileHelper _directConversionInfFileHelper;
        private readonly IJobInfoManager _jobInfoManager;
        private readonly IJobInfoQueue _jobInfoQueue;

        public DirectConversion(
            IDirectConversionHelper directConversionHelper,
            IDirectConversionInfFileHelper directConversionInfFileHelper,
            IJobInfoManager jobInfoManager,
            IJobInfoQueue jobInfoQueue)
        {
            _directConversionHelper = directConversionHelper;
            _directConversionInfFileHelper = directConversionInfFileHelper;
            _jobInfoManager = jobInfoManager;
            _jobInfoQueue = jobInfoQueue;
        }

        public bool CanConvertDirectly(string file)
        {
            return _directConversionHelper.CanConvertDirectly(file);
        }

        public void ConvertDirectly(string file)
        {
            var infFile = _directConversionInfFileHelper.TransformToInfFile(file);
            if (string.IsNullOrEmpty(infFile))
                return;

            Logger.Debug("Adding new job.");
            var jobInfo = _jobInfoManager.ReadFromInfFile(infFile);
            _jobInfoQueue.Add(jobInfo);
        }
    }
}
