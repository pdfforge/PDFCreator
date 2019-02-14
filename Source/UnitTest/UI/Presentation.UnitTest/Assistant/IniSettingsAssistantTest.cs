using NSubstitute;
using NUnit.Framework;
using pdfforge.DataStorage;
using pdfforge.DataStorage.Storage;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction.DialogInteractions;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities.Threading;
using System.Collections.Generic;
using Translatable;

namespace Presentation.UnitTest.Assistant
{
    [TestFixture]
    public class IniSettingsAssistantTest
    {
        [SetUp]
        public void Setup()
        {
            _invoker = Substitute.For<IInteractionInvoker>();
            _settingsHelper = Substitute.For<ISettingsProvider>();

            // add printers to settings, so there are missing printers
            var pdfCreatorSettings = new PdfCreatorSettings();
            pdfCreatorSettings.ApplicationSettings.PrinterMappings.Add(new PrinterMapping("Printer1", "1234"));
            pdfCreatorSettings.ApplicationSettings.PrinterMappings.Add(new PrinterMapping("Printer2", "1234"));
            _settingsHelper.Settings.Returns(pdfCreatorSettings);

            _settingsManager = Substitute.For<ISettingsManager>();
            _settingsManager.GetSettingsProvider().Returns(_settingsHelper);

            _dataStorageFactory = Substitute.For<IDataStorageFactory>();
            _storage = Substitute.For<IStorage>();
            _dataStorageFactory.BuildIniStorage(Arg.Any<string>()).Returns(_storage);

            _printerProvider = Substitute.For<IPrinterProvider>();
            _printerProvider.GetPDFCreatorPrinters().Returns(new List<string>() { "Printer1", "Printer3" }); // return printers for unused printer check

            _uacAssistant = Substitute.For<IUacAssistant>();

            _iniSettingsLoader = Substitute.For<IIniSettingsLoader>();
            _iniSettingsLoader.LoadIniSettings(Arg.Any<string>()).Returns(pdfCreatorSettings);

            _creatorIniSettingsAssistant = new CreatorIniSettingsAssistant(_invoker,
                new TranslationUpdater(new TranslationFactory(), new ThreadManager()), _settingsManager, _dataStorageFactory,
                _iniSettingsLoader, _printerProvider, _uacAssistant);
        }

        private IInteractionInvoker _invoker;
        private CreatorIniSettingsAssistant _creatorIniSettingsAssistant;
        private IStorage _storage;
        private IDataStorageFactory _dataStorageFactory;
        private ISettingsProvider _settingsHelper;
        private ISettingsManager _settingsManager;
        private IIniSettingsLoader _iniSettingsLoader;
        private IPrinterProvider _printerProvider;
        private IUacAssistant _uacAssistant;

        private void SetSuccessfulOpenFileInteraction(string filename)
        {
            _invoker.When(x => x.Invoke(Arg.Any<OpenFileInteraction>())).Do(info =>
            {
                var interaction = info.Arg<OpenFileInteraction>();
                interaction.Success = true;
                interaction.FileName = filename;
            });
        }

        [Test]
        public void LoadIniSettings_IfNoFileNameWasSelected_ReturnsWithoutLoad()
        {
            _creatorIniSettingsAssistant.LoadIniSettings();
            _dataStorageFactory.DidNotReceive().BuildIniStorage(Arg.Any<string>());
        }

        [Test]
        public void LoadIniSettings_IfOverwriteIsAllowed_LoadsSelectedFile()
        {
            const string filename = "MyFileName";
            SetSuccessfulOpenFileInteraction(filename);

            _invoker.When(x => x.Invoke(Arg.Any<MessageInteraction>())).Do(info => info.Arg<MessageInteraction>().Response = MessageResponse.Yes);
            _settingsHelper.CheckValidSettings(Arg.Any<PdfCreatorSettings>()).Returns(true);

            _creatorIniSettingsAssistant.LoadIniSettings();

            _iniSettingsLoader.Received().LoadIniSettings(filename);
            _settingsManager.Received().ApplyAndSaveSettings(Arg.Any<PdfCreatorSettings>());
        }

