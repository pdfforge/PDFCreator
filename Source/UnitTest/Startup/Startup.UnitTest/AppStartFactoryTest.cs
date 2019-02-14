using Microsoft.Win32;
using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.DirectConversion;
using pdfforge.PDFCreator.Core.Printing.Printing;
using pdfforge.PDFCreator.Core.Services.Logging;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Startup;
using pdfforge.PDFCreator.Core.Startup.AppStarts;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UnitTest.Startup
{
    [TestFixture]
    public class AppStartFactoryTest
    {
        private IPathUtil _pathUtil;
        private AppStartFactory _appStartFactory;
        private IAppStartResolver _resolver;

        [SetUp]
        public void Setup()
        {
            LoggingHelper.InitConsoleLogger("Test", LoggingLevel.Off);

            var startupConditions = Substitute.For<ICheckAllStartupConditions>();
            var starter = Substitute.For<IMaybePipedApplicationStarter>();
            starter.StartupConditions.Returns(startupConditions);

            _resolver = Substitute.For<IAppStartResolver>();
            // We need some special syntax here to make NSubsitute work here: .Returns(x => new MainWindowStart(...));
            _resolver.ResolveAppStart<MainWindowStart>().Returns(x => new MainWindowStart(null, starter, Substitute.For<IPdfArchitectCheck>(), Substitute.For<IMainWindowThreadLauncher>()));
            _resolver.ResolveAppStart<DragAndDropStart>().Returns(x => new DragAndDropStart(Substitute.For<IFileConversionAssistant>(), starter));
            _resolver.ResolveAppStart<PrintFileStart>().Returns(x => new PrintFileStart(Substitute.For<ICheckAllStartupConditions>(), Substitute.For<IPrintFileHelper>(), null, Substitute.For<IStoredParametersManager>()));
            _resolver.ResolveAppStart<DirectConversionStart>().Returns(x => new DirectConversionStart(null, starter, null, null));
            _resolver.ResolveAppStart<NewPrintJobStart>().Returns(x => new NewPrintJobStart(null, null, null, starter, null));
            _resolver.ResolveAppStart<InitializeDefaultSettingsStart>().Returns(x => new InitializeDefaultSettingsStart(null, null, null, Substitute.For<IInstallationPathProvider>(), Substitute.For<IDataStorageFactory>()));
            _resolver.ResolveAppStart<StoreLicenseForAllUsersStart>().Returns(x => new StoreLicenseForAllUsersStart(null, null, new InstallationPathProvider("", "", "", RegistryHive.CurrentUser)));

            _pathUtil = Substitute.For<IPathUtil>();
            var directConversionHelper = Substitute.For<IDirectConversionHelper>();
            _appStartFactory = new AppStartFactory(_resolver, _pathUtil, directConversionHelper);

            // TODO: Add tests of thr Run() method as it is testable now
        }

        [Test]
        public void Called_MultipleRootedFiles_ReturnsDragAndDropStart()
        {
            string[] args = { @"\RootedFile1.xxx", @"\RootedFile2.xxx", @"\RootedFile3.xxx" };
            _pathUtil.IsValidRootedPath(Arg.Any<string>()).Returns(true);

            _appStartFactory.CreateApplicationStart(args);

            _resolver.Received().ResolveAppStart<DragAndDropStart>();
        }

        [Test]
        public void Called_RootedAndNotRootedFiles_ReturnsMainWindowStart()
        {
            string[] args = { @"\RootedFile1.xxx", @"NotRootedFile.xxx", @"\RootedFile2.xxx" };
            _pathUtil.IsValidRootedPath(@"\RootedFile1.xxx").Returns(true);
            _pathUtil.IsValidRootedPath("NotRootedFile.xxx").Returns(false);

            _appStartFactory.CreateApplicationStart(args);

            _resolver.Received().ResolveAppStart<DirectConversionStart>();
        }

        [Test]
        public void Called_RootedDirectory_ReturnsMainWindowStart()
        {
            var rootedDir = @"\RootedDirectory";
            string[] args = { rootedDir };
            _pathUtil.IsValidRootedPath(rootedDir).Returns(true);

            _appStartFactory.CreateApplicationStart(args);

            _resolver.Received().ResolveAppStart<DragAndDropStart>();
        }

        [Test]
        public void Called_RootedFileAndRootedDirectory_ReturnsMainWindowStart()
        {
            string[] args = { @"\RootedDirectory", @"\RootedFile1.xxx" };

            _appStartFactory.CreateApplicationStart(args);

            _resolver.Received().ResolveAppStart<MainWindowStart>();
        }

        [Test]
        public void Called_WithEmptyCommandLine_ReturnsMainWindowStartUp()
        {
            string[] args = { };

            _appStartFactory.CreateApplicationStart(args);

            _resolver.Received().ResolveAppStart<MainWindowStart>();
        }

        [Test]
        public void Called_WithFilesAndNonsenseParameter_ReturnsMainWindowStart()
        {
            string[] args = { @"\RootedFile1.xxx", "/NonSenseParameter", @"\RootedFile.xxx", @"\RootedFile2.xxx" };

            _appStartFactory.CreateApplicationStart(args);

            _resolver.Received().ResolveAppStart<MainWindowStart>();
        }

        [Test]
        public void Called_WithInitializeSettings_ReturnsInitializeJobStart()
        {
            string[] args = { "-InitializeSettings" };

            var appStart = _appStartFactory.CreateApplicationStart(args);

            _resolver.Received().ResolveAppStart<InitializeSettingsStart>();
        }

        [Test]
        public void Called_WithJobInfo_ReturnsNewPrintJobStart()
        {
            var jobFile = @"C:\test.inf";
            string[] args = { $"/InfoDataFile={jobFile}" };

            var appStart = _appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<NewPrintJobStart>(appStart);
            Assert.AreEqual(jobFile, ((NewPrintJobStart)appStart).NewJobInfoFile, "Wrong JobInfoFile.");
        }

        [Test]
        public void Called_WithJobInfoAndInitializeSettings_ReturnsNewPrintJobStart()
        {
            var jobFile = @"C:\test.inf";
            string[] args = { $"/InfoDataFile={jobFile}", "/InitializeSettings" };

            var appStart = _appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<NewPrintJobStart>(appStart);
        }

        [Test]
        public void Called_WithJobInfoAndManagePrintJobs_ReturnsNewPrintJobStartWithManagePrintJobs()
        {
            var jobFile = @"C:\test.inf";
            string[] args = { $"-InfoDataFile={jobFile}", "-ManagePrintJobs" };

            var appStart = _appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<NewPrintJobStart>(appStart);
            Assert.IsTrue(((NewPrintJobStart)appStart).AppStartParameters.ManagePrintJobs);
        }

        [Test]
        public void Called_WithManagePrintJobs_ReturnsManagePrintJobsStartUp()
        {
            string[] args = { "/ManagePrintJobs" };

            var appStart = _appStartFactory.CreateApplicationStart(args);

            var maybePipedStart = appStart as MaybePipedStart;

            Assert.IsTrue(maybePipedStart.AppStartParameters.ManagePrintJobs);
        }

        [Test]
        public void Called_WithNonSense_ReturnsMainWindowStartUp()
        {
            string[] args = { "NonSense123" };

            _appStartFactory.CreateApplicationStart(args);

            _resolver.Received().ResolveAppStart<MainWindowStart>();
        }

        [Test]
        public void Called_WithNonSenseParameter_ReturnsMainWindowStartUp()
        {
            string[] args = { "/NonSenseParameter123" };

            _appStartFactory.CreateApplicationStart(args);

            _resolver.Received().ResolveAppStart<MainWindowStart>();
        }

        [Test]
        public void Called_WithNonSenseParameterHyphen_ReturnsMainWindowStartUp()
        {
            string[] args = { "-NonSenseParameter123" };

            _appStartFactory.CreateApplicationStart(args);

            _resolver.Received().ResolveAppStart<MainWindowStart>();
        }

        [Test]
        public void Called_WithNotRootedFilePath_ReturnsMainWindowStart()
        {
            string[] args = { "NotRootedFile.xxx" };

            _appStartFactory.CreateApplicationStart(args);

            _resolver.Received().ResolveAppStart<MainWindowStart>();
        }

        [Test]
        public void Called_WithPdfFile_ReturnsNewPsJobStart()
        {
            var jobFile = @"C:\test.pdf";
            string[] args = { $"/PdfFile={jobFile}" };

            var appStart = _appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<DirectConversionStart>(appStart);
            Assert.AreEqual(jobFile, ((DirectConversionStart)appStart).DirectConversionFiles[0], "Wrong File in DirectConversionStart.");
        }

        [Test]
        public void Called_WithPdfFileAndOutputFileParameter_SetsOutputFilePathInDirectConversionStart()
        {
            var outputFilePath = "D:\\TestOutputFile.txt";
            var jobFile = @"C:\test.pdf";
            string[] args = { $"/PdfFile={jobFile}", @"/Outputfile=" + outputFilePath };

            var appStart = _appStartFactory.CreateApplicationStart(args);
            var newPdfJobStart = (DirectConversionStart)appStart;

            Assert.AreEqual(outputFilePath, newPdfJobStart.AppStartParameters.OutputFile);
        }

        [Test]
        public void Called_WithPdfFileAndProfileParameter_SetsProfileInDirectConversionStart()
        {
            var profile = "SomeFancyProfile";
            var jobFile = @"C:\test.pdf";
            string[] args = { $"/PdfFile={jobFile}", @"/Profile=" + profile };

            var appStart = _appStartFactory.CreateApplicationStart(args);
            var newPdfJobStart = (DirectConversionStart)appStart;

            Assert.AreEqual(profile, newPdfJobStart.AppStartParameters.Profile);
        }

        [Test]
        public void Called_WithPsFileAndOutputFileParameter_SetsOutputFilePathInDirectConversionStart()
        {
            var outputFilePath = "D:\\TestOutputFile.txt";
            var jobFile = @"C:\test.pdf";
            string[] args = { $"/PsFile={jobFile}", @"/Outputfile=" + outputFilePath };

            var appStart = _appStartFactory.CreateApplicationStart(args);
            var newPsJobStart = (DirectConversionStart)appStart;

            Assert.AreEqual(outputFilePath, newPsJobStart.AppStartParameters.OutputFile);
        }

        [Test]
        public void Called_WithPsFileAndProfileParameter_SetsProfileInDirectConversionStart()
        {
            var profile = "SomeFancyProfile";
            var jobFile = @"C:\test.pdf";
            string[] args = { $"/PsFile={jobFile}", @"/Profile=" + profile };

            var appStart = _appStartFactory.CreateApplicationStart(args);
            var newPsJobStart = (DirectConversionStart)appStart;

            Assert.AreEqual(profile, newPsJobStart.AppStartParameters.Profile);
        }

        [Test]
        public void Called_WithPdfFileAndManagePrintJobs_ReturnsNewPrintJobStartWithManagePrintJobs()
        {
            var jobFile = @"C:\test.pdf";
            string[] args = { $"/PdfFile={jobFile}", "/ManagePrintJobs" };

            var appStart = _appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<DirectConversionStart>(appStart);
            Assert.IsTrue(((DirectConversionStart)appStart).AppStartParameters.ManagePrintJobs);
        }

        [Test]
        public void Called_WithPrintFileAndPrinterParameter_ReturnsPrintFileStartUpWithCorrectPrinter()
        {
            string[] args = { "/PrintFile=C:\\Test.txt", "/PrinterName=TestPrinter" };

            var appStart = _appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<PrintFileStart>(appStart);
            Assert.AreEqual("TestPrinter", ((PrintFileStart)appStart).AppStartParameters.Printer);
        }

        [Test]
        public void Called_WithPrintFileParameter_ReturnsPrintFileStartUp()
        {
            string[] args = { "/PrintFile=C:\\Test.txt" };

            var appStart = _appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<PrintFileStart>(appStart);
            Assert.AreEqual("C:\\Test.txt", ((PrintFileStart)appStart).PrintFile);
        }

        [Test]
        public void Called_WithPsFile_ReturnsDirectConversionJobStart()
        {
            var jobFile = @"C:\test.ps";
            string[] args = { $"/PsFile={jobFile}" };

            var appStart = _appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<DirectConversionStart>(appStart);
            Assert.AreEqual(jobFile, ((DirectConversionStart)appStart).DirectConversionFiles[0], "Wrong File in DirectConversionStart.");
        }

        [Test]
        public void Called_WithPsFileAndManagePrintJobs_ReturnsDirectConversionJobStartWithManagePrintJobs()
        {
            var jobFile = @"C:\test.ps";
            string[] args = { $"/PsFile={jobFile}", "/ManagePrintJobs" };

            var appStart = _appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<DirectConversionStart>(appStart);
            Assert.IsTrue(((DirectConversionStart)appStart).AppStartParameters.ManagePrintJobs);
        }

        [Test]
        public void Called_WithInitializeCustomSettings_ReturnsInitializeCustomSettingsStart()
        {
            string[] args = { @"/InitializeDefaultSettings=SettingsFile.ini" };

            var appStart = _appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<InitializeDefaultSettingsStart>(appStart);
            Assert.AreEqual("SettingsFile.ini", ((InitializeDefaultSettingsStart)appStart).SettingsFile);
        }

        [Test]
        public void Called_WithInitializeCustomSettingsWithoutSettingsFile_ReturnsInitializeCustomSettingsStartWithSettingsFileNull()
        {
            string[] args = { @"/InitializeDefaultSettings" };

            var appStart = _appStartFactory.CreateApplicationStart(args);

            Assert.IsAssignableFrom<InitializeDefaultSettingsStart>(appStart);
            Assert.AreEqual(null, ((InitializeDefaultSettingsStart)appStart).SettingsFile);
        }

        [Test]
        public void Called_WithStoreLicenseForAllUsers_ReturnsStoreLicenseForAllUsersStart()
        {
            string[] args = { @"/StoreLicenseForAllUsers" };

            _appStartFactory.CreateApplicationStart(args);

            _resolver.Received().ResolveAppStart<StoreLicenseForAllUsersStart>();
        }

        [Test]
        public void Called_WithStoreLicenseForAllUsersWithLicenseServerCode_AppStartContainsCode()
        {
            var code = "abcdefg";
            var licenseKey = "MY-KEY";

            string[] args = { @"/StoreLicenseForAllUsers", @"/LicenseServerCode=" + code, @"/LicenseKey=" + licenseKey };

            var appStart = (StoreLicenseForAllUsersStart)_appStartFactory.CreateApplicationStart(args);

            Assert.AreEqual(code, appStart.LicenseServerCode);
            Assert.AreEqual(licenseKey, appStart.LicenseKey);
        }
    }
}
