using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Core.JobInfoQueue;
using System.Collections.Generic;
using System.Linq;

namespace pdfforge.PDFCreator.Core.DirectConversion
{
    public interface IDirectConversion
    {
        void ConvertDirectly(IList<string> files, AppStartParameters appStartParameters = null);

        bool IsDirectConversion(string file);

        bool IsImageConversion(string file);

        void ConvertImagesDirectly(IList<string> files, AppStartParameters appStartParameters = null);
    }

    public class DirectConversion : IDirectConversion
    {
        internal static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IDirectConversionHelper _directConversionHelper;
        private readonly IDirectConversionInfFileHelper _directConversionInfFileHelper;
        private readonly IJobInfoManager _jobInfoManager;
        private readonly IJobInfoQueue _jobInfoQueue;
        private readonly IDirectImageConversionHelper _directImageConversionHelper;

        public DirectConversion(
            IDirectConversionHelper directConversionHelper,
            IDirectConversionInfFileHelper directConversionInfFileHelper,
            IJobInfoManager jobInfoManager,
            IJobInfoQueue jobInfoQueue,
            IDirectImageConversionHelper directImageConversionHelper)
        {
            _directConversionHelper = directConversionHelper;
            _directConversionInfFileHelper = directConversionInfFileHelper;
            _jobInfoManager = jobInfoManager;
            _jobInfoQueue = jobInfoQueue;
            _directImageConversionHelper = directImageConversionHelper;
        }

        public bool IsDirectConversion(string file)
        {
            return _directConversionHelper.IsDirectConversion(file);
        }

        public void ConvertDirectly(IList<string> files, AppStartParameters appStartParameters = null)
        {
            var infFile = "";
            if (appStartParameters != null && appStartParameters.Merge)
                infFile = _directConversionInfFileHelper.TransformToInfFileWithMerge(files, appStartParameters);
            else
            {
                infFile = appStartParameters != null ?
                    _directConversionInfFileHelper.TransformToInfFile(files.First(), appStartParameters) :
                    _directConversionInfFileHelper.TransformToInfFile(files.First());
            }

            if (string.IsNullOrEmpty(infFile))
                return;

            Logger.Debug("Adding new job.");
            var jobInfo = _jobInfoManager.ReadFromInfFile(infFile);
            _jobInfoQueue.Add(jobInfo);
        }

        public bool IsImageConversion(string file)
        {
            return _directConversionHelper.IsImageConversion(file);
        }

        public void ConvertImagesDirectly(IList<string> files, AppStartParameters appStartParameters = null)
        {
            var infFile = _directImageConversionHelper.TransformToInfFileDirectImageConversion(files, appStartParameters);

            if (string.IsNullOrEmpty(infFile))
                return;

            Logger.Debug("Adding new job.");
            var jobInfo = _jobInfoManager.ReadFromInfFile(infFile);
            _jobInfoQueue.Add(jobInfo);
        }
    }
}
