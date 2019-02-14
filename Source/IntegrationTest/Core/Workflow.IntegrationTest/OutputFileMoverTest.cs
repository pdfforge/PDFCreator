using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Query;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Workflow.Output;
using pdfforge.PDFCreator.Core.Workflow.Queries;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.IO;
using Rhino.Mocks;
using System.IO;
using System.Threading.Tasks;
using SystemInterface.IO;
using SystemWrapper.IO;
using Arg = NSubstitute.Arg;

namespace pdfforge.PDFCreator.IntegrationTest.Core.Workflow
{
    [TestFixture]
    internal class OutputFileMoverTest
    {
        [SetUp]
        public void SetUp()
        {
            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder("DevicesGeneralTests");

            _singleTempOutputfile = new[] { @"output1.pdf" };
            _multipleTempOutputFiles = new[] { @"output1.png", @"output2.png", @"output3.png" };
            _multipleTempOutputFilesWithTwoDigits = new[]
            {
                @"output1.png", @"output2.png", @"output3.png",
                @"output4.png", @"output5.png", @"output6.png",
                @"output7.png", @"output8.png", @"output9.png",
                @"output10.png"
            };

            _countRetypeOutputFilename = 0;
            _cancelRetypeFilename = false;

            _pathUtil = new PathUtil(new PathWrap(), new DirectoryWrap());

            _queryRetypeFileName = Substitute.For<IRetypeFileNameQuery>();
            _queryRetypeFileName.RetypeFileNameQuery(Arg.Any<string>(), Arg.Any<OutputFormat>(), Arg.Any<RetypeReason>()).Returns(RetypeOutputFilename);
        }

        private QueryResult<string> RetypeOutputFilename(CallInfo callInfo)
        {
            _countRetypeOutputFilename++;
            var result = new QueryResult<string>();
            result.Data = RetypedFilename + _countRetypeOutputFilename;
            result.Success = !_cancelRetypeFilename;
            return result;
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
        }

        private TestHelper _th;

        /// <summary>
        ///     Filename for temporary outputfile (output.pdf)
        /// </summary>
        private string[] _singleTempOutputfile;

        /// <summary>
        ///     Filenames for temporary outputfiles (output.png, output2.png, output3.png)
        /// </summary>
        private string[] _multipleTempOutputFiles;

        /// <summary>
        ///     Filenames for temporary outputfiles (output.png, output2.png, ..., output10.png)
        /// </summary>
        private string[] _multipleTempOutputFilesWithTwoDigits;

        private IPathUtil _pathUtil;

        private int _countRetypeOutputFilename;
        private const string RetypedFilename = "RetypeFilename";
        private bool _cancelRetypeFilename;
        private IRetypeFileNameQuery _queryRetypeFileName;

        private string RemoveExtension(string filePath)
        {
            var directory = PathSafe.GetDirectoryName(filePath);
            var fileWithoutExtension = PathSafe.GetFileNameWithoutExtension(filePath);
            return Path.Combine(directory, fileWithoutExtension);
        }

        private AutosaveOutputFileMover BuildAutosaveOutputFileMover(IFile fileStub)
        {
            return new AutosaveOutputFileMover(new DirectoryWrap(), fileStub, _pathUtil, Substitute.For<IDirectoryHelper>());
        }

        private InteractiveOutputFileMover BuildInteractiveOutputFileMover(IFile fileStub)
        {
            return new InteractiveOutputFileMover(new DirectoryWrap(), fileStub, _pathUtil, _queryRetypeFileName, new InvokeImmediatelyDispatcher(), Substitute.For<IDirectoryHelper>());
        }

        [Test]
        public async Task MoveOutputFiles_MultipleFiles_AutoSaveWithEnsureUniqueFilenames_FirstFileExists_FirstAttemptToCopyFails_FileExists_FirstOutputfileHasContinuedAppendix()
        {
            var fileStub = MockRepository.GenerateStub<IFile>();
            fileStub.Stub(x => x.Copy(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything))
                .Throw(new IOException("IoException"))
                .Repeat.Once();
            fileStub.Stub(x => x.Exists(Arg<string>.Is.Anything)).Return(true).Repeat.Once();
            //Simulate existing file in first request for unique filename
            fileStub.Stub(x => x.Exists(Arg<string>.Is.Anything)).IgnoreArguments().Return(false).Repeat.Once();
            fileStub.Stub(x => x.Exists(Arg<string>.Is.Anything)).Return(true).Repeat.Once();
            //Simulate existing file after first attempt to copy has failed.
            fileStub.Stub(x => x.Exists(Arg<string>.Is.Anything)).IgnoreArguments().Return(false);
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Pdf);
            var outputFileMover = BuildAutosaveOutputFileMover(fileStub);
            _th.Job.Profile.AutoSave.Enabled = true;
            _th.Job.Profile.AutoSave.EnsureUniqueFilenames = true;
            _th.Job.TempOutputFiles = _multipleTempOutputFiles;
            var filenameTemplate = _th.Job.OutputFileTemplate; //Save it, because it can change in MoveOutputFiles

