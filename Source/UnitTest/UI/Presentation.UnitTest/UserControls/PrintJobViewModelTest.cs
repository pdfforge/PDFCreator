using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Query;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Core.Workflow.ComposeTargetFilePath;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using pdfforge.PDFCreator.Core.Workflow.Queries;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;
using pdfforge.PDFCreator.UI.Presentation.Workflow;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using pdfforge.PDFCreator.Utilities.IO;
using pdfforge.PDFCreator.Utilities.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using SystemInterface.IO;
using Translatable;

namespace Presentation.UnitTest.UserControls
{
    [TestFixture]
    public class PrintJobViewModelTest
    {
        private PdfCreatorSettings _settings;
        private ConversionProfile _pdfProfile;
        private ConversionProfile _pngProfile;
        private IJobInfoQueue _jobInfoQueue;
        private IFileNameQuery _saveFileQuery;
        private UnitTestInteractionRequest _interactionRequest;
        private readonly PrintJobViewTranslation _translation = new PrintJobViewTranslation();
        private IFile _file;
        private static string _filepathFromSaveDialog = @"DirectoryFromSaveDialog\FilenameFromSaveDialog.pdf";
        private readonly string _folderFromSaveDialog = PathSafe.GetDirectoryName(_filepathFromSaveDialog);
        private readonly string _filenameFromSaveDialog = PathSafe.GetFileName(_filepathFromSaveDialog);
        private IDirectoryHelper _directoryHelper;
        private IInteractiveProfileChecker _interactiveProfileChecker;
        private ITargetFilePathComposer _targetFilePathComposer;

        private string _expectedFileNamePdf = "expectedFilename.pdf";
        private string _expectedFileNamePng = "expectedFilename.png";
        private string _expectedFolder = "X:\\ExpectedFolder";

        [SetUp]
        public void Setup()
        {
            _settings = new PdfCreatorSettings();
            _jobInfoQueue = Substitute.For<IJobInfoQueue>();
            _interactionRequest = new UnitTestInteractionRequest();
            _file = Substitute.For<IFile>();
            _directoryHelper = Substitute.For<IDirectoryHelper>();
            _interactiveProfileChecker = Substitute.For<IInteractiveProfileChecker>();

            _pdfProfile = new ConversionProfile
            {
                Name = "PDF Profile",
                OutputFormat = OutputFormat.Pdf,
                FileNameTemplate = _expectedFileNamePdf,
                TargetDirectory = _expectedFolder
            };

            _pngProfile = new ConversionProfile
            {
                Name = "PNG Profile",
                OutputFormat = OutputFormat.Png,
                FileNameTemplate = _expectedFileNamePng,
                TargetDirectory = _expectedFolder
            };

            _settings.ConversionProfiles.Add(_pdfProfile);
            _settings.ConversionProfiles.Add(_pngProfile);
        }

        private PrintJobViewModel BuildViewModel(bool saveDialogResult = true)
        {
            MockSaveFileDialog(saveDialogResult);
            var settingsProvider = Substitute.For<ISettingsProvider>();
            settingsProvider.Settings.Returns(_settings);

            _targetFilePathComposer = Substitute.For<ITargetFilePathComposer>();
            _targetFilePathComposer.ComposeTargetFilePath(Arg.Any<Job>()).ReturnsForAnyArgs(j => j.ArgAt<Job>(0).OutputFileTemplate);

            return new PrintJobViewModel(settingsProvider, new TranslationUpdater(new TranslationFactory(), new ThreadManager()),
                _jobInfoQueue, _saveFileQuery, _interactionRequest, new DesignTimeCommandLocator(),
                null, null, null, null, _file, null, null, _directoryHelper, _interactiveProfileChecker, _targetFilePathComposer);
        }

        private Job BuildJob(ConversionProfile profile)
        {
            var job = new Job(new JobInfo(), profile, new Accounts());

            job.OutputFileTemplate = @"X:\NotEmpty\ToTrigger\PropertyChanged.pdf";

            return job;
        }

        private void MockSaveFileDialog(bool success)
        {
            _saveFileQuery = Substitute.For<IFileNameQuery>();
            var outputFilenameResult = new OutputFilenameResult(_filepathFromSaveDialog, OutputFormat.Pdf);
            var queryResultOutputFilenameResult = new QueryResult<OutputFilenameResult>(success, outputFilenameResult);
            _saveFileQuery.GetFileName(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<OutputFormat>())
                .Returns(queryResultOutputFilenameResult);
        }

