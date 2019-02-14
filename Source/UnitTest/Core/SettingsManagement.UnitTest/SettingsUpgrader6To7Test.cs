using NUnit.Framework;
using pdfforge.DataStorage;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.SettingsManagement;
using System.Linq;

namespace pdfforge.PDFCreator.UnitTest.Core.SettingsManagement
{
    [TestFixture]
    internal class SettingsUpgrader6To7Test
    {
        private const int NumDefaultServers = 3;

        private Data CreateV6Storage()
        {
            var data = Data.CreateDataStorage();
            data.SetValue(@"ApplicationProperties\SettingsVersion", "6");
            data.SetValue(@"ApplicationSettings\TitleReplacement\numClasses", "0");

            return data;
        }

        private PdfCreatorSettings CreateSettings(Data data)
        {
            var settings = new PdfCreatorSettings();
            settings.ReadValues(data, "");
            return settings;
        }

        [Test]
        public void DataWithVersion6_UpgradeRequiredToVersion7_ReturnsTrue()
        {
            var data = CreateV6Storage();
            var upgrader = new CreatorSettingsUpgrader(data);

            Assert.IsTrue(upgrader.RequiresUpgrade(7));
        }

        [Test]
        public void DataWithVersion6_UpgradeToVersion7_SetsVersionTo7()
        {
            var data = CreateV6Storage();
            var upgrader = new CreatorSettingsUpgrader(data);

            upgrader.Upgrade(7);

            Assert.AreEqual("7", data.GetValue(@"ApplicationProperties\SettingsVersion"));
        }

        [Test]
        public void WithAutoSave_ExtractsTargetDirectory()
        {
            var expectedDirectory = "abc";
            var data = CreateV6Storage();
            data.SetValue(@"ConversionProfiles\numClasses", "1");
            data.SetValue(@"ConversionProfiles\0\AutoSave\Enabled", "True");
            data.SetValue(@"ConversionProfiles\0\AutoSave\TargetDirectory", expectedDirectory);

            var upgrader = new CreatorSettingsUpgrader(data);

            upgrader.Upgrade(7);

            var settings = CreateSettings(data);
            Assert.AreEqual(expectedDirectory, settings.ConversionProfiles[0].TargetDirectory);
        }

        [Test]
        public void WithInteractive_ExtractsDefaultFolder()
        {
            var expectedDirectory = "abc";
            var data = CreateV6Storage();
            data.SetValue(@"ConversionProfiles\numClasses", "1");
            data.SetValue(@"ConversionProfiles\0\AutoSave\Enabled", "False");
            data.SetValue(@"ConversionProfiles\0\SaveDialog\Folder", expectedDirectory);

            var upgrader = new CreatorSettingsUpgrader(data);

            upgrader.Upgrade(7);

            var settings = CreateSettings(data);
            Assert.AreEqual(expectedDirectory, settings.ConversionProfiles[0].TargetDirectory);
        }

        [Test]
        public void WithAutoSaveDisAbled_DoesNotExtractTargetDirectory()
        {
            var data = CreateV6Storage();
            data.SetValue(@"ConversionProfiles\numClasses", "1");
            data.SetValue(@"ConversionProfiles\0\AutoSave\Enabled", "False");
            data.SetValue(@"ConversionProfiles\0\AutoSave\TargetDirectory", "abc");

            var upgrader = new CreatorSettingsUpgrader(data);

            upgrader.Upgrade(7);

            var settings = CreateSettings(data);
            Assert.AreEqual("", settings.ConversionProfiles[0].TargetDirectory);
        }

        private void StoreTimeServerAccount(Data data, int profileId, TimeServerAccount account)
        {
            data.SetValue(@"ConversionProfiles\numClasses", (profileId + 1).ToString());
            data.SetValue($@"ConversionProfiles\{profileId}\PdfSettings\Signature\Enabled", "True");
            data.SetValue($@"ConversionProfiles\{profileId}\PdfSettings\Signature\TimeServerIsSecured", account.IsSecured.ToString());
            data.SetValue($@"ConversionProfiles\{profileId}\PdfSettings\Signature\TimeServerLoginName", account.UserName);
            data.SetValue($@"ConversionProfiles\{profileId}\PdfSettings\Signature\TimeServerPassword", Data.Encrypt(account.Password));
            data.SetValue($@"ConversionProfiles\{profileId}\PdfSettings\Signature\TimeServerUrl", account.Url);
        }

