using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Printing;
using pdfforge.PDFCreator.Core.Printing.Port;
using pdfforge.PDFCreator.Core.Services.Logging;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.COM;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SystemWrapper.IO;
using SystemWrapper.Microsoft.Win32;
using Translatable;

namespace pdfforge.PDFCreator.IntegrationTest.UI.COM
{
    [TestFixture]
    internal class QueueTest
    {
        [OneTimeSetUp]
        public void CleanDependencies()
        {
            ComDependencyBuilder.ResetDependencies();
            ComTestHelper.ModifyAndBuildComDependencies();
        }

        [SetUp]
        public void SetUp()
        {
            var dependencies = ComTestHelper.ModifyAndBuildComDependencies();

            LoggingHelper.InitConsoleLogger("PDFCreatorTest", LoggingLevel.Off);

            _queue = new Queue();
            _queue.Initialize();

            var translationHelper = new TranslationHelper(new DefaultSettingsProvider(), new AssemblyHelper(GetType().Assembly), new TranslationFactory(), null);
            translationHelper.InitTranslator("None");

            var folderProvider = new FolderProvider(new PrinterPortReader(new RegistryWrap()), new PathWrap());

            var testPageCreator = new TestPageCreator(new ApplicationNameProvider("FREE"), new VersionHelper(new Version(1, 0, 0, 0)), new OsHelper());
            _testPageHelper = new TestPageHelper(folderProvider, dependencies.QueueAdapter.JobInfoQueue, new JobInfoManager(new LocalTitleReplacerProvider(new List<TitleReplacement>()), null), testPageCreator);
        }

        [TearDown]
        public void TearDown()
        {
            while (_queue.Count > 0)
                _queue.DeleteJob(0);

            _queue.ReleaseCom();
        }

        private Queue _queue;
        private TestPageHelper _testPageHelper;

        private void CreateTestPages(int n)
        {
            for (var i = 0; i < n; i++)
            {
                _testPageHelper.CreateTestPage();
            }
        }

        //This test is trivial since without any implementation of _queue.Clear(), it works.
        //But it shows, if the implementation fullfills trivial conditions
        [Test]
        public void Clear_IfQueueEmpty_DoNothing()
        {
            _queue.Clear();
            var isQueueZero = _queue.Count == 0;

            Assert.IsTrue(isQueueZero);
        }

        [Test]
        public void Clear_IfQueueHasElements_CountEquals0()
        {
            //Create meanless print jobs
            CreateTestPages(5);
            _queue.Clear();

            var isQueueZero = _queue.Count == 0;

            Assert.IsTrue(isQueueZero);
        }

        [Test]
        public void DeleteJob_IfIndexIsNegativ_ThrowCOMExeption()
        {
            CreateTestPages(2);

            var ex = Assert.Throws<COMException>(() => _queue.DeleteJob(-1));
            StringAssert.Contains("The given index was out of range.", ex.Message);
        }

        [Test]
        public void DeleteJob_IfIndexTooBig_ThrowCOMExeption()
        {
            CreateTestPages(2);

            var ex = Assert.Throws<COMException>(() => _queue.DeleteJob(3));
            StringAssert.Contains("The given index was out of range.", ex.Message);
        }

        [Test]
        public void DeleteJob_ReducedCountAfterDeletion()
        {
            CreateTestPages(4);

            _queue.DeleteJob(3);

            var count = _queue.Count;

            Assert.AreEqual(3, count);
        }

        [Test]
        public void GetJobByIndex_IfIndexOutOfRange_ThrowsCOMException()
        {
            CreateTestPages(3);

            var ex = Assert.Throws<COMException>(() => _queue.GetJobByIndex(3));
            StringAssert.Contains("Invalid index. Please check the index parameter.", ex.Message);
        }

        [Test]
        public void MergeAllJobs_IfQueueCountExactly1_QueueCountEquals1()
        {
            CreateTestPages(1);
            _queue.MergeAllJobs();

            var jobCount = _queue.Count;

            Assert.AreEqual(1, jobCount);
        }

        [Test]
        public void MergeAllJobs_IfQueueCountGreater1_QueueCountEquals1()
        {
            CreateTestPages(4);
            _queue.MergeAllJobs();

            var jobCount = _queue.Count;

            Assert.AreEqual(1, jobCount);
        }

        [Test]
        public void MergeAllJobs_IfQueueEmpty_ThrowsCOMException()
        {
            var ex = Assert.Throws<COMException>(() => _queue.MergeAllJobs());
            StringAssert.Contains("The queue must not be empty.", ex.Message);
        }

        [Test]
        public void WaitForJobs_IfLessJobsEnteredThanExpected_ReturnsFalse()
        {
            CreateTestPages(5);
            var hasTooFewEntered = !_queue.WaitForJobs(6, 0);

            Assert.IsTrue(hasTooFewEntered);
        }

        [Test]
        public void WaitForJobs_IfTimeoutOver_ReturnFalse()
        {
            var isNotTimedOut = _queue.WaitForJobs(2, 0);

            Assert.IsFalse(isNotTimedOut);
        }
    }
}
