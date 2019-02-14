using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.ConverterInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using pdfforge.PDFCreator.Core.Workflow.Output;
using pdfforge.PDFCreator.Utilities.IO;
using System;
using System.IO;
using System.Threading.Tasks;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UnitTest.Core.Workflow
{
    [TestFixture]
    public class JobRunnerTest
    {
        [SetUp]
        public void Setup()
        {
            _outputFileMover = Substitute.For<IOutputFileMover>();
            _pdfProcessor = Substitute.For<IPdfProcessor>();
            _converter = Substitute.For<IConverter>();
            _actionManager = Substitute.For<IActionManager>();
            _jobCleanUp = Substitute.For<IJobCleanUp>();
            _directory = Substitute.For<IDirectory>();
            _directoryHelper = Substitute.For<IDirectoryHelper>();
        }

        private IOutputFileMover _outputFileMover;
        private IPdfProcessor _pdfProcessor;
        private IConverter _converter;
        private IActionManager _actionManager;
        private IJobCleanUp _jobCleanUp;
        private IDirectory _directory;
        private IDirectoryHelper _directoryHelper;

        private const string TempFolder = @"X:\SomeFolder\Temp";

        private JobRunner BuildJobRunner()
        {
            var tokenReplacerFactory = Substitute.For<ITokenReplacerFactory>();

            var tempFolderProvider = Substitute.For<ITempFolderProvider>();
            tempFolderProvider.TempFolder.Returns(TempFolder);

            var converterFactory = Substitute.For<IConverterFactory>();
            converterFactory.GetCorrectConverter(Arg.Any<JobType>()).Returns(x => _converter);

            _converter
                .When(x => x.DoConversion(Arg.Any<Job>()))
                .Do(x =>
                {
                    var job = x.Arg<Job>();
                    job.TempOutputFiles.Add(Path.Combine(TempFolder, "file1.pdf"));
                });

            _outputFileMover
                .When(x => x.MoveOutputFiles(Arg.Any<Job>()))
                .Do(x =>
                {
                    var job = x.Arg<Job>();

                    foreach (var tmpFile in job.TempOutputFiles)
                    {
                        job.OutputFiles.Add(tmpFile);
                    }
                });

            return new JobRunner(tokenReplacerFactory, _pdfProcessor, converterFactory, _actionManager,
                _jobCleanUp, tempFolderProvider, _directory, _directoryHelper);
        }

        private Job BuildJob()
        {
            var jobInfo = new JobInfo
            {
                InfFile = @"M:\Spool\aaa-bbb-ccc.inf",
                JobType = JobType.PsJob
            };

            jobInfo.SourceFiles.Add(new SourceFileInfo
            {
                Author = "Author",
                ClientComputer = "ClientComputer",
                Copies = 1,
                DocumentTitle = "My Title",
                Filename = @"Q:\Spool\MyFile.ps",
                JobCounter = 42,
                JobId = 1024,
                PrinterName = "PDFCreator",
                SessionId = 1,
                TotalPages = 1,
                Type = JobType.PsJob,
                WinStation = "WinStation"
            });

            var job = new Job(jobInfo, new ConversionProfile(), new Accounts());

            return job;
        }

        private IPreConversionAction BuildPreConversionAction(ErrorCode? errorCode = null)
        {
            var result = new ActionResult();

            if (errorCode != null)
                result.Add(errorCode.Value);

            var action = Substitute.For<IPreConversionAction>();
            action.ProcessJob(Arg.Any<Job>()).Returns(result);

            return action;
        }

        private IPostConversionAction BuildPostConversionAction(ErrorCode? errorCode = null)
        {
            var result = new ActionResult();

            if (errorCode != null)
                result.Add(errorCode.Value);

            var action = Substitute.For<IPostConversionAction>();
            action.ProcessJob(Arg.Any<Job>()).Returns(result);

            return action;
        }

        [Test]
        public async Task RunJob_CallsActionManager()
        {
            var jobRunner = BuildJobRunner();
            var job = BuildJob();

            await jobRunner.RunJob(job, _outputFileMover);

            _actionManager.Received().GetApplicablePreConversionActions(job);
            _actionManager.Received().GetApplicablePostConversionActions(job);
        }

        [Test]
        public async Task RunJob_CallsAllPreConversionActions()
        {
            var jobRunner = BuildJobRunner();
            var job = BuildJob();

            var actions = new[]
            {
                BuildPreConversionAction(),
                BuildPreConversionAction()
            };

            _actionManager.GetApplicablePreConversionActions(job).Returns(actions);

            await jobRunner.RunJob(job, _outputFileMover);

            foreach (var action in actions)
            {
                action.Received().ProcessJob(job);
            }
        }

        [Test]
        public async Task RunJob_CallsAllPostConversionActions()
        {
            var jobRunner = BuildJobRunner();
            var job = BuildJob();

            var actions = new[]
            {
                BuildPostConversionAction(),
                BuildPostConversionAction()
            };

            _actionManager.GetApplicablePostConversionActions(job).Returns(actions);

            await jobRunner.RunJob(job, _outputFileMover);

            foreach (var action in actions)
            {
                action.Received().ProcessJob(job);
            }
        }

        [Test]
        public async Task RunJob_CallsCleanUp()
        {
            var jobRunner = BuildJobRunner();
            var job = BuildJob();

            await jobRunner.RunJob(job, _outputFileMover);

            _jobCleanUp.Received().DoCleanUp(job.JobTempFolder, job.JobInfo.SourceFiles, job.JobInfo.InfFile);
            _directoryHelper.Received().DeleteCreatedDirectories();
        }

        [Test]
        public async Task RunJob_CallsOutputFileMover()
        {
            var jobRunner = BuildJobRunner();
            var job = BuildJob();

            await jobRunner.RunJob(job, _outputFileMover);

            await _outputFileMover.Received().MoveOutputFiles(job);
        }

        [Test]
        public async Task RunJob_CallsProcessPdf_IfRequired()
        {
            var jobRunner = BuildJobRunner();
            var job = BuildJob();

            _pdfProcessor.ProcessingRequired(Arg.Any<ConversionProfile>()).Returns(true);

            await jobRunner.RunJob(job, _outputFileMover);

            _pdfProcessor.Received().ProcessPdf(job);
        }

        [Test]
        public async Task RunJob_DoesNotProcessPdf_IfNotRequired()
        {
            var jobRunner = BuildJobRunner();
            var job = BuildJob();

            _pdfProcessor.ProcessingRequired(Arg.Any<ConversionProfile>()).Returns(false);

            await jobRunner.RunJob(job, _outputFileMover);

            _pdfProcessor.DidNotReceive().ProcessPdf(job);
        }

        [Test]
        public async Task RunJob_InitializesProcessorBeforeConversion()
        {
            var jobRunner = BuildJobRunner();
            var job = BuildJob();

            var isInitialized = false;

            _pdfProcessor.When(x => x.Init(job)).Do(x => { isInitialized = true; });

            _converter.When(x => x.DoConversion(job)).Do(x =>
            {
                if (!isInitialized) throw new Exception();
            });

            await jobRunner.RunJob(job, _outputFileMover);

            _converter.Received().DoConversion(job);
        }

        [Test]
        public void RunJob_OnFailingPreConversionAction_ThrowsProcessingException()
        {
            var jobRunner = BuildJobRunner();
            var job = BuildJob();

            const ErrorCode errorCode = ErrorCode.MailClient_GenericError;

            var actions = new[]
            {
                BuildPreConversionAction(),
                BuildPreConversionAction(errorCode)
            };

            _actionManager.GetApplicablePreConversionActions(job).Returns(actions);

            var exception = Assert.ThrowsAsync<ProcessingException>(() => jobRunner.RunJob(job, _outputFileMover));

            Assert.AreEqual(errorCode, exception.ErrorCode);
        }

        [Test]
        public void RunJob_OnFailingPostConversionAction_ThrowsProcessingException()
        {
            var jobRunner = BuildJobRunner();
            var job = BuildJob();

            const ErrorCode errorCode = ErrorCode.MailClient_GenericError;

            var actions = new[]
            {
                BuildPostConversionAction(),
                BuildPostConversionAction(errorCode)
            };

            _actionManager.GetApplicablePostConversionActions(job).Returns(actions);

            var exception = Assert.ThrowsAsync<ProcessingException>(() => jobRunner.RunJob(job, _outputFileMover));

            Assert.AreEqual(errorCode, exception.ErrorCode);
        }

        [Test]
        public async Task RunJob_StartsConversion()
        {
            var jobRunner = BuildJobRunner();
            var job = BuildJob();

            await jobRunner.RunJob(job, _outputFileMover);

            _converter.Received().DoConversion(job);
        }

        [Test]
        public void RunJob_WhenOutputFileMoverCancels_JobStateIsCancelled()
        {
            var jobRunner = BuildJobRunner();
            var job = BuildJob();

            _outputFileMover.When(x => x.MoveOutputFiles(job)).Do(x => { throw new AbortWorkflowException(""); });

            Assert.ThrowsAsync<AbortWorkflowException>(() => jobRunner.RunJob(job, _outputFileMover));
        }

        [Test]
        public void RunJob_WithNoOutputFiles_ThrowsProcessingException()
        {
            var jobRunner = BuildJobRunner();
            var job = BuildJob();

            _outputFileMover.When(x => x.MoveOutputFiles(job)).Do(x => { job.OutputFiles.Clear(); });

            Assert.ThrowsAsync<ProcessingException>(() => jobRunner.RunJob(job, _outputFileMover));
        }

        [Test]
        public async Task RunJob_WithoutProblems_Succeeds()
        {
            var jobRunner = BuildJobRunner();
            var job = BuildJob();

            await jobRunner.RunJob(job, _outputFileMover);

            Assert.IsTrue(job.Completed);
        }
    }
}
