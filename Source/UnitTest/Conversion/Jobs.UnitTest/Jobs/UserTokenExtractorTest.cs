using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PsParser;
using PsParser;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Jobs.Jobs
{
    [TestFixture]
    public class UserTokenExtractorTest
    {
        private const string PsFile = "file.ps";
        private UserTokenExtractor _userTokenExtractor;
        private IPsParser _psParser;
        private IPsParserFactory _psParserFactory;

        [SetUp]
        public void SetUp()
        {
            _psParser = Substitute.For<IPsParser>();
            _psParser.UserTokens.Returns(new List<UserToken>());
            _psParserFactory = Substitute.For<IPsParserFactory>();
            _psParserFactory.BuildPsParser(PsFile, Arg.Any<string>(), Arg.Any<string>()).Returns(_psParser);

            _userTokenExtractor = new UserTokenExtractor(_psParserFactory);
        }

        [Test]
        public void ExtractUserTokenFromPsFileTest_SeperatorSquareBrakets_CallsPsParserFactoryWithSquareBrackets()
        {
            _userTokenExtractor.ExtractUserTokenFromPsFile(PsFile, UserTokenSeperator.SquareBrackets);

            _psParserFactory.Received(1).BuildPsParser(PsFile, "[[[", "]]]");
        }

        [Test]
        public void ExtractUserTokenFromPsFileTest_SeperatorAngleBrakets_CallsPsParserFactoryWithAngleBrackets()
        {
            _userTokenExtractor.ExtractUserTokenFromPsFile(PsFile, UserTokenSeperator.AngleBrackets);

            _psParserFactory.Received(1).BuildPsParser(PsFile, "<<<", ">>>");
        }

        [Test]
        public void ExtractUserTokenFromPsFileTest_SeperatorCurlyBrakets_CallsPsParserFactoryWithCurlyBrackets()
        {
            _userTokenExtractor.ExtractUserTokenFromPsFile(PsFile, UserTokenSeperator.CurlyBrackets);

            _psParserFactory.Received(1).BuildPsParser(PsFile, "{{{", "}}}");
        }

        [Test]
        public void ExtractUserTokenFromPsFileTest_SeperatorRoundBrakets_CallsPsParserFactoryWithRoundBrackets()
        {
            _userTokenExtractor.ExtractUserTokenFromPsFile(PsFile, UserTokenSeperator.RoundBrackets);

            _psParserFactory.Received(1).BuildPsParser(PsFile, "(((", ")))");
        }

        [Test]
        public void ExtractUserTokenFromPsFileTest_CorrectUseOfPsParser()
        {
            _userTokenExtractor.ExtractUserTokenFromPsFile(PsFile, UserTokenSeperator.SquareBrackets);

            Received.InOrder(() =>
            {
                _psParser.Analyse();
                _psParser.RemoveParameterLinesFromPSFile();
                _psParser.CloseStream();
            });
        }

        [Test]
        public void ExtractUserTokenFromPsFileTest_PsParserUserTokenIsNull_ReturnsEmptyUserToken()
        {
            _psParser.UserTokens.ReturnsNull();

            var userToken = _userTokenExtractor.ExtractUserTokenFromPsFile(PsFile, UserTokenSeperator.SquareBrackets);

            Assert.IsEmpty(userToken.KeyValueDict);
        }

        [Test]
        public void ExtractUserTokenFromPsFileTest_PsParserExxtractsUserTokens_ReturnedUserTokenKeyValueDictIncludesTokens()
        {
            var userTokensList = new List<UserToken>();
            var ut1 = new UserToken("key1", "value1");
            userTokensList.Add(ut1);
            var ut2 = new UserToken("key2", "value2");
            userTokensList.Add(ut2);
            _psParser.UserTokens.Returns(userTokensList);

            var userToken = _userTokenExtractor.ExtractUserTokenFromPsFile(PsFile, UserTokenSeperator.SquareBrackets);

            Assert.AreEqual(2, userToken.KeyValueDict.Count, "Wrong number of key value pairs in user token");
            Assert.AreEqual(ut1.Value, userToken.KeyValueDict[ut1.Key], "Wrong value for user token key");
            Assert.AreEqual(ut2.Value, userToken.KeyValueDict[ut2.Key], "Wrong value for user token key");
        }
    }
}
