using NUnit.Framework;
using pdfforge.DataStorage;
using pdfforge.PDFCreator.Core.SettingsManagement;

namespace pdfforge.PDFCreator.UnitTest.Core.SettingsManagement
{
    [TestFixture]
    internal class SettingsUpgrader2To3Test
    {
        [Test]
        public void DataWithVersion2_UpgradeRequiredToVersion3_ReturnsTrue()
        {
            var data = Data.CreateDataStorage();
            data.SetValue(@"ApplicationProperties\SettingsVersion", "2");

            var upgrader = new CreatorSettingsUpgrader(data);

            Assert.IsTrue(upgrader.RequiresUpgrade(3));
        }

        [Test]
        public void DataWithVersion2_UpgradeToVersion3_SetsVersionTo3()
        {
            var data = Data.CreateDataStorage();
            data.SetValue(@"ApplicationProperties\SettingsVersion", "2");
            var upgrader = new CreatorSettingsUpgrader(data);

            upgrader.Upgrade(3);

            Assert.AreEqual("3", data.GetValue(@"ApplicationProperties\SettingsVersion"));
        }

        [Test]
        public void Test_RenamePdfAasPdfA2b()
        {
            var data = Data.CreateDataStorage();
            data.SetValue(@"ApplicationProperties\SettingsVersion", "2");
            data.SetValue(@"ConversionProfiles\numClasses", "2");
            data.SetValue(@"ConversionProfiles\0\OutputFormat", "PdfA");
            data.SetValue(@"ConversionProfiles\1\OutputFormat", "PdfA");

            var upgrader = new CreatorSettingsUpgrader(data);

            upgrader.Upgrade(3);

            Assert.AreEqual("PdfA2B", data.GetValue(@"ConversionProfiles\0\OutputFormat"));
            Assert.AreEqual("PdfA2B", data.GetValue(@"ConversionProfiles\1\OutputFormat"));
        }

        [Test]
        public void Test_OutputformatNamesExceptPdfARemainUnchanged()
        {
            var data = Data.CreateDataStorage();
            data.SetValue(@"ApplicationProperties\SettingsVersion", "2");
            data.SetValue(@"ConversionProfiles\numClasses", "5");
            data.SetValue(@"ConversionProfiles\0\OutputFormat", "Pdf");
            data.SetValue(@"ConversionProfiles\1\OutputFormat", "PdfX");
            data.SetValue(@"ConversionProfiles\2\OutputFormat", "Jpeg");
            data.SetValue(@"ConversionProfiles\3\OutputFormat", "Png");
            data.SetValue(@"ConversionProfiles\4\OutputFormat", "Tif");

            var upgrader = new CreatorSettingsUpgrader(data);

            upgrader.Upgrade(3);

            Assert.AreEqual("Pdf", data.GetValue(@"ConversionProfiles\0\OutputFormat"));
            Assert.AreEqual("PdfX", data.GetValue(@"ConversionProfiles\1\OutputFormat"));
            Assert.AreEqual("Jpeg", data.GetValue(@"ConversionProfiles\2\OutputFormat"));
            Assert.AreEqual("Png", data.GetValue(@"ConversionProfiles\3\OutputFormat"));
            Assert.AreEqual("Tif", data.GetValue(@"ConversionProfiles\4\OutputFormat"));
        }
    }
}
