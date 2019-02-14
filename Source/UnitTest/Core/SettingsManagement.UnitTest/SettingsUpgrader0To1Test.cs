using NUnit.Framework;
using pdfforge.DataStorage;
using pdfforge.PDFCreator.Core.SettingsManagement;

namespace pdfforge.PDFCreator.UnitTest.Core.SettingsManagement
{
    [TestFixture]
    internal class SettingsUpgrader0To1Test
    {
        [Test]
        public void DataWithVersion0_UpgradeToVersion1_SetsVersionTo1()
        {
            var data = Data.CreateDataStorage();
            data.SetValue(@"ApplicationProperties\SettingsVersion", "0");
            var upgrader = new CreatorSettingsUpgrader(data);

            upgrader.Upgrade(1);

            Assert.AreEqual("1", data.GetValue(@"ApplicationProperties\SettingsVersion"));
        }

        [Test]
        public void Test_RenameEncryptionLevels()
        {
            var data = Data.CreateDataStorage();
            data.SetValue(@"ConversionProfiles\numClasses", "3");
            data.SetValue(@"ConversionProfiles\0\PdfSettings\Security\EncryptionLevel", "Low40Bit");
            data.SetValue(@"ConversionProfiles\1\PdfSettings\Security\EncryptionLevel", "Medium128Bit");
            data.SetValue(@"ConversionProfiles\2\PdfSettings\Security\EncryptionLevel", "High128BitAes");
            var upgrader = new CreatorSettingsUpgrader(data);

            upgrader.Upgrade(1);

            Assert.AreEqual("Rc40Bit", data.GetValue(@"ConversionProfiles\0\PdfSettings\Security\EncryptionLevel"), "Did not rename Rc40Bit Encryption");
            Assert.AreEqual("Rc128Bit", data.GetValue(@"ConversionProfiles\1\PdfSettings\Security\EncryptionLevel"), "Did not rename Rc128Bit Encryption");
            Assert.AreEqual("Aes128Bit", data.GetValue(@"ConversionProfiles\2\PdfSettings\Security\EncryptionLevel"), "Did not rename Aes128Bit Encryption");
        }

        [Test]
        public void Test_AddTitleTemplateWithDefaultValue()
        {
            var data = Data.CreateDataStorage();
            data.SetValue(@"ConversionProfiles\numClasses", "2");
            var upgrader = new CreatorSettingsUpgrader(data);

            upgrader.Upgrade(1);

            Assert.AreEqual("<PrintJobName>", data.GetValue(@"ConversionProfiles\0\TitleTemplate"));
            Assert.AreEqual("<PrintJobName>", data.GetValue(@"ConversionProfiles\1\TitleTemplate"));
        }

        [Test]
        public void Test_MoveDefaultFormatValueToOutputFormat()
        {
            var data = Data.CreateDataStorage();
            data.SetValue(@"ConversionProfiles\numClasses", "2");
            data.SetValue(@"ConversionProfiles\0\DefaultFormat", "Pdf");
            data.SetValue(@"ConversionProfiles\1\DefaultFormat", "Png");
            var upgrader = new CreatorSettingsUpgrader(data);

            upgrader.Upgrade(1);

            Assert.AreEqual("Pdf", data.GetValue(@"ConversionProfiles\0\OutputFormat"));
            Assert.AreEqual("Png", data.GetValue(@"ConversionProfiles\1\OutputFormat"));
        }

        [Test]
        public void Test_RemoveOldDefaultFormat()
        {
            var data = Data.CreateDataStorage();
            data.SetValue(@"ConversionProfiles\numClasses", "2");
            data.SetValue(@"ConversionProfiles\0\DefaultFormat", "Pdf");
            data.SetValue(@"ConversionProfiles\1\DefaultFormat", "Png");
            var upgrader = new CreatorSettingsUpgrader(data);

            upgrader.Upgrade(1);

            Assert.AreEqual("", data.GetValue(@"ConversionProfiles\0\DefaultFormat"));
            Assert.AreEqual("", data.GetValue(@"ConversionProfiles\1\DefaultFormat"));
        }
    }
}
