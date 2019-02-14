using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Core.Workflow.ComposeTargetFilePath;
using pdfforge.PDFCreator.Utilities;

namespace Presentation.UnitTest.Workflow.ComposeTargetFilePath
{
    [TestFixture]
    public class TargetFilePathComposerTest
    {
        private ITargetFilePathComposer _targetFilePathComposer;
        private IPathUtil _pathUtil;
        private ILastSaveDirectoryHelper _lastSaveDirectoryHelper;
        private Job _job;

        [SetUp]
        public void SetUp()
        {
            _pathUtil = Substitute.For<IPathUtil>();
            _lastSaveDirectoryHelper = Substitute.For<ILastSaveDirectoryHelper>();

            _targetFilePathComposer = new TargetFilePathComposer(_pathUtil, _lastSaveDirectoryHelper);

            _job = new Job(new JobInfo(), new ConversionProfile(), new Accounts());
        }

        [Test]
        public void OutputPath_OutputFileParameterIsSet_OutputFileParameterIsValidPath_ReturnOutputFileParameter()
        {
            var outputFileParameter = "OutputFileParameter";
            _job.JobInfo.OutputFileParameter = outputFileParameter;
            _pathUtil.IsValidRootedPath(outputFileParameter).Returns(true);

            var composedFilePath = _targetFilePathComposer.ComposeTargetFilePath(_job);

            Assert.AreEqual(outputFileParameter, composedFilePath);
        }

        /*
        [Test]
        public void OutputFolder_TargetDirectoryIsEmpty_AutoSaveDisabled_LastSaveDirIsNotEmpty_ReturnLastSaveDir()
        {
            _job.Profile.TargetDirectory = "";
            _job.Profile.AutoSave.Enabled = false;
            _lastSaveDirectoryHelper.IsEnabled(_job).Returns(true);
            var lastSaveDirectory = "lastSaveDirectory";
            _lastSaveDirectoryHelper.GetDirectory().Returns(lastSaveDirectory);

            var composedFilePath = _targetFilePathComposer.ComposeTargetFilePath(_job);

            Assert.AreEqual(lastSaveDirectory, composedFilePath);
        }
        */
        //Todo: Make "safe" methods of PathUtil static to write test without stupid mocking
    }
}