        [Test]
        public async Task SetJob_UpdatesJob()
        {
            var vm = BuildViewModel();
            var job = BuildJob(_pdfProfile);

            var task = vm.ExecuteWorkflowStep(job);

            await AbortStep(vm, task);

            Assert.AreSame(vm.Job, job);
        }

        private async Task AbortStep(PrintJobViewModel viewModel, Task task)
        {
            // Ignore further logic in step
            try
            {
                viewModel.CancelCommand.Execute(null);
            }
            catch
            {
            }

            await task.TimeoutAfter(TimeSpan.FromMilliseconds(100));
        }

        [Test]
        public void SetJob_RaisesPropertyChanged()
        {
            var changedProperties = new List<string>();
            var vm = BuildViewModel();
            vm.PropertyChanged += (sender, args) => changedProperties.Add(args.PropertyName);
            var job = BuildJob(_pdfProfile);

            vm.ExecuteWorkflowStep(job);

            var expectedProperties = new[]
            {
                nameof(vm.Job),
                nameof(vm.SelectedProfile),
                nameof(vm.OutputFilename),
                nameof(vm.OutputFolder),
                nameof(vm.OutputFormat),
                nameof(vm.Metadata)
            };

            CollectionAssert.AreEquivalent(expectedProperties, changedProperties);
        }

        [Test]
        public void SetJob_UpdatesFilenameProperties()
        {
            var vm = BuildViewModel();
            var changedProperties = new List<string>();
            vm.PropertyChanged += (sender, args) => changedProperties.Add(args.PropertyName);
            var job = BuildJob(_pdfProfile);
            _targetFilePathComposer.ComposeTargetFilePath(job).Returns(@"X:\FromTargetFilePathComposer\SomeFile.pdf");

            vm.SetJob(job);

            Assert.Contains(nameof(vm.OutputFolder), changedProperties);
            Assert.AreEqual(@"X:\FromTargetFilePathComposer", vm.OutputFolder);
            Assert.Contains(nameof(vm.OutputFilename), changedProperties);
            Assert.AreEqual("SomeFile.pdf", vm.OutputFilename);
        }

        [Test]
        public void OutputFormat_WhenSet_UpdatesFilename()
        {
            var vm = BuildViewModel();
            vm.SetJob(BuildJob(_pdfProfile));
            var oldOutputFileName = vm.OutputFilename;

            vm.OutputFormat = OutputFormat.Txt;

            Assert.AreEqual(PathSafe.ChangeExtension(oldOutputFileName, ".txt"), vm.OutputFilename);
        }

        [Test]
        public async Task SaveCommand_Execute_JobProfileIsValid_DoesNotThrowException()
        {
            var vm = BuildViewModel();
            var job = BuildJob(_pdfProfile);

            var task = vm.ExecuteWorkflowStep(job);

            Assert.DoesNotThrow(() => vm.SaveCommand.Execute(null));

            await AbortStep(vm, task);
        }

        [Test]
        public async Task SaveCommand_Execute_JobProfileIsValid_Finishes()
        {
            var vm = BuildViewModel();
            var job = BuildJob(_pdfProfile);
            _interactiveProfileChecker.CheckWithErrorResultInOverlay(job).Returns(true);

            var task = vm.ExecuteWorkflowStep(job);

            vm.SaveCommand.Execute(null);

            await task.TimeoutAfter(TimeSpan.FromMilliseconds(100));
        }

        [Test]
        public async Task SaveCommand_Execute_JobProfileIsValid_ProfileCheckCopiesJobPasswords()
        {
            var expectedPassword = "PDF Owner Password";
            var vm = BuildViewModel();
            var job = BuildJob(_pdfProfile);
            _interactiveProfileChecker.CheckWithErrorResultInOverlay(job).Returns(true);
            job.Profile.PdfSettings.Security.OwnerPassword = expectedPassword;

            var task = vm.ExecuteWorkflowStep(job);
            vm.SaveCommand.Execute(null);

            await task.TimeoutAfter(TimeSpan.FromMilliseconds(100));

            Assert.AreEqual(expectedPassword, job.Passwords.PdfOwnerPassword);
        }

