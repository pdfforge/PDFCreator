using NUnit.Framework;
using PDFCreator.TestUtilities.Properties;
using pdfforge.PDFCreator.Conversion.Ghostscript;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.ITextProcessing;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Services.Logging;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Core.Workflow.Output;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SystemWrapper;
using SystemWrapper.IO;

namespace PDFCreator.TestUtilities
{
    public class TestHelper
    {
        public const string SpecialCharactersString = "Täßting# Spècînál ChârÜcktérs iİıI вѣдѣ вѣди дūпт 形音碼她们它们";

        //default passwords
        private const string UserPassword = "User";

        private const string OwnerPassword = "Owner";
        private const string SignaturePassword = "Test1";
        private readonly IGhostscriptDiscovery _ghostscriptDiscovery;
        private readonly IJobRunner _jobRunner;

        private ConversionProfile _profile;

        public string TmpInfFile;
        public List<string> TmpPsFiles;

        public TestHelper(IJobRunner jobRunner)
        {
            _jobRunner = jobRunner;
            _ghostscriptDiscovery = new PaketGhostscriptDiscovery(new AssemblyHelper(GetType().Assembly));
            _accounts = new Accounts();
        }

        public string TmpTestFolder { get; set; }

        public ApplicationSettings AppSettings { get; set; }

        public ConversionProfile Profile
        {
            get { return Job != null ? Job.Profile : _profile; }
            set
            {
                if (Job != null)
                    Job.Profile = value;
                else
                    _profile = value;
            }
        }

        /// <summary>
        ///     Note: You first have to generate a Job!
        /// </summary>
        public Job Job { get; set; }

        /// <summary>
        ///     Note: You first have to generate a Job!
        /// </summary>
        public JobInfo JobInfo { get; set; }

        private Accounts _accounts;

        public void SetAccounts(Accounts accounts)
        {
            _accounts = accounts;
        }

        public void InitTempFolder(string testName)
        {
            TmpTestFolder = TempFileHelper.CreateTempFolder("PdfCreatorTest\\" + testName);

            AppSettings = new ApplicationSettings();
            Profile = new ConversionProfile();
            Profile.OpenViewer = false;
            Profile.ShowProgress = false;
        }

        private void DoCleanUp()
        {
            if (Directory.Exists(TmpTestFolder))
                Directory.Delete(TmpTestFolder, true);

            TempFileHelper.CleanUp();
        }

        public void CleanUp()
        {
            Retry.Do(DoCleanUp, retryInterval: TimeSpan.FromMilliseconds(250), retryCount: 10);
        }

        private List<string> GeneratePSFileList(PSfiles psFiles, string tmpTestFolder)
        {
            var list = new List<string>();
            string testFilePath;

            switch (psFiles)
            {
                case PSfiles.ElevenTextPages:
                    testFilePath = Path.Combine(tmpTestFolder, psFiles + ".ps");
                    File.WriteAllBytes(testFilePath, Resources.ElevenTextPagesPS);
                    list.Add(testFilePath);
                    break;

                case PSfiles.EmptyPage:
                    testFilePath = Path.Combine(tmpTestFolder, psFiles + ".ps");
                    File.WriteAllBytes(testFilePath, Resources.EmptyPagePS);
                    list.Add(testFilePath);
                    break;

                case PSfiles.LandscapePage:
                    testFilePath = Path.Combine(tmpTestFolder, psFiles + ".ps");
                    File.WriteAllBytes(testFilePath, Resources.LandscapePagePS);
                    list.Add(testFilePath);
                    break;

                case PSfiles.PDFCreatorTestpage:
                    testFilePath = Path.Combine(tmpTestFolder, psFiles + ".ps");
                    File.WriteAllBytes(testFilePath, Resources.PDFCreatorTestpagePS);
                    list.Add(testFilePath);
                    break;

                case PSfiles.ThreePDFCreatorTestpages:
                    testFilePath = Path.Combine(tmpTestFolder, "PDFCreatorTestpage.ps");
                    File.WriteAllBytes(testFilePath, Resources.PDFCreatorTestpagePS);
                    list.Add(testFilePath);
                    list.Add(testFilePath);
                    list.Add(testFilePath);
                    break;

                case PSfiles.PortraitLandscapeLandscapeLandscapePortrait:
                    var portrait = Path.Combine(tmpTestFolder, "Portrait.ps");
                    File.WriteAllBytes(portrait, Resources.PortraitPagePS);
                    var landscape = Path.Combine(tmpTestFolder, "Landscape.ps");
                    File.WriteAllBytes(landscape, Resources.LandscapePagePS);
                    list.Add(portrait);
                    list.Add(landscape);
                    list.Add(landscape);
                    list.Add(landscape);
                    list.Add(portrait);
                    break;

                case PSfiles.PortraitPage:
                    testFilePath = Path.Combine(tmpTestFolder, psFiles + ".ps");
                    File.WriteAllBytes(testFilePath, Resources.PortraitPagePS);
                    list.Add(testFilePath);
                    break;

                case PSfiles.SixEmptyPages:
                    testFilePath = Path.Combine(TmpTestFolder, "EmptyPage.ps");
                    File.WriteAllBytes(testFilePath, Resources.EmptyPagePS);
                    list.Add(testFilePath);
                    list.Add(testFilePath);
                    list.Add(testFilePath);
                    list.Add(testFilePath);
                    list.Add(testFilePath);
                    list.Add(testFilePath);
                    break;
            }
            return list;
        }

