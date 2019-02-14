using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Tokens;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using SystemInterface;
using SystemInterface.IO;
using SystemWrapper;
using SystemWrapper.IO;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Jobs.Jobs
{
    [TestFixture]
    internal class TokenReplacerFactoryTest
    {
        [SetUp]
        public void SetUp()
        {
            _jobInfo = new JobInfo();

            var sourceFileInfo = new SourceFileInfo();
            sourceFileInfo.ClientComputer = "someComputer";
            sourceFileInfo.JobCounter = 3;
            sourceFileInfo.JobId = 14;
            sourceFileInfo.PrinterName = "SomePrinter";
            sourceFileInfo.SessionId = 121;
            _jobInfo.SourceFiles.Add(sourceFileInfo);

            var metadata = new Metadata();
            metadata.PrintJobAuthor = "someAuthor";
            metadata.PrintJobName = "somePrintJobName";
            metadata.Author = "someAuthor";
            metadata.Title = "someTitle";
            metadata.Subject = "someSubject";
            metadata.Keywords = "SomeKeyword";
            _jobInfo.Metadata = metadata;

            _tokenReplacerFactory = new TokenReplacerFactory(new DateTimeProvider(), new EnvironmentWrap(), new PathWrap(), new PathUtil(new PathWrap(), new DirectoryWrap()));

            _pathUtil = Substitute.For<IPathUtil>();

            _environment = MockRepository.GenerateStub<IEnvironment>();
            _environment.Stub(e => e.UserName).Return("someUser");
            _environment.Stub(e => e.MachineName).Return("someMachine");

            var dateTime = MockRepository.GenerateStub<IDateTimeProvider>();
            dateTime.Stub(dt => dt.Now()).Return(new DateTime(2000, 1, 1, 1, 1, 1));

            _job = new Job(_jobInfo, new ConversionProfile(), new Accounts());
            _job.NumberOfCopies = 5;
            _job.NumberOfPages = 14;

            _job.OutputFiles = new List<string> { "outputFile", "test" };

            var path = MockRepository.GenerateStub<IPath>();
            path.Stub(p => p.GetFullPath("outputFile")).Return("fullPath");
            path.Stub(p => p.GetFileNameWithoutExtension("C:\\thedir\\thefile.txt")).Return("thefile");
            path.Stub(p => p.GetDirectoryName("C:\\thedir\\thefile.txt")).Return("C:\\thedir");
            path.Stub(p => p.GetFileName("")).IgnoreArguments().Return("file");

            _tokenReplacerFactory = new TokenReplacerFactory(dateTime, _environment, path, _pathUtil);
        }

        private JobInfo _jobInfo;
        private TokenReplacerFactory _tokenReplacerFactory;
        private IPathUtil _pathUtil;
        private IEnvironment _environment;
        private Job _job;

        [TestCase("ClientComputer", ExpectedResult = "someComputer")]
        [TestCase("Counter", ExpectedResult = "3")]
        [TestCase("JobId", ExpectedResult = "14")]
        [TestCase("PrinterName", ExpectedResult = "SomePrinter")]
        [TestCase("SessionId", ExpectedResult = "121")]
        [TestCase("PrintJobAuthor", ExpectedResult = "someAuthor")]
        [TestCase("PrintJobName", ExpectedResult = "somePrintJobName")]
        [TestCase("Username", ExpectedResult = "someUser")]
        [TestCase("ComputerName", ExpectedResult = "someMachine")]
        [TestCase("Author", ExpectedResult = "someAuthor")]
        [TestCase("Title", ExpectedResult = "someTitle")]
        public string BuildTokenReplacerFromJobInfo_ReturnsCorrectTokenValues(string tokenName)
        {
            var tokenReplacer = _tokenReplacerFactory.BuildTokenReplacerFromJobInfo(_jobInfo);
            var token = tokenReplacer.GetToken(tokenName);
            return token.GetValue();
        }

        [TestCase("Author")]
        [TestCase("Title")]
        [TestCase("DateTime")]
        [TestCase("ComputerName")]
        [TestCase("Username")]
        [TestCase("Environment")]
        [TestCase("ClientComputer")]
        [TestCase("Counter")]
        [TestCase("JobId")]
        [TestCase("PrinterName")]
        [TestCase("SessionId")]
        [TestCase("PrintJobAuthor")]
        [TestCase("PrintJobName")]
        [TestCase("InputFilename")]
        [TestCase("InputDirectory")]
        public void BuildTokenReplacerWithoutOutputfiles_ReturnsTokenReplacerWithJobTokens(string tokenName)
        {
            var tokenreplacer = _tokenReplacerFactory.BuildTokenReplacerWithoutOutputfiles(_job);
            var tokenList = tokenreplacer.GetTokenNames(false);

            Assert.Contains(tokenName, tokenList);
        }

        [TestCase("NumberOfCopies", ExpectedResult = "5")]
        [TestCase("NumberOfPages", ExpectedResult = "14")]
        public string BuildTokenReplacerWithoutOutputfiles_ReturnsCorrectTokenValues(string tokenName)
        {
            var tokenreplacer = _tokenReplacerFactory.BuildTokenReplacerWithoutOutputfiles(_job);
            var token = tokenreplacer.GetToken(tokenName);
            return token.GetValue();
        }

        [TestCase("OutputFilePath", ExpectedResult = "fullPath")]
        [TestCase("OutputFilenames", ExpectedResult = "file, file")]
        public string BuildTokenReplacerWithOutputfiles_ReturnsCorrectTokenValues(string tokenName)
        {
            var tokenreplacer = _tokenReplacerFactory.BuildTokenReplacerWithOutputfiles(_job);
            var token = tokenreplacer.GetToken(tokenName);
            return token.GetValue();
        }

        [Test]
        public void BuildTokenReplacerFromJobInfo_IfAuthorContainsTokens_ReturnsTokenReplacerWithCorrectAuthorToken()
        {
            _jobInfo.Metadata.Author = "<PrinterName>";
            var tokenReplacer = _tokenReplacerFactory.BuildTokenReplacerFromJobInfo(_jobInfo);
            var token = tokenReplacer.GetToken("Author");

            Assert.AreEqual(_jobInfo.SourceFiles[0].PrinterName, token.GetValue());
        }

        [Test]
        public void BuildTokenReplacerFromJobInfo_IfAuthorContainsTokens_ReturnsTokenReplacerWithCorrectTitleToken()
        {
            _jobInfo.Metadata.Title = "<PrinterName>";
            var tokenReplacer = _tokenReplacerFactory.BuildTokenReplacerFromJobInfo(_jobInfo);
            var token = tokenReplacer.GetToken("Title");

            Assert.AreEqual(_jobInfo.SourceFiles[0].PrinterName, token.GetValue());
        }

        [Test]
        public void BuildTokenReplacerFromJobInfo_ISubjectContainsTokens_ReturnsTokenReplacerWithCorrectTitleToken()
        {
            _jobInfo.Metadata.Subject = "<PrinterName>";

            var tokenReplacer = _tokenReplacerFactory.BuildTokenReplacerFromJobInfo(_jobInfo);

            var token = tokenReplacer.GetToken("Subject");
            Assert.AreEqual(_jobInfo.SourceFiles[0].PrinterName, token.GetValue());
        }

        [Test]
        public void BuildTokenReplacerFromJobInfo_IfKeywordsContainsTokens_ReturnsTokenReplacerWithCorrectTitleToken()
        {
            _jobInfo.Metadata.Keywords = "<PrinterName>";
            var tokenReplacer = _tokenReplacerFactory.BuildTokenReplacerFromJobInfo(_jobInfo);
            var token = tokenReplacer.GetToken("Keywords");

            Assert.AreEqual(_jobInfo.SourceFiles[0].PrinterName, token.GetValue());
        }

        [Test]
        public void BuildTokenReplacerFromJobInfo_EmptyOriginalFilePathDocumentInfoIsNoValidRootedPath_InputFilenameTokenHasPrintJobName()
        {
            _jobInfo.OriginalFilePath = "";
            _jobInfo.SourceFiles[0].DocumentTitle = "InvalidPath";
            _pathUtil.IsValidRootedPath(_jobInfo.SourceFiles[0].DocumentTitle).Returns(false);
            _jobInfo.Metadata.PrintJobName = "PrintJobName";

            var tokenReplacer = _tokenReplacerFactory.BuildTokenReplacerFromJobInfo(_jobInfo);

            var inputFilenameToken = tokenReplacer.GetToken("InputFilename");
            Assert.AreEqual(_jobInfo.Metadata.PrintJobName, inputFilenameToken.GetValue(), "Wrong value for InputFilename");
        }

        [Test]
        public void BuildTokenReplacerFromJobInfo_EmptyOriginalFilePathDocumentInfoIsNoValidRootedPath_InputDirectoryTokenIsEmpty()
        {
            _jobInfo.OriginalFilePath = "";
            _jobInfo.SourceFiles[0].DocumentTitle = "InvalidPath";
            _pathUtil.IsValidRootedPath(_jobInfo.SourceFiles[0].DocumentTitle).Returns(false);
            _jobInfo.Metadata.PrintJobName = "PrintJobName";

            var tokenReplacer = _tokenReplacerFactory.BuildTokenReplacerFromJobInfo(_jobInfo);

            var inputDirectoryToken = tokenReplacer.GetToken("InputDirectory");
            Assert.AreEqual("", inputDirectoryToken.GetValue(), "Wrong value for InputDirectory");
            var inputFilePathToken = tokenReplacer.GetToken("InputFilePath");
            Assert.AreEqual(inputDirectoryToken.GetValue(), inputFilePathToken.GetValue(), "Values of InputDirectory- and InputFilePath-Token must be equal");
        }

        [Test]
        public void BuildTokenReplacerFromJobInfo_OriginalFilePathIsSetDocumentInfoIsValidRootedPath_InputFilenameTokenHasOriginalFilenameWithoutExtension()
        {
            _jobInfo.OriginalFilePath = @"\\OriginalDirectory\OriginalFilename.jpg";
            _jobInfo.SourceFiles[0].DocumentTitle = "InvalidPath";
            _pathUtil.IsValidRootedPath(_jobInfo.SourceFiles[0].DocumentTitle).Returns(true);
            _jobInfo.Metadata.PrintJobName = "PrintJobName";

            var tokenReplacer = _tokenReplacerFactory.BuildTokenReplacerFromJobInfo(_jobInfo);

            var inputFilenameToken = tokenReplacer.GetToken("InputFilename");
            Assert.AreEqual(Path.GetFileNameWithoutExtension(_jobInfo.OriginalFilePath), inputFilenameToken.GetValue(), "Wrong value for InputFilename");
        }

        [Test]
        public void BuildTokenReplacerFromJobInfo_OriginalFilePathIsSetDocumentInfoIsValidRootedPath_InputDirectoryTokenHasOriginalDirectory()
        {
            _jobInfo.OriginalFilePath = @"\\Network\OriginalDirectory\OriginalFilename.jpg";
            _jobInfo.SourceFiles[0].DocumentTitle = "InvalidPath";
            _pathUtil.IsValidRootedPath(_jobInfo.SourceFiles[0].DocumentTitle).Returns(true);
            _jobInfo.Metadata.PrintJobName = "PrintJobName";

            var tokenReplacer = _tokenReplacerFactory.BuildTokenReplacerFromJobInfo(_jobInfo);

            var inputDirectoryToken = tokenReplacer.GetToken("InputDirectory");
            Assert.AreEqual(Path.GetDirectoryName(_jobInfo.OriginalFilePath), inputDirectoryToken.GetValue(), "Wrong value for InputDirectory");
            var inputFilePathToken = tokenReplacer.GetToken("InputFilePath");
            Assert.AreEqual(inputDirectoryToken.GetValue(), inputFilePathToken.GetValue(), "Values of InputDirectory- and InputFilePath-Token must be equal");
        }

        [Test]
        public void BuildTokenReplacerFromJobInfo_OriginalFilePathIsNotSetDocumentInfoIsValidRootedPath_InputFilenameTokenHasCocumentTitleFilenameWithoutExtension()
        {
            _jobInfo.OriginalFilePath = "";
            _jobInfo.SourceFiles[0].DocumentTitle = @"\\DocumentTitleDirectory\DocumentTitleFilename.jpg";
            _pathUtil.IsValidRootedPath(_jobInfo.SourceFiles[0].DocumentTitle).Returns(true);
            _jobInfo.Metadata.PrintJobName = "PrintJobName";

            var tokenReplacer = _tokenReplacerFactory.BuildTokenReplacerFromJobInfo(_jobInfo);

            var inputFilenameToken = tokenReplacer.GetToken("InputFilename");
            Assert.AreEqual(Path.GetFileNameWithoutExtension(_jobInfo.SourceFiles[0].DocumentTitle), inputFilenameToken.GetValue(), "Wrong value for InputFilename");
        }

        [Test]
        public void BuildTokenReplacerFromJobInfo_OriginalFilePathIsNotSetDocumentInfoIsValidRootedPath_InputDirectoryTokenHasDocumentTitleDirectory()
        {
            _jobInfo.OriginalFilePath = "";
            _jobInfo.SourceFiles[0].DocumentTitle = @"\\Network\DocumentTitleDirectory\DocumentTitleFilename.jpg";
            _pathUtil.IsValidRootedPath(_jobInfo.SourceFiles[0].DocumentTitle).Returns(true);
            _jobInfo.Metadata.PrintJobName = "PrintJobName";

            var tokenReplacer = _tokenReplacerFactory.BuildTokenReplacerFromJobInfo(_jobInfo);

            var inputDirectoryToken = tokenReplacer.GetToken("InputDirectory");
            var expectedDirectory = Path.GetDirectoryName(_jobInfo.SourceFiles[0].DocumentTitle);
            Assert.AreEqual(expectedDirectory, inputDirectoryToken.GetValue(), "Wrong value for InputDirectory");
            var inputFilePathToken = tokenReplacer.GetToken("InputFilePath");
            Assert.AreEqual(inputDirectoryToken.GetValue(), inputFilePathToken.GetValue(), "Values of InputDirectory- and InputFilePath-Token must be equal");
        }

        [Test]
        public void BuildTokenReplacerFromJobInfo_ReturnsCorrectTokenValueForDateTime()
        {
            var expectedDate = new DateTime(2000, 01, 01, 01, 01, 01);

            var tokenReplacer = _tokenReplacerFactory.BuildTokenReplacerFromJobInfo(_jobInfo);
            var token = tokenReplacer.GetToken("DateTime");
            Assert.AreEqual(expectedDate.ToString(CultureInfo.CurrentUICulture), token.GetValue());
        }

        [Test]
        public void BuildTokenReplacerFromJobInfo_ReturnsTokenReplacerWithEnvironmentToken()
        {
            var tokenReplacer = _tokenReplacerFactory.BuildTokenReplacerFromJobInfo(_jobInfo);
            var token = tokenReplacer.GetToken("Environment");

            Assert.IsTrue(token is EnvironmentToken);
        }
    }
}
