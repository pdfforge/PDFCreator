using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.UsageStatistics;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Threading.Tasks;
using Arg = NSubstitute.Arg;

namespace UsageStatistics.UnitTest
{
    [TestFixture]
    public class UsageStatisticsTest
    {
        private IUsageStatisticsManager _usageStatisticsManager;
        private IVersionHelper _versionHelper;
        private IMachineIdGenerator _machineIdGenerator;
        private IUsageStatisticsSender _usageStatisticsSender;
        private IOsHelper _osHelper;
        private ApplicationNameProvider _applicationNameProvider;

        [SetUp]
        public void SetUp()
        {
            _osHelper = Substitute.For<IOsHelper>();
            _versionHelper = Substitute.For<IVersionHelper>();
            _usageStatisticsSender = Substitute.For<IUsageStatisticsSender>();
            _machineIdGenerator = Substitute.For<IMachineIdGenerator>();
            _applicationNameProvider = new ApplicationNameProvider("testedition");

            _usageStatisticsManager = new UsageStatisticsManager(_usageStatisticsSender, _osHelper, _machineIdGenerator, _applicationNameProvider, _versionHelper);
        }

        private Job BuildJob()
        {
            var jobInfo = new JobInfo();
            jobInfo.Metadata = new Metadata()
            {
                Author = "test author"
            };
            var job = new Job(jobInfo, new ConversionProfile(), new Accounts());

            job.Profile.OutputFormat = OutputFormat.Pdf;
            job.Profile.AutoSave.Enabled = true;
            job.Profile.ShowQuickActions = true;

            job.Profile.OpenViewer = true;
            job.Profile.OpenWithPdfArchitect = true;
            job.NumberOfCopies = 10;

            job.Profile.DropboxSettings.Enabled = true;
            job.Profile.Ftp.Enabled = true;
            job.Profile.EmailSmtpSettings.Enabled = true;
            job.Profile.EmailClientSettings.Enabled = true;
            job.Profile.HttpSettings.Enabled = true;
            job.Profile.Printing.Enabled = true;

            job.Profile.BackgroundPage.Enabled = true;
            job.Profile.CoverPage.Enabled = true;
            job.Profile.AttachmentPage.Enabled = true;
            job.Profile.Stamping.Enabled = true;

            job.Profile.PdfSettings.Security.Enabled = true;
            job.Profile.PdfSettings.Signature.Enabled = true;
            job.Profile.Scripting.Enabled = true;
            job.Profile.CustomScript.Enabled = true;
            job.Profile.UserTokens.Enabled = true;

            return job;
        }

        [Test]
        public void SendUserStatistics_HttpHandlerPostAsyncRecivesOneCall()
        {
            var job = BuildJob();
            var duration = TimeSpan.MaxValue;
            var status = "test status";
            _usageStatisticsManager.EnableUsageStatistics = true;
            _usageStatisticsManager.SendUsageStatistics(duration, job, status);

            _usageStatisticsSender.Received(1).SendAsync(Arg.Any<IUsageMetric>());
        }

        [Test]
        public void SendServiceStatistics_HttpHandlerPostAsyncRecivesOneCall()
        {
            var serviceUpTime = TimeSpan.MaxValue;
            _usageStatisticsManager.EnableUsageStatistics = true;
            _usageStatisticsManager.SendServiceStatistics(serviceUpTime);

            _usageStatisticsSender.Received(1).SendAsync(Arg.Any<IUsageMetric>());
        }

        [Test]
        public void SendServiceStatistics_HttpHandlerPostAsyncDoesntReciveCall()
        {
            var serviceUpTime = TimeSpan.MaxValue;
            _usageStatisticsManager.EnableUsageStatistics = false;
            _usageStatisticsManager.SendServiceStatistics(serviceUpTime);

            _usageStatisticsSender.DidNotReceive().SendAsync(Arg.Any<IUsageMetric>());
        }

        [Test]
        public void SendUserStatistics_UsageStatisticsIsDisabled_HttpHandlerPostAsyncDoesntReciveCall()
        {
            var job = BuildJob();
            var duration = TimeSpan.MaxValue;
            var status = "test status";
            _usageStatisticsManager.EnableUsageStatistics = false;
            _usageStatisticsManager.SendUsageStatistics(duration, job, status);

            _usageStatisticsSender.DidNotReceive().SendAsync(Arg.Any<IUsageMetric>());
        }

