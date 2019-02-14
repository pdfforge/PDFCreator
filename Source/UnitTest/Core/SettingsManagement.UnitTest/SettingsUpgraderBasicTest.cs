using NUnit.Framework;
using pdfforge.DataStorage;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement;

namespace pdfforge.PDFCreator.UnitTest.Core.SettingsManagement
{
    [TestFixture]
    internal class SettingsUpgraderBasicTest
    {
        [Test]
        public void NumberOfUpdateMethodEqualsVersionInSettingsHelper()
        {
            var data = Data.CreateDataStorage();
            var upgrader = new CreatorSettingsUpgrader(data);

            var settingsVersion = CreatorAppSettings.ApplicationSettingsVersion;

            Assert.AreEqual(settingsVersion, upgrader.NumberOfUpgradeMethods());
        }

        [Test]
        public void VersionInDefaultPdfCreatorSettingsEqualsVersionInSettingsHelper()
        {
            var data = Data.CreateDataStorage();
            var upgrader = new CreatorSettingsUpgrader(data);

            var pdfCreatorSettings = new PdfCreatorSettings();

            Assert.AreEqual(pdfCreatorSettings.CreatorAppSettings.SettingsVersion, upgrader.NumberOfUpgradeMethods());
        }

        [Test]
        public void EmptyData_GetVersion_Returns0()
        {
            var data = Data.CreateDataStorage();
            var upgrader = new CreatorSettingsUpgrader(data);

            Assert.AreEqual(0, upgrader.SettingsVersion);
        }

        [Test]
        public void DataWithNonIntVersion_GetVersion_Returns0()
        {
            var data = Data.CreateDataStorage();
            data.SetValue(@"ApplicationProperties\SettingsVersion", "xyz");

            var upgrader = new CreatorSettingsUpgrader(data);

            Assert.AreEqual(0, upgrader.SettingsVersion);
        }

        [Test]
        public void DataWithVersion0_GetVersion_Returns0()
        {
            var data = Data.CreateDataStorage();
            data.SetValue(@"ApplicationProperties\SettingsVersion", "0");

            var upgrader = new CreatorSettingsUpgrader(data);

            Assert.AreEqual(0, upgrader.SettingsVersion);
        }

        [Test]
        public void DataWithVersion1_GetVersion_Returns1()
        {
            var data = Data.CreateDataStorage();
            data.SetValue(@"ApplicationProperties\SettingsVersion", "1");

            var upgrader = new CreatorSettingsUpgrader(data);

            Assert.AreEqual(1, upgrader.SettingsVersion);
        }

        [Test]
        public void DataWithVersion0_UpgradeRequiredToVersion0_ReturnsFalse()
        {
            var data = Data.CreateDataStorage();
            data.SetValue(@"ApplicationProperties\SettingsVersion", "0");

            var upgrader = new CreatorSettingsUpgrader(data);

            Assert.IsFalse(upgrader.RequiresUpgrade(0));
        }

        [Test]
        public void DataWithVersion1_UpgradeRequiredToVersion0_ReturnsFalse()
        {
            var data = Data.CreateDataStorage();
            data.SetValue(@"ApplicationProperties\SettingsVersion", "1");

            var upgrader = new CreatorSettingsUpgrader(data);

            Assert.IsFalse(upgrader.RequiresUpgrade(0));
        }

        [Test]
        public void DataWithVersion0_UpgradeRequiredToVersion1_ReturnsTrue()
        {
            var data = Data.CreateDataStorage();
            data.SetValue(@"ApplicationProperties\SettingsVersion", "0");

            var upgrader = new CreatorSettingsUpgrader(data);

            Assert.IsTrue(upgrader.RequiresUpgrade(1));
        }
    }
}