        private void AssertTimeServersAreEqual(TimeServerAccount first, TimeServerAccount second)
        {
            Assert.AreEqual(first.IsSecured, second.IsSecured);
            Assert.AreEqual(first.UserName, second.UserName);
            Assert.AreEqual(first.Password, second.Password);
            Assert.AreEqual(first.Url, second.Url);
        }

        [Test]
        public void WithTimeServer_ExtractsTimeserverAccount()
        {
            var account = new TimeServerAccount
            {
                IsSecured = true,
                UserName = "tsuser",
                Password = "test",
                Url = "http://mytimeserver.local",
            };

            var data = CreateV6Storage();
            StoreTimeServerAccount(data, 0, account);

            var upgrader = new CreatorSettingsUpgrader(data);

            upgrader.Upgrade(7);

            var settings = CreateSettings(data);
            Assert.IsNotEmpty(settings.ApplicationSettings.Accounts.TimeServerAccounts);
            Assert.IsNotNull(settings.ConversionProfiles[0].PdfSettings.Signature.TimeServerAccountId);
            Assert.IsNotEmpty(settings.ConversionProfiles[0].PdfSettings.Signature.TimeServerAccountId);

            var storedAccount = settings.ApplicationSettings.Accounts.TimeServerAccounts.First(a => a.Url == account.Url);
            AssertTimeServersAreEqual(account, storedAccount);
        }

        [Test]
        public void WithSameTimeServerTwice_ExtractsSingleTimeserverAccount()
        {
            var account = new TimeServerAccount
            {
                IsSecured = true,
                UserName = "tsuser",
                Password = "test",
                Url = "http://mytimeserver.local",
            };
            var data = CreateV6Storage();
            data.SetValue(@"ConversionProfiles\numClasses", "2");
            StoreTimeServerAccount(data, 0, account);
            StoreTimeServerAccount(data, 1, account);

            var upgrader = new CreatorSettingsUpgrader(data);

            upgrader.Upgrade(7);

            var settings = CreateSettings(data);
            var expectedNumberofProfiles = NumDefaultServers + 1;
            Assert.AreEqual(expectedNumberofProfiles, settings.ApplicationSettings.Accounts.TimeServerAccounts.Count);
            Assert.IsNotNull(settings.ConversionProfiles[0].PdfSettings.Signature.TimeServerAccountId);
            Assert.IsNotEmpty(settings.ConversionProfiles[0].PdfSettings.Signature.TimeServerAccountId);

            Assert.IsNotNull(settings.ConversionProfiles[1].PdfSettings.Signature.TimeServerAccountId);
            Assert.IsNotEmpty(settings.ConversionProfiles[1].PdfSettings.Signature.TimeServerAccountId);
        }

        [Test]
        public void WithTwoTimeServers_ExtractsBothTimeserverAccount()
        {
            var firstAccount = new TimeServerAccount
            {
                IsSecured = true,
                UserName = "tsuser1",
                Password = "test1",
                Url = "http://mytimeserver1.local",
            };
            var secondAccount = new TimeServerAccount
            {
                IsSecured = true,
                UserName = "tsuser2",
                Password = "test2",
                Url = "http://mytimeserver2.local",
            };

            var data = CreateV6Storage();
            data.SetValue(@"ConversionProfiles\numClasses", "2");
            StoreTimeServerAccount(data, 0, firstAccount);
            StoreTimeServerAccount(data, 1, secondAccount);

            var upgrader = new CreatorSettingsUpgrader(data);

            upgrader.Upgrade(7);

            var settings = CreateSettings(data);
            var expectedNumberofProfiles = NumDefaultServers + 2;
            Assert.AreEqual(expectedNumberofProfiles, settings.ApplicationSettings.Accounts.TimeServerAccounts.Count);
            Assert.IsNotNull(settings.ConversionProfiles[0].PdfSettings.Signature.TimeServerAccountId);
            Assert.IsNotEmpty(settings.ConversionProfiles[0].PdfSettings.Signature.TimeServerAccountId);

            Assert.IsNotNull(settings.ConversionProfiles[1].PdfSettings.Signature.TimeServerAccountId);
            Assert.IsNotEmpty(settings.ConversionProfiles[1].PdfSettings.Signature.TimeServerAccountId);

            AssertTimeServersAreEqual(firstAccount, settings.ApplicationSettings.Accounts.TimeServerAccounts.First(a => a.Url == firstAccount.Url));
            AssertTimeServersAreEqual(secondAccount, settings.ApplicationSettings.Accounts.TimeServerAccounts.First(a => a.Url == secondAccount.Url));
        }