        [Test]
        public async Task SendUsageStatistics_JobUsageStatsMetricHoldsProperValues()
        {
            var job = BuildJob();
            var duration = TimeSpan.MaxValue;
            var status = "test status";
            var product = _applicationNameProvider.ApplicationNameWithEdition.ToLower().Replace(" ", "_");

            JobUsageStatisticsMetric metric = new JobUsageStatisticsMetric();
            _usageStatisticsManager.EnableUsageStatistics = true;

            _usageStatisticsSender.When(sender => sender.SendAsync(Arg.Any<IUsageMetric>())).Do(info =>
            {
                metric = info.Arg<JobUsageStatisticsMetric>();
            });

            await _usageStatisticsManager.SendUsageStatistics(duration, job, status);

            Assert.AreEqual("JobMetric", metric.EventName);
            Assert.AreEqual(product, metric.Product);
            Assert.AreEqual(_machineIdGenerator.GetMachineId(), metric.MachineId);
            Assert.AreEqual(_versionHelper.ApplicationVersion, metric.Version);
            Assert.AreEqual(OutputFormat.Pdf.ToString(), metric.OutputFormat);
            Assert.AreEqual(status, metric.Status);
            Assert.AreEqual((long)duration.TotalMilliseconds, metric.Duration);
            Assert.AreEqual(job.JobInfo.TotalPages, metric.TotalPages);
            Assert.AreEqual(job.NumberOfCopies, metric.NumberOfCopies);

            Assert.AreEqual(job.Profile.DropboxSettings.Enabled, metric.Dropbox);
            Assert.AreEqual(job.Profile.Ftp.Enabled, metric.Ftp);
            Assert.AreEqual(job.Profile.EmailSmtpSettings.Enabled, metric.Smtp);
            Assert.AreEqual(job.Profile.HttpSettings.Enabled, metric.Http);
            Assert.AreEqual(job.Profile.Printing.Enabled, metric.Print);

            Assert.AreEqual(job.Profile.CoverPage.Enabled, metric.Cover);
            Assert.AreEqual(job.Profile.BackgroundPage.Enabled, metric.Background);
            Assert.AreEqual(job.Profile.AttachmentPage.Enabled, metric.Attachment);
            Assert.AreEqual(job.Profile.Stamping.Enabled, metric.Stamp);

            Assert.AreEqual(job.Profile.PdfSettings.Security.Enabled, metric.Encryption);
            Assert.AreEqual(job.Profile.PdfSettings.Signature.Enabled, metric.Signature);

            Assert.AreEqual(job.Profile.Scripting.Enabled, metric.Script);
            Assert.AreEqual(job.Profile.CustomScript.Enabled, metric.CustomScript);
            Assert.AreEqual(job.Profile.UserTokens.Enabled, metric.UserToken);
        }

        [Test]
        public async Task SendServiceStatistics_ServiceUsageStatisticsMetricHoldsProperValues()
        {
            var duration = TimeSpan.MaxValue;
            var product = _applicationNameProvider.ApplicationNameWithEdition.ToLower().Replace(" ", "_");

            ServiceUsageStatisticsMetric metric = new ServiceUsageStatisticsMetric();
            _usageStatisticsManager.EnableUsageStatistics = true;

            _usageStatisticsSender.When(sender => sender.SendAsync(Arg.Any<IUsageMetric>())).Do(info =>
           {
               metric = info.Arg<ServiceUsageStatisticsMetric>();
           });

            await _usageStatisticsManager.SendServiceStatistics(duration);

            Assert.AreEqual("ServiceMetric", metric.EventName);
            Assert.AreEqual(product, metric.Product);
            Assert.AreEqual(_machineIdGenerator.GetMachineId(), metric.MachineId);
            Assert.AreEqual(_versionHelper.ApplicationVersion, metric.Version);
            Assert.AreEqual(_osHelper.GetWindowsVersion(), metric.OperatingSystem);
            Assert.AreEqual((long)duration.TotalMilliseconds, metric.ServiceUptime);
        }
    }
}