        [Test]
        public void SaveCommand_Execute_JobProfileIsValid_NoUserNotification()
        {
            var expectedPassword = "PDF Owner Password";
            var vm = BuildViewModel();
            var job = BuildJob(_pdfProfile);
            job.Profile.PdfSettings.Security.OwnerPassword = expectedPassword;
            _interactiveProfileChecker.CheckWithErrorResultInOverlay(job).Returns(true);

            vm.ExecuteWorkflowStep(job);
            vm.SaveCommand.Execute(null);

            _interactionRequest.AssertWasNotRaised<MessageInteraction>();
        }

        [Test]
        public async Task SaveCommand_Execute_JobProfileIsNotValid_DoesNotCallFinish()
        {
            var vm = BuildViewModel();
            var job = BuildJob(_pdfProfile);
            var task = vm.ExecuteWorkflowStep(job);

            vm.SaveCommand.Execute(null);

            Assert.IsFalse(task.IsCompleted);
            await AbortStep(vm, task);
        }

        [Test]
        public async Task SaveCommand_Execute_ProfileIsValid_FilePathFromSaveDialog_FileDoesNotExist_DoesNotRaiseInteraction_CallsFinishEvent()
        {
            var vm = BuildViewModel();
            var job = BuildJob(_pdfProfile);
            var task = vm.ExecuteWorkflowStep(job);
            _interactiveProfileChecker.CheckWithErrorResultInOverlay(job).Returns(true);

            await vm.BrowseFileCommandAsync.ExecuteAsync(null);
            _file.Exists(Arg.Any<string>()).Returns(false);

            vm.SaveCommand.Execute(null);

            await task.TimeoutAfter(TimeSpan.FromMilliseconds(100));

            _interactionRequest.AssertWasNotRaised<MessageInteraction>();
        }

        [Test]
        public async Task SaveCommand_Execute_ProfileIsValid_FilePathFromSaveDialog_FileExists_DoesNotRaiseInteraction_CallsFinish()
        {
            var vm = BuildViewModel();
            var job = BuildJob(_pdfProfile);
            var task = vm.ExecuteWorkflowStep(job);
            _interactiveProfileChecker.CheckWithErrorResultInOverlay(job).Returns(true);

            await vm.BrowseFileCommandAsync.ExecuteAsync(null);
            _file.Exists(Arg.Any<string>()).Returns(true);

            vm.SaveCommand.Execute(null);

            await task.TimeoutAfter(TimeSpan.FromMilliseconds(100));

            _interactionRequest.AssertWasNotRaised<MessageInteraction>();
        }

        [Test]
        public async Task SaveCommand_Execute_ProfileIsValid_FilePathNotFromSaveDialog_FileDoesNotExist_DoesNotRaiseInteraction_CallsFinish()
        {
            var vm = BuildViewModel();
            var job = BuildJob(_pdfProfile);
            var task = vm.ExecuteWorkflowStep(job);
            _interactiveProfileChecker.CheckWithErrorResultInOverlay(job).Returns(true);

            await vm.BrowseFileCommandAsync.ExecuteAsync(null);
            _file.Exists(Arg.Any<string>()).Returns(false);
            vm.OutputFilename += "not" + _filenameFromSaveDialog;
            vm.OutputFolder += "not" + _folderFromSaveDialog;

            vm.SaveCommand.Execute(null);

            await task.TimeoutAfter(TimeSpan.FromMilliseconds(100));

            _interactionRequest.AssertWasNotRaised<MessageInteraction>();
        }

