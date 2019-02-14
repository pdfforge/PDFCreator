using NSubstitute;
using NUnit.Framework;
using pdfforge.Mail;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Utilities.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Jobs.Actions
{
    [TestFixture]
    public class EmailClientActionTest
    {
        [SetUp]
        public void SetUp()
        {
            _tokenReplacer = new TokenReplacer();

            _profile = new ConversionProfile();
            _profile.EmailClientSettings.Enabled = true;
            _profile.EmailClientSettings.Subject = "testsubject";
            _profile.EmailClientSettings.Content = "This is content\r\nwith line breaks";
            _profile.EmailClientSettings.AddSignature = false;
            _profile.EmailClientSettings.Recipients = "test@local";

            _job = new Job(new JobInfo(), _profile, new Accounts());
            _job.TokenReplacer = _tokenReplacer;
            _job.OutputFiles = new[] { @"C:\Temp\file1.pdf" }.ToList();
            _job.Profile = _profile;

            _mailSignatureHelper = Substitute.For<IMailSignatureHelper>();
            _mailSignatureHelper.ComposeMailSignature().Returns(SignatureText);

            _mockMailClient = new MockMailClient();

            _emailClientFactory = Substitute.For<IEmailClientFactory>();
            _emailClientFactory.CreateEmailClient().Returns(_mockMailClient);
        }

        private Job _job;
        private ConversionProfile _profile;
        private IEmailClientFactory _emailClientFactory;
        private MockMailClient _mockMailClient;
        private TokenReplacer _tokenReplacer;
        private IMailSignatureHelper _mailSignatureHelper;

        private const string SignatureText = "Email automatically created by the free PDFCreator";

        private EMailClientAction BuildAction()
        {
            return new EMailClientAction(_emailClientFactory, _mailSignatureHelper);
        }

        [Test]
        public void EmailClientAction_BodyWithToken_MailContainsReplacedBody()
        {
            var action = BuildAction();
            _profile.EmailClientSettings.Content = "some content \r\nwith line breaks <foo>";
            _tokenReplacer.AddStringToken("foo", "bar");

            action.ProcessJob(_job);

            Assert.AreEqual(_mockMailClient.Mails[0].Body, "some content \r\nwith line breaks bar");
        }

        [Test]
        public void EmailClientAction_CouldNotStartClient_ReturnsActionresultWithId100()
        {
            var action = BuildAction();
            _mockMailClient.WillFail = true;

            var result = action.ProcessJob(_job);

            Assert.AreEqual(ErrorCode.MailClient_GenericError, result[0]);
        }

        [Test]
        public void EmailClientAction_NoClientInstalled_ReturnsActionresultWithId101()
        {
            _emailClientFactory = Substitute.For<IEmailClientFactory>();
            _emailClientFactory.CreateEmailClient().Returns(x => null);
            var action = BuildAction();

            var result = action.ProcessJob(_job);

            Assert.AreEqual(ErrorCode.MailClient_NoCompatibleEmailClientInstalled, result[0]);
        }

        [Test]
        public void EmailClientAction_SubjectWithToken_MailContainsReplacedSubject()
        {
            var action = BuildAction();
            _profile.EmailClientSettings.Subject = "my subject <foo>";
            _tokenReplacer.AddStringToken("foo", "bar");

            action.ProcessJob(_job);

            Assert.AreEqual(_mockMailClient.Mails[0].Subject, "my subject bar");
        }

        [Test]
        public void EmailClientAction_WhenExceptionIsThrown_ReturnsActionresultWithId999()
        {
            var action = BuildAction();
            _mockMailClient.ExceptionThrown = new Exception();

            var result = action.ProcessJob(_job);

            Assert.AreEqual(ErrorCode.MailClient_GenericError, result[0]);
        }

        [Test]
        public void EmailClientAction_WithBody_MailContainsBody()
        {
            var action = BuildAction();
            _profile.EmailClientSettings.Content = "some content \r\nwith line breaks";

            action.ProcessJob(_job);

            Assert.AreEqual(_mockMailClient.Mails[0].Body, _profile.EmailClientSettings.Content);
        }

        [Test]
        public void EmailClientAction_WithEmptyRecipients_OnlyContainsValidInMail()
        {
            var action = BuildAction();
            _profile.EmailClientSettings.Recipients = "a@local; ; b@local";

            action.ProcessJob(_job);

            var mail = _mockMailClient.Mails[0];
            Assert.AreEqual(new[] { "a@local", "b@local" }.ToList(), mail.Recipients.Select(r => r.Address));
        }

        [Test]
        public void EmailClientAction_WithMultipleRecipientsSeperatedByCommas_AllRecipientsListedInMail()
        {
            var action = BuildAction();
            _profile.EmailClientSettings.Recipients = "a@local, b@local, c@local";

            action.ProcessJob(_job);

            var mail = _mockMailClient.Mails[0];
            Assert.AreEqual(new[] { "a@local", "b@local", "c@local" }, mail.Recipients.Select(r => r.Address));
        }

        [Test]
        public void EmailClientAction_WithMultipleRecipientsSeperatedBySemicolons_AllRecipientsListedInMail()
        {
            var action = BuildAction();
            _profile.EmailClientSettings.Recipients = "a@local; b@local; c@local";

            action.ProcessJob(_job);

            var mail = _mockMailClient.Mails[0];
            Assert.AreEqual(new[] { "a@local", "b@local", "c@local" }, mail.Recipients.Select(r => r.Address));
        }

        [Test]
        public void EmailClientAction_WithMultipleRecipientsInCcAndBcc_AllRecipientsListedInMail()
        {
            var action = BuildAction();
            _profile.EmailClientSettings.Recipients = "a@local;";
            _profile.EmailClientSettings.RecipientsCc = "b@local";
            _profile.EmailClientSettings.RecipientsBcc = ";c@local";

            action.ProcessJob(_job);

            var mail = _mockMailClient.Mails[0];
            var formattedRecipients = mail.Recipients.Select(r => r.Type + ":" + r.Address);
            Assert.AreEqual(new[] { "To:a@local", "Cc:b@local", "Bcc:c@local" }, formattedRecipients);
        }

        [Test]
        public void EmailClientAction_WithoutSignature_MailBodyDoesNotContainSignature()
        {
            var action = BuildAction();
            _profile.EmailClientSettings.AddSignature = false;

            action.ProcessJob(_job);

            Assert.IsFalse(_mockMailClient.Mails[0].Body.Contains(SignatureText));
        }

        [Test]
        public void EmailClientAction_WithSignature_MailBodyContainsSignature()
        {
            var action = BuildAction();
            _profile.EmailClientSettings.AddSignature = true;

            action.ProcessJob(_job);

            Assert.IsTrue(_mockMailClient.Mails[0].Body.Contains(SignatureText));
        }

        [Test]
        public void EmailClientAction_WithSimpleJob_EmailIsProcessedByClient()
        {
            var action = BuildAction();

            action.ProcessJob(_job);

            Assert.IsNotEmpty(_mockMailClient.Mails);
        }

        [Test]
        public void EmailClientAction_WithSubject_MailContainsSubject()
        {
            var action = BuildAction();
            _profile.EmailClientSettings.Subject = "my subject";

            action.ProcessJob(_job);

            Assert.AreEqual(_mockMailClient.Mails[0].Subject, _profile.EmailClientSettings.Subject);
        }

        [Test]
        public void EmailClientAction_AttachesFiles()
        {
            var action = BuildAction();

            _profile.EmailClientSettings.Subject = "my subject";

            action.ProcessJob(_job);

            Assert.AreEqual(1, _mockMailClient.Mails[0].Attachments.Count);
        }

        [Test]
        public void EmailClientAction_DropboxShareLinks_DoesNotAttachFiles()
        {
            var action = BuildAction();
            _profile.DropboxSettings.Enabled = true;
            _profile.DropboxSettings.CreateShareLink = true;

            _profile.EmailClientSettings.Subject = "my subject";
            _profile.EmailClientSettings.Content = "<DropboxFullLinks>";

            action.ProcessJob(_job);

            Assert.AreEqual(0, _mockMailClient.Mails[0].Attachments.Count);
        }

        [Test]
        public void EmailClientAction_DropboxEnabledWithoutShareLinks_AttachesFiles()
        {
            var action = BuildAction();
            _profile.DropboxSettings.Enabled = true;
            _profile.DropboxSettings.CreateShareLink = false;

            _profile.EmailClientSettings.Subject = "my subject";
            _profile.EmailClientSettings.Content = "<DropboxFullLinks>";

            action.ProcessJob(_job);

            Assert.AreEqual(1, _mockMailClient.Mails[0].Attachments.Count);
        }

        [Test]
        public void EmailClientAction_DropboxEnabledWithoutToken_AttachesFiles()
        {
            var action = BuildAction();
            _profile.DropboxSettings.Enabled = true;
            _profile.DropboxSettings.CreateShareLink = true;

            _profile.EmailClientSettings.Subject = "my subject";

            action.ProcessJob(_job);

            Assert.AreEqual(1, _mockMailClient.Mails[0].Attachments.Count);
        }

        [Test]
        public void CheckEmailClientInstalled_ClientCanBeCreated_ReturnsTrue()
        {
            var action = BuildAction();

            Assert.IsTrue(action.CheckEmailClientInstalled());
        }

        [Test]
        public void CheckEmailClientInstalled_ClientCannotBeCreated_ReturnsFalse()
        {
            _emailClientFactory.CreateEmailClient().Returns(x => null);
            var action = BuildAction();

            Assert.IsFalse(action.CheckEmailClientInstalled());
        }

        [Test]
        public void IsEnabled_WhenEnabled_ReturnsTrue()
        {
            var action = BuildAction();
            _profile.EmailClientSettings.Enabled = true;

            Assert.IsTrue(action.IsEnabled(_profile));
        }

        [Test]
        public void IsEnabled_WhenNotEnabled_ReturnsFalse()
        {
            var action = BuildAction();
            _profile.EmailClientSettings.Enabled = false;

            Assert.IsFalse(action.IsEnabled(_profile));
        }
    }

    internal class MockMailClient : IEmailClient
    {
        /// <summary>
        ///     If true, the result of ShowEmailClient will be false.
        /// </summary>
        public bool WillFail { get; set; }

        /// <summary>
        ///     If not null, this Exception will be thrown during ShowEmailClient
        /// </summary>
        public Exception ExceptionThrown { get; set; }

        /// <summary>
        ///     List of mails that were "sent"
        /// </summary>
        public IList<Email> Mails { get; } = new List<Email>();

        public bool ShowEmailClient(Email email)
        {
            Mails.Add(email);

            if (ExceptionThrown != null)
                throw ExceptionThrown;

            return !WillFail;
        }

        public bool IsClientInstalled { get; set; } = true;
    }
}
