using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Utilities.Tokens;
using System;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Jobs.Actions
{
    [TestFixture]
    public class SmtpActionTest
    {
        private SmtpMailAction _smtpAction;
        private ConversionProfile _profile;
        private Accounts _accounts;
        private SmtpAccount _smtpTestAccount;
        private IMailSignatureHelper _mailSignatureHelper;

        [SetUp]
        public void SetUp()
        {
            _smtpTestAccount = new SmtpAccount();
            _smtpTestAccount.AccountId = "TestAccountId";
            _smtpTestAccount.Address = "randomAdress@randomHost.random";
            _smtpTestAccount.UserName = "randomUsername";
            _smtpTestAccount.Server = "randomHost";
            _smtpTestAccount.Port = 25;
            _smtpTestAccount.Ssl = false;
            _smtpTestAccount.Password = "1234";

            _profile = new ConversionProfile();
            _profile.EmailSmtpSettings.Enabled = true;
            _profile.EmailSmtpSettings.AccountId = _smtpTestAccount.AccountId;
            _profile.EmailSmtpSettings.Recipients = "randomRecipient@RandomHost.random";
            _profile.EmailSmtpSettings.Subject = "";
            _profile.EmailSmtpSettings.Content = "";
            _profile.EmailSmtpSettings.AddSignature = false;

            _accounts = new Accounts();
            _accounts.SmtpAccounts.Add(_smtpTestAccount);

            _mailSignatureHelper = Substitute.For<IMailSignatureHelper>();
            _mailSignatureHelper.ComposeMailSignature().Returns("Signature!");

            _smtpAction = new SmtpMailAction(_mailSignatureHelper);
        }

        [Test]
        public void IsEnabled_ReturnsProfileSmtpEnabled()
        {
            _profile.EmailSmtpSettings.Enabled = true;
            Assert.IsTrue(_smtpAction.IsEnabled(_profile));

            _profile.EmailSmtpSettings.Enabled = false;
            Assert.IsFalse(_smtpAction.IsEnabled(_profile));
        }

        [Test]
        public void ApplyTokens_ReplacesTokensForAllRecipientsAndSwitchesSemiColonTo()
        {
            var token = "<Token>";
            var tokenKey = "Token";
            var tokenValue = "TokenValue";
            var tokenReplacer = new TokenReplacer();
            tokenReplacer.AddStringToken(tokenKey, tokenValue);
            var givenRecipients = $"{token};{token}, {token}";
            var expectedRecipients = $"{tokenValue},{tokenValue}, {tokenValue}";
            _profile.EmailSmtpSettings.Recipients = givenRecipients;
            _profile.EmailSmtpSettings.RecipientsCc = givenRecipients;
            _profile.EmailSmtpSettings.RecipientsBcc = givenRecipients;
            var job = new Job(null, _profile, _accounts);
            job.TokenReplacer = tokenReplacer;

            _smtpAction.ApplyPreSpecifiedTokens(job);

            Assert.AreEqual(expectedRecipients, _profile.EmailSmtpSettings.Recipients, "Recipients");
            Assert.AreEqual(expectedRecipients, _profile.EmailSmtpSettings.RecipientsCc, "RecipientsCc");
            Assert.AreEqual(expectedRecipients, _profile.EmailSmtpSettings.RecipientsBcc, "RecipientsBcc");
        }

        [Test]
        public void Check_ValidSettings_ReturnsTrue()
        {
            _profile.AutoSave.Enabled = false;

            var result = _smtpAction.Check(_profile, _accounts, CheckLevel.Profile);

            Assert.IsTrue(result, "Unexpected errorcodes: " + result);
        }

        [Test]
        public void Check_ValidAutoSaveSettings_ReturnsTrue()
        {
            _profile.AutoSave.Enabled = true;

            var result = _smtpAction.Check(_profile, _accounts, CheckLevel.Profile);

            Assert.IsTrue(result, "Unexpected errorcodes: " + result);
        }

        [Test]
        public void Check_NoAccount_ResultContainsAssociatedCode()
        {
            _profile.EmailSmtpSettings.AccountId = "Some unavailable Account ID";

            var result = _smtpAction.Check(_profile, _accounts, CheckLevel.Profile);

            Assert.Contains(ErrorCode.Smtp_NoAccount, result, "Did not detect missing SMTP Account");
            result.Remove(ErrorCode.Smtp_NoAccount);
            Assert.IsTrue(result, "Unexpected errorcodes: " + result);
        }

        [Test]
        public void Check_NoAdress_ResultContainsAssociatedCode()
        {
            _smtpTestAccount.Address = "";

            var result = _smtpAction.Check(_profile, _accounts, CheckLevel.Profile);

            Assert.Contains(ErrorCode.Smtp_NoEmailAddress, result, "Did not detect missing SMTP adress.");
            result.Remove(ErrorCode.Smtp_NoEmailAddress);
            Assert.IsTrue(result, "Unexpected errorcodes: " + result);
        }

        [Test]
        public void Check_NoServer_ResultContainsAssociatedCode()
        {
            _smtpTestAccount.Server = "";

            var result = _smtpAction.Check(_profile, _accounts, CheckLevel.Profile);

            Assert.Contains(ErrorCode.Smtp_NoServerSpecified, result, "Did not detect missing Server.");
            result.Remove(ErrorCode.Smtp_NoServerSpecified);
            Assert.IsTrue(result, "Unexpected errorcodes: " + result);
        }

        [Test]
        public void Check_InvalidPort_ResultContainsAssociatedCode()
        {
            _smtpTestAccount.Port = -1;

            var result = _smtpAction.Check(_profile, _accounts, CheckLevel.Profile);

            Assert.Contains(ErrorCode.Smtp_InvalidPort, result, "Did not detect invalid SMTP port.");
            result.Remove(ErrorCode.Smtp_InvalidPort);
            Assert.IsTrue(result, "Unexpected errorcodes: " + result);
        }

        [Test]
        public void Check_NoUserName_ResultContainsAssociatedCode()
        {
            _smtpTestAccount.UserName = "";

            var result = _smtpAction.Check(_profile, _accounts, CheckLevel.Profile);

            Assert.Contains(ErrorCode.Smtp_NoUserSpecified, result, "Did not detect missing SMTP UserName.");
            result.Remove(ErrorCode.Smtp_NoUserSpecified);
            Assert.IsTrue(result, "Unexpected errorcodes: " + result);
        }

        [Test]
        public void Check_NoRecipients_ResultContainsAssociatedCode()
        {
            _profile.EmailSmtpSettings.Recipients = "";

            var result = _smtpAction.Check(_profile, _accounts, CheckLevel.Profile);

            Assert.Contains(ErrorCode.Smtp_NoRecipients, result, "Did not detect missing SMTP Recipients.");
            result.Remove(ErrorCode.Smtp_NoRecipients);
            Assert.IsTrue(result, "Unexpected errorcodes: " + result);
        }

        [Test]
        public void Check_NoAutoSave_NoPasswords_ResultIsTrue()
        {
            _profile.AutoSave.Enabled = false;
            _smtpTestAccount.Password = "";

            var result = _smtpAction.Check(_profile, _accounts, CheckLevel.Profile);

            Assert.IsTrue(result, "Unexpected errorcodes: " + result);
        }

        [Test]
        public void Check_AutoSave_NoPasswords_ResultContainsAssociatedCode()
        {
            _profile.AutoSave.Enabled = true;
            _smtpTestAccount.Password = "";

            var result = _smtpAction.Check(_profile, _accounts, CheckLevel.Profile);

            Assert.Contains(ErrorCode.Smtp_NoPasswordSpecified, result, "CheckJob did not detect missing SMTP password for autosaving.");
            result.Remove(ErrorCode.Smtp_NoPasswordSpecified);
            Assert.IsTrue(result, "Unexpected errorcodes: " + result);
        }

        [Test]
        public void Check_MultipleErrors()
        {
            _profile.AutoSave.Enabled = false;

            _smtpTestAccount.Address = "";
            _smtpTestAccount.UserName = "";
            _smtpTestAccount.Server = "";
            _smtpTestAccount.Port = -1;
            _smtpTestAccount.Password = ""; //Should be ignored when autosave is disabled

            _profile.EmailSmtpSettings.Recipients = "";

            var result = _smtpAction.Check(_profile, _accounts, CheckLevel.Profile);
            Assert.Contains(ErrorCode.Smtp_NoEmailAddress, result, "CheckJob did not detect missing SMTP adress.");
            result.Remove(ErrorCode.Smtp_NoEmailAddress);
            Assert.Contains(ErrorCode.Smtp_NoServerSpecified, result, "CheckJob did not detect missing SMTP Server.");
            result.Remove(ErrorCode.Smtp_NoServerSpecified);
            Assert.Contains(ErrorCode.Smtp_InvalidPort, result, "CheckJob did not detect invalid SMTP port.");
            result.Remove(ErrorCode.Smtp_InvalidPort);
            Assert.Contains(ErrorCode.Smtp_NoUserSpecified, result, "CheckJob did not detect missing SMTP username.");
            result.Remove(ErrorCode.Smtp_NoUserSpecified);
            Assert.Contains(ErrorCode.Smtp_NoRecipients, result, "CheckJob did not detect missing SMTP recipients.");
            result.Remove(ErrorCode.Smtp_NoRecipients);
            Assert.IsTrue(result, "Unexpected errorcodes: " + result);
        }

        [Test]
        public void Check_AutoSave_MultipleErrors()
        {
            _profile.AutoSave.Enabled = true;

            _smtpTestAccount.Address = "";
            _smtpTestAccount.UserName = "";
            _smtpTestAccount.Server = "";
            _smtpTestAccount.Port = -1;
            _smtpTestAccount.Password = "";

            _profile.EmailSmtpSettings.Recipients = "";

            var result = _smtpAction.Check(_profile, _accounts, CheckLevel.Profile);
            Assert.Contains(ErrorCode.Smtp_NoEmailAddress, result, "CheckJob did not detect missing SMTP adress.");
            result.Remove(ErrorCode.Smtp_NoEmailAddress);
            Assert.Contains(ErrorCode.Smtp_NoServerSpecified, result, "CheckJob did not detect missing SMTP Server.");
            result.Remove(ErrorCode.Smtp_NoServerSpecified);
            Assert.Contains(ErrorCode.Smtp_InvalidPort, result, "CheckJob did not detect invalid SMTP port.");
            result.Remove(ErrorCode.Smtp_InvalidPort);
            Assert.Contains(ErrorCode.Smtp_NoUserSpecified, result, "CheckJob did not detect missing SMTP username.");
            result.Remove(ErrorCode.Smtp_NoUserSpecified);
            Assert.Contains(ErrorCode.Smtp_NoRecipients, result, "CheckJob did not detect missing SMTP recipients.");
            result.Remove(ErrorCode.Smtp_NoRecipients);
            Assert.Contains(ErrorCode.Smtp_NoPasswordSpecified, result, "CheckJob did not detect missing SMTP password for autosaving.");
            result.Remove(ErrorCode.Smtp_NoPasswordSpecified);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }
    }
}
