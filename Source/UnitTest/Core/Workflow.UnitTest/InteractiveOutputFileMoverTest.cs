using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Query;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using pdfforge.PDFCreator.Core.Workflow.Output;
using pdfforge.PDFCreator.Core.Workflow.Queries;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.IO;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UnitTest.Core.Workflow
{
    [TestFixture]
    public class InteractiveOutputFileMoverTest
    {
        private InteractiveOutputFileMover _outputFileMover;
        private IDirectory _directory;
        private IFile _file;
        private IPathUtil _pathUtil;
        private IRetypeFileNameQuery _retypeQuery;
        private IDispatcher _dispatcher;
        private Job _job;
        private string[] _singleTempOutputfile;
        private string[] _multipleTempOutputFiles;
        private string[] _multipleTempOutputFilesWithTwoDigits;
        private const string RetypedFilename = "C:\\retype_";

        [SetUp]
        public void Setup()
        {
            var jobInfo = new JobInfo();
            _job = new Job(jobInfo, new ConversionProfile(), new Accounts());

            _singleTempOutputfile = new[] { @"output1.pdf" };
            _multipleTempOutputFiles = new[] { @"output1.png", @"output2.png", @"output3.png" };
            _multipleTempOutputFilesWithTwoDigits = new[]
            {
                @"output1.png", @"output2.png", @"output3.png",
                @"output4.png", @"output5.png", @"output6.png",
                @"output7.png", @"output8.png", @"output9.png",
                @"output10.png"
            };

            _directory = Substitute.For<IDirectory>();

            _file = Substitute.For<IFile>();
            _file.Exists(Arg.Any<string>()).Returns(true);

            _pathUtil = Substitute.For<IPathUtil>();
            _pathUtil.IsValidRootedPath(Arg.Any<string>()).Returns(true);

            _retypeQuery = Substitute.For<IRetypeFileNameQuery>();

            int retypeCount = 0;
            _retypeQuery = Substitute.For<IRetypeFileNameQuery>();
            _retypeQuery
                .RetypeFileNameQuery(Arg.Any<string>(), Arg.Any<OutputFormat>(), Arg.Any<RetypeReason>())
                .Returns(x => new QueryResult<string>(true, $"{RetypedFilename}{++retypeCount}." + _job.Profile.OutputFormat.ToString().ToLower()));

            _dispatcher = Substitute.For<InvokeImmediatelyDispatcher>();

            BuildOutputFileMover();
        }

        private void BuildOutputFileMover()
        {
            _outputFileMover = new InteractiveOutputFileMover(_directory, _file, _pathUtil, _retypeQuery, _dispatcher, Substitute.For<IDirectoryHelper>());
        }

        [Test]
        public void InvalidRootedPath_UserCancels_ThrowAbortWorklowException()
        {
            var invalidRootedPath = "XX:\\File.pdf";
            var format = OutputFormat.PdfX;
            _pathUtil.IsValidRootedPath(invalidRootedPath).Returns(false);
            _job.OutputFileTemplate = invalidRootedPath;
            _job.Profile.OutputFormat = format;

            _retypeQuery.RetypeFileNameQuery(invalidRootedPath, format, RetypeReason.InvalidRootedPath).Returns(new QueryResult<string>(false, ""));

            Assert.ThrowsAsync<AbortWorkflowException>(() => _outputFileMover.MoveOutputFiles(_job));
        }

        [Test]
        public void InvalidRootedPath_UserProceeds_NewPathFromUserQueryIsSetAsOutputFilenameTemplate()
        {
            var invalidRootedPath = "XXXXXXXXXXXXXXXXX:\\InvalidRootedPath.pdf";
            var format = OutputFormat.PdfX;
            _pathUtil.IsValidRootedPath(invalidRootedPath).Returns(false);
            _job.OutputFileTemplate = invalidRootedPath;
            _job.Profile.OutputFormat = format;
            var pathFromUser = "Y:\\ValidRootedPath.pdf";
            _retypeQuery.RetypeFileNameQuery(invalidRootedPath, format, RetypeReason.InvalidRootedPath).Returns(new QueryResult<string>(true, pathFromUser));

            Assert.DoesNotThrowAsync(() => _outputFileMover.MoveOutputFiles(_job));

            Assert.AreEqual(pathFromUser, _job.OutputFileTemplate);
        }

        [Test]
        public void FirstAttemptToCopyFirstFileFails_OnRetypeOutputFilenameGetsCalled_CancelInRetypeFilename_JobOutFilesAreEmpty()
        {
            _file.When(x => x.Copy(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>()))
                 .Do(x => { throw new IOException(); });

            _retypeQuery.RetypeFileNameQuery(Arg.Any<string>(), Arg.Any<OutputFormat>(), Arg.Any<RetypeReason>()).Returns(new QueryResult<string>(false, null));

            _job.Profile.AutoSave.Enabled = false;

            _job.TempOutputFiles = _multipleTempOutputFiles;

            _retypeQuery.RetypeFileNameQuery(Arg.Any<string>(), Arg.Any<OutputFormat>(), Arg.Any<RetypeReason>()).Returns(new QueryResult<string>(false, null));

            Assert.ThrowsAsync<AbortWorkflowException>(() => _outputFileMover.MoveOutputFiles(_job));

            Assert.AreEqual(1, _retypeQuery.ReceivedCalls().Count(), "RetypeOutputFilename was not called exactly once");
            Assert.IsEmpty(_job.OutputFiles, "Outputfiles are not empty.");
        }

        [Test]
        public void FirstAttemptToCopyThirdFileFails_ThrowProcssingExceptionAndDoNotCallRetypeQuery()
        {
            _file.When(x => x.Copy(_multipleTempOutputFiles[2], Arg.Any<string>(), Arg.Any<bool>()))
                 .Do(x =>
                {
                    throw new IOException();
                });

            _job.Profile.AutoSave.Enabled = false;

            _job.TempOutputFiles = _multipleTempOutputFiles;

            Assert.ThrowsAsync<ProcessingException>(() => _outputFileMover.MoveOutputFiles(_job));

            Assert.AreEqual(0, _retypeQuery.ReceivedCalls().Count(), "RetypeQuery mut not be called.");
        }

        [Test]
        public void CopyThirdFileFailsTwice_ThrowsException()
        {
            _file.When(x => x.Copy(_multipleTempOutputFiles[2], Arg.Any<string>(), Arg.Any<bool>()))
                .Do(x =>
                {
                    throw new IOException();
                });

            _job.Profile.AutoSave.Enabled = false;

            _job.TempOutputFiles = _multipleTempOutputFiles;

            Assert.ThrowsAsync<ProcessingException>(() => _outputFileMover.MoveOutputFiles(_job));

            Assert.AreEqual(0, _retypeQuery.ReceivedCalls().Count(), "RetypeOutputFilename was called.");
        }

        [Test]
        public async Task MoveOutFiles_MultipleFilesInteractive_FirstThreeAttemptsToCopyFirstFileFail_OnRetypeOutputFilenameGetsCalledTriple_NewValueForOutputFilenameTemplateAndOutputfile()
        {
            _file = new FailingFileCopy(3);
            _job.Profile.AutoSave.Enabled = false;
            _job.Profile.OutputFormat = OutputFormat.Png;
            _job.TempOutputFiles = _multipleTempOutputFiles;
            BuildOutputFileMover();

            await _outputFileMover.MoveOutputFiles(_job);

            Assert.AreEqual(3, _retypeQuery.ReceivedCalls().Count(), "RetypeOutputFilename was called more than once");
            Assert.AreEqual($"{RetypedFilename}3.png", _job.OutputFileTemplate,
                "OutputFileTemplate is not the one from RetypeOutputFilename");
            Assert.AreEqual($"{RetypedFilename}3" + "1" + ".png", _job.OutputFiles[0],
                "First outputfile is not the one from RetypeOutputFilename");
            Assert.AreEqual($"{RetypedFilename}3" + "2" + ".png", _job.OutputFiles[1],
                "Second outputfile is not the one from RetypeOutputFilename");
            Assert.AreEqual($"{RetypedFilename}3" + "3" + ".png", _job.OutputFiles[2],
                "Third outputfile is not the one from RetypeOutputFilename");
        }

        [Test]
        public async Task FirstAttemptToCopyFirstFileFails_OnRetypeOutputFilenameGetsCalled_NewValueForOutputFilenameTemplateAndOutputfiles()
        {
            _file = new FailingFileCopy(1);
            _job.Profile.AutoSave.Enabled = false;
            _job.Profile.OutputFormat = OutputFormat.Png;
            _job.TempOutputFiles = _multipleTempOutputFiles;
            BuildOutputFileMover();

            await _outputFileMover.MoveOutputFiles(_job);

            Assert.AreEqual(1, _retypeQuery.ReceivedCalls().Count(), "RetypeOutputFilename was called more than once");
            Assert.AreEqual(RetypedFilename + "1.png", _job.OutputFileTemplate,
                "OutputFileTemplate is not the one from RetypeOutputFilename");
            Assert.AreEqual(RetypedFilename + "1" + "1" + ".png", _job.OutputFiles[0],
                "First outputfile is not the one from RetypeOutputFilename");
            Assert.AreEqual(RetypedFilename + "1" + "2" + ".png", _job.OutputFiles[1],
                "Second outputfile is not the one from RetypeOutputFilename");
            Assert.AreEqual(RetypedFilename + "1" + "3" + ".png", _job.OutputFiles[2],
                "Third outputfile is not the one from RetypeOutputFilename");
        }

        [Test]
        public void SingleFile_FirstAttemptToCopyFails_OnRetypeOutputFilenameGetsCalled_CancelInRetypeFilename_JobOutFilesAreEmpty()
        {
            _file = new FailingFileCopy(1);
            _job.Profile.AutoSave.Enabled = false;
            _job.Profile.OutputFormat = OutputFormat.Pdf;
            _retypeQuery.RetypeFileNameQuery(Arg.Any<string>(), Arg.Any<OutputFormat>(), Arg.Any<RetypeReason>()).Returns(new QueryResult<string>(false, null));
            _job.TempOutputFiles = _singleTempOutputfile;
            BuildOutputFileMover();

            Assert.ThrowsAsync<AbortWorkflowException>(async () => await _outputFileMover.MoveOutputFiles(_job));

            Assert.AreEqual(1, _retypeQuery.ReceivedCalls().Count(), "RetypeOutputFilename was called more than once");
            Assert.IsEmpty(_job.OutputFiles, "Outputfiles are not empty.");
        }

        [Test]
        public async Task SingleFile_FirstAttemptToCopyFails_OnRetypeOutputFilenameGetsCalled_NewValueForOutputFilenameTemplateAndOutputfile()
        {
            _file = new FailingFileCopy(1);
            _job.Profile.AutoSave.Enabled = false;
            _job.Profile.OutputFormat = OutputFormat.Pdf;
            _job.TempOutputFiles = _singleTempOutputfile;
            BuildOutputFileMover();

            await _outputFileMover.MoveOutputFiles(_job);

            Assert.AreEqual(1, _retypeQuery.ReceivedCalls().Count(), "RetypeOutputFilename was called more than once");
            Assert.AreEqual(RetypedFilename + "1.pdf", _job.OutputFileTemplate,
                "OutputFileTemplate is not the one from RetypeOutputFilename");
            Assert.AreEqual(RetypedFilename + "1" + ".pdf", _job.OutputFiles[0],
                "First outputfile is not the one from RetypeOutputFilename");
        }

        [Test]
        public async Task SingleFile_ThreeAttemptsToCopyFail_OnRetypeOutputFilenameGetsCalled_NewValueForOutputFilenameTemplateAndOutputfile()
        {
            _file = new FailingFileCopy(3);
            _job.Profile.AutoSave.Enabled = false;
            _job.Profile.OutputFormat = OutputFormat.Pdf;
            _job.TempOutputFiles = _singleTempOutputfile;
            BuildOutputFileMover();

            await _outputFileMover.MoveOutputFiles(_job);

            Assert.AreEqual(3, _retypeQuery.ReceivedCalls().Count(), "RetypeOutputFilename was called more than once");
            Assert.AreEqual(RetypedFilename + "3.pdf", _job.OutputFileTemplate,
                "OutputFileTemplate is not the one from RetypeOutputFilename");
            Assert.AreEqual(RetypedFilename + "3" + ".pdf", _job.OutputFiles[0],
                "First outputfile is not the one from RetypeOutputFilename");
        }

        [Test]
        public void DeleteFileFails_DoesNotThrowException()
        {
            _file.When(x => x.Delete(Arg.Any<string>()))
                 .Do(x => { throw new IOException(); });

            _retypeQuery.RetypeFileNameQuery(Arg.Any<string>(), Arg.Any<OutputFormat>(), Arg.Any<RetypeReason>()).Returns(new QueryResult<string>(false, null));

            _job.Profile.AutoSave.Enabled = false;

            _job.TempOutputFiles = _multipleTempOutputFiles;

            _retypeQuery.RetypeFileNameQuery(Arg.Any<string>(), Arg.Any<OutputFormat>(), Arg.Any<RetypeReason>()).Returns(new QueryResult<string>(false, null));

            Assert.DoesNotThrow(() => _outputFileMover.MoveOutputFiles(_job));
        }
    }
}