            await outputFileMover.MoveOutputFiles(_th.Job);

            fileStub.AssertWasCalled(x => x.Copy("ignore", "ignore2", true),
                options => options.IgnoreArguments().Repeat.Times(4));
            //Four times, once for each file and once for failed attempt
            fileStub.AssertWasCalled(x => x.Delete("ignore"), options => options.IgnoreArguments().Repeat.Times(3));
            //Three times, once for each file
            var templateWithoutExtension = RemoveExtension(filenameTemplate);
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[0],
                "First outputfile is not based on template from job.");
            StringAssert.EndsWith("1_3.png", _th.Job.OutputFiles[0], "Failure in file appendix.");
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[1],
                "Second outputfile is not based on template from job.");
            StringAssert.EndsWith("2.png", _th.Job.OutputFiles[1], "Failure in file appendix.");
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[2],
                "Third outputfile is not based on template from job.");
            StringAssert.EndsWith("3.png", _th.Job.OutputFiles[2], "Failure in file appendix.");
        }

        [Test]
        public async Task MoveOutputFiles_MultipleFiles_AutoSaveWithEnsureUniqueFilenames_FirstFileExists_FirstOutputfileMustHaveAppendix()
        {
            var fileStub = MockRepository.GenerateStub<IFile>();
            fileStub.Stub(x => x.Copy(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything))
                .Repeat.Once();
            fileStub.Stub(x => x.Exists(Arg<string>.Is.Anything)).Return(true).Repeat.Once();
            //Simulate existing file in first request for unique filename
            fileStub.Stub(x => x.Exists(Arg<string>.Is.Anything)).IgnoreArguments().Return(false);
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Pdf);
            var outputFileMover = BuildAutosaveOutputFileMover(fileStub);
            _th.Job.Profile.AutoSave.Enabled = true;
            _th.Job.Profile.AutoSave.EnsureUniqueFilenames = true;
            _th.Job.TempOutputFiles = _multipleTempOutputFiles;
            var filenameTemplate = _th.Job.OutputFileTemplate; //Save it, because it can change in MoveOutputFiles

            await outputFileMover.MoveOutputFiles(_th.Job);

            fileStub.AssertWasCalled(x => x.Copy("ignore", "ignore2", true),
                options => options.IgnoreArguments().Repeat.Times(3)); //Three times, once for each file
            fileStub.AssertWasCalled(x => x.Delete("ignore"), options => options.IgnoreArguments().Repeat.Times(3));
            //Three times, once for each file
            var templateWithoutExtension = RemoveExtension(filenameTemplate);
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[0],
                "First outputfile is not based on template from job.");
            StringAssert.EndsWith("1_2.png", _th.Job.OutputFiles[0], "Failure in file appendix.");
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[1],
                "Second outputfile is not based on template from job.");
            StringAssert.EndsWith("2.png", _th.Job.OutputFiles[1], "Failure in file appendix.");
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[2],
                "Third outputfile is not based on template from job.");
            StringAssert.EndsWith("3.png", _th.Job.OutputFiles[2], "Failure in file appendix.");
        }

        [Test]
        public async Task MoveOutputFiles_MultipleFiles_AutoSaveWithEnsureUniqueFilenames_ThirdFileExists_FirstAttemptToCopyFails_FileExists_ThirdOutputfileHasContinuedAppendix()
        {
            var fileStub = MockRepository.GenerateStub<IFile>();
            fileStub.Stub(x => x.Copy(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything))
                .Repeat.Twice();
            fileStub.Stub(x => x.Copy(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything))
                .Throw(new IOException("IoException"))
                .Repeat.Once();
            fileStub.Stub(x => x.Exists(Arg<string>.Is.Anything)).Return(false).Repeat.Twice();
            fileStub.Stub(x => x.Exists(Arg<string>.Is.Anything)).IgnoreArguments().Return(true).Repeat.Twice();
            fileStub.Stub(x => x.Exists(Arg<string>.Is.Anything)).IgnoreArguments().Return(false);
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Pdf);
            var outputFileMover = BuildAutosaveOutputFileMover(fileStub);
            _th.Job.Profile.AutoSave.Enabled = true;
            _th.Job.Profile.AutoSave.EnsureUniqueFilenames = true;
            _th.Job.TempOutputFiles = _multipleTempOutputFiles;
            var filenameTemplate = _th.Job.OutputFileTemplate; //Save it, because it can change in MoveOutputFiles

            await outputFileMover.MoveOutputFiles(_th.Job);

            fileStub.AssertWasCalled(x => x.Copy("ignore", "ignore2", true),
                options => options.IgnoreArguments().Repeat.Times(4));
            //Four times, once for each file and once for failed attempt
            fileStub.AssertWasCalled(x => x.Delete("ignore"), options => options.IgnoreArguments().Repeat.Times(3));
            //Three times, once for each file
            var templateWithoutExtension = RemoveExtension(filenameTemplate);
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[0],
                "First outputfile is not based on template from job.");
            StringAssert.EndsWith("1.png", _th.Job.OutputFiles[0], "Failure in file appendix.");
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[1],
                "Second outputfile is not based on template from job.");
            StringAssert.EndsWith("2.png", _th.Job.OutputFiles[1], "Failure in file appendix.");
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[2],
                "Third outputfile is not based on template from job.");
            StringAssert.EndsWith("3_3.png", _th.Job.OutputFiles[2], "Failure in file appendix.");
        }

        [Test]
        public async Task MoveOutputFiles_MultipleFiles_AutoSaveWithEnsureUniqueFilenames_ThirdFileExists_ThirdOutputfileMustHaveAppendix()
        {
            var fileStub = MockRepository.GenerateStub<IFile>();
            fileStub.Stub(x => x.Copy(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything))
                .Repeat.Once();
            fileStub.Stub(x => x.Exists(Arg<string>.Is.Anything)).Return(false).Repeat.Twice();
            //Simulate existing file in third request for unique filename
            fileStub.Stub(x => x.Exists(Arg<string>.Is.Anything)).Return(true).Repeat.Once();
            fileStub.Stub(x => x.Exists(Arg<string>.Is.Anything)).Return(false);
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Pdf);
            var outputFileMover = BuildAutosaveOutputFileMover(fileStub);
            _th.Job.Profile.AutoSave.Enabled = true;
            _th.Job.Profile.AutoSave.EnsureUniqueFilenames = true;
            _th.Job.TempOutputFiles = _multipleTempOutputFiles;
            var filenameTemplate = _th.Job.OutputFileTemplate; //Save it, because it can change in MoveOutputFiles

            await outputFileMover.MoveOutputFiles(_th.Job);

            fileStub.AssertWasCalled(x => x.Copy("ignore", "ignore2", true),
                options => options.IgnoreArguments().Repeat.Times(3)); //Three times, once for each file
            fileStub.AssertWasCalled(x => x.Delete("ignore"), options => options.IgnoreArguments().Repeat.Times(3));
            //Three times, once for each file
            var templateWithoutExtension = RemoveExtension(filenameTemplate);
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[0],
                "First outputfile is not based on template from job.");
            StringAssert.EndsWith("1.png", _th.Job.OutputFiles[0], "Failure in file appendix.");
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[1],
                "Second outputfile is not based on template from job.");
            StringAssert.EndsWith("2.png", _th.Job.OutputFiles[1], "Failure in file appendix.");
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[2],
                "Third outputfile is not based on template from job.");
            StringAssert.EndsWith("3_2.png", _th.Job.OutputFiles[2], "Failure in file appendix.");
        }

        [Test]
        public async Task MoveOutputFiles_MultipleFiles_AutoSaveWithoutEnsureUniqueFilenames_FirstFileExists_OutputfileMustNotHaveAppendix()
        {
            var fileStub = MockRepository.GenerateStub<IFile>();
            fileStub.Stub(x => x.Copy(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything))
                .Repeat.Once();
            fileStub.Stub(x => x.Exists(Arg<string>.Is.Anything)).Return(true).Repeat.Once();
            //Simulate existing file in first request for unique filename
            fileStub.Stub(x => x.Exists(Arg<string>.Is.Anything)).IgnoreArguments().Return(false);
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Pdf);
            var outputFileMover = BuildAutosaveOutputFileMover(fileStub);
            _th.Job.Profile.AutoSave.Enabled = true;
            _th.Job.Profile.AutoSave.EnsureUniqueFilenames = false;
            _th.Job.TempOutputFiles = _multipleTempOutputFiles;
            var filenameTemplate = _th.Job.OutputFileTemplate; //Save it, because it can change in MoveOutputFiles

            await outputFileMover.MoveOutputFiles(_th.Job);

            fileStub.AssertWasCalled(x => x.Copy("ignore", "ignore2", true),
                options => options.IgnoreArguments().Repeat.Times(3)); //Three times, once for each file
            fileStub.AssertWasCalled(x => x.Delete("ignore"), options => options.IgnoreArguments().Repeat.Times(3));
            //Three times, once for each file
            var templateWithoutExtension = RemoveExtension(filenameTemplate);
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[0],
                "First outputfile is not based on template from job.");
            StringAssert.EndsWith("1.png", _th.Job.OutputFiles[0], "Failure in file appendix.");
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[1],
                "Second outputfile is not based on template from job.");
            StringAssert.EndsWith("2.png", _th.Job.OutputFiles[1], "Failure in file appendix.");
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[2],
                "Third outputfile is not based on template from job.");
            StringAssert.EndsWith("3.png", _th.Job.OutputFiles[2], "Failure in file appendix.");
        }

        [Test]
        public async Task MoveOutputFiles_MultipleFiles_AutoSaveWithoutEnsureUniqueFilenames_ThirdFileExists_ThirdOutputfileMustNotHaveAppendix()
        {
            var fileStub = MockRepository.GenerateStub<IFile>();
            fileStub.Stub(x => x.Copy(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything))
                .Repeat.Once();
            fileStub.Stub(x => x.Exists(Arg<string>.Is.Anything)).Return(false).Repeat.Twice();
            //Simulate existing file in third request for unique filename
            fileStub.Stub(x => x.Exists(Arg<string>.Is.Anything)).Return(true).Repeat.Once();
            fileStub.Stub(x => x.Exists(Arg<string>.Is.Anything)).Return(false);
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Pdf);
            var outputFileMover = BuildAutosaveOutputFileMover(fileStub);
            _th.Job.Profile.AutoSave.Enabled = true;
            _th.Job.Profile.AutoSave.EnsureUniqueFilenames = false;
            _th.Job.TempOutputFiles = _multipleTempOutputFiles;
            var filenameTemplate = _th.Job.OutputFileTemplate; //Save it, because it can change in MoveOutputFiles

            await outputFileMover.MoveOutputFiles(_th.Job);

            fileStub.AssertWasCalled(x => x.Copy("ignore", "ignore2", true),
                options => options.IgnoreArguments().Repeat.Times(3)); //Three times, once for each file
            fileStub.AssertWasCalled(x => x.Delete("ignore"), options => options.IgnoreArguments().Repeat.Times(3));
            //Three times, once for each file
            var templateWithoutExtension = RemoveExtension(filenameTemplate);
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[0],
                "First outputfile is not based on template from job.");
            StringAssert.EndsWith("1.png", _th.Job.OutputFiles[0], "Failure in file appendix.");
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[1],
                "Second outputfile is not based on template from job.");
            StringAssert.EndsWith("2.png", _th.Job.OutputFiles[1], "Failure in file appendix.");
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[2],
                "Third outputfile is not based on template from job.");
            StringAssert.EndsWith("3.png", _th.Job.OutputFiles[2], "Failure in file appendix.");
        }

        [Test]
        public async Task MoveOutputFiles_MultipleFiles_FileAppendixIncrements()
        {
            var fileStub = MockRepository.GenerateStub<IFile>();
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Png);
            var outputFileMover = BuildAutosaveOutputFileMover(fileStub);
            _th.Job.TempOutputFiles = _multipleTempOutputFiles;
            _th.Job.Profile.AutoSave.Enabled = true;
            var filenameTemplate = _th.Job.OutputFileTemplate; //Save it, because it can change in MoveOutputFiles

            await outputFileMover.MoveOutputFiles(_th.Job);

            fileStub.AssertWasCalled(x => x.Copy("ignore", "ignore2", true),
                options => options.IgnoreArguments().Repeat.Times(3)); //Three times, once for each file
            fileStub.AssertWasCalled(x => x.Delete("ignore"), options => options.IgnoreArguments().Repeat.Times(3));
            //Three times, once for each file
            Assert.AreEqual(3, _th.Job.OutputFiles.Count, "Wrong number of outputfiles.");
            var templateWithoutExtension = RemoveExtension(filenameTemplate);
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[0],
                "First outputfile is not based on template from job.");
            StringAssert.EndsWith("1.png", _th.Job.OutputFiles[0], "Failure in file appendix.");
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[1],
                "Second outputfile is not based on template from job.");
            StringAssert.EndsWith("2.png", _th.Job.OutputFiles[1], "Failure in file appendix.");
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[2],
                "Third outputfile is not based on template from job.");
            StringAssert.EndsWith("3.png", _th.Job.OutputFiles[2], "Failure in file appendix.");
        }

        [Test]
        public async Task MoveOutputFiles_MultipleFiles_FirstAttemptToCopyFirstFileFailsSecondIsSuccessful_FirstOutputfileMustHaveAppendix()
        {
            var fileStub = MockRepository.GenerateStub<IFile>();
            fileStub.Stub(x => x.Copy(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything))
                .Throw(new IOException("IoException"))
                .Repeat.Once();
            fileStub.Stub(x => x.Exists(Arg<string>.Is.Anything)).Return(true).Repeat.Once();
            //Simulate existing file in first request for unique filename
            fileStub.Stub(x => x.Exists(Arg<string>.Is.Anything)).IgnoreArguments().Return(false);
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Png);
            var outputFileMover = BuildAutosaveOutputFileMover(fileStub);
            _th.Job.TempOutputFiles = _multipleTempOutputFiles;
            var filenameTemplate = _th.Job.OutputFileTemplate; //Save it, because it can change in MoveOutputFiles
            _th.Job.Profile.AutoSave.Enabled = true;

            await outputFileMover.MoveOutputFiles(_th.Job);

            fileStub.AssertWasCalled(x => x.Copy("ignore", "ignore2", true),
                options => options.IgnoreArguments().Repeat.Times(4));
            //Four times, once for each file and one for "failed" attempt
            fileStub.AssertWasCalled(x => x.Delete("ignore"), options => options.IgnoreArguments().Repeat.Times(3));
            //Three times, once for each file
            var templateWithoutExtension = RemoveExtension(filenameTemplate);
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[0],
                "First outputfile is not based on template from job.");
            StringAssert.EndsWith("1_2.png", _th.Job.OutputFiles[0], "Failure in file appendix.");
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[1],
                "Second outputfile is not based on template from job.");
            StringAssert.EndsWith("2.png", _th.Job.OutputFiles[1], "Failure in file appendix.");
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[2],
                "Third outputfile is not based on template from job.");
            StringAssert.EndsWith("3.png", _th.Job.OutputFiles[2], "Failure in file appendix.");
        }

        [Test]
        public async Task MoveOutputFiles_MultipleFiles_FirstAttemptToCopyThirdFileFailsSecondIsSuccessful_FirstOutputfileMustHaveAppendix()
        {
            var fileStub = MockRepository.GenerateStub<IFile>();
            fileStub.Stub(x => x.Copy(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything))
                .Repeat.Twice(); //Copying for first two files is successful
            fileStub.Stub(x => x.Copy(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything))
                .Throw(new IOException("IoException"))
                .Repeat.Once();
            fileStub.Stub(x => x.Exists(Arg<string>.Is.Anything)).Return(true).Repeat.Once();
            //Simulate existing file in first request for unique filename
            fileStub.Stub(x => x.Exists(Arg<string>.Is.Anything)).IgnoreArguments().Return(false);
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Png);
            var outputFileMover = BuildInteractiveOutputFileMover(fileStub);
            _th.Job.TempOutputFiles = _multipleTempOutputFiles;
            var filenameTemplate = _th.Job.OutputFileTemplate; //Save it, because it can change in MoveOutputFiles

            await outputFileMover.MoveOutputFiles(_th.Job);

            fileStub.AssertWasCalled(x => x.Copy("ignore", "ignore2", true),
                options => options.IgnoreArguments().Repeat.Times(4));
            //Four times, once for each file and one for "failed" attempt
            fileStub.AssertWasCalled(x => x.Delete("ignore"), options => options.IgnoreArguments().Repeat.Times(3));
            //Four times, once for each file one for "failed" attempt
            var templateWithoutExtension = RemoveExtension(filenameTemplate);
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[0],
                "First outputfile is not based on template from job.");
            StringAssert.EndsWith("1.png", _th.Job.OutputFiles[0], "Failure in file appendix.");
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[1],
                "Second outputfile is not based on template from job.");
            StringAssert.EndsWith("2.png", _th.Job.OutputFiles[1], "Failure in file appendix.");
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[2],
                "Third outputfile is not based on template from job.");
            StringAssert.EndsWith("3_2.png", _th.Job.OutputFiles[2], "Failure in file appendix.");
        }

        [Test]
        public void MoveOutputFiles_MultipleFiles_TwoAttemptsToCopyFirstFileFail_ThrowsProcessingException()
        {
            var fileStub = MockRepository.GenerateStub<IFile>();
            fileStub.Stub(x => x.Copy("ignore", "ignore2", true)).IgnoreArguments().Throw(new IOException());
            //Deny all attempts to copy

            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Png);
            var outputFileMover = BuildAutosaveOutputFileMover(fileStub);
            _th.Job.TempOutputFiles = _multipleTempOutputFiles;
            _th.Job.Profile.AutoSave.Enabled = true;

            var ex = Assert.ThrowsAsync<ProcessingException>(() => outputFileMover.MoveOutputFiles(_th.Job));
            Assert.AreEqual(ErrorCode.Conversion_ErrorWhileCopyingOutputFile, ex.ErrorCode);

            fileStub.AssertWasCalled(x => x.Copy("", "", true), options => options.IgnoreArguments().Repeat.Twice());
            //DeviceException should be thrown after second denied copy call
            fileStub.AssertWasNotCalled(x => x.Delete("ignore"), options => options.IgnoreArguments());
            //Delete never gets called
        }

        [Test]
        public async Task MoveOutputFiles_MultipleFilesWithTwoDigits_FirstAttemptToCopyFirstFileFails_OnRetypeOutputFilenameGetsCalled_NewValueForOutputFilenameTemplateAndOutputfiles()
        {
            var fileStub = MockRepository.GenerateStub<IFile>();
            fileStub.Stub(x => x.Copy(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything))
                .Throw(new IOException("IoException"))
                .Repeat.Once();
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Png);
            var outputFileMover = BuildInteractiveOutputFileMover(fileStub);
            _th.Job.Profile.AutoSave.Enabled = false;

            _th.Job.TempOutputFiles = _multipleTempOutputFilesWithTwoDigits;

            await outputFileMover.MoveOutputFiles(_th.Job);

            Assert.AreEqual(1, _countRetypeOutputFilename, "RetypeOutputFilename was called more than once");
            Assert.AreEqual(RetypedFilename + "1", _th.Job.OutputFileTemplate,
                "OutputFileTemplate is not the one from RetypeOutputFilename");
            Assert.AreEqual(RetypedFilename + "1" + "01" + ".png", _th.Job.OutputFiles[0],
                "First outputfile is not the one from RetypeOutputFilename");
            Assert.AreEqual(RetypedFilename + "1" + "02" + ".png", _th.Job.OutputFiles[1],
                "Second outputfile is not the one from RetypeOutputFilename");
            Assert.AreEqual(RetypedFilename + "1" + "03" + ".png", _th.Job.OutputFiles[2],
                "Third outputfile is not the one from RetypeOutputFilename");
        }

        [Test]
        public async Task MoveOutputFiles_MultipleFilesWithTwoDigits_OutputfileAppendixMustHaveTwoDigits()
        {
            var fileStub = MockRepository.GenerateStub<IFile>();
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Pdf);
            var outputFileMover = BuildAutosaveOutputFileMover(fileStub);
            _th.Job.Profile.AutoSave.Enabled = true;
            _th.Job.TempOutputFiles = _multipleTempOutputFilesWithTwoDigits;
            var filenameTemplate = _th.Job.OutputFileTemplate; //Save it, because it can change in MoveOutputFiles

            await outputFileMover.MoveOutputFiles(_th.Job);

            fileStub.AssertWasCalled(x => x.Copy("ignore", "ignore2", true),
                options => options.IgnoreArguments().Repeat.Times(10)); //Ten times, once for each file
            fileStub.AssertWasCalled(x => x.Delete("ignore"), options => options.IgnoreArguments().Repeat.Times(10));
            //Ten times, once for each file
            Assert.AreEqual(10, _th.Job.OutputFiles.Count, "Wrong number of outputfiles.");
            var templateWithoutExtension = RemoveExtension(filenameTemplate);
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[0],
                "First outputfile is not based on template from job.");
            StringAssert.EndsWith("01.png", _th.Job.OutputFiles[0], "Failure in file appendix.");
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[1],
                "Second outputfile is not based on template from job.");
            StringAssert.EndsWith("02.png", _th.Job.OutputFiles[1], "Failure in file appendix.");
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[2],
                "Third outputfile is not based on template from job.");
            StringAssert.EndsWith("03.png", _th.Job.OutputFiles[2], "Failure in file appendix.");
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[0],
                "Fouth outputfile is not based on template from job.");
            StringAssert.EndsWith("04.png", _th.Job.OutputFiles[3], "Failure in file appendix.");
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[1],
                "Fifth outputfile is not based on template from job.");
            StringAssert.EndsWith("05.png", _th.Job.OutputFiles[4], "Failure in file appendix.");
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[2],
                "Sixth outputfile is not based on template from job.");
            StringAssert.EndsWith("06.png", _th.Job.OutputFiles[5], "Failure in file appendix.");
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[0],
                "Seventh outputfile is not based on template from job.");
            StringAssert.EndsWith("07.png", _th.Job.OutputFiles[6], "Failure in file appendix.");
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[1],
                "Eighth outputfile is not based on template from job.");
            StringAssert.EndsWith("08.png", _th.Job.OutputFiles[7], "Failure in file appendix.");
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[2],
                "Nineth outputfile is not based on template from job.");
            StringAssert.EndsWith("09.png", _th.Job.OutputFiles[8], "Failure in file appendix.");
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[2],
                "Tenth outputfile is not based on template from job.");
            StringAssert.EndsWith("10.png", _th.Job.OutputFiles[9], "Failure in file appendix.");
        }

        [Test]
        public async Task MoveOutputFiles_SingleFile_AutoSaveWithEnsureUniqueFilenames_FileExists_FirstAttemptToCopyFails_FileExists_OutputfileHasContinuedAppendix()
        {
            var fileStub = MockRepository.GenerateStub<IFile>();
            fileStub.Stub(x => x.Copy(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything))
                .Throw(new IOException("IoException"))
                .Repeat.Once();
            fileStub.Stub(x => x.Exists(Arg<string>.Is.Anything)).Return(true).Repeat.Once();
            //Simulate existing file in first request for unique filename
            fileStub.Stub(x => x.Exists(Arg<string>.Is.Anything)).IgnoreArguments().Return(false).Repeat.Once();
            fileStub.Stub(x => x.Exists(Arg<string>.Is.Anything)).Return(true).Repeat.Once();
            //Simulate existing file after first attempt to copy has failed.
            fileStub.Stub(x => x.Exists(Arg<string>.Is.Anything)).IgnoreArguments().Return(false);
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);
            var outputFileMover = BuildAutosaveOutputFileMover(fileStub);
            _th.Job.Profile.AutoSave.Enabled = true;
            _th.Job.Profile.AutoSave.EnsureUniqueFilenames = true;
            _th.Job.TempOutputFiles = _singleTempOutputfile;
            var filenameTemplate = _th.Job.OutputFileTemplate; //Save it, because it can change in MoveOutputFiles

            await outputFileMover.MoveOutputFiles(_th.Job);

            fileStub.AssertWasCalled(x => x.Copy("ignore", "ignore", false),
                options => options.IgnoreArguments().Repeat.Twice()); //Copy should be caled twice
            fileStub.AssertWasCalled(x => x.Delete("ignore"), options => options.IgnoreArguments().Repeat.Once());
            //Delete must only be called once
            var templateWithoutExtension = RemoveExtension(filenameTemplate);
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[0],
                "Outputfile is not based on template from job.");
            StringAssert.EndsWith("_3.pdf", _th.Job.OutputFiles[0], "Failure in file appendix.");
        }

        [Test]
        public async Task MoveOutputFiles_SingleFile_AutoSaveWithEnsureUniqueFilenames_FileExists_OutputfileMustHaveAppendix()
        {
            var fileStub = MockRepository.GenerateStub<IFile>();
            fileStub.Stub(x => x.Exists(Arg<string>.Is.Anything)).Return(true).Repeat.Once();
            //Simulate existing file in first request for unique filename
            fileStub.Stub(x => x.Exists(Arg<string>.Is.Anything)).IgnoreArguments().Return(false);
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);
            var outputFileMover = BuildAutosaveOutputFileMover(fileStub);
            _th.Job.Profile.AutoSave.Enabled = true;
            _th.Job.Profile.AutoSave.EnsureUniqueFilenames = true;
            _th.Job.TempOutputFiles = _singleTempOutputfile;
            var filenameTemplate = _th.Job.OutputFileTemplate; //Save it, because it can change in MoveOutputFiles

            await outputFileMover.MoveOutputFiles(_th.Job);

            fileStub.AssertWasCalled(x => x.Copy("ignore", "ignore", false),
                options => options.IgnoreArguments().Repeat.Once()); //Copy must only be called once
            fileStub.AssertWasCalled(x => x.Delete("ignore"), options => options.IgnoreArguments().Repeat.Once());
            //Delete must only be called once
            var templateWithoutExtension = RemoveExtension(filenameTemplate);
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[0],
                "Outputfile is not based on template from job.");
            StringAssert.EndsWith("_2.pdf", _th.Job.OutputFiles[0], "Failure in file appendix");
        }

        [Test]
        public async Task MoveOutputFiles_SingleFile_AutoSaveWithoutEnsureUniqueFilenames_FileExists_OutputfileMustNotHaveAppendix()
        {
            var fileStub = MockRepository.GenerateStub<IFile>();
            fileStub.Stub(x => x.Exists(Arg<string>.Is.Anything)).Return(true).Repeat.Once();
            //Simulate existing file in first request for unique filename
            fileStub.Stub(x => x.Exists(Arg<string>.Is.Anything)).IgnoreArguments().Return(false);
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);
            var outputFileMover = BuildAutosaveOutputFileMover(fileStub);
            _th.Job.Profile.AutoSave.Enabled = true;
            _th.Job.Profile.AutoSave.EnsureUniqueFilenames = false;
            _th.Job.TempOutputFiles = _singleTempOutputfile;
            var filenameTemplate = _th.Job.OutputFileTemplate; //Save it, because it can change in MoveOutputFiles

            await outputFileMover.MoveOutputFiles(_th.Job);

            fileStub.AssertWasCalled(x => x.Copy("ignore", "ignore", false),
                options => options.IgnoreArguments().Repeat.Once()); //Copy must only be called once
            fileStub.AssertWasCalled(x => x.Delete("ignore"), options => options.IgnoreArguments().Repeat.Once());
            //Delete must only be called once
            StringAssert.AreEqualIgnoringCase(filenameTemplate, _th.Job.OutputFiles[0],
                "Outputfile is not the template in job.");
        }

        [Test]
        public async Task MoveOutputFiles_SingleFile_EverythingSuccessful()
        {
            var fileStub = MockRepository.GenerateStub<IFile>();
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);
            var outputFileMover = BuildInteractiveOutputFileMover(fileStub);
            _th.Job.TempOutputFiles = _singleTempOutputfile;
            var filenameTemplate = _th.Job.OutputFileTemplate; //Save it, because it can change in MoveOutputFiles

            await outputFileMover.MoveOutputFiles(_th.Job);

            fileStub.AssertWasCalled(x => x.Copy("ignore", "ignore2", true),
                options => options.IgnoreArguments().Repeat.Once());
            fileStub.AssertWasCalled(x => x.Delete("ignore"), options => options.IgnoreArguments().Repeat.Once());
            //Delete must only be called once
            Assert.AreEqual(1, _th.Job.OutputFiles.Count, "Wrong number of outputfiles.");
            StringAssert.AreEqualIgnoringCase(filenameTemplate, _th.Job.OutputFiles[0],
                "Outputfile is not the template in job.");
        }

        [Test]
        public async Task MoveOutputFiles_SingleFile_FirstAttemptToCopyFailsSecondIsSuccessful_OutputfileMustHaveAppendix()
        {
            var fileStub = MockRepository.GenerateStub<IFile>();
            fileStub.Stub(x => x.Copy(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything))
                .Throw(new IOException("IoException"))
                .Repeat.Once();
            fileStub.Stub(x => x.Exists(Arg<string>.Is.Anything)).Return(true).Repeat.Once();
            //Simulate existing file in first request for unique filename
            fileStub.Stub(x => x.Exists(Arg<string>.Is.Anything)).IgnoreArguments().Return(false);
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);
            _th.Job.Profile.AutoSave.Enabled = true;

            var outputFileMover = BuildAutosaveOutputFileMover(fileStub);

            _th.Job.TempOutputFiles = _singleTempOutputfile;
            var filenameTemplate = _th.Job.OutputFileTemplate; //Save it, because it can change in MoveOutputFiles

            await outputFileMover.MoveOutputFiles(_th.Job);

            fileStub.AssertWasCalled(x => x.Delete("ignore"), options => options.IgnoreArguments().Repeat.Once());
            //Delete must only be called once
            var templateWithoutExtension = RemoveExtension(filenameTemplate);
            StringAssert.StartsWith(templateWithoutExtension, _th.Job.OutputFiles[0],
                "Outputfile is not based on template from job.");
            StringAssert.EndsWith("_2.pdf", _th.Job.OutputFiles[0], "Failure in file appendix");
        }

        [Test]
        public void MoveOutputFiles_SingleFile_TwoAttemptsToCopyFail_ThrowsProcessingException()
        {
            var fileStub = MockRepository.GenerateStub<IFile>();
            fileStub.Stub(x => x.Copy("ignore", "ignore2", true)).IgnoreArguments().Throw(new IOException(""));
            //Deny all attempts to copy
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Pdf);
            var outputFileMover = BuildAutosaveOutputFileMover(fileStub);
            _th.Job.TempOutputFiles = _singleTempOutputfile;
            _th.Job.Profile.AutoSave.Enabled = true;

            var ex = Assert.ThrowsAsync<ProcessingException>(() => outputFileMover.MoveOutputFiles(_th.Job));

            Assert.AreEqual(ErrorCode.Conversion_ErrorWhileCopyingOutputFile, ex.ErrorCode);

            fileStub.AssertWasCalled(x => x.Copy("", "", true), options => options.IgnoreArguments().Repeat.Twice());
            //DeviceException should be thrown after second denied copy call
            fileStub.AssertWasNotCalled(x => x.Delete("ignore"), options => options.IgnoreArguments());
            //Delete never gets called
        }
    }
}