        public void AddOutputFileToJob(TestFile file, string fileName = "")
        {
            Job.OutputFiles.Add(GenerateTestFile(file, fileName));
        }

        public void ResetProfileToDefault()
        {
            _profile = new ConversionProfile();
            _profile.OpenViewer = false;
        }

        /// <summary>
        ///     Generates a job with default testpasswords (if required by the profile settings).
        ///     Therefore an INF file and the required PS Files will be created and set in the jobs JobInfo.
        /// </summary>
        /// <param name="psFiles">select test content according to psFiles</param>
        /// <param name="outputformat">set output format</param>
        public void GenerateGsJob(PSfiles psFiles, OutputFormat outputformat)
        {
            _profile.OutputFormat = outputformat;

            GenerateInfFileWithPsFiles(psFiles);
            var jobInfoReader = new JobInfoManager(new LocalTitleReplacerProvider(new List<TitleReplacement>()), null);
            JobInfo = jobInfoReader.ReadFromInfFile(TmpInfFile);

            Job = new Job(JobInfo, _profile, _accounts);

            var extension = outputformat.ToString();
            if (outputformat == OutputFormat.PdfA1B || outputformat == OutputFormat.PdfA2B || outputformat == OutputFormat.PdfA3B || outputformat == OutputFormat.PdfX)
                extension = "pdf";
            Job.OutputFileTemplate = TmpInfFile.Replace(".inf", "." + extension);
            Job.Passwords.PdfUserPassword = _profile.PdfSettings.Security.RequireUserPassword ? UserPassword : null;
            Job.Passwords.PdfOwnerPassword = _profile.PdfSettings.Security.Enabled ? OwnerPassword : null;
            Job.Passwords.PdfSignaturePassword = _profile.PdfSettings.Signature.Enabled ? SignaturePassword : null;

            EnableLogging();
            InitMissingData();
        }

        /// <summary>
        ///     Generates a job with default testpasswords (if required by the profile settings)
        ///     and sets a tesfile as output, without running it.
        ///     Note: The INF- and the PS file will be created with the content of the PDFCreatorTestpage
        /// </summary>
        /// <param name="testFileAsOutput">Testfile setted as output</param>
        ///
        public void GenerateGsJob_WithSetOutput(TestFile testFileAsOutput)
        {
            GenerateGsJob_WithSetOutput(testFileAsOutput, testFileAsOutput.ToString());
        }

