using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.DataStorage.Storage;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.SettingsManagement;
using System.IO;
using System.Text;

namespace pdfforge.IntegrationTest.Presentation.IntegrationTest
{
    [TestFixture]
    internal class SettingsUpgraderTest
    {
        [SetUp]
        public void Setup()
        {
            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder("SettingsTest");
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
        }

        private TestHelper _th;

        [Test]
        public void SettingsUpdateV3To4_VersionBecomes4_TiffColorBlackWihteGetsBlackWhiteG4Fax()
        {
            const int oldSettingsVersion = 3;
            const int newSettingsVersion = 4;

            var iniFile = Path.Combine(_th.TmpTestFolder, "TestSettings.ini");

            var iniStorage = new IniStorage(iniFile, Encoding.UTF8);
            var builder = new CreatorDefaultSettingsBuilder();
            var oldSettings = (PdfCreatorSettings)builder.CreateDefaultSettings("PDFCreator", "English");

            oldSettings.ConversionProfiles[0].TiffSettings.Color = TiffColor.BlackWhiteG4Fax;
            oldSettings.CreatorAppSettings.SettingsVersion = oldSettingsVersion;

            oldSettings.SaveData(iniStorage);

            var settingsFromIni = File.ReadAllText(iniFile);
            settingsFromIni = settingsFromIni.Replace("BlackWhiteG4Fax", "BlackWhite");
            File.WriteAllText(iniFile, settingsFromIni);

            var settings = (PdfCreatorSettings)new CreatorDefaultSettingsBuilder().CreateEmptySettings();

            var storage = new CreatorSettingsMigrationStorage(iniStorage, newSettingsVersion);
            settings.LoadData(storage);

            Assert.AreEqual(newSettingsVersion, settings.CreatorAppSettings.SettingsVersion, "Did not update SettingsVersion.");
            Assert.AreEqual(TiffColor.BlackWhiteG4Fax, settings.ConversionProfiles[0].TiffSettings.Color, "Did not update TiffColor BlackWhite to BlackWhiteG4Fax");
        }
    }
}
