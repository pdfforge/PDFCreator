using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement;

namespace pdfforge.PDFCreator.UnitTest.Core.SettingsManagement
{
    /// <summary>
    ///     Tests for settings classes. We cannot rely on this. They merely make sure that there is no exception while reading,
    ///     storing or copying the settings.
    ///     They do not guarantee the the generated classes to their work as expected
    /// </summary>
    [TestFixture]
    public class SettingsTest
    {
        [Test]
        public void GetProfileByGuid_WithExistingProfile_ReturnsProfile()
        {
            var settings = new PdfCreatorSettings();
            var p1 = new ConversionProfile { Guid = "g1", Name = "p1" };
            var p2 = new ConversionProfile { Guid = "g2", Name = "p2" };

            settings.ConversionProfiles.Add(p1);
            settings.ConversionProfiles.Add(p2);

            Assert.AreSame(p2, settings.GetProfileByGuid("g2"));
        }

        [Test]
        public void GetProfileByGuid_WithNonexistantProfile_ReturnsProfile()
        {
            var settings = new PdfCreatorSettings();
            var p1 = new ConversionProfile { Guid = "g1", Name = "p1" };
            var p2 = new ConversionProfile { Guid = "g2", Name = "p2" };

            settings.ConversionProfiles.Add(p1);
            settings.ConversionProfiles.Add(p2);

            Assert.IsNull(settings.GetProfileByGuid("g3"));
        }

        [Test]
        public void GetProfileByName_WithExistingProfile_ReturnsProfile()
        {
            var settings = new PdfCreatorSettings();
            var p1 = new ConversionProfile { Guid = "g1", Name = "p1" };
            var p2 = new ConversionProfile { Guid = "g2", Name = "p2" };

            settings.ConversionProfiles.Add(p1);
            settings.ConversionProfiles.Add(p2);

            Assert.AreSame(p1, settings.GetProfileByName("p1"));
        }

        [Test]
        public void GetProfileByName_WithNonexistantProfile_ReturnsProfile()
        {
            var settings = new PdfCreatorSettings();
            var p1 = new ConversionProfile { Guid = "g1", Name = "p1" };
            var p2 = new ConversionProfile { Guid = "g2", Name = "p2" };

            settings.ConversionProfiles.Add(p1);
            settings.ConversionProfiles.Add(p2);

            Assert.IsNull(settings.GetProfileByName("p3"));
        }

        [Test]
        public void Settings_Copy_EqualsOriginal()
        {
            var settings = new PdfCreatorSettings();
            settings.ConversionProfiles.Add(new ConversionProfile { Guid = "p1" });
            settings.ConversionProfiles.Add(new ConversionProfile { Guid = "p2" });

            var clone = settings.Copy();

            Assert.AreEqual(settings.ToString(), clone.ToString(), "string representations do not match");
            Assert.AreEqual(settings.ConversionProfiles[1].Guid, "p2", "A GUID does not match");
            Assert.IsTrue(clone.Equals(settings), "Equals method returns false");
        }

        [Test]
        public void Settings_Save_ExpectsNothing()
        {
            var settings = new PdfCreatorSettings();
            settings.ConversionProfiles.Add(new ConversionProfile { Guid = "p1" });
            settings.ConversionProfiles.Add(new ConversionProfile { Guid = "p2" });
            var storage = new DataStorageFactory().BuildIniStorage("");
            settings.SaveData(storage);
        }
    }
}
