using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.DataStorage.Storage;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Workflow;

namespace pdfforge.PDFCreator.IntegrationTest.Core.Workflow
{
    [TestFixture]
    internal class PrinterMappingTest
    {
        [SetUp]
        public void SetupSettings()
        {
            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            _jobBuilder = container.GetInstance<IJobBuilder>();

            var ini = new IniStorage("");

            _testHelper = container.GetInstance<TestHelper>();
            _testHelper.InitTempFolder("MappingTest");

            _settings = new PdfCreatorSettings();
            _settings.ConversionProfiles.Add(new ConversionProfile { Guid = "Profile0", Name = "Profile0" });
            _settings.ConversionProfiles.Add(new ConversionProfile { Guid = "Profile1", Name = "Profile1" });

            _testHelper.GenerateGsJob(PSfiles.EmptyPage, OutputFormat.Pdf);
        }

        [TearDown]
        public void Cleanup()
        {
            _testHelper?.CleanUp();
        }

        private PdfCreatorSettings _settings;
        private TestHelper _testHelper;
        private IJobBuilder _jobBuilder;

        [Test]
        public void PrinterMapping_WithInvalidPrinter_SelectsDefaultProfile()
        {
            _settings.ApplicationSettings.PrinterMappings.Add(new PrinterMapping("InvalidPrinter", "Profile1"));

            var job = _jobBuilder.BuildJobFromJobInfo(_testHelper.JobInfo, _settings);

            Assert.AreEqual(_settings.ConversionProfiles[0], job.Profile, "Default profile should have been mapped");
        }

        [Test]
        public void PrinterMapping_WithInvalidProfile_SelectsDefaultProfile()
        {
            _settings.ApplicationSettings.PrinterMappings.Add(new PrinterMapping("PDFCreator", "NonExistentProfile"));

            var job = _jobBuilder.BuildJobFromJobInfo(_testHelper.JobInfo, _settings);
            Assert.AreEqual(_settings.ConversionProfiles[0], job.Profile, "Default profile should have been mapped");
        }

        [Test]
        public void PrinterMapping_WithProfileMapping_PreselectsProfile()
        {
            _settings.ApplicationSettings.PrinterMappings.Add(new PrinterMapping("PDFCreator", "Profile1"));

            var job = _jobBuilder.BuildJobFromJobInfo(_testHelper.JobInfo, _settings);
            Assert.AreEqual(_settings.ConversionProfiles[1].Name, job.Profile.Name, "Profile names should match");
            Assert.AreEqual(_settings.ConversionProfiles[1], job.Profile, "Profile1 should have been mapped");
        }

        [Test]
        public void PrinterMapping_WithProfileMappingWithOtherCasing_PreselectsProfile()
        {
            _settings.ApplicationSettings.PrinterMappings.Add(new PrinterMapping("pdfcreator", "Profile1"));

            var job = _jobBuilder.BuildJobFromJobInfo(_testHelper.JobInfo, _settings);
            Assert.AreEqual(_settings.ConversionProfiles[1], job.Profile, "Profile1 should have been mapped");
        }
    }
}