        [Test]
        public async Task SaveCommand_Execute_ProfileIsValid_FilePathNotFromSaveDialog_FileExist_NotifysUserWithCorrectInteraction()
        {
            var vm = BuildViewModel();
            var job = BuildJob(_pdfProfile);
            var task = vm.ExecuteWorkflowStep(job);
            _interactiveProfileChecker.CheckWithErrorResultInOverlay(job).Returns(true);

            await vm.BrowseFileCommandAsync.ExecuteAsync(null);
            _file.Exists(Arg.Any<string>()).Returns(true);
            vm.OutputFilename = "not" + _filenameFromSaveDialog;
            vm.OutputFolder = "not" + _folderFromSaveDialog;
            var expectedDir = PathSafe.Combine(vm.OutputFolder, vm.OutputFilename);

            vm.SaveCommand.Execute(null);

            var interaction = _interactionRequest.AssertWasRaised<MessageInteraction>();
            Assert.AreEqual(_translation.ConfirmSaveAs.ToUpper(), interaction.Title, "Title");
            Assert.AreEqual(_translation.GetFileAlreadyExists(expectedDir), interaction.Text, "Message");
            Assert.AreEqual(MessageIcon.Exclamation, interaction.Icon, "Icon");
            Assert.AreEqual(MessageOptions.YesNo, interaction.Buttons, "Buttons");

            Assert.IsFalse(task.IsCompleted);
            await AbortStep(vm, task);
        }

        [Test]
        public async Task SaveCommand_Execute_ProfileIsValid_UserCanceledSaveFileDialog_FileExist_NotifysUserWithCorrectInteraction()
        {
            var vm = BuildViewModel(saveDialogResult: false); //User cancels SaveFileDialog
            var job = BuildJob(_pdfProfile);
            var task = vm.ExecuteWorkflowStep(job);
            _interactiveProfileChecker.CheckWithErrorResultInOverlay(job).Returns(true);
            await vm.BrowseFileCommandAsync.ExecuteAsync(null);
            _file.Exists(Arg.Any<string>()).Returns(true);
            vm.OutputFilename = _filenameFromSaveDialog;
            vm.OutputFolder = _folderFromSaveDialog;
            var expectedDir = PathSafe.Combine(vm.OutputFolder, vm.OutputFilename);

            vm.SaveCommand.Execute(null);

            var interaction = _interactionRequest.AssertWasRaised<MessageInteraction>();
            Assert.AreEqual(_translation.ConfirmSaveAs.ToUpper(), interaction.Title, "Title");
            Assert.AreEqual(_translation.GetFileAlreadyExists(expectedDir), interaction.Text, "Message");
            Assert.AreEqual(MessageIcon.Exclamation, interaction.Icon, "Icon");
            Assert.AreEqual(MessageOptions.YesNo, interaction.Buttons, "Buttons");

            Assert.IsFalse(task.IsCompleted);
            await AbortStep(vm, task);
        }

        [Test]
        public async Task SaveCommand_Execute_ProfileIsValid_FilePathNotFromSaveDialog_FileExist_NotifysUser_UserCancels_DoNotCallFinish()
        {
            var vm = BuildViewModel();
            var job = BuildJob(_pdfProfile);
            var task = vm.ExecuteWorkflowStep(job);
            _interactionRequest.Raise(Arg.Do<MessageInteraction>(i => i.Response = MessageResponse.No)); //User cancels
            await vm.BrowseFileCommandAsync.ExecuteAsync(null);
            _file.Exists(Arg.Any<string>()).Returns(true);
            vm.OutputFilename = "not" + _filenameFromSaveDialog;
            vm.OutputFolder = "not" + _folderFromSaveDialog;

            vm.SaveCommand.Execute(null);

            Assert.IsFalse(task.IsCompleted);
            await AbortStep(vm, task);
        }

        [Test]
        public async Task SaveCommand_Execute_ProfileIsValid_FilePathNotFromSaveDialog_FileExist_NotifysUser_UserApplies_DoesNotFinish()
        {
            var vm = BuildViewModel();
            var job = BuildJob(_pdfProfile);
            var task = vm.ExecuteWorkflowStep(job);
            _interactionRequest.Raise(Arg.Do<MessageInteraction>(i => i.Response = MessageResponse.Yes)); //User applies
            await vm.BrowseFileCommandAsync.ExecuteAsync(null);
            _file.Exists(Arg.Any<string>()).Returns(true);
            vm.OutputFilename = "not" + _filenameFromSaveDialog;
            vm.OutputFolder = "not" + _folderFromSaveDialog;

            vm.SaveCommand.Execute(null);

            Assert.IsFalse(task.IsCompleted);
            await AbortStep(vm, task);
        }

