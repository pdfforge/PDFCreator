using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using pdfforge.Mail;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using System;
using System.Linq;
using SystemInterface.IO;

namespace Presentation.UnitTest.Helper
{
    [TestFixture]
    public class ClientTestEmailTest
    {
        private ClientTestEmail _clientTestEmail;
        private IEmailClientFactory _emailClientFactory;
        private IEmailClient _emailClient;
        private EmailClientSettings _emailClientSettings;
        private IPath _pathWrap;
        private IDirectory _directoryWrap;
        private IFile _fileWrap;
        private IMailSignatureHelper _mailSignatureHelper;

        [SetUp]
        public void Setup()
        {
            _pathWrap = Substitute.For<IPath>();
            _directoryWrap = Substitute.For<IDirectory>();
            _fileWrap = Substitute.For<IFile>();
            _emailClient = Substitute.For<IEmailClient>();
            _emailClientFactory = Substitute.For<IEmailClientFactory>();
            _emailClientFactory.CreateEmailClient().Returns(_emailClient);
            _mailSignatureHelper = Substitute.For<IMailSignatureHelper>();
            _clientTestEmail = new ClientTestEmail(_emailClientFactory, _mailSignatureHelper, _pathWrap,
                _directoryWrap, _fileWrap, new TokenHelper(new DesignTimeTranslationUpdater()));

            _emailClientSettings = new EmailClientSettings();
        }

        [Test]
        public void SendTestMail_EmailClientFactoryReturnsNull_ReturnsFalse()
        {
            _emailClientFactory.CreateEmailClient().ReturnsNull();

            Assert.IsFalse(_clientTestEmail.SendTestEmail(_emailClientSettings));
        }

        [Test]
        public void SendTestMail_EmailClientGetsCalledWithCorrectEmailContent()
        {
            _emailClientSettings.Recipients = "recipient";
            _emailClientSettings.Content = "content";
            _emailClientSettings.Subject = "subject";
            _emailClientSettings.Html = true;

            var currentEmail = new Email();
            _emailClient.When(x => x.ShowEmailClient(Arg.Any<Email>())).Do(
                x =>
                {
                    currentEmail = x[0] as Email;
                });

            _clientTestEmail.SendTestEmail(_emailClientSettings);

            Assert.AreEqual(1, currentEmail.Recipients.Count, "Wrong number of recipients");
            Assert.IsTrue(currentEmail.Recipients.Any(r => r.Address == "recipient"), "Wrong recipient");
            Assert.AreEqual("content", currentEmail.Body, "Wrong mail body");
            Assert.AreEqual("subject", currentEmail.Subject, "Wrong subject");
            Assert.IsTrue(currentEmail.Html, "Wrong Html setting");
        }

        [Test]
        public void SendTestMail_SignatureGetsAttachedToContent()
        {
            _emailClientSettings.Content = "content";
            var currentEmail = new Email();
            _emailClient.When(x => x.ShowEmailClient(Arg.Any<Email>())).Do(
                x =>
                {
                    currentEmail = x[0] as Email;
                });
            _mailSignatureHelper.ComposeMailSignature().Returns("Signature");

            _clientTestEmail.SendTestEmail(_emailClientSettings);

            _mailSignatureHelper.Received().ComposeMailSignature();
            Assert.AreEqual("contentSignature", currentEmail.Body, "Wrong mail body");
        }

        [Test]
        public void
            SendTestMail_EmailClientGetsCalledWithCorrectEmailContent_MultipleRecepientsWithColonSemiColonAndSpaces()
        {
            const string recipient =
                "recipient1@pdffrog.quak, recipient2@pdffrog.quak   ; recipient3@pdffrog.quak         ";
            _emailClientSettings.Recipients = recipient;
            var currentEmail = new Email();
            _emailClient.When(x => x.ShowEmailClient(Arg.Any<Email>())).Do(
                x =>
                {
                    currentEmail = x[0] as Email;
                });

            _clientTestEmail.SendTestEmail(_emailClientSettings);

            Assert.AreEqual(3, currentEmail.Recipients.Count, "Wrong number of recipients");
            Assert.IsTrue(currentEmail.Recipients.Any(r => r.Address == "recipient1@pdffrog.quak"), "Missing first recipient");
            Assert.IsTrue(currentEmail.Recipients.Any(r => r.Address == "recipient2@pdffrog.quak"), "Missing second recipient");
            Assert.IsTrue(currentEmail.Recipients.Any(r => r.Address == "recipient3@pdffrog.quak"), "Missing third recipient");
        }

        [Test]
        public void SendTestMail_EmailClientGetsCalledWithCorrectEmailContent_AttachmentFile()
        {
            var currentEmail = new Email();
            _pathWrap.GetTempPath().Returns("temp");

            _emailClient.When(x => x.ShowEmailClient(Arg.Any<Email>())).Do(
                x =>
                {
                    currentEmail = x[0] as Email;
                });

            _clientTestEmail.SendTestEmail(_emailClientSettings);

            Received.InOrder(() =>
            {
                _pathWrap.GetTempPath();
                _directoryWrap.CreateDirectory(@"temp\PDFCreator-Test\SendSmtpTestmail");
                _fileWrap.WriteAllText(@"temp\PDFCreator-Test\SendSmtpTestmail\PDFCreator Mail Client Test.pdf", "");
            });

            Assert.AreEqual(1, currentEmail.Attachments.Count, "Test mail has more than one attachment");
            Assert.AreEqual(@"temp\PDFCreator-Test\SendSmtpTestmail\PDFCreator Mail Client Test.pdf",
                currentEmail.Attachments.First().Filename);
        }

        [Test]
        public void SendTestMail_ReplacesTokens()
        {
            var currentEmail = new Email();
            _pathWrap.GetTempPath().Returns("temp");
            _emailClientSettings.Subject = "<ComputerName>";
            _emailClientSettings.Content = "<ComputerName>";

            _emailClient.When(x => x.ShowEmailClient(Arg.Any<Email>())).Do(
                x =>
                {
                    currentEmail = x[0] as Email;
                });

            _clientTestEmail.SendTestEmail(_emailClientSettings);

            Assert.AreEqual(Environment.MachineName, currentEmail.Subject, "Tokens were not replaced in subject");
            Assert.AreEqual(Environment.MachineName, currentEmail.Body, "Tokens were not replaced in body");
        }

        [Test]
        public void SendTestMail_RunsWithoutException_ReturnsTrue()
        {
            Assert.IsTrue(_clientTestEmail.SendTestEmail(_emailClientSettings));
        }

        [Test]
        public void SendTestMail_RunsWithException_ReturnsFalse()
        {
            _emailClient.ShowEmailClient(Arg.Any<Email>()).Throws(new Exception());

            Assert.IsFalse(_clientTestEmail.SendTestEmail(_emailClientSettings));
        }
    }
}
