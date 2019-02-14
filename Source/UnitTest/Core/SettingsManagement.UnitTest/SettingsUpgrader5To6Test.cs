using NUnit.Framework;
using pdfforge.DataStorage;
using pdfforge.PDFCreator.Core.SettingsManagement;

namespace pdfforge.PDFCreator.UnitTest.Core.SettingsManagement
{
    [TestFixture]
    internal partial class SettingsUpgrader5To6Test
    {
        [Test]
        public void DataWithVersion5_UpgradeRequiredToVersion6_ReturnsTrue()
        {
            var data = Data.CreateDataStorage();
            data.SetValue(@"ApplicationProperties\SettingsVersion", "5");

            var upgrader = new CreatorSettingsUpgrader(data);

            Assert.IsTrue(upgrader.RequiresUpgrade(6));
        }

        [Test]
        public void DataWithVersion5_UpgradeToVersion6_SetsVersionTo6()
        {
            var data = Data.CreateDataStorage();
            data.SetValue(@"ApplicationProperties\SettingsVersion", "5");
            var upgrader = new CreatorSettingsUpgrader(data);

            upgrader.Upgrade(6);

            Assert.AreEqual("6", data.GetValue(@"ApplicationProperties\SettingsVersion"));
        }

        [Test]
        public void Test_MoveEMailClientSectionToEmailClientSettings()
        {
            var data = Data.CreateDataStorage();
            data.SetValue(@"ApplicationProperties\SettingsVersion", "5");

            data.SetValue(@"ConversionProfiles\numClasses", "2");

            data.SetValue(@"ConversionProfiles\0\EmailClient\AddSignature", "True");
            data.SetValue(@"ConversionProfiles\0\EmailClient\Content", "Content");
            data.SetValue(@"ConversionProfiles\0\EmailClient\Enabled", "True");
            data.SetValue(@"ConversionProfiles\0\EmailClient\Recipients", "Recipients");
            data.SetValue(@"ConversionProfiles\0\EmailClient\Subject", "Subject");

            var upgrader = new CreatorSettingsUpgrader(data);

            upgrader.Upgrade(6);

            Assert.AreEqual("True", data.GetValue(@"ConversionProfiles\0\EmailClientSettings\AddSignature"));
            Assert.AreEqual("Content", data.GetValue(@"ConversionProfiles\0\EmailClientSettings\Content"));
            Assert.AreEqual("True", data.GetValue(@"ConversionProfiles\0\EmailClientSettings\Enabled"));
            Assert.AreEqual("Recipients", data.GetValue(@"ConversionProfiles\0\EmailClientSettings\Recipients"));
            Assert.AreEqual("Subject", data.GetValue(@"ConversionProfiles\0\EmailClientSettings\Subject"));
        }

        [Test]
        public void Test_MoveEMailSmtpSectionToEmailSmtpSettings()
        {
            var data = Data.CreateDataStorage();
            data.SetValue(@"ApplicationProperties\SettingsVersion", "5");

            data.SetValue(@"ConversionProfiles\numClasses", "2");

            data.SetValue(@"ConversionProfiles\0\EmailSmtp\Address", "Address");
            data.SetValue(@"ConversionProfiles\0\EmailSmtp\AddSignature", "False");
            data.SetValue(@"ConversionProfiles\0\EmailSmtp\Content", "Content");
            data.SetValue(@"ConversionProfiles\0\EmailSmtp\Enabled", "True");
            data.SetValue(@"ConversionProfiles\0\EmailSmtp\Password", "Password");
            data.SetValue(@"ConversionProfiles\0\EmailSmtp\Port", "1");
            data.SetValue(@"ConversionProfiles\0\EmailSmtp\Recipients", "Recipients");
            data.SetValue(@"ConversionProfiles\0\EmailSmtp\SameTextAsClientMail", "SameTextAsClientMail");
            data.SetValue(@"ConversionProfiles\0\EmailSmtp\Server", "Server");
            data.SetValue(@"ConversionProfiles\0\EmailSmtp\Ssl", "True");
            data.SetValue(@"ConversionProfiles\0\EmailSmtp\Subject", "Subject");
            data.SetValue(@"ConversionProfiles\0\EmailSmtp\UserName", "UserName");

            var upgrader = new CreatorSettingsUpgrader(data);

            upgrader.Upgrade(6);

            Assert.AreEqual("Address", data.GetValue(@"ConversionProfiles\0\EmailSmtpSettings\Address"));
            Assert.AreEqual("False", data.GetValue(@"ConversionProfiles\0\EmailSmtpSettings\AddSignature"));
            Assert.AreEqual("Content", data.GetValue(@"ConversionProfiles\0\EmailSmtpSettings\Content"));
            Assert.AreEqual("True", data.GetValue(@"ConversionProfiles\0\EmailSmtpSettings\Enabled"));
            Assert.AreEqual("Password", data.GetValue(@"ConversionProfiles\0\EmailSmtpSettings\Password"));
            Assert.AreEqual("1", data.GetValue(@"ConversionProfiles\0\EmailSmtpSettings\Port"));
            Assert.AreEqual("Recipients", data.GetValue(@"ConversionProfiles\0\EmailSmtpSettings\Recipients"));
            Assert.AreEqual("SameTextAsClientMail", data.GetValue(@"ConversionProfiles\0\EmailSmtpSettings\SameTextAsClientMail"));
            Assert.AreEqual("Server", data.GetValue(@"ConversionProfiles\0\EmailSmtpSettings\Server"));
            Assert.AreEqual("True", data.GetValue(@"ConversionProfiles\0\EmailSmtpSettings\Ssl"));
            Assert.AreEqual("Subject", data.GetValue(@"ConversionProfiles\0\EmailSmtpSettings\Subject"));
            Assert.AreEqual("UserName", data.GetValue(@"ConversionProfiles\0\EmailSmtpSettings\UserName"));
        }
    }
}
