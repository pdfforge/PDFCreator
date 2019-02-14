using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using System;

namespace SettingsUnitTest
{
    [TestFixture]
    public class SettingsTest
    {
        [Test]
        public void Settings_ImplementPropertyChanged()
        {
            var wasRaised = false;
            var settings = new ApplicationSettings();
            settings.PropertyChanged += (sender, args) => wasRaised = true;

            settings.NextUpdate = DateTime.Today;

            Assert.IsTrue(wasRaised);
        }

        [Test]
        public void Settings_SetProperty_DoesNotCheckForEquality()
        {
            // This test ansures that Fody.PropertyChanged does not suppress assignments,
            // when assigned value equals the old one (as it does with a copy)
            // we want the same reference here!
            var settings = new PdfCreatorSettings();
            var appSettingsCopy = settings.ApplicationSettings.Copy();

            settings.ApplicationSettings = appSettingsCopy;

            Assert.AreSame(appSettingsCopy, settings.ApplicationSettings);
        }
    }
}
