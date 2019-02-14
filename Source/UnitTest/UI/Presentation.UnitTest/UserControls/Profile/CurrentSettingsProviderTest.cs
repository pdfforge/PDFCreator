using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation;

namespace Presentation.UnitTest.UserControls.Profile
{
    [TestFixture]
    public class CurrentSettingsProviderTest
    {
        private PdfCreatorSettings _settings;

        [SetUp]
        public void Setup()
        {
            _settings = new PdfCreatorSettings();
            _settings.ConversionProfiles.Add(new ConversionProfile());
        }

        public CurrentSettingsProvider BuildCurrentSettingsProvider()
        {
            var settingsProvider = Substitute.For<ISettingsProvider>();
            settingsProvider.Settings.Returns(_settings);

            return new CurrentSettingsProvider(settingsProvider);
        }

        [Test]
        public void Settings_AreClonedProperly()
        {
            var currentSettingsProvider = BuildCurrentSettingsProvider();

            Assert.AreEqual(_settings, currentSettingsProvider.Settings);
            Assert.AreNotSame(_settings, currentSettingsProvider.Settings);

            Assert.AreSame(_settings.ApplicationSettings, currentSettingsProvider.Settings.ApplicationSettings);
            Assert.AreSame(_settings.CreatorAppSettings, currentSettingsProvider.Settings.CreatorAppSettings);
        }
    }
}
