using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using pdfforge.PDFCreator.Core.Workflow.Output;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.IO;
using System.IO;
using System.Threading.Tasks;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UnitTest.Core.Workflow
{
    internal class AutoSaveOutputFileMoverTest
    {
        private AutosaveOutputFileMover _autosaveOutputFileMover;
        private IFile _file;
        private IPathUtil _pathUtil;
        private Job _job;
        private string[] _singleTempOutputfile;
        private IDirectoryHelper _directoryHelper;

        [SetUp]
        public void Setup()
        {
            var jobInfo = new JobInfo();
            _job = new Job(jobInfo, new ConversionProfile(), new Accounts());
            _job.OutputFileTemplate = @"X:\temp\test.pdf";

            _singleTempOutputfile = new[] { @"output1.pdf" };

            _file = Substitute.For<IFile>();
            _file.Exists(Arg.Any<string>()).Returns(true);
            _directoryHelper = Substitute.For<IDirectoryHelper>();

            var path = Substitute.For<IPath>();

            _pathUtil = new PathUtil(path, Substitute.For<IDirectory>());

            _autosaveOutputFileMover = new AutosaveOutputFileMover(Substitute.For<IDirectory>(), _file, _pathUtil, _directoryHelper);
        }

        [Test]
        public void MoveOutPutFiles_InvalidRootedPath_ThrowsAbortWorkflowException()
        {
            _job.OutputFileTemplate = @"test.pdf";
            Assert.ThrowsAsync<AbortWorkflowException>(() => _autosaveOutputFileMover.MoveOutputFiles(_job));
        }

        [Test]
        public async Task SingleFile_WhenOutputFileExists_UsesEnsureUniqueFilename()
        {
            var outputFile = _singleTempOutputfile[0];

            _file.Exists(Arg.Is<string>(x => x != outputFile)).Returns(false);

            _job.Profile.AutoSave.Enabled = true;
            _job.Profile.AutoSave.EnsureUniqueFilenames = true;

            _job.TempOutputFiles = _singleTempOutputfile;

            await _autosaveOutputFileMover.MoveOutputFiles(_job);

            _job.TempOutputFiles = _singleTempOutputfile;

            await _autosaveOutputFileMover.MoveOutputFiles(_job);

            Assert.AreNotEqual(outputFile, _job.OutputFiles[0], "EnsureUniqueFilename was not applied");
        }

        [Test]
        public async Task SingleFile_Calls_DirectoryHelper()
        {
            await _autosaveOutputFileMover.MoveOutputFiles(_job);

            _directoryHelper.Received(1).CreateDirectory(Path.GetDirectoryName(_job.OutputFileTemplate));
        }
    }
}
