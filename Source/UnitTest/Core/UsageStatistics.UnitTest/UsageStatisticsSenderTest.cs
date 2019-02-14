using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.UsageStatistics;
using System;
using System.Net.Http;

namespace UsageStatistics.UnitTest
{
    [TestFixture]
    public class UsageStatisticsSenderTest
    {
        private IUsageStatisticsSender _usageStatisticsSender;
        private IHttpHandler _httpHandler;
        private ServiceUsageStatisticsMetric _serviceUsageStatisticsMetric;
        private JobUsageStatisticsMetric _jobUsageStatisticsMetric;

        [SetUp]
        public void SetUp()
        {
            _httpHandler = Substitute.For<IHttpHandler>();
            _usageStatisticsSender = new UsageStatisticsSender(_httpHandler);

            _serviceUsageStatisticsMetric = new ServiceUsageStatisticsMetric()
            {
                MachineId = "some machineId",
                OperatingSystem = "some operating system",
                Product = "some product name",
                TotalDocuments = 1,
                TotalUsers = 1,
                ServiceUptime = TimeSpan.TicksPerMillisecond,
                Version = "some version number"
            };

            _jobUsageStatisticsMetric = new JobUsageStatisticsMetric()
            {
                Attachment = true,
                Background = true,
                Cover = true,
                Dropbox = false,
                Duration = TimeSpan.TicksPerMillisecond,
                Encryption = false,
                Ftp = true,
                Http = true,
                Smtp = false,
                MachineId = "some machineId",
                Product = "some product",
                Mailclient = true,
                NumberOfCopies = 1,
                OutputFormat = OutputFormat.Pdf.ToString(),
                Print = false,
                Script = false,
                Signature = false,
                Stamp = true,
                Status = "Success",
                TotalPages = 1,
                UserToken = false,
                Version = "some version number"
            };
        }

        [Test]
        public void UserStatisticsSender_SendServiceStatistics_HttpHandlerPostAsyncRecivesOneCall()
        {
            _usageStatisticsSender.Send(_serviceUsageStatisticsMetric);

            _httpHandler.Received(1).PostAsync(Arg.Any<Uri>(), Arg.Any<HttpContent>());
        }

        [Test]
        public void UserStatisticsSender_SendJobStatistics_HttpHandlerPostAsyncRecivesOneCall()
        {
            _usageStatisticsSender.Send(_jobUsageStatisticsMetric);

            _httpHandler.Received(1).PostAsync(Arg.Any<Uri>(), Arg.Any<HttpContent>());
        }
    }
}
