using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.DataStorage;
using pdfforge.DataStorage.Storage;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Utilities.Tokens;
using Ploeh.AutoFixture;
using System.Text;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.Jobs.JobInfo
{
    [TestFixture]
    public class SourceFileInfoDataReaderTest
    {
        private SourceFileInfoDataReader _sourceFileInfoDataReader;
        private string _tempFileInfoFile;
        private Data _data;
        private IniStorage _infFileIniStorage;
        private SourceFileInfo _sfi;

        [SetUp]
        public void SetUp()
        {
            _sourceFileInfoDataReader = new SourceFileInfoDataReader();
            _tempFileInfoFile = TempFileHelper.CreateTempFile(nameof(SourceFileInfoDataReaderTest), "TestFileInfo.inf");
            _data = Data.CreateDataStorage();
            _infFileIniStorage = new IniStorage(_tempFileInfoFile, Encoding.GetEncoding("Unicode"));

            Fixture fixture = new Fixture();
            _sfi = fixture.Create<SourceFileInfo>();
        }

        [TearDown]
        public void TearDown()
        {
            TempFileHelper.CleanUp();
        }

        [Test]
        public void WriteAndLoadSourceFileInfo_SourceFileInfosAreEqual()
        {
            var section = "SomeSection";

            _sourceFileInfoDataReader.WriteSourceFileInfoToData(_data, section, _sfi);
            _infFileIniStorage.WriteData(_data);
            var loadedSourceFileInfo = _sourceFileInfoDataReader.ReadSourceFileInfoFromData(_tempFileInfoFile, _data, section);

            _sfi.Filename = loadedSourceFileInfo.Filename;
            AssertSfiAreEqual(_sfi, loadedSourceFileInfo);
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
