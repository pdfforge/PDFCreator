using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.ComImplementation;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Threading;
using SystemInterface.IO;
using Translatable;
using ThreadPool = pdfforge.PDFCreator.Core.ComImplementation.ThreadPool;

namespace ComImplementation.UnitTest
{
    [TestFixture]
    internal class PrintJobAdapterTest
    {
        private ObservableCollection<ConversionProfile> _profiles;
        private Job _job;
        private IDirectory _directory;
        private IConversionWorkflow _conversionWorkflow;
        private WorkflowResult _workflowResult;
        private IJobInfoQueue _jobInfoQueue;
        private IPathUtil _pathUtil;

        [SetUp]
        public void Setup()
        {
            _workflowResult = WorkflowResult.Finished;

            _jobInfoQueue = Substitute.For<IJobInfoQueue>();

            _profiles = new ObservableCollection<ConversionProfile>();
            _profiles.Add(new ConversionProfile
            {
                Guid = "SomeGuid"
            });
            _profiles.Add(new ConversionProfile
            {
                Guid = "AnotherGuid",
                AutoSave = new AutoSave
                {
                    Enabled = true
                }
            });

            _conversionWorkflow = Substitute.For<IConversionWorkflow>();
            _conversionWorkflow.RunWorkflow(Arg.Any<Job>())
                .Returns(x =>
                {
                    var job = x.Arg<Job>();
                    job.Completed = true;
                    return _workflowResult;
                });
        }

        private PrintJobAdapter BuildPrintJobAdapter()
        {
            var settings = new PdfCreatorSettings();
            settings.ConversionProfiles = _profiles;
            var settingsProvider = Substitute.For<ISettingsProvider>();
            settingsProvider.Settings.Returns(settings);

            var comWorkflowFactory = Substitute.For<IComWorkflowFactory>();

            comWorkflowFactory.BuildWorkflow(Arg.Any<string>())
                .Returns(x => _conversionWorkflow);

            var jobInfo = new JobInfo
            {
                Metadata = new Metadata()
                {
                    Title = "Test"
                }
            };
            jobInfo.SourceFiles.Add(new SourceFileInfo());
            _job = new Job(jobInfo, _profiles[0], new Accounts());

            _directory = Substitute.For<IDirectory>();

            _pathUtil = Substitute.For<IPathUtil>();
            _pathUtil.IsValidRootedPath(Arg.Any<string>()).Returns(true);

            var printJobAdapter = new PrintJobAdapter(settingsProvider, comWorkflowFactory, new ThreadPool(), _jobInfoQueue, new ErrorCodeInterpreter(new TranslationFactory()), _directory, _pathUtil);
            printJobAdapter.Job = _job;

            return printJobAdapter;
        }

        [Test]
        public void SetProfileByGuid_InsertsCopyOfProfileInJob()
        {
            var expectedProfile = _profiles[1];
            var printJobAdapter = BuildPrintJobAdapter();

            printJobAdapter.SetProfileByGuid(expectedProfile.Guid);

            Assert.AreEqual(expectedProfile, printJobAdapter.Job.Profile);
            Assert.AreNotSame(expectedProfile, printJobAdapter.Job.Profile);
        }

        [Test]
        public void SetProfileByGuid_WithUnknownGuid_ThrowsComException()
        {
            var printJobAdapter = BuildPrintJobAdapter();

            Assert.Throws<COMException>(() => printJobAdapter.SetProfileByGuid("Invalid GUID"));
        }

        [Test]
        public void SetProfileSetting_UpdatesProfile()
        {
            var printJobAdapter = BuildPrintJobAdapter();

            printJobAdapter.SetProfileSetting("PdfSettings.ColorModel", "Cmyk");

            Assert.AreEqual(ColorModel.Cmyk, printJobAdapter.Job.Profile.PdfSettings.ColorModel);
        }

        [Test]
        public void SetProfileSetting_WithUnknownProperty_ThrowsComException()
        {
            var printJobAdapter = BuildPrintJobAdapter();

            Assert.Throws<COMException>(() => printJobAdapter.SetProfileSetting("Unknown.Profile.Setting", "Cmyk"));
        }

        [Test]
        public void SetProfileSetting_WithUnaccessibleProperty_ThrowsComException()
        {
            var printJobAdapter = BuildPrintJobAdapter();

            Assert.Throws<COMException>(() => printJobAdapter.SetProfileSetting("AutoSave.Enabled", "True"));
        }

        [Test]
        public void GetProfileSetting_GetsValueFromProfile()
        {
            var printJobAdapter = BuildPrintJobAdapter();

            var value = printJobAdapter.GetProfileSetting("PdfSettings.ColorModel");

            Assert.AreEqual(_profiles[0].PdfSettings.ColorModel.ToString(), value);
        }

        [Test]
        public void GetProfileSetting_WithInvalidSetting_ThrowsComException()
        {
            var printJobAdapter = BuildPrintJobAdapter();

            Assert.Throws<COMException>(() => printJobAdapter.GetProfileSetting("Unknown.Profile.Setting"));
        }

