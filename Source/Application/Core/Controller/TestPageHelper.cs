using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Core.JobInfoQueue;
using System;
using System.IO;
using System.Text;

namespace pdfforge.PDFCreator.Core.Controller
{
    public interface ITestPageHelper
    {
        void CreateTestPage(string profile = "");
    }

    public class TestPageHelper : ITestPageHelper
    {
        private readonly IJobInfoManager _jobInfoManager;
        private readonly TestPageCreator _testPageCreator;
        private readonly IJobInfoQueue _jobInfoQueue;
        private readonly string _spoolFolder;

        public TestPageHelper(ISpoolerProvider spoolerProvider, IJobInfoQueue jobInfoQueue, IJobInfoManager jobInfoManager, TestPageCreator testPageCreator)
        {
            _jobInfoQueue = jobInfoQueue;
            _jobInfoManager = jobInfoManager;
            _testPageCreator = testPageCreator;
            _spoolFolder = spoolerProvider.SpoolFolder;
        }

        /// <summary>
        ///     Creates a testpage in the spool folder and adds it to the JobInfoQueue
        /// </summary>
        public void CreateTestPage(string profile = "")
        {
            var tempPath = Path.Combine(_spoolFolder, Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempPath);

            var psFileContent = _testPageCreator.GetTestFileContent();
            var psFilePath = Path.Combine(tempPath, "testpage.ps");
            File.WriteAllText(psFilePath, psFileContent);

            var infFileContent = _testPageCreator.GetInfFileContent("testpage.ps");
            var infFilePath = Path.Combine(tempPath, "testpage.inf");

            File.WriteAllText(infFilePath, infFileContent, Encoding.Unicode);

            var testPageJob = _jobInfoManager.ReadFromInfFile(infFilePath);

            if (!string.IsNullOrEmpty(profile))
                testPageJob.ProfileParameter = profile;

            _jobInfoQueue.Add(testPageJob);
        }
    }
}
