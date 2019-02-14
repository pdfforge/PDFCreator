using NUnit.Framework;
using pdfforge.DataStorage;
using pdfforge.PDFCreator.Core.SettingsManagement;

namespace pdfforge.PDFCreator.UnitTest.Core.SettingsManagement
{
    [TestFixture]
    internal class CreatorSettingsUpgrader1To2Test
    {
        [Test]
        public void DataWithVersion1_UpgradeRequiredToVersion2_ReturnsTrue()
        {
            var data = Data.CreateDataStorage();
            data.SetValue(@"ApplicationProperties\SettingsVersion", "1");

            var upgrader = new CreatorSettingsUpgrader(data);

            Assert.IsTrue(upgrader.RequiresUpgrade(2));
        }

        [Test]
        public void DataWithVersion1_UpgradeToVersion2_SetsVersionTo2()
        {
            var data = Data.CreateDataStorage();
            data.SetValue(@"ApplicationProperties\SettingsVersion", "2");
            var upgrader = new CreatorSettingsUpgrader(data);

            upgrader.Upgrade(2);

            Assert.AreEqual("2", data.GetValue(@"ApplicationProperties\SettingsVersion"));
        }

        [Test]
        public void Test_RenameLastUsedProfilGuidToLastUsedProfilEGuid_withE()
        {
            var data = Data.CreateDataStorage();
            data.SetValue(@"ApplicationProperties\SettingsVersion", "1");
            data.SetValue(@"ApplicationSettings\LastUsedProfilGuid", "SomeTestGuid");

            var upgrader = new CreatorSettingsUpgrader(data);

            upgrader.Upgrade(2);

            Assert.AreEqual("SomeTestGuid", data.GetValue(@"ApplicationSettings\LastUsedProfileGuid"));
            Assert.AreEqual("", data.GetValue(@"ApplicationSettings\LastUsedProfilGuid"));
        }

        [Test]
        public void Test_MoveAddBackgroundToSectionInBackgroundPage()
        {
            var data = Data.CreateDataStorage();
            data.SetValue(@"ApplicationProperties\SettingsVersion", "1");
            data.SetValue(@"ConversionProfiles\numClasses", "2");
            data.SetValue(@"ConversionProfiles\0\AttachmentPage\AddBackground", "true");
            data.SetValue(@"ConversionProfiles\1\AttachmentPage\AddBackground", "true");

            var upgrader = new CreatorSettingsUpgrader(data);

            upgrader.Upgrade(2);

            Assert.AreEqual("true", data.GetValue(@"ConversionProfiles\0\BackgroundPage\OnAttachment"));
            Assert.AreEqual("true", data.GetValue(@"ConversionProfiles\1\BackgroundPage\OnAttachment"));
        }

        [Test]
        public void Test_MoveAddCoverToSectionInCoverPage()
        {
            var data = Data.CreateDataStorage();
            data.SetValue(@"ApplicationProperties\SettingsVersion", "1");
            data.SetValue(@"ConversionProfiles\numClasses", "2");
            data.SetValue(@"ConversionProfiles\0\CoverPage\AddBackground", "true");
            data.SetValue(@"ConversionProfiles\1\CoverPage\AddBackground", "true");

            var upgrader = new CreatorSettingsUpgrader(data);

            upgrader.Upgrade(2);

            Assert.AreEqual("true", data.GetValue(@"ConversionProfiles\0\BackgroundPage\OnCover"), "1");
            Assert.AreEqual("true", data.GetValue(@"ConversionProfiles\1\BackgroundPage\OnCover"), "2");
        }
    }
}
