using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.JobEvents;
using pdfforge.PDFCreator.Core.UsageStatistics;
using System;

namespace UsageStatistics.UnitTest
{
    [TestFixture]
    public class ServiceEventsHandlerTest
    {
        private IUsageStatisticsManager _usageStatisticsManager;
        private ServiceEventsHandler _serviceEventsHandler;

        [SetUp]
        public void SetUp()
        {
            _usageStatisticsManager = Substitute.For<IUsageStatisticsManager>();
            _serviceEventsHandler = new ServiceEventsHandler(_usageStatisticsManager);
        }

        private Job BuildJob()
        {
            var jobInfo = new JobInfo();
            var job = new Job(jobInfo, new ConversionProfile(), new Accounts());

            return job;
        }

        [Test]
        public void HandleJobCompleted_UsageStatisticsManagerSenUserStatistics_RecivedOneCall()
        {
            var job = BuildJob();
            var duration = TimeSpan.MaxValue;
            var status = "Success";

            _serviceEventsHandler.HandleJobCompleted(job, duration);

            _usageStatisticsManager.Received(1).SendUsageStatistics(Arg.Any<TimeSpan>(), Arg.Any<Job>(), status);
        }

        [Test]
        public void HandleJobFailed_UsageStatisticsManagerSendUserStatistics_RecivedOneCall()
        {
            var job = BuildJob();
            var duration = TimeSpan.MaxValue;
            var status = FailureReason.Error;

            _serviceEventsHandler.HandleJobFailed(job, duration, status);

            _usageStatisticsManager.Received(1).SendUsageStatistics(Arg.Any<TimeSpan>(), Arg.Any<Job>(), status.ToString());
        }

        [Test]
        public void HandleServiceStopped_UsageStatisticsManagerSendServiceStatistics_RecivedOneCall()
        {
            var serviceUpTime = TimeSpan.MaxValue;

            _serviceEventsHandler.HandleServiceStopped(serviceUpTime);

            _usageStatisticsManager.Received(1).SendServiceStatistics(serviceUpTime);
        }
    }
}
