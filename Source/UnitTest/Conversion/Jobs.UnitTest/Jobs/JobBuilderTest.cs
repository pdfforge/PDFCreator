using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Utilities;
using System.Linq;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Jobs.Jobs
{
    [TestFixture]
    public class JobBuilderTest
    {
        private IMailSignatureHelper _mailSignatureHelper;
        private IVersionHelper _versionHelper;
        private ApplicationNameProvider _applicationNameProvider;
        private PdfCreatorSettings _settings;
        private JobInfo _jobInfo;
        private ConversionProfile _conversionProfile;
        private const string EditionName = "TestEdition";
        private const string Version = "1.2.3";

        [SetUp]
        public void SetUp()
        {
            _mailSignatureHelper = Substitute.For<IMailSignatureHelper>();
            _jobInfo = new JobInfo();
            _settings = new PdfCreatorSettings();
            _conversionProfile = new ConversionProfile();
            _settings.ConversionProfiles.Add(_conversionProfile);

            _versionHelper = Substitute.For<IVersionHelper>();
            _versionHelper.FormatWithThreeDigits().Returns(Version);
            _applicationNameProvider = new ApplicationNameProvider(EditionName);
        }

        [Test]
        public void JobBuilder_SetsCorrectProducerInJob()
        {
            var jobBuilder = new JobBuilderFree(_mailSignatureHelper, _versionHelper, _applicationNameProvider);

            var job = jobBuilder.BuildJobFromJobInfo(_jobInfo, _settings);

            Assert.AreEqual("PDFCreator" + " " + EditionName + " " + Version, job.Producer);
        }

        [Test]
        public void JobBuilderFree_SkipSaveFileDialogIsAlwaysFalse()
        {
            var jobBuilder = new JobBuilderFree(_mailSignatureHelper, _versionHelper, _applicationNameProvider);

            jobBuilder.BuildJobFromJobInfo(_jobInfo, _settings);

            Assert.IsFalse(_settings.ConversionProfiles.First().SkipPrintDialog);
        }

        [Test]
        public void JobBuilderPlus_SkipSaveFileDialogIsTrue_ReturnsTrue()
        {
            var jobBuilder = new JobBuilderPlus(_mailSignatureHelper, _versionHelper, _applicationNameProvider);
            _settings.ConversionProfiles.First().SkipPrintDialog = true;

            jobBuilder.BuildJobFromJobInfo(_jobInfo, _settings);

            Assert.IsTrue(_settings.ConversionProfiles.First().SkipPrintDialog);
        }

        [Test]
        public void JobBuilderPlus_SkipSaveFileDialogIsFalse_ReturnsFalse()
        {
            var jobBuilder = new JobBuilderPlus(_mailSignatureHelper, _versionHelper, _applicationNameProvider);
            _settings.ConversionProfiles.First().SkipPrintDialog = false;

            jobBuilder.BuildJobFromJobInfo(_jobInfo, _settings);

            Assert.IsFalse(_settings.ConversionProfiles.First().SkipPrintDialog);
        }
    }
}