        public void GenerateGsJob_WithSetOutput(TestFile testFileAsOutput, string fileName)
        {
            var testFile = GenerateTestFile(testFileAsOutput, fileName);

            switch (testFileAsOutput)
            {
                case TestFile.PDFCreatorTestpageJPG:
                    _profile.OutputFormat = OutputFormat.Jpeg;
                    GenerateGsJob(PSfiles.PDFCreatorTestpage, _profile.OutputFormat);
                    break;

                case TestFile.Cover2PagesSixEmptyPagesPDF:
                case TestFile.Cover2PagesSixEmptyPagesAttachment3PagesPDF:
                case TestFile.SixEmptyPagesPDF:
                case TestFile.SixEmptyPagesAttachment3PagesPDF:
                    _profile.OutputFormat = OutputFormat.Pdf;
                    GenerateGsJob(PSfiles.SixEmptyPages, _profile.OutputFormat);
                    break;

                case TestFile.PDFCreatorTestpage_GS9_19_PDF_A_1b:
                    _profile.OutputFormat = OutputFormat.PdfA1B;
                    GenerateGsJob(PSfiles.PDFCreatorTestpage, _profile.OutputFormat);
                    break;

                case TestFile.PDFCreatorTestpage_GS9_19_PDF_A_2b:
                    _profile.OutputFormat = OutputFormat.PdfA2B;
                    GenerateGsJob(PSfiles.PDFCreatorTestpage, _profile.OutputFormat);
                    break;

                case TestFile.PDFCreatorTestpage_GS9_19_PDF_X:
                    _profile.OutputFormat = OutputFormat.PdfX;
                    GenerateGsJob(PSfiles.PDFCreatorTestpage, _profile.OutputFormat);
                    break;

                case TestFile.PDFCreatorTestpageTIF:
                    _profile.OutputFormat = OutputFormat.Tif;
                    GenerateGsJob(PSfiles.PDFCreatorTestpage, _profile.OutputFormat);
                    break;

                case TestFile.PDFCreatorTestpageTXT:
                    _profile.OutputFormat = OutputFormat.Txt;
                    GenerateGsJob(PSfiles.PDFCreatorTestpage, _profile.OutputFormat);
                    break;

                case TestFile.PDFCreatorTestpagePNG:
                    _profile.OutputFormat = OutputFormat.Png;
                    GenerateGsJob(PSfiles.PDFCreatorTestpage, _profile.OutputFormat);
                    break;
                // ReSharper disable once RedundantCaseLabel
                case TestFile.PDFCreatorTestpage_GS9_19_PDF:
                default:
                    GenerateGsJob(PSfiles.PDFCreatorTestpage, _profile.OutputFormat);
                    break;
            }

            Job.OutputFiles = Job.TempOutputFiles;
            Job.TempOutputFiles.Clear();
            Job.TempOutputFiles.Add(testFile);
            Job.OutputFileTemplate = testFile;
        }

        /// <summary>
        /// </summary>
        /// <param name="psFiles"></param>
        /// <returns></returns>
        public string GenerateInfFileWithPsFiles(PSfiles psFiles)
        {
            TmpInfFile = Path.Combine(TmpTestFolder, psFiles + ".inf");

            TmpPsFiles = GeneratePSFileList(psFiles, TmpTestFolder);

            var sb = new StringBuilder();

            for (var i = 1; i <= TmpPsFiles.Count; i++)
            {
                sb.AppendLine("[" + i + "]");
                sb.AppendLine("SessionId=1");
                sb.AppendLine("WinStation=Console");
                sb.AppendLine("UserName=SampleUser1234");
                sb.AppendLine("ClientComputer=\\PC1");
                sb.AppendLine("SpoolFileName=" + TmpPsFiles[i - 1]);
                sb.AppendLine("PrinterName=PDFCreator");
                sb.AppendLine("JobId=1");
                sb.AppendLine("DocumentTitle=Title");
                AddTotalPagesAndCopies(psFiles, sb);
                sb.AppendLine("");
            }

            File.WriteAllText(TmpInfFile, sb.ToString(), Encoding.GetEncoding("Unicode"));

            return TmpInfFile;
        }

        private void AddTotalPagesAndCopies(PSfiles psFiles, StringBuilder sb)
        {
            switch (psFiles)
            {
                case PSfiles.ElevenTextPages:
                    sb.AppendLine("TotalPages=11");
                    sb.AppendLine("Copies=1");
                    break;

                case PSfiles.EmptyPage:
                case PSfiles.LandscapePage:
                case PSfiles.PortraitPage:
                case PSfiles.PDFCreatorTestpage:
                    sb.AppendLine("Copies=1");
                    sb.AppendLine("TotalPages=1");
                    break;

                case PSfiles.PortraitLandscapeLandscapeLandscapePortrait:
                    sb.AppendLine("Copies=1");
                    sb.AppendLine("TotalPages=1");
                    break;

                case PSfiles.SixEmptyPages:
                    sb.AppendLine("Copies=6");
                    sb.AppendLine("TotalPages=1");
                    break;

                case PSfiles.ThreePDFCreatorTestpages:
                    sb.AppendLine("Copies=3");
                    sb.AppendLine("TotalPages=1");
                    break;
            }
        }

        private void EnableLogging()
        {
            LoggingHelper.InitConsoleLogger("PDFCreator-Test", LoggingLevel.Off);
        }

        public void RunGsJob()
        {
            SetUpGhostscript();
            InitMissingData();

            var outputFileMover = new AutosaveOutputFileMover(new DirectoryWrap(), new FileWrap(), new PathUtil(new PathWrap(), new DirectoryWrap()), new DirectoryHelper(new DirectoryWrap()));

            _jobRunner.RunJob(Job, outputFileMover);
        }

