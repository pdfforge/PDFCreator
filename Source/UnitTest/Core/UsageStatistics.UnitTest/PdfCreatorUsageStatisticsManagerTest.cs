using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.UsageStatistics;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Threading;
using System;

namespace UsageStatistics.UnitTest
{
    [TestFixture]
    public class PdfCreatorUsageStatisticsTest
    {
        private IPdfCreatorUsageStatisticsManager _pdfCreatorUsageStatisticsManager;
        private IOsHelper _osHelper;
        private IVersionHelper _versionHelper;
        private IMachineIdGenerator _machineIdGenerator;
        private IUsageStatisticsSender _usageStatisticsSender;
        private ISettingsProvider _settingsProvider;
        private PdfCreatorSettings _settings;
        private IGpoSettings _gpoSettings;
        private IThreadManager _threadManager;
        private ApplicationNameProvider _applicationNameProvider;
        private readonly TimeSpan _duration = TimeSpan.MaxValue;
        private readonly string _status = "test status";

        [SetUp]
        public void SetUp()
        {
            _osHelper = Substitute.For<IOsHelper>();
            _versionHelper = Substitute.For<IVersionHelper>();
            _versionHelper.ApplicationVersion.Returns(info => new Version());
            _usageStatisticsSender = Substitute.For<IUsageStatisticsSender>();
            _machineIdGenerator = Substitute.For<IMachineIdGenerator>();
            _machineIdGenerator.GetMachineId().Returns("testmachinid");
            _applicationNameProvider = new ApplicationNameProvider("testedition");
            _settings = new PdfCreatorSettings();
            _settingsProvider = Substitute.For<ISettingsProvider>();
            _settingsProvider.Settings.Returns(_settings);
            _gpoSettings = Substitute.For<IGpoSettings>();
            _threadManager = Substitute.For<IThreadManager>();

            _pdfCreatorUsageStatisticsManager = BuildPdfCreatorUsageStatisticsManager(_threadManager);
        }

        private PdfCreatorUsageStatisticsManager BuildPdfCreatorUsageStatisticsManager(IThreadManager threadManager)
        {
            return new PdfCreatorUsageStatisticsManager(_usageStatisticsSender, _osHelper, _machineIdGenerator,
                _applicationNameProvider, _versionHelper, _settingsProvider, _gpoSettings, threadManager);
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
        public void SendPdfCreatorStatistics_UsageStatisticsIsEnabled_HttpHandlerPostReceivesOneCall()
        {
            var job = BuildJob();

            _settingsProvider.Settings.ApplicationSettings.UsageStatistics.Enable = true;

            _threadManager.When(x => x.StartSynchronizedThread(Arg.Any<ISynchronizedThread>()))
                .Do(info =>
                {
                    var t = info.Arg<ISynchronizedThread>();
                    t.Start();
                    t.Join();
                });

            _pdfCreatorUsageStatisticsManager.SendUsageStatistics(_duration, job, _status);
            _usageStatisticsSender.Received(1).Send(Arg.Any<IUsageMetric>());
        }

        [Test]
        public void SendPdfCreatorStatistics_UsageStatisticsIsDisabled_HttpHandlerPostDoesntReceiveCall()
        {
            var job = BuildJob();

            _settingsProvider.Settings.ApplicationSettings.UsageStatistics.Enable = false;
            _pdfCreatorUsageStatisticsManager.SendUsageStatistics(_duration, job, _status);

            _threadManager.DidNotReceive().StartSynchronizedThread(Arg.Any<ISynchronizedThread>());
            _usageStatisticsSender.DidNotReceive().SendAsync(Arg.Any<IUsageMetric>());
        }

        [Test]
        public void SendPdfCreatorStatistics_UsageStatisticsIsDisabledByGpoEnabled_HttpHandlerPostDoesntReceiveCall()
        {
            var job = BuildJob();

            _settingsProvider.Settings.ApplicationSettings.UsageStatistics.Enable = true;
            _gpoSettings.DisableUsageStatistics.Returns(true);
            _pdfCreatorUsageStatisticsManager.SendUsageStatistics(_duration, job, _status);

            _threadManager.DidNotReceive().StartSynchronizedThread(Arg.Any<ISynchronizedThread>());
            _usageStatisticsSender.DidNotReceive().SendAsync(Arg.Any<IUsageMetric>());
        }

        [Test]
        public void SendPdfCreatorStatistics_UsageStatsMetricHoldsProperValues()
        {
            var job = BuildJob();
            var duration = TimeSpan.MaxValue;
            var status = "test status";
            PdfCreatorUsageStatisticsMetric metric = new PdfCreatorUsageStatisticsMetric();
            _settingsProvider.Settings.ApplicationSettings.UsageStatistics.Enable = true;
            _gpoSettings.DisableUsageStatistics.Returns(false);

            _threadManager.When(x => x.StartSynchronizedThread(Arg.Any<ISynchronizedThread>()))
                .Do(info =>
                {
                    var t = info.Arg<ISynchronizedThread>();
                    t.Start();
                    t.Join();
                });

            _usageStatisticsSender.When(sender => sender.Send(Arg.Any<IUsageMetric>())).Do(info =>
            {
                metric = info.Arg<PdfCreatorUsageStatisticsMetric>();
            });

            _pdfCreatorUsageStatisticsManager.SendUsageStatistics(duration, job, status);

            Assert.AreEqual(_machineIdGenerator.GetMachineId(), metric.MachineId);
            Assert.AreEqual(OutputFormat.Pdf.ToString(), metric.OutputFormat);
            Assert.AreEqual(Mode.AutoSave, metric.Mode);
            Assert.AreEqual(job.Profile.ShowQuickActions, metric.QuickActions);

            Assert.AreEqual(job.Profile.OpenViewer, metric.OpenViewer);
            Assert.AreEqual(job.Profile.OpenWithPdfArchitect, metric.OpenWithPdfArchitect);
            Assert.AreEqual(job.JobInfo.TotalPages, metric.TotalPages);
            Assert.AreEqual(job.NumberOfCopies, metric.NumberOfCopies);

            Assert.AreEqual(_versionHelper.ApplicationVersion.ToString(), metric.Version);
            Assert.AreEqual((long)duration.TotalMilliseconds, metric.Duration);

            Assert.AreEqual(job.Profile.DropboxSettings.Enabled, metric.Dropbox);
            Assert.AreEqual(job.Profile.Ftp.Enabled, metric.Ftp);
            Assert.AreEqual(job.Profile.EmailSmtpSettings.Enabled, metric.Smtp);
            Assert.AreEqual(job.Profile.EmailClientSettings.Enabled, metric.Mailclient);
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

            Assert.AreEqual(status, metric.Status);
        }
    }
}
