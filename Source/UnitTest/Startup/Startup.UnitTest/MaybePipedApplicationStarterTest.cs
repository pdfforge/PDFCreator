using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Core.Communication;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.JobHistory;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Startup.AppStarts;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.Utilities.Threading;
using System;
using System.IO;
using System.Threading;

namespace pdfforge.PDFCreator.UnitTest.Startup
{
    [TestFixture]
    public class MaybePipedApplicationStarterTest
    {
        [SetUp]
        public void Setup()
        {
            _settingsManager = Substitute.For<ISettingsManager>();
            _pipeServerManager = Substitute.For<IPipeServerManager>();
            _pipeServerManager.StartServer().Returns(true);
            _threadManager = Substitute.For<IThreadManager>();
            _folderCleanUp = Substitute.For<IPdfCreatorFolderCleanUp>();
            _updateAssistant = Substitute.For<IUpdateAssistant>();
            _startupConditions = Substitute.For<ICheckAllStartupConditions>();
            _notificationService = Substitute.For<INotificationService>();
        }

        private IPipeServerManager _pipeServerManager;
        private ISettingsManager _settingsManager;
        private IThreadManager _threadManager;
        private IPdfCreatorFolderCleanUp _folderCleanUp;
        private IUpdateAssistant _updateAssistant;
        private ICheckAllStartupConditions _startupConditions;
        private IJobInfoQueueManager _jobInfoQueueManager;
        private INotificationService _notificationService;
        private IJobHistoryManager _jobHistoryManager;

        private MaybePipedApplicationStarter BuildMaybePipedApplicationStarter(int retries = 1)
        {
            _jobInfoQueueManager = Substitute.For<IJobInfoQueueManager>();
            var jobInfoQueue = Substitute.For<IJobInfoQueue>();
            var staticPropertiesHack = Substitute.For<IStaticPropertiesHack>();
            var spooledJobFinder = Substitute.For<ISpooledJobFinder>();
            _jobHistoryManager = Substitute.For<IJobHistoryManager>();

            var starter = new MaybePipedApplicationStarter(_settingsManager, _updateAssistant, _startupConditions,
                _threadManager, _pipeServerManager, _jobInfoQueueManager, jobInfoQueue, staticPropertiesHack, _folderCleanUp,
                spooledJobFinder, _notificationService, _jobHistoryManager);

            starter.Retries = retries;

            return starter;
        }

        [Test]
        public void PipeServerCannotBeStarted_RetriesStartApplication()
        {
            _pipeServerManager.StartServer().Returns(false);

            var starter = BuildMaybePipedApplicationStarter(2);

            starter.SendMessageOrStartApplication(() => "", () => false, false);

            _pipeServerManager.Received(2).StartServer();
        }

        [Test]
        public void SendMessageOrStartApplication_AllCallsFails_RetriesFiveTimes()
        {
            var expectedMessage = "TestMessage";

            _pipeServerManager.IsServerRunning().Returns(true);
            var starter = BuildMaybePipedApplicationStarter(5);

            starter.SendMessageOrStartApplication(() => expectedMessage, () => false, false);

            _pipeServerManager.Received(starter.Retries).TrySendPipeMessage(expectedMessage);
        }

        [Test]
        public void SendMessageOrStartApplication_LoadsSettings()
        {
            var starter = BuildMaybePipedApplicationStarter(1);

            starter.SendMessageOrStartApplication(() => "", () => false, false);

            Received.InOrder(() =>
            {
                _settingsManager.LoadAllSettings();
                _pipeServerManager.IsServerRunning();
                _settingsManager.LoadAllSettings();
            });
        }

        [Test]
        public void SendMessageOrStartApplication_PipeFailsThreeTimes_SuccessOnThirdTry()
        {
            var expectedMessage = "My Message";
            var starter = BuildMaybePipedApplicationStarter(5);

            _pipeServerManager.IsServerRunning().Returns(true);
            _pipeServerManager.TrySendPipeMessage(expectedMessage)
                .Returns(false, false, false, true);

            starter.SendMessageOrStartApplication(() => expectedMessage, () => false, false);

            _pipeServerManager.Received(4).TrySendPipeMessage(expectedMessage);
        }

        [Test]
        public void SendMessageOrStartApplication_PipeServerIsRunning_SendsMessageViaPipe()
        {
            var expectedMessage = "TestMessage";

            _pipeServerManager.IsServerRunning().Returns(true);
            _pipeServerManager.TrySendPipeMessage(expectedMessage).Returns(true);
            var starter = BuildMaybePipedApplicationStarter(1);

            starter.SendMessageOrStartApplication(() => expectedMessage, () => false, false);

            _pipeServerManager.Received(1).TrySendPipeMessage(expectedMessage);
        }

        [Test]
        public void SendMessageOrStartApplication_StartFailsThreeTimes_SuccessOnThirdTry()
        {
            var counter = 0;
            var starter = BuildMaybePipedApplicationStarter(5);

            Func<bool> startFunction = () => counter++ >= 3;

            starter.SendMessageOrStartApplication(() => "", startFunction, false);

            Assert.AreEqual(4, counter);
        }

