using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.JobInfoQueue;
using pdfforge.PDFCreator.Utilities.Spool;
using SystemInterface.Diagnostics;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.TestPage
{
    public class TestPageHelper : TestPageHelperBase
    {
        private readonly IJobInfoManager _jobInfoManager;
        private readonly IJobInfoQueue _jobInfoQueue;
        private readonly string _spoolFolder;

        public TestPageHelper(ISpoolerProvider spoolerProvider, IJobInfoQueue jobInfoQueue, IJobInfoManager jobInfoManager,
            ITestPageCreator testPageCreator, IDirectory directory, IProcess process) : base(testPageCreator, directory, process)
        {
            _jobInfoQueue = jobInfoQueue;
            _jobInfoManager = jobInfoManager;
            _spoolFolder = spoolerProvider.SpoolFolder;
        }

        protected override string GetSpoolFolder() => _spoolFolder;

        protected override void PrintTestPage(string infFilePath, ConversionProfile profile = null)
        {
            var jobInfo = GetJobInfo(infFilePath, profile);
            _jobInfoQueue.Add(jobInfo);
        }

        protected override void CleanUp(string tempFolderPath)
        {
            // Clean up is done when the job finishes (non-server)
        }

        private JobInfo GetJobInfo(string infFilePath, ConversionProfile profile)
        {
            var testPageJobInfo = _jobInfoManager.ReadFromInfFile(infFilePath);
            if (profile != null)
            {
                testPageJobInfo.ProfileParameter = profile.Name;
            }

            return testPageJobInfo;
        }
    }
}