        private void StoreFtpAccount(Data data, int profileId, FtpAccount account)
        {
            data.SetValue(@"ConversionProfiles\numClasses", (profileId + 1).ToString());
            data.SetValue($@"ConversionProfiles\{profileId}\Ftp\Enabled", "True");
            data.SetValue($@"ConversionProfiles\{profileId}\Ftp\UserName", account.UserName);
            data.SetValue($@"ConversionProfiles\{profileId}\Ftp\Password", Data.Encrypt(account.Password));
            data.SetValue($@"ConversionProfiles\{profileId}\Ftp\Server", account.Server);
        }

        private void AssertFtpAccountsAreEqual(FtpAccount first, FtpAccount second)
        {
            Assert.AreEqual(first.UserName, second.UserName);
            Assert.AreEqual(first.Password, second.Password);
            Assert.AreEqual(first.Server, second.Server);
        }

        [Test]
        public void WithFtp_ExtractsFtpAccount()
        {
            var account = new FtpAccount
            {
                UserName = "ftpuser",
                Password = "ftppass",
                Server = "ftp.local"
            };
            var data = CreateV6Storage();
            StoreFtpAccount(data, 0, account);

            var upgrader = new CreatorSettingsUpgrader(data);

            upgrader.Upgrade(7);

            var settings = CreateSettings(data);
            Assert.IsNotEmpty(settings.ApplicationSettings.Accounts.FtpAccounts);
            Assert.IsNotNull(settings.ConversionProfiles[0].Ftp.AccountId);
            Assert.IsNotEmpty(settings.ConversionProfiles[0].Ftp.AccountId);
            AssertFtpAccountsAreEqual(account, settings.ApplicationSettings.Accounts.FtpAccounts[0]);
        }

        private void StoreSmtpAccount(Data data, int profileId, SmtpAccount account)
        {
            data.SetValue(@"ConversionProfiles\numClasses", (profileId + 1).ToString());
            data.SetValue($@"ConversionProfiles\{profileId}\EmailSmtpSettings\Enabled", "True");
            data.SetValue($@"ConversionProfiles\{profileId}\EmailSmtpSettings\Address", account.Address);
            data.SetValue($@"ConversionProfiles\{profileId}\EmailSmtpSettings\Password", Data.Encrypt(account.Password));
            data.SetValue($@"ConversionProfiles\{profileId}\EmailSmtpSettings\Port", account.Port.ToString());
            data.SetValue($@"ConversionProfiles\{profileId}\EmailSmtpSettings\Server", account.Server);
            data.SetValue($@"ConversionProfiles\{profileId}\EmailSmtpSettings\Ssl", account.Ssl.ToString());
            data.SetValue($@"ConversionProfiles\{profileId}\EmailSmtpSettings\UserName", account.UserName);
        }

        private void AssertSmtpAccountsAreEqual(SmtpAccount first, SmtpAccount second)
        {
            Assert.AreEqual(first.UserName, second.UserName);
            Assert.AreEqual(first.Password, second.Password);
            Assert.AreEqual(first.Server, second.Server);
            Assert.AreEqual(first.Port, second.Port);
        }