        [Test]
        public void SendMessageOrStartApplication_WhenStartIsNotSuccessful_DoesNotStartCleanupThread()
        {
            var starter = BuildMaybePipedApplicationStarter(1);

            starter.SendMessageOrStartApplication(() => "", () => false, false);

            _threadManager.DidNotReceiveWithAnyArgs().StartSynchronizedThread(Arg.Any<ISynchronizedThread>());
            _threadManager.DidNotReceiveWithAnyArgs().StartSynchronizedThread(Arg.Any<ThreadStart>(), Arg.Any<string>());
            _folderCleanUp.DidNotReceive().CleanSpoolFolder(TimeSpan.FromDays(1));
            _folderCleanUp.DidNotReceive().CleanTempFolder(TimeSpan.FromDays(1));
        }

        [Test]
        public void SendMessageOrStartApplication_WhenStartIsNotSuccessful_DoesNotStartUpdateThread()
        {
            var starter = BuildMaybePipedApplicationStarter(1);

            starter.SendMessageOrStartApplication(() => "", () => false, false);

            _threadManager.DidNotReceiveWithAnyArgs().StartSynchronizedThread(Arg.Any<ISynchronizedThread>());
            _threadManager.DidNotReceiveWithAnyArgs().StartSynchronizedThread(Arg.Any<ThreadStart>(), Arg.Any<string>());
            _updateAssistant.DidNotReceive().UpdateProcedure(Arg.Any<bool>());
        }

        [Test]
        public void SendMessageOrStartApplication_WhenStartIsSuccessful_StartsCleanupThread()
        {
            var starter = BuildMaybePipedApplicationStarter(1);

            _threadManager
                .When(x => x.StartSynchronizedThread(Arg.Any<ThreadStart>(), Arg.Any<string>()))
                .Do(x =>
                {
                    var t = new Thread(x.Arg<ThreadStart>());
                    t.Start();
                    t.Join();
                });

            starter.SendMessageOrStartApplication(() => "", () => true, false);

            _folderCleanUp.Received(1).CleanSpoolFolder(TimeSpan.FromDays(1));
            _folderCleanUp.Received(1).CleanTempFolder(TimeSpan.FromDays(1));
        }

        [Test]
        public void SendMessageOrStartApplication_WhenStartIsSuccessful_StartsUpdateThread()
        {
            var starter = BuildMaybePipedApplicationStarter(1);

            _threadManager
                .When(x => x.StartSynchronizedThread(Arg.Any<ThreadStart>(), Arg.Any<string>()))
                .Do(x =>
                {
                    var t = new Thread(x.Arg<ThreadStart>());
                    t.Start();
                    t.Join();
                });

            starter.SendMessageOrStartApplication(() => "", () => true, false);

            _updateAssistant.Received(1).UpdateProcedure(true);
        }

        [Test]
        public void SendMessageOrStartApplication_WhenThrowingApplication_DoesNotCatchException()
        {
            var starter = BuildMaybePipedApplicationStarter(1);

            Assert.Throws<IOException>(() => starter.SendMessageOrStartApplication(() => "", () => { throw new IOException(); }, true));
        }

        [Test]
        public void SendMessageOrStartApplication_WithManagePrintJobs_CallsManagePrintJobs()
        {
            var starter = BuildMaybePipedApplicationStarter(2);

            starter.SendMessageOrStartApplication(() => "", () => true, true);

            _jobInfoQueueManager.Received(1).ManagePrintJobs();
        }

        [Test]
        public void TryStartApplication_WhenApplicationStartFails_StopsPipeServer()
        {
            var starter = BuildMaybePipedApplicationStarter(2);

            var counter = 0;
            Func<bool> startFunc = () =>
            {
                if (++counter <= 1)
                    throw new Exception();
                return true;
            };

            Assert.Throws<Exception>(() => starter.SendMessageOrStartApplication(() => "", startFunc, true));

            Received.InOrder(() =>
            {
                _settingsManager.LoadAllSettings();
                _pipeServerManager.IsServerRunning();
                _pipeServerManager.StartServer();
                _settingsManager.LoadAllSettings();
                // Expect Shutdown directly, as this will stop the pipe server after an error
                _pipeServerManager.Shutdown();
            });
        }

        [Test]
        public void UsesStartupConditions()
        {
            var starter = BuildMaybePipedApplicationStarter(1);

            Assert.AreSame(_startupConditions, starter.StartupConditions);
        }

        [Test]
        public void ValidateShutdownSequence()
        {
            var starter = BuildMaybePipedApplicationStarter(0);

            starter.SendMessageOrStartApplication(() => "", () => false, false);

            Received.InOrder(() =>
            {
                _settingsManager.LoadAllSettings();
                _threadManager.WaitForThreads();
                _pipeServerManager.PrepareShutdown();
                _threadManager.Shutdown();
                _settingsManager.SaveCurrentSettings();
                _jobHistoryManager.Save();
                _pipeServerManager.Shutdown();
            });
        }
    }
}
