using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.JobEvents;
using System;

namespace pdfforge.PDFCreator.UnitTest.Core.Services.UnitTest.JobEvents
{
    [TestFixture]
    public class JobEventsManagerTest
    {
        private IJobEventsHandler _handler1;
        private IJobEventsHandler _handler2;

        [SetUp]
        public void Setup()
        {
            _handler1 = Substitute.For<IJobEventsHandler>();
            _handler2 = Substitute.For<IJobEventsHandler>();
        }

        private JobEventsManager BuildJobEventsManager(params IJobEventsHandler[] handlers)
        {
            return new JobEventsManager(handlers);
        }

        private Job BuildJob()
        {
            return new Job(new JobInfo(), new ConversionProfile(), new Accounts());
        }

        [Test]
        public void RaiseJobStarted_CallsAllHandlers()
        {
            var jobEventsManager = BuildJobEventsManager(_handler1, _handler2);
            var job = BuildJob();

            jobEventsManager.RaiseJobStarted(job, "");

            _handler1.Received(1).HandleJobStarted(job, "");
            _handler2.Received(1).HandleJobStarted(job, "");
        }

        [Test]
        public void RaiseJobStarted_FirstHGandlerThrowsException_SecondHandlerIsExecuted()
        {
            var jobEventsManager = BuildJobEventsManager(_handler1, _handler2);
            var job = BuildJob();
            _handler1.When(x => x.HandleJobStarted(job, "")).Throw<Exception>();

            jobEventsManager.RaiseJobStarted(job, "");

            _handler2.Received(1).HandleJobStarted(job, "");
        }

        [Test]
        public void RaiseCompletedStarted_CallsAllHandlers()
        {
            var jobEventsManager = BuildJobEventsManager(_handler1, _handler2);
            var job = BuildJob();
            var duration = TimeSpan.FromSeconds(1);

            jobEventsManager.RaiseJobCompleted(job, duration);

            _handler1.Received(1).HandleJobCompleted(job, duration);
            _handler2.Received(1).HandleJobCompleted(job, duration);
        }

        [Test]
        public void RaiseFailedStarted_CallsAllHandlers()
        {
            var jobEventsManager = BuildJobEventsManager(_handler1, _handler2);
            var job = BuildJob();
            var duration = TimeSpan.FromSeconds(1);

            jobEventsManager.RaiseJobFailed(job, duration, FailureReason.Error);

            _handler1.Received(1).HandleJobFailed(job, duration, FailureReason.Error);
            _handler2.Received(1).HandleJobFailed(job, duration, FailureReason.Error);
        }
    }
}