        [Test]
        public async Task SaveCommand_Execute_ProfileIsValid_Path_Is_Too_Long()
        {
            var vm = BuildViewModel();
            var job = BuildJob(_pdfProfile);
            var task = vm.ExecuteWorkflowStep(job);

            _interactionRequest.Raise(Arg.Do<MessageInteraction>(i =>
            {
                i.Response = MessageResponse.OK;
            }));

            await vm.BrowseFileCommandAsync.ExecuteAsync(null);
            _file.Exists(Arg.Any<string>()).Returns(false);

            vm.OutputFilename = "PDFCreatorTesfasdfsdfadsfsdfasdfasdfasdtpageaPDFCreatorTestpageaPDFCreatorTestpageaPDFCreatorTestpageaPDFCreatorTestpageaPDFCreatorTestpageaPDFCreatorTestpageaPDFCreatorTestpageaPDFCreatorTestpageaPDFCreatorTestpageaPDFCreatorTestpageaPDFCreatorTestpageaPDFeaPDFCa.pdf";
            vm.OutputFolder = "c:\\";

            vm.SaveCommand.Execute(null);

            Assert.IsFalse(task.IsCompleted);
            await AbortStep(vm, task);
        }

        [Test]
        public async Task SaveCommand_Execute_FolderPathIsInvalid_MessageInteractionIsRaised()
        {
            var vm = BuildViewModel();
            var job = BuildJob(_pdfProfile);
            var task = vm.ExecuteWorkflowStep(job);

            _interactionRequest.Raise(Arg.Do<MessageInteraction>(i =>
            {
                i.Response = MessageResponse.OK;
            }));

            await vm.BrowseFileCommandAsync.ExecuteAsync(null);
            _file.Exists(Arg.Any<string>()).Returns(false);

            vm.OutputFolder = @"c:\<<\";

            vm.SaveCommand.Execute(null);

            Assert.IsFalse(task.IsCompleted);
            await AbortStep(vm, task);
        }

        [Test]
        public async Task CancelCommand_Execute_ThrowsAbortWorkflowException()
        {
            var vm = BuildViewModel();
            var job = BuildJob(_pdfProfile);

            var task = vm.ExecuteWorkflowStep(job);

            Assert.Throws<AbortWorkflowException>(() => vm.CancelCommand.Execute(null));

            await task.TimeoutAfter(TimeSpan.FromMilliseconds(100));
        }

        [Test]
        public async Task CancelCommand_Execute_CallsFinished()
        {
            var vm = BuildViewModel();
            var job = BuildJob(_pdfProfile);

            var task = vm.ExecuteWorkflowStep(job);

            try
            {
                vm.CancelCommand.Execute(null);
            }
            catch { }

            await task.TimeoutAfter(TimeSpan.FromMilliseconds(100));
        }

        [Test]
        public async Task MergeCommand_Execute_ThrowsManagePrintJobsException()
        {
            var vm = BuildViewModel();
            var job = BuildJob(_pdfProfile);

            var task = vm.ExecuteWorkflowStep(job);

            Assert.Throws<ManagePrintJobsException>(() => vm.MergeCommand.Execute(null));

            await task.TimeoutAfter(TimeSpan.FromMilliseconds(100));
        }

        [Test]
        public async Task MergeCommand_Execute_CallsFinished()
        {
            var vm = BuildViewModel();
            var job = BuildJob(_pdfProfile);

            var task = vm.ExecuteWorkflowStep(job);

            try
            {
                vm.MergeCommand.Execute(null);
            }
            catch { }

            await task.TimeoutAfter(TimeSpan.FromMilliseconds(100));
        }

        [TestCase(0, "")]
        [TestCase(1, "")]
        [TestCase(2, "2")]
        [TestCase(99, "99")]
        [TestCase(100, "99+")]
        public void MergeHint_WithEmptyQueue_Isempty(int numberOfPrintJobs, string expectedHint)
        {
            _jobInfoQueue.Count.Returns(numberOfPrintJobs);
            var vm = BuildViewModel();

            Assert.AreEqual(expectedHint, vm.NumberOfPrintJobsHint);
        }

        [Test]
        public async Task BrowseFolderCommand_WhenExecuted_OpensWithCurrentFilename()
        {
            var vm = BuildViewModel();
            vm.SetJob(BuildJob(_pdfProfile));

            _saveFileQuery
                .GetFileName(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<OutputFormat>())
                .Returns(new QueryResult<OutputFilenameResult>(false, new OutputFilenameResult("", OutputFormat.Pdf)));

            await vm.BrowseFileCommandAsync.ExecuteAsync(null);

            _saveFileQuery.Received().GetFileName(vm.OutputFolder, vm.OutputFilename, vm.OutputFormat);
        }