        [Test]
        public void GetProfileSetting_WithEmptySettingName_ThrowsComException()
        {
            var printJobAdapter = BuildPrintJobAdapter();

            Assert.Throws<COMException>(() => printJobAdapter.GetProfileSetting(""));
        }

        [Test]
        public void GetProfileSetting_WithUnaccessibleProperty_ThrowsComException()
        {
            var printJobAdapter = BuildPrintJobAdapter();

            Assert.Throws<COMException>(() => printJobAdapter.GetProfileSetting("AutoSave.Enabled"));
        }

        [Test]
        public void ConvertTo_CallsConversionWorkflow()
        {
            var printJobAdapter = BuildPrintJobAdapter();
            _directory.Exists("X:\\").Returns(true);

            printJobAdapter.ConvertTo("X:\\test.pdf");

            _conversionWorkflow.Received().RunWorkflow(_job);
        }

        [Test]
        public void ConvertTo_WhenSuccessful_SetsSuccessfulFlag()
        {
            var printJobAdapter = BuildPrintJobAdapter();
            _directory.Exists("X:\\").Returns(true);

            printJobAdapter.ConvertTo("X:\\test.pdf");

            Assert.IsTrue(printJobAdapter.IsFinished);
            Assert.IsTrue(printJobAdapter.IsSuccessful);
        }

        [Test]
        public void ConvertTo_WhenSuccessful_RemovesJobFromQueue()
        {
            var printJobAdapter = BuildPrintJobAdapter();
            _directory.Exists("X:\\").Returns(true);

            printJobAdapter.ConvertTo("X:\\test.pdf");

            _jobInfoQueue.Received().Remove(_job.JobInfo, true);
        }

        [Test]
        public void ConvertTo_WhenSuccessful_CallsJobFinishedEvent()
        {
            var printJobAdapter = BuildPrintJobAdapter();
            _directory.Exists("X:\\").Returns(true);
            bool wasCalled = false;
            printJobAdapter.JobFinished += (sender, args) => wasCalled = true;

            printJobAdapter.ConvertTo("X:\\test.pdf");

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void ConvertTo_WhenAbortedByUser_NotSuccessful()
        {
            _workflowResult = WorkflowResult.AbortedByUser;
            var printJobAdapter = BuildPrintJobAdapter();
            _directory.Exists("X:\\").Returns(true);

            printJobAdapter.ConvertTo("X:\\test.pdf");

            Assert.IsTrue(printJobAdapter.IsFinished);
            Assert.IsFalse(printJobAdapter.IsSuccessful);
        }

        [Test]
        public void ConvertTo_WhenFailed_NotSuccessful()
        {
            _workflowResult = WorkflowResult.Error;
            var printJobAdapter = BuildPrintJobAdapter();
            _directory.Exists("X:\\").Returns(true);

            Assert.Throws<COMException>(() => printJobAdapter.ConvertTo("X:\\test.pdf"));

            Assert.IsTrue(printJobAdapter.IsFinished);
            Assert.IsFalse(printJobAdapter.IsSuccessful);
        }

        [Test]
        public void ConvertToAsync_ConvertsSuccessfully()
        {
            var printJobAdapter = BuildPrintJobAdapter();
            _directory.Exists("X:\\").Returns(true);
            var resetEvent = new ManualResetEventSlim();
            printJobAdapter.JobFinished += (sender, args) => resetEvent.Set();

            printJobAdapter.ConvertToAsync("X:\\test.pdf");

            resetEvent.Wait(TimeSpan.FromMilliseconds(100));
            Assert.IsTrue(printJobAdapter.IsFinished);
            Assert.IsTrue(printJobAdapter.IsSuccessful);
        }

        [Test]
        public void ConvertTo_WithoutSourceFiles_IsIgnored()
        {
            var printJobAdapter = BuildPrintJobAdapter();
            _directory.Exists("X:\\").Returns(true);
            _job.JobInfo.SourceFiles.Clear();

            printJobAdapter.ConvertTo("X:\\test.pdf");

            Assert.IsFalse(printJobAdapter.IsSuccessful);
        }

        [Test]
        public void ConvertTo_WithEmptyFilename_ThrowsComException()
        {
            var printJobAdapter = BuildPrintJobAdapter();
            _directory.Exists("X:\\").Returns(true);
            _job.JobInfo.SourceFiles.Clear();

            Assert.Throws<COMException>(() => printJobAdapter.ConvertToAsync(null));

            Assert.IsFalse(printJobAdapter.IsSuccessful);
        }

        [Test]
        public void ConvertTo_WhenTargetDirectoryDoesNotExist_ThrowsComException()
        {
            var printJobAdapter = BuildPrintJobAdapter();
            _directory.Exists("X:\\").Returns(true);
            _job.JobInfo.SourceFiles.Clear();

            Assert.Throws<COMException>(() => printJobAdapter.ConvertToAsync(@"X:\UnknownFolder\test.pdf"));

            Assert.IsFalse(printJobAdapter.IsSuccessful);
        }
    }
}