        [Test]
        public void LoadIniSettings_IfOverwriteWasNotAllowed_ReturnsWithoutLoad()
        {
            const string filename = "MyFileName";
            SetSuccessfulOpenFileInteraction(filename);

            _creatorIniSettingsAssistant.LoadIniSettings();
            _dataStorageFactory.DidNotReceive().BuildIniStorage(Arg.Any<string>());
        }

        [Test]
        public void LoadIniSettings_WithInvalidSettings_DisplayWarningAndDoesNotApplySettings()
        {
            const string filename = "MyFileName";
            SetSuccessfulOpenFileInteraction(filename);

            _invoker.When(x => x.Invoke(Arg.Any<MessageInteraction>())).Do(info => { info.Arg<MessageInteraction>().Response = MessageResponse.Yes; });
            _settingsHelper.CheckValidSettings(Arg.Any<PdfCreatorSettings>()).Returns(false);

            _creatorIniSettingsAssistant.LoadIniSettings();

            _invoker.Received(2).Invoke(Arg.Any<MessageInteraction>());

            _settingsManager.DidNotReceive().ApplyAndSaveSettings(Arg.Any<PdfCreatorSettings>());
        }

        [Test]
        public void SaveIniSettings_IfNoFilenameWasSelected_ReturnsWithoutSave()
        {
            _creatorIniSettingsAssistant.SaveIniSettings();
            _dataStorageFactory.DidNotReceive().BuildIniStorage(Arg.Any<string>());
        }

        [Test]
        public void SaveIniSettings_SavesToSelectedFile()
        {
            const string filename = "MyFileName";

            _invoker.When(x => x.Invoke(Arg.Any<SaveFileInteraction>())).Do(info =>
            {
                var interaction = info.Arg<SaveFileInteraction>();
                interaction.Success = true;
                interaction.FileName = filename;
            });

            var appSettings = new ApplicationSettings();

            _creatorIniSettingsAssistant.SaveIniSettings();

            _storage.Received().WriteData(Arg.Any<Data>());
        }

        [Test]
        public void LoadIniSettings_HasNoUnusedPrinters_DeleteUnusedPrintersIsNotCalled()
        {
            const string filename = "MyFileName";
            SetSuccessfulOpenFileInteraction(filename);

            _invoker.When(x => x.Invoke(Arg.Any<MessageInteraction>()))
                    .Do(info => info.Arg<MessageInteraction>().Response = MessageResponse.Yes);
            _settingsHelper.CheckValidSettings(Arg.Any<PdfCreatorSettings>()).Returns(true);

            _printerProvider.GetPDFCreatorPrinters()
                .Returns(new List<string>()); // return empty list for installed PDFCreator printers, so there are no unused printers

            _creatorIniSettingsAssistant.LoadIniSettings();

            _iniSettingsLoader.Received().LoadIniSettings(filename);
            _settingsManager.Received().ApplyAndSaveSettings(Arg.Any<PdfCreatorSettings>());
        }

        [Test]
        public void LoadIniSettings_HasNoMissingPrinters_NoPrintersAreAdded()
        {
            _iniSettingsLoader.LoadIniSettings(Arg.Any<string>())
                .Returns(info => new PdfCreatorSettings()); // return empty settings, so there are no missing printers

            const string filename = "MyFileName";
            SetSuccessfulOpenFileInteraction(filename);

            _invoker.When(x => x.Invoke(Arg.Any<MessageInteraction>()))
                .Do(info => info.Arg<MessageInteraction>().Response = MessageResponse.Yes);
            _settingsHelper.CheckValidSettings(Arg.Any<PdfCreatorSettings>()).Returns(true);

            _creatorIniSettingsAssistant.LoadIniSettings();

            _iniSettingsLoader.Received().LoadIniSettings(filename);
            _settingsManager.Received().ApplyAndSaveSettings(Arg.Any<PdfCreatorSettings>());
        }
    }
}