        [Test]
        public async Task BrowseFolderCommand_WhenChangingOutputFormat_SetsViewModelOutputFormat()
        {
            var expectedOutputFormat = OutputFormat.Png;
            var vm = BuildViewModel();
            vm.SetJob(BuildJob(_pdfProfile));

            _saveFileQuery
                .GetFileName(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<OutputFormat>())
                .Returns(x =>
                {
                    var directory = x.ArgAt<string>(0);
                    var filename = x.ArgAt<string>(1);
                    return new QueryResult<OutputFilenameResult>(true, new OutputFilenameResult(Path.Combine(directory, filename), expectedOutputFormat));
                });

            await vm.BrowseFileCommandAsync.ExecuteAsync(null);

            Assert.AreEqual(expectedOutputFormat, vm.OutputFormat);
            Assert.AreEqual(expectedOutputFormat, vm.Job.Profile.OutputFormat);
        }

        [Test]
        public async Task BrowseFolderCommand_WhenExecutedSuccessfully_SetsOutputFolder()
        {
            var expectedPath = @"Z:\Temp\Folder\Name\test.pdf";
            var vm = BuildViewModel();
            vm.SetJob(BuildJob(_pdfProfile));
            _saveFileQuery
                .GetFileName(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<OutputFormat>())
                .Returns(new QueryResult<OutputFilenameResult>(true, new OutputFilenameResult(expectedPath, OutputFormat.Pdf)));

            await vm.BrowseFileCommandAsync.ExecuteAsync(null);

            Assert.AreEqual(expectedPath, vm.Job.OutputFileTemplate);
            StringAssert.StartsWith(expectedPath, vm.Job.OutputFileTemplate);
        }

        [Test]
        public async Task BrowseFolderCommand_WhenCancelled_DoesNotSetOutputFolder()
        {
            var vm = BuildViewModel();
            vm.SetJob(BuildJob(_pdfProfile));
            _saveFileQuery
                .GetFileName(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<OutputFormat>())
                .Returns(new QueryResult<OutputFilenameResult>(false, null));
            var oldOutputFolder = vm.OutputFolder;

            await vm.BrowseFileCommandAsync.ExecuteAsync(null);

            Assert.AreEqual(oldOutputFolder, vm.OutputFolder);
            StringAssert.StartsWith(oldOutputFolder, vm.Job.OutputFileTemplate);
        }

        [Test]
        public void SetOutputFormatCommand_WithOutputFormat_SetsOutputFormatInJob()
        {
            var expectedOutputFormat = OutputFormat.Jpeg;

            var vm = BuildViewModel();
            var job = BuildJob(_pdfProfile);
            vm.SetJob(job);

            vm.SetOutputFormatCommand.Execute(expectedOutputFormat);

            Assert.AreEqual(vm.OutputFormat, expectedOutputFormat);
            Assert.AreEqual(job.Profile.OutputFormat, expectedOutputFormat);
        }

        [Test]
        public void SetOutputFormatCommand_FilenameWithoutKnownExtension_AddsExtension()
        {
            var vm = BuildViewModel();
            var job = BuildJob(_pdfProfile);
            vm.SetJob(job);
            vm.OutputFilename = "Test.abc";

            vm.SetOutputFormatCommand.Execute(OutputFormat.Jpeg);

            Assert.AreEqual("Test.abc.jpg", vm.OutputFilename);
        }

        [Test]
        public void SetOutputFormatCommand_FilenameWithKnownExtension_SetsExtension()
        {
            var vm = BuildViewModel();
            var job = BuildJob(_pdfProfile);
            vm.SetJob(job);
            vm.OutputFilename = "Test.abc.pdf";

            vm.SetOutputFormatCommand.Execute(OutputFormat.Jpeg);

            Assert.AreEqual("Test.abc.jpg", vm.OutputFilename);
        }

