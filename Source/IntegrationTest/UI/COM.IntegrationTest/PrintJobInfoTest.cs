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
using SystemWrapper.IO;
using SystemWrapper.Microsoft.Win32;
using Translatable;

namespace pdfforge.PDFCreator.IntegrationTest.UI.COM
{
    [TestFixture]
    internal class PrintJobInfoTest
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

            var assembly = GetType().Assembly;
            var assemblyHelper = new AssemblyHelper(assembly);

            var translationHelper = new TranslationHelper(new DefaultSettingsProvider(), assemblyHelper, new TranslationFactory(), null);
            translationHelper.InitTranslator("None");

            var folderProvider = new FolderProvider(new PrinterPortReader(new RegistryWrap()), new PathWrap());

            var testPageCreator = new TestPageCreator(new ApplicationNameProvider("FREE"), new VersionHelper(new Version(1, 0, 0, 0)), new OsHelper());
            _testPageHelper = new TestPageHelper(folderProvider, dependencies.QueueAdapter.JobInfoQueue, new JobInfoManager(new LocalTitleReplacerProvider(new List<TitleReplacement>()), null), testPageCreator);

            CreateTestPages(1);

            _printJob = _queue.NextJob;
            _printJobInfo = _printJob.PrintJobInfo;
        }

        [TearDown]
        public void TearDown()
        {
            _queue.ReleaseCom();
        }

        private Queue _queue;
        private PrintJob _printJob;
        private PrintJobInfo _printJobInfo;
        private TestPageHelper _testPageHelper;

        private void CreateTestPages(int n)
        {
            for (var i = 0; i < n; i++)
            {
                _testPageHelper.CreateTestPage();
            }
        }

        [Test]
        public void PrintJobInfo_IfAccessed_NotNull()
        {
            Assert.NotNull(_printJobInfo);
        }

        [Test]
        public void PrintJobInfo_IfAuthorIsSet_PropertyContentEqualsAuthor()
        {
            var author = "SomeAuthor";
            _printJobInfo.PrintJobAuthor = author;

            Assert.AreSame(author, _printJobInfo.PrintJobAuthor);
        }

        [Test]
        public void PrintJobInfo_IfKeywordsAreSet_PropertyContentEqualsKeywords()
        {
            var keywords = "SomeKeywords";
            _printJobInfo.Keywords = keywords;

            Assert.AreSame(keywords, _printJobInfo.Keywords);
        }

        [Test]
        public void PrintJobInfo_IfNameIsSet_PropertyContentEqualsName()
        {
            var name = "SomeName";
            _printJobInfo.PrintJobName = name;

            Assert.AreSame(name, _printJobInfo.PrintJobName);
        }

        [Test]
        public void PrintJobInfo_IfSubjectIsSet_PropertyContentEqualsSubject()
        {
            var subject = "SomeSubject";
            _printJobInfo.Subject = subject;

            Assert.AreSame(subject, _printJobInfo.Subject);
        }

        [Test]
        public void PrintJobInfo_IfTestPageIsPrinted_PrintJobAuthorNotNullOrEmpty()
        {
            var jobAuthor = _printJobInfo.PrintJobAuthor;

            Assert.IsFalse(string.IsNullOrEmpty(jobAuthor));
        }

        [Test]
        public void PrintJobInfo_IfTestPageIsPrinted_PrintJobNameContainsTestpage()
        {
            var jobName = _printJobInfo.PrintJobName;

            StringAssert.Contains("testpage", jobName.ToLower());
        }

        [Test]
        public void PrintJobInfo_KeywordsInitiallyNullOrEmpty()
        {
            var keywords = _printJobInfo.Keywords;

            Assert.IsTrue(string.IsNullOrEmpty(keywords));
        }

        [Test]
        public void PrintJobInfo_PrintJobNameNotNullOrEmpty()
        {
            var jobName = _printJobInfo.PrintJobName;

            Assert.IsFalse(string.IsNullOrEmpty(jobName));
        }

        [Test]
        public void PrintJobInfo_SubjectInitiallyNullOrEmpty()
        {
            var subject = _printJobInfo.Subject;

            Assert.IsTrue(string.IsNullOrEmpty(subject));
        }
    }
}
