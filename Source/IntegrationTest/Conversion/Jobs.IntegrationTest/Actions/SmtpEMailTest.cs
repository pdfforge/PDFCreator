using NSubstitute;
using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Utilities.Tokens;
using Ploeh.AutoFixture;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Toolbelt.Net.Smtp;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.Jobs.Actions
{
    [TestFixture]
    internal class SmtpEmailTest
    {
        private TestHelper _th;
        private TokenReplacer _tokenReplacer;

        private SmtpAccount _smtpAccount;
        private Accounts _accounts;
        private string _mailServer;
        private string _userName;
        private readonly string _eMailAddress = "someone@localhost.local";
        private string _smtpPassword;
        private int _smtpPort;
        private SmtpServerForUnitTest _smtpServer;
        private IMailSignatureHelper _mailSignatureHelper;

        [SetUp]
        public void SetUp()
        {
            var fixture = new Fixture();

            _userName = fixture.Create<string>();
            _smtpPassword = fixture.Create<string>();

            _mailSignatureHelper = Substitute.For<IMailSignatureHelper>();
            _mailSignatureHelper.ComposeMailSignature().Returns("");

            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder("SmtpEmailTest");

            _tokenReplacer = new TokenReplacer();
            _tokenReplacer.AddStringToken("ReplaceThis", "Replaced");

            var serverIp = IPAddress.Loopback;
            _smtpPort = FindFreeTcpPort();
            _mailServer = serverIp.ToString();
            _smtpServer = new SmtpServerForUnitTest(serverIp, _smtpPort, new[] { new NetworkCredential(_userName, _smtpPassword) });
            _smtpServer.Start();
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();

            _smtpServer?.Dispose();
        }

        private int FindFreeTcpPort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

        private void InitTest()
        {
            _smtpAccount = new SmtpAccount();
            _smtpAccount.AccountId = "SmtpTestID";
            _smtpAccount.UserName = _userName;
            _smtpAccount.Address = _eMailAddress;
            _smtpAccount.Server = _mailServer;
            _smtpAccount.Port = _smtpPort;
            _smtpAccount.Ssl = false;

            _th.Profile.EmailSmtpSettings.Enabled = true;
            _th.Profile.EmailSmtpSettings.AccountId = _smtpAccount.AccountId;
            _th.Profile.EmailSmtpSettings.Recipients = _eMailAddress + "," + _eMailAddress;
            _th.Profile.EmailSmtpSettings.Subject = "Title <ReplaceThis>";
            _th.Profile.EmailSmtpSettings.Content = "Content <ReplaceThis>";
            _th.Profile.EmailSmtpSettings.AddSignature = false;

            _accounts = new Accounts();
            _accounts.SmtpAccounts.Add(_smtpAccount);
        }

        private void TestSmtpEmail()
        {
            var mail = _smtpServer.ReceivedMessages.FirstOrDefault();

            Assert.IsNotNull(mail, "First inbox mail does not exist");

            //Check Sender
            Assert.AreEqual(_eMailAddress, mail.From.Address, "Wrong sender");
            //Check recipients
            var recipients = new List<string>(_th.Profile.EmailSmtpSettings.Recipients.Trim().Replace(";", ",").Split(','));
            var recipientsMail = new List<string>();
            foreach (var address in mail.To)
                recipientsMail.Add(address.ToString());
            Assert.AreEqual(recipients, recipientsMail, "Not all the recipients were added in mail");
            //Check Subject
            Assert.AreEqual(_th.Job.TokenReplacer.ReplaceTokens(_th.Profile.EmailSmtpSettings.Subject), mail.Subject, "Incorrect mail-subject");

            var contentType = _th.Profile.EmailSmtpSettings.Html
                ? "text/html"
                : "text/plain";

            Assert.IsTrue(mail.Data.Any(s => s.StartsWith($"Content-Type: {contentType};", StringComparison.InvariantCultureIgnoreCase)), $"The content-type was not {contentType}");

            var content = _th.Job.TokenReplacer.ReplaceTokens(_th.Profile.EmailSmtpSettings.Content);
            Assert.IsTrue(mail.Body.Contains(content), "Mail body does not contain content string");
            //Check attachments
            Assert.AreEqual(_th.Job.OutputFiles.Count, mail.Attachments.Length, "Incorrect number of attached files");
            for (var j = 0; j < _th.Job.OutputFiles.Count; j++)
            {
                var file = mail.Attachments[j];
                Assert.AreEqual(file.Name, Path.GetFileName(_th.Job.OutputFiles[j]), "Name of " + _th.Job.OutputFiles[j] + " has changed.");

                Assert.AreEqual(File.ReadAllBytes(_th.Job.OutputFiles[j]), file.ContentBytes, "Data of " + _th.Job.OutputFiles[j] + " has changed");
            }
        }

        [Test]
        public void TestWith3FilesAttached()
        {
            InitTest();

            _th.GenerateGsJob_WithSetOutput(TestFile.PDFCreatorTestpage_GS9_19_PDF);
            _th.Job.OutputFiles.Add(_th.Job.OutputFiles[0]);
            _th.Job.OutputFiles.Add(_th.Job.OutputFiles[0]);
            _th.Job.Passwords.SmtpPassword = _smtpPassword;
            _th.Job.TokenReplacer = _tokenReplacer;
            _th.Job.Accounts = _accounts;

            var smtpAction = new SmtpMailAction(_mailSignatureHelper);

            smtpAction.ProcessJob(_th.Job);

            TestSmtpEmail();
        }

        [Test]
        public void TestWith1FileAttachedWithHtml()
        {
            InitTest();

            _th.GenerateGsJob_WithSetOutput(TestFile.PDFCreatorTestpage_GS9_19_PDF);
            _th.Profile.EmailSmtpSettings.Html = true;
            _th.Job.Passwords.SmtpPassword = _smtpPassword;
            _th.Job.TokenReplacer = _tokenReplacer;
            _th.Job.Accounts = _accounts;

            var smtpAction = new SmtpMailAction(_mailSignatureHelper);

            smtpAction.ProcessJob(_th.Job);

            TestSmtpEmail();
        }
    }
}