        [Test]
        public void SetOutputFormatCommand_WithOutputFormat_RaisesPropertyChanged()
        {
            var outputFormatChanged = false;

            var vm = BuildViewModel();
            vm.PropertyChanged += (o, a) =>
            {
                if (a.PropertyName == nameof(vm.OutputFormat))
                    outputFormatChanged = true;
            };
            vm.SetJob(BuildJob(_pdfProfile));

            vm.SetOutputFormatCommand.Execute(OutputFormat.Jpeg);

            Assert.IsTrue(outputFormatChanged);
        }

        [Test]
        public void Profiles_WhenProfileIsSelected_OutputFormatIsUpdated()
        {
            var vm = BuildViewModel();
            var job = BuildJob(_pdfProfile);
            vm.SetJob(job);

            var profilesView = CollectionViewSource.GetDefaultView(vm.Profiles);
            profilesView.MoveCurrentToNext();

            Assert.AreEqual(OutputFormat.Png, vm.OutputFormat);
            Assert.AreEqual(OutputFormat.Png, vm.Job.Profile.OutputFormat);
            StringAssert.EndsWith(".png", vm.OutputFilename);
        }

        [Test]
        public void Profiles_UseDeepCopy()
        {
            var vm = BuildViewModel();

            var pdfProfile = vm.Profiles.First(x => x.Guid == _pdfProfile.Guid);
            pdfProfile.Name = "test";

            Assert.AreNotEqual(_pdfProfile.Name, pdfProfile.Name);
            Assert.AreNotSame(_pdfProfile, pdfProfile);
        }

        [Test]
        public void HaveProperOutputFolder_ChangeProfileWithEmptyOutputFolder_FolderDoesNotChange()
        {
            var emptyProfile = new ConversionProfile
            {
                Name = "PDF Profile",
                OutputFormat = OutputFormat.Pdf,
                FileNameTemplate = "X:\\test.pdf",
                TargetDirectory = ""
            };

            _settings.ConversionProfiles.Add(emptyProfile);

            var vm = BuildViewModel();
            var job = BuildJob(_pdfProfile);
            vm.ExecuteWorkflowStep(job);
            vm.OutputFilename = "testFile.pdf";
            vm.OutputFolder = "c:\\Test\\Folder\\";
            vm.SelectedProfile = emptyProfile;

            Assert.AreNotEqual(string.Empty, vm.OutputFolder);
        }

        [Test]
        public void ExecuteWorkflowStep_SetJob_SetsMetadataProperties()
        {
            var vm = BuildViewModel();
            var job = BuildJob(_pdfProfile);

            job.Profile.TitleTemplate = "New Title";
            job.Profile.AuthorTemplate = "New Author";
            job.Profile.SubjectTemplate = "New Subject";
            job.Profile.KeywordTemplate = "New Keyword";

            vm.ExecuteWorkflowStep(job);

            Assert.AreEqual(job.Profile.TitleTemplate, vm.Metadata.Title);
            Assert.AreEqual(job.Profile.AuthorTemplate, vm.Metadata.Author);
            Assert.AreEqual(job.Profile.SubjectTemplate, vm.Metadata.Subject);
            Assert.AreEqual(job.Profile.KeywordTemplate, vm.Metadata.Keywords);
        }

        [Test]
        public async Task BrowseFileCommand_CallsDirectoryHelper()
        {
            var vm = BuildViewModel(saveDialogResult: false);
            vm.SetJob(BuildJob(_pdfProfile));
            MockSaveFileDialog(false);

            await vm.BrowseFileCommandAsync.ExecuteAsync(null);

            _directoryHelper.Received(1).CreateDirectory(vm.OutputFolder);
        }

        [Test]
        public async Task BrowseFileCommand_PathTooLongException_DoesRetry()
        {
            var callCounter = 0;
            var vm = BuildViewModel(saveDialogResult: false);
            vm.SetJob(BuildJob(_pdfProfile));
            _saveFileQuery.WhenForAnyArgs(
                x => x.GetFileName(null, null, OutputFormat.Pdf)).Do(x =>
            {
                MockSaveFileDialog(false);
                callCounter++;

                if (callCounter == 1)
                    throw new PathTooLongException();
            });

            await vm.BrowseFileCommandAsync.ExecuteAsync(null);

            _interactionRequest.AssertWasRaised<MessageInteraction>();
        }
    }
}
