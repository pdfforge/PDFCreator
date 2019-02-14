using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.Workflow.Steps;
using pdfforge.PDFCreator.Utilities;
using Ploeh.AutoFixture;

namespace Presentation.UnitTest.Workflow.Steps
{
    [TestFixture]
    public class SignaturePasswordStepTest
    {
        private Job _job;
        private SignaturePasswordStep _step;
        private ISignaturePasswordCheck _signaturePasswordCheck;
        private ConversionProfile _conversionProfile;
        private readonly IFixture _fixture = new Fixture();

        [SetUp]
        public void Setup()
        {
            _signaturePasswordCheck = Substitute.For<ISignaturePasswordCheck>();
            _conversionProfile = new ConversionProfile();
            _conversionProfile.PdfSettings.Signature.Enabled = true;
            _conversionProfile.PdfSettings.Signature.CertificateFile = _fixture.Create<string>();

            _job = new Job(null, _conversionProfile, new Accounts());
            _job.Passwords.PdfSignaturePassword = _fixture.Create<string>();

            _step = new SignaturePasswordStep(_signaturePasswordCheck);
        }

        [Test]
        public void IsRequired_SigningIsDisabled_ReturnsFalse()
        {
            _conversionProfile.PdfSettings.Signature.Enabled = false;

            var result = _step.IsStepRequired(_job);

            Assert.IsFalse(result);
        }

        [Test]
        public void IsRequired_SigningIsEnabled_PasswordCheckIsValid_ReturnsFalse()
        {
            _conversionProfile.PdfSettings.Signature.Enabled = true;
            _signaturePasswordCheck.IsValidPassword(
                    _conversionProfile.PdfSettings.Signature.CertificateFile,
                    _job.Passwords.PdfSignaturePassword)
                .Returns(true);

            var result = _step.IsStepRequired(_job);

            _signaturePasswordCheck.Received().IsValidPassword(
                _conversionProfile.PdfSettings.Signature.CertificateFile,
                _job.Passwords.PdfSignaturePassword);

            Assert.IsFalse(result);
        }

        [Test]
        public void IsRequired_SigningIsEnabled_PasswordCheckIsInvalid_ReturnsTrue()
        {
            _conversionProfile.PdfSettings.Signature.Enabled = true;
            _signaturePasswordCheck.IsValidPassword(
                    _conversionProfile.PdfSettings.Signature.CertificateFile,
                    _job.Passwords.PdfSignaturePassword)
                .Returns(false);

            var result = _step.IsStepRequired(_job);

            _signaturePasswordCheck.Received().IsValidPassword(
                _conversionProfile.PdfSettings.Signature.CertificateFile,
                _job.Passwords.PdfSignaturePassword);

            Assert.IsTrue(result);
        }
    }
}
