using Microsoft.Win32;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Startup.AppStarts;
using pdfforge.PDFCreator.Core.StartupInterface;
using System;

namespace pdfforge.PDFCreator.UnitTest.Startup.AppStarts
{
    [TestFixture]
    public class InitializeDefaultSettingsStartTest
    {
        private InitializeDefaultSettingsStart _initializeDefaultSettingsStart;
        private IIniSettingsLoader _iniSettingsLoader;
        private ISettingsProvider _settingsProvider;
        private IInstallationPathProvider _pathProvider;
        private IDataStorageFactory _storageFactory;
        private PdfCreatorSettings _settings;

        [SetUp]
        public void Setup()
        {
            var checkAllStartupConditions = Substitute.For<ICheckAllStartupConditions>();
            _iniSettingsLoader = Substitute.For<IIniSettingsLoader>();
            _settings = new PdfCreatorSettings();
            _iniSettingsLoader.LoadIniSettings(Arg.Any<string>()).Returns(_settings);
            _settingsProvider = Substitute.For<ISettingsProvider>();
            _settingsProvider.CheckValidSettings(_settings).Returns(true);
            _pathProvider = Substitute.For<IInstallationPathProvider>();
            _storageFactory = Substitute.For<IDataStorageFactory>();

            _initializeDefaultSettingsStart = new InitializeDefaultSettingsStart(checkAllStartupConditions,
                _iniSettingsLoader, _settingsProvider, _pathProvider, _storageFactory);
        }

        [Test]
        public void Run_IniSettingsLoaderReturnsNull_StartUpResultIsInvalidSettingsFile()
        {
            _iniSettingsLoader.LoadIniSettings(Arg.Any<string>()).ReturnsNull();

            var result = _initializeDefaultSettingsStart.Run();

            Assert.AreEqual(ExitCode.InvalidSettingsFile, result);
        }

        [Test]
        public void Run_SettingsProviderCheckValidSettingsReturnsFalse_StartUpResultIsInvalidSettingsInGivenFile()
        {
            _settingsProvider.CheckValidSettings(_settings).Returns(false);

            var result = _initializeDefaultSettingsStart.Run();

            Assert.AreEqual(ExitCode.InvalidSettingsInGivenFile, result);
        }

        [Test]
        public void Run_StorageFactoryBuildRegistryStorage_StartUpResultIsErrorWhileSavingDefaultSettings()
        {
            _storageFactory.When(x => x.BuildRegistryStorage(RegistryHive.Users, Arg.Any<string>()))
                .Do(x => { throw new Exception(); });

            var result = _initializeDefaultSettingsStart.Run();

            Assert.AreEqual(ExitCode.ErrorWhileSavingDefaultSettings, result);
        }

        [Test]
        public void Run_SuccesfulRun_StartUpResultIsOk()
        {
            _pathProvider.SettingsRegistryPath.Returns("ProviderPath");

            var result = _initializeDefaultSettingsStart.Run();

            _storageFactory.Received().BuildRegistryStorage(RegistryHive.Users, ".Default\\ProviderPath");
            Assert.AreEqual(ExitCode.Ok, result);
        }
    }
}