        [Test]
        public void WithSmtp_ExtractsSmtpAccount()
        {
            var account = new SmtpAccount
            {
                Address = "test@email.de",
                Password = "smtppass",
                Port = 123,
                Server = "smtp.local",
                Ssl = true,
                UserName = "smtpuser"
            };

            var data = CreateV6Storage();
            StoreSmtpAccount(data, 0, account);

            var upgrader = new CreatorSettingsUpgrader(data);

            upgrader.Upgrade(7);

            var settings = CreateSettings(data);
            Assert.IsNotEmpty(settings.ApplicationSettings.Accounts.SmtpAccounts);
            Assert.IsNotNull(settings.ConversionProfiles[0].EmailSmtpSettings.AccountId);
            Assert.IsNotEmpty(settings.ConversionProfiles[0].EmailSmtpSettings.AccountId);
            AssertSmtpAccountsAreEqual(account, settings.ApplicationSettings.Accounts.SmtpAccounts[0]);
        }

        [Test]
        public void TitleReplacement_StartWithEmptyReplace_RemainsUnchanged()
        {
            var data = CreateV6Storage();
            data.SetValue(@"ApplicationSettings\TitleReplacement\numClasses", "1");
            data.SetValue(@"ApplicationSettings\TitleReplacement\0\ReplacementType", "Start");
            data.SetValue(@"ApplicationSettings\TitleReplacement\0\Search", "123");
            data.SetValue(@"ApplicationSettings\TitleReplacement\0\Replace", "");

            var upgrader = new CreatorSettingsUpgrader(data);

            upgrader.Upgrade(7);

            var settings = CreateSettings(data);
            Assert.IsNotEmpty(settings.ApplicationSettings.TitleReplacement);
            Assert.AreEqual(ReplacementType.Start, settings.ApplicationSettings.TitleReplacement.First().ReplacementType);
            Assert.AreEqual("123", settings.ApplicationSettings.TitleReplacement.First().Search);
        }

        [Test]
        public void TitleReplacement_StartWithReplace_ConvertsToRegEx()
        {
            var data = CreateV6Storage();
            data.SetValue(@"ApplicationSettings\TitleReplacement\numClasses", "1");
            data.SetValue(@"ApplicationSettings\TitleReplacement\0\ReplacementType", "Start");
            data.SetValue(@"ApplicationSettings\TitleReplacement\0\Search", "123");
            data.SetValue(@"ApplicationSettings\TitleReplacement\0\Replace", "xyz");

            var upgrader = new CreatorSettingsUpgrader(data);

            upgrader.Upgrade(7);

            var settings = CreateSettings(data);
            Assert.IsNotEmpty(settings.ApplicationSettings.TitleReplacement);
            Assert.AreEqual(ReplacementType.RegEx, settings.ApplicationSettings.TitleReplacement.First().ReplacementType);
            Assert.AreEqual("123", settings.ApplicationSettings.TitleReplacement.First().Search);
            Assert.AreEqual("xyz", settings.ApplicationSettings.TitleReplacement.First().Replace);
        }

        [Test]
        public void TitleReplacement_StartWithReplace_ConvertsToRegExWithEscapedChars()
        {
            var data = CreateV6Storage();
            data.SetValue(@"ApplicationSettings\TitleReplacement\numClasses", "1");
            data.SetValue(@"ApplicationSettings\TitleReplacement\0\ReplacementType", "Start");
            data.SetValue(@"ApplicationSettings\TitleReplacement\0\Search", "123");
            data.SetValue(@"ApplicationSettings\TitleReplacement\0\Replace", "x.y*z");

            var upgrader = new CreatorSettingsUpgrader(data);

            upgrader.Upgrade(7);

            var settings = CreateSettings(data);
            Assert.IsNotEmpty(settings.ApplicationSettings.TitleReplacement);
            Assert.AreEqual(ReplacementType.RegEx, settings.ApplicationSettings.TitleReplacement.First().ReplacementType);
            Assert.AreEqual("123", settings.ApplicationSettings.TitleReplacement.First().Search);
            Assert.AreEqual(@"x\.y\*z", settings.ApplicationSettings.TitleReplacement.First().Replace);
        }
    }
}
