using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.UnitTest.Core.Workflow
{
    [TestFixture]
    public class JobUpdaterTest
    {
        private IJobDataUpdater _jobDataUpdater;
        private ITokenReplacerFactory _tokenReplacerFactory;
        private TokenReplacer _tokenReplacer;
        private const string AuthorTokenValue = "Author Token Value";
        private const string TitleTokenValue = "Title Token Value";
        private const string SubjectTokenValue = "Test Subject";
        private const string KeywordTokenValue = "Test Keywords";
        private IPageNumberCalculator _pageNumberCalculator;
        private IUserTokenExtractor _userTokenExtractor;
        private Job _job;
        private const string PSFile1 = "PSFile1.ps";
        private const string PSFile2 = "PSFile2.ps";
        private readonly UserToken _userToken1 = new UserToken();
        private readonly UserToken _userToken2 = new UserToken();

        [SetUp]
        public void SetUp()
        {
            _tokenReplacerFactory = Substitute.For<ITokenReplacerFactory>();
            _tokenReplacer = new TokenReplacer();
            _tokenReplacer.AddStringToken("author", AuthorTokenValue);
            _tokenReplacer.AddStringToken("title", TitleTokenValue);
            _tokenReplacerFactory.BuildTokenReplacerWithoutOutputfiles(Arg.Any<Job>()).Returns(_tokenReplacer);
            _pageNumberCalculator = Substitute.For<IPageNumberCalculator>();
            _userTokenExtractor = Substitute.For<IUserTokenExtractor>();

            _jobDataUpdater = new JobDataUpdater(_tokenReplacerFactory, _pageNumberCalculator, _userTokenExtractor);

            var jobInfo = new JobInfo();
            jobInfo.Metadata = new Metadata();
            jobInfo.Metadata.Author = "<author>";
            jobInfo.Metadata.Title = "<title>";
            jobInfo.Metadata.Subject = "Test Subject";
            jobInfo.Metadata.Keywords = "Test Keywords";
            var sfi1 = new SourceFileInfo();
            sfi1.Filename = PSFile1;
            jobInfo.SourceFiles.Add(sfi1);
            var sfi2 = new SourceFileInfo();
            sfi2.Filename = PSFile2;
            jobInfo.SourceFiles.Add(sfi2);

            var profile = new ConversionProfile();
            _job = new Job(jobInfo, profile, new Accounts());

            _userTokenExtractor.ExtractUserTokenFromPsFile(PSFile1, _job.Profile.UserTokens.Seperator).Returns(_userToken1);
            _userTokenExtractor.ExtractUserTokenFromPsFile(PSFile2, _job.Profile.UserTokens.Seperator).Returns(_userToken2);
        }

        [TestCase(0, 1, "0 should be defaulted to 0")]
        [TestCase(1, 1, "NumberOfCopies not extracted from first source file")]
        [TestCase(3, 3, "NumberOfCopies not extracted from first source file")]
        public void UpdateTokensAndMetadata_TestNumberOfCopies(int numberOfCopies, int numberOfCopiesExpected,
            string message)
        {
            _job.JobInfo.SourceFiles[0].Copies = numberOfCopies;
            _job.JobInfo.SourceFiles[1].Copies = 2;

            _jobDataUpdater.UpdateTokensAndMetadata(_job);

            Assert.AreEqual(numberOfCopiesExpected, _job.NumberOfCopies, message);
        }

        [Test]
        public void UpdateTokensAndMetadata_PageNumberCalculatorCallsGetNumberOfPagesOnce()
        {
            _jobDataUpdater.UpdateTokensAndMetadata(_job);

            _pageNumberCalculator.Received(1).GetNumberOfPages(_job);
        }

        [Test]
        public void UpdateTokensAndMetadata_UserTokensDisabled_UserTokenExtractorExtractUserTokensFromPsFileWasNotCalled
            ()
        {
            _job.Profile.UserTokens.Enabled = false;

            _jobDataUpdater.UpdateTokensAndMetadata(_job);

            _userTokenExtractor.DidNotReceiveWithAnyArgs().ExtractUserTokenFromPsFile(Arg.Any<string>(), Arg.Any<UserTokenSeperator>());
        }

        [Test]
        public void
            UpdateTokensAndMetadata_UserTokensEnabled_UserTokenExtractorExtractUserTokensFromPsFileWasCalledForEveryPSFileInSourceFileInfos
            ()
        {
            _job.Profile.UserTokens.Enabled = true;

            _jobDataUpdater.UpdateTokensAndMetadata(_job);

            _userTokenExtractor.Received(1).ExtractUserTokenFromPsFile(PSFile1, _job.Profile.UserTokens.Seperator);
            Assert.AreSame(_userToken1, _job.JobInfo.SourceFiles[0].UserToken);
            Assert.IsTrue(_job.JobInfo.SourceFiles[0].UserTokenEvaluated);
            _userTokenExtractor.Received(1).ExtractUserTokenFromPsFile(PSFile2, _job.Profile.UserTokens.Seperator);
            Assert.AreSame(_userToken2, _job.JobInfo.SourceFiles[1].UserToken);
            Assert.IsTrue(_job.JobInfo.SourceFiles[1].UserTokenEvaluated);
        }

        [Test]
        public void UpdateTokensAndMetadata_UserTokenEvaluatedTrue_DidNotExtractUserToken()
        {
            _job.Profile.UserTokens.Enabled = true;
            _job.JobInfo.SourceFiles[0].UserTokenEvaluated = true;

            _jobDataUpdater.UpdateTokensAndMetadata(_job);

            _userTokenExtractor.DidNotReceive().ExtractUserTokenFromPsFile(PSFile1, _job.Profile.UserTokens.Seperator);
            _userTokenExtractor.Received(1).ExtractUserTokenFromPsFile(PSFile2, _job.Profile.UserTokens.Seperator);
        }

        [Test]
        public void UpdateTokensAndMetadata_MultipleCalls_UserTokensAreOnlyExtractedOnce()
        {
            _job.Profile.UserTokens.Enabled = true;

            _jobDataUpdater.UpdateTokensAndMetadata(_job);
            _jobDataUpdater.UpdateTokensAndMetadata(_job);

            _userTokenExtractor.Received(1).ExtractUserTokenFromPsFile(PSFile1, _job.Profile.UserTokens.Seperator);
            _userTokenExtractor.Received(1).ExtractUserTokenFromPsFile(PSFile2, _job.Profile.UserTokens.Seperator);
        }

        [Test]
        public void UpdateTokensAndMetadata_TokenReplacerFactoryBuildTokenReplacerWithoutOutputfilesWasCalled()
        {
            _jobDataUpdater.UpdateTokensAndMetadata(_job);

            _tokenReplacerFactory.Received(1).BuildTokenReplacerWithoutOutputfiles(_job);
            Assert.AreSame(_tokenReplacer, _job.TokenReplacer);
        }

        [Test(Description = "Not a real unit test. Tokenreplacer must replace token correctly to succeed.")]
        public void UpdateTokensAndMetadata_ReplaceTokensInMetadataWasExecuted()
        {
            _jobDataUpdater.UpdateTokensAndMetadata(_job);

            Assert.AreEqual(AuthorTokenValue, _job.JobInfo.Metadata.Author);
            Assert.AreEqual(TitleTokenValue, _job.JobInfo.Metadata.Title);
            Assert.AreEqual(SubjectTokenValue, _job.JobInfo.Metadata.Subject);
            Assert.AreEqual(KeywordTokenValue, _job.JobInfo.Metadata.Keywords);
        }
    }
}