        private void InitMissingData()
        {
            var jobDataUpdater = new JobDataUpdater(new TokenReplacerFactory(new DateTimeProvider(), new EnvironmentWrap(), new PathWrap(), new PathUtil(new PathWrap(), new DirectoryWrap())), new PageNumberCalculator(new ITextPdfProcessor(new FileWrap())), new UserTokenExtractorDummy());

            jobDataUpdater.UpdateTokensAndMetadata(Job);

            //PDF-Tools does not set empty values when setting the XMP-Metadata for PDF/A.
            //Our way of testing requires all values to be set, so we need to set content.
            Job.JobInfo.Metadata.Title = "Test Title";
            Job.JobInfo.Metadata.Author = "Test Author";
            Job.JobInfo.Metadata.Keywords = "Test Keywords";
            Job.JobInfo.Metadata.Subject = "Test Subject";
        }

        private void SetUpGhostscript()
        {
            var gsVersion = _ghostscriptDiscovery.GetGhostscriptInstance();
            Assert.IsNotNull(gsVersion, "No Ghostscript instance found");
        }

        /// <summary>
        ///     Allows to set the template for the full file template without having to respecify the path
        /// </summary>
        /// <param name="filename"></param>
        public void SetFilenameTemplate(string filename)
        {
            var outputFolder = Path.GetDirectoryName(Job.OutputFileTemplate) ?? "";
            Job.OutputFileTemplate = Path.Combine(outputFolder, filename);
        }

        /// <summary>
        ///     Creates testfiles according to the TestFile enum and returns its path.
        ///     Uses testFile name for the file name
        /// </summary>
        /// <param name="testFile">Choose a testfile from the enum</param>
        /// <returns>Path to the created testfile</returns>
        public string GenerateTestFile(TestFile testFile)
        {
            return GenerateTestFile(testFile, testFile.ToString());
        }

