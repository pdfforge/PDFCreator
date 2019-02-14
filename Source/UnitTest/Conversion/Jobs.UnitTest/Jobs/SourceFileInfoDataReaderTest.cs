using NUnit.Framework;
using pdfforge.DataStorage;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Utilities.Tokens;
using Ploeh.AutoFixture;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Jobs.Jobs
{
    [TestFixture]
    public class SourceFileInfoDataReaderTest
    {
        private IFixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
        }

        [TestCase(JobType.PsJob)]
        [TestCase(JobType.XpsJob)]
        public void ValidSourceFileInfo_CanBeWrittenAndReadAgain(JobType type)
        {
            var sfi = BuildSourceFileInfo(type);
            var infFileName = "somefile.inf";
            var reader = new SourceFileInfoDataReader();
            var data = Data.CreateDataStorage();

            reader.WriteSourceFileInfoToData(data, "theSection", sfi);

            var result = reader.ReadSourceFileInfoFromData(infFileName, data, "theSection");

            AssertSfiAreEqual(result, sfi);
        }

        [Test]
        public void IfDataIsEmpty_SetsDefaults()
        {
            var reader = new SourceFileInfoDataReader();
            var data = Data.CreateDataStorage();

            var result = reader.ReadSourceFileInfoFromData(@"some\inf\file", data, "theSection");

            Assert.IsNotNull(result);
        }

        private SourceFileInfo BuildSourceFileInfo(JobType type)
        {
            var userToken = new UserToken();
            userToken.AddKeyValuePair("testkey", "test value");

            _fixture.Freeze(userToken);

            return _fixture.Create<SourceFileInfo>();
        }

        private static void AssertSfiAreEqual(SourceFileInfo expectedSfi, SourceFileInfo actualSfi)
        {
            var type = typeof(SourceFileInfo);
            foreach (var propertyInfo in type.GetProperties())
            {
                if (propertyInfo.PropertyType == typeof(UserToken))
                    continue;

                Assert.AreEqual(propertyInfo.GetValue(expectedSfi), propertyInfo.GetValue(actualSfi), $"Failed Property: {propertyInfo.Name}");
            }
        }
    }
}
