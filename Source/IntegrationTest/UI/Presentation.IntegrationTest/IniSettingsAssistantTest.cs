using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction.DialogInteractions;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using System;
using System.IO;

namespace pdfforge.IntegrationTest.Presentation.IntegrationTest
{
    [TestFixture]
    public class IniSettingsAssistantTest
    {
        private IUacAssistant _uacAssistant;
        private IPrinterProvider _printerProvider;

        [SetUp]
        public void Setup()
        {
            _uacAssistant = Substitute.For<IUacAssistant>();
            _printerProvider = Substitute.For<IPrinterProvider>();
        }

        private IInteractionInvoker BuildInvoker(string fileName)
        {
            var invoker = Substitute.For<IInteractionInvoker>();
            invoker.When(x => x.Invoke(Arg.Any<SaveFileInteraction>())).Do(info =>
            {
                var interaction = info.Arg<SaveFileInteraction>();
                interaction.FileName = fileName;
                interaction.Success = true;
            });

            invoker.When(x => x.Invoke(Arg.Any<OpenFileInteraction>())).Do(info =>
            {
                var interaction = info.Arg<OpenFileInteraction>();
                interaction.FileName = fileName;
                interaction.Success = true;
            });

            invoker.When(x => x.Invoke(Arg.Any<MessageInteraction>())).Do(info => info.Arg<MessageInteraction>().Response = MessageResponse.Yes);

            return invoker;
        }

        private PdfCreatorSettings CreateSettings()
        {
            var profileBuilder = new CreatorDefaultSettingsBuilder();
            var settings = (PdfCreatorSettings)profileBuilder.CreateDefaultSettings("MySpecialPrinter", "Elbisch");
            settings.ApplicationSettings.NextUpdate = new DateTime(2020, 1, 15);
            settings.ConversionProfiles.Add(new ConversionProfile());
            return settings;
        }

        [Test]
        public void SaveAndLoad_ResultsInExactlyEqualSettings()
        {
            var tempFolder = TempFileHelper.CreateTempFolder("IniSettingsTest");
            var fileName = Path.Combine(tempFolder, "savedSettings.ini");

            var settings = CreateSettings();
            settings.ApplicationSettings.JobHistory.LastDateFrom = DateTime.Today.AddDays(-1);
            settings.ApplicationSettings.JobHistory.LastDateTo = DateTime.Today;

            settings.ApplicationSettings.RssFeed.LatestRssUpdate = DateTime.Today;

            var invoker = BuildInvoker(fileName);

            var uacAssistant = Substitute.For<IUacAssistant>();

            var settingsHelper = Substitute.For<ISettingsProvider>();

            settingsHelper.Settings.Returns(settings);
            settingsHelper.CheckValidSettings(Arg.Any<PdfCreatorSettings>()).Returns(delegate (CallInfo x)
            {
                return x.Arg<PdfCreatorSettings>().ConversionProfiles.Count > 0;
            });

            PdfCreatorSettings loadedSettings = null;

            var settingsMananger = Substitute.For<ISettingsManager>();
            settingsMananger.GetSettingsProvider().Returns(settingsHelper);
            settingsMananger.When(x => x.ApplyAndSaveSettings(Arg.Any<PdfCreatorSettings>())).Do(x => loadedSettings = x.Arg<PdfCreatorSettings>());

            var defaultSettingsBuilder = Substitute.For<IDefaultSettingsBuilder>();

            var pdfCreatorSettings = new PdfCreatorSettings();
            // pdfCreatorSettings.ConversionProfiles.Add(new ConversionProfile());

            defaultSettingsBuilder.CreateEmptySettings().Returns(pdfCreatorSettings);

            //var migrationStorageFactory = Substitute.For<IMigrationStorageFactory>();
            var migrationStorageFactory = new MigrationStorageFactory((baseStorage, targetVersion) => new CreatorSettingsMigrationStorage(baseStorage, targetVersion));
            //migrationStorageFactory.GetMigrationStorage(Arg.Any<IStorage>(), Arg.Any<int>()).Returns(new IniStorage(""));
            var iniSettingsLoader = new IniSettingsLoader(new DataStorageFactory(), defaultSettingsBuilder, migrationStorageFactory);
            var iniSettingsAssistant = new CreatorIniSettingsAssistant(invoker, new DesignTimeTranslationUpdater(), settingsMananger, new DataStorageFactory(), iniSettingsLoader, _printerProvider, _uacAssistant);

            iniSettingsAssistant.SaveIniSettings();
            iniSettingsAssistant.LoadIniSettings();

            var titles = settings.ApplicationSettings.TitleReplacement;
            var loadedTitles = loadedSettings.ApplicationSettings.TitleReplacement;
            for (var i = 0; i < titles.Count; i++)
            {
                Assert.AreEqual(titles[i], loadedTitles[i]);
            }
            Assert.AreEqual(settings.ApplicationSettings.TitleReplacement, loadedSettings.ApplicationSettings.TitleReplacement);
            Assert.AreEqual(settings.ApplicationSettings, loadedSettings.ApplicationSettings);
            Assert.AreEqual(settings.ConversionProfiles, loadedSettings.ConversionProfiles);

            Assert.AreEqual(settings.ToString(), loadedSettings.ToString());

            TempFileHelper.CleanUp();
        }
    }
}