        public string GenerateTestFile(TestFile testFile, string fileName)
        {
            var testFileName = fileName;
            if (string.IsNullOrWhiteSpace(testFileName))
            {
                testFileName = testFile.ToString();
            }

            var testfilePath = Path.Combine(TmpTestFolder, testFileName + ".pdf");

            switch (testFile)
            {
                case TestFile.Attachment3PagesPDF:
                    File.WriteAllBytes(testfilePath, Resources.Attachment3PagesPDF);
                    break;

                case TestFile.Background3PagesPDF:
                    File.WriteAllBytes(testfilePath, Resources.Background3PagesPDF);
                    break;

                case TestFile.CertificationFileP12:
                    testfilePath = testfilePath.Replace(".pdf", ".p12");
                    File.WriteAllBytes(testfilePath, Resources.CertificationFileP12);
                    break;

                case TestFile.CertificationFile_ExpiredP12:
                    testfilePath = testfilePath.Replace(".pdf", ".p12");
                    File.WriteAllBytes(testfilePath, Resources.CertificationFile_ExpiredP12);
                    break;

                case TestFile.Cover2PagesPDF:
                    File.WriteAllBytes(testfilePath, Resources.Cover2PagesPDF);
                    break;

                case TestFile.Cover2PagesSixEmptyPagesPDF:
                    File.WriteAllBytes(testfilePath, Resources.Cover2PagesSixEmptyPagesPDF);
                    break;

                case TestFile.Cover2PagesSixEmptyPagesAttachment3PagesPDF:
                    File.WriteAllBytes(testfilePath, Resources.Cover2PagesSixEmptyPagesAttachment3PagesPDF);
                    break;

                case TestFile.FourRotatingPDFCreatorTestpagesPDF:
                    File.WriteAllBytes(testfilePath, Resources.FourRotatingPDFCreatorTestpagesPDF);
                    break;

                case TestFile.PageRotation0PDF:
                    File.WriteAllBytes(testfilePath, Resources.PageRotation0PDF);
                    break;

                case TestFile.PageRotation180PDF:
                    File.WriteAllBytes(testfilePath, Resources.PageRotation180PDF);
                    break;

                case TestFile.PageRotation270PDF:
                    File.WriteAllBytes(testfilePath, Resources.PageRotation270PDF);
                    break;

                case TestFile.PageRotation90PDF:
                    File.WriteAllBytes(testfilePath, Resources.PageRotation90PDF);
                    break;

                case TestFile.PDFCreatorTestpage_GS9_19_PDF:
                    File.WriteAllBytes(testfilePath, Resources.PDFCreatorTestpage_GS9_19_PDF);
                    break;

                case TestFile.PDFCreatorTestpage_GS9_19_PDF_A_1b:
                    File.WriteAllBytes(testfilePath, Resources.PDFCreatorTestpage_GS9_19_PDF_A_1b);
                    break;

                case TestFile.PDFCreatorTestpage_GS9_19_PDF_A_2b:
                    File.WriteAllBytes(testfilePath, Resources.PDFCreatorTestpage_GS9_19_PDF_A_2b);
                    break;

                case TestFile.PDFCreatorTestpage_GS9_19_PDF_X:
                    File.WriteAllBytes(testfilePath, Resources.PDFCreatorTestpage_GS9_19_PDF_X);
                    break;

                case TestFile.PortraitLandscapeLandscapeLandscapePortraitPDF:
                    File.WriteAllBytes(testfilePath, Resources.PortraitLandscapeLandscapeLandscapePortraitPDF);
                    break;

                case TestFile.SixEmptyPagesPDF:
                    File.WriteAllBytes(testfilePath, Resources.SixEmptyPagesPDF);
                    break;

                case TestFile.SixEmptyPagesAttachment3PagesPDF:
                    File.WriteAllBytes(testfilePath, Resources.SixEmptyPagesAttachment3PagesPDF);
                    break;

                case TestFile.ScriptCopyFilesToDirectoryCMD:
                    testfilePath = testfilePath.Replace(".pdf", ".cmd");
                    File.WriteAllText(testfilePath, Resources.ScriptCopyFilesToDirectoryCMD);
                    break;

                case TestFile.ThreePDFCreatorTestpagesPDF:
                    File.WriteAllBytes(testfilePath, Resources.ThreePDFCreatorTestpagesPDF);
                    break;

                case TestFile.PDFCreatorTestpagePs:
                    testfilePath = testfilePath.Replace(".pdf", ".ps");
                    File.WriteAllBytes(testfilePath, Resources.PDFCreatorTestpagePS);
                    break;

                case TestFile.PDFCreatorTestpageJPG:
                    testfilePath = testfilePath.Replace(".pdf", ".jpg");
                    Resources.PDFCreatorTestpageJPG.Save(testfilePath.Replace(".pdf", ".jpg"));
                    break;

                case TestFile.PDFCreatorTestpageTIF:
                    testfilePath = testfilePath.Replace(".pdf", ".tif");
                    Resources.PDFCreatorTestpageTIF.Save(testfilePath);
                    break;

                case TestFile.PDFCreatorTestpageTXT:
                    testfilePath = testfilePath.Replace(".pdf", ".txt");
                    File.WriteAllText(testfilePath, Resources.PDFCreatorTestpageTXT);
                    break;

                case TestFile.PDFCreatorTestpagePNG:
                    testfilePath = testfilePath.Replace(".pdf", ".png");
                    Resources.PDFCreatorTestpagePNG.Save(testfilePath);

                    break;
            }

            return testfilePath;
        }
    }

    public enum TestFile
    {
        Attachment3PagesPDF,
        Background3PagesPDF,
        Cover2PagesPDF,
        Cover2PagesSixEmptyPagesPDF,
        Cover2PagesSixEmptyPagesAttachment3PagesPDF,
        CertificationFileP12,
        CertificationFile_ExpiredP12,
        FourRotatingPDFCreatorTestpagesPDF,
        PageRotation0PDF,
        PageRotation180PDF,
        PageRotation270PDF,
        PageRotation90PDF,
        PDFCreatorTestpage_GS9_19_PDF,
        PDFCreatorTestpage_GS9_19_PDF_A_1b,
        PDFCreatorTestpage_GS9_19_PDF_A_2b,
        PDFCreatorTestpage_GS9_19_PDF_X,
        PortraitLandscapeLandscapeLandscapePortraitPDF,
        SixEmptyPagesPDF,
        SixEmptyPagesAttachment3PagesPDF,
        ScriptCopyFilesToDirectoryCMD,
        ThreePDFCreatorTestpagesPDF,
        PDFCreatorTestpagePs,
        PDFCreatorTestpageJPG,
        PDFCreatorTestpageTIF,
        PDFCreatorTestpageTXT,
        PDFCreatorTestpagePNG
    }

    public enum PSfiles
    {
        ElevenTextPages,
        LandscapePage,
        PDFCreatorTestpage,
        ThreePDFCreatorTestpages,
        PortraitPage,
        PortraitLandscapeLandscapeLandscapePortrait,
        EmptyPage,
        SixEmptyPages
    }
}
