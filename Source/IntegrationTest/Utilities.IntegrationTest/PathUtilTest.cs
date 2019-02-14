using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.PDFCreator.Utilities;
using Ploeh.AutoFixture;
using System;
using SystemWrapper.IO;

namespace PDFCreator.Utilities.IntegrationTest
{
    [TestFixture]
    internal class PathUtilTest
    {
        private IPathUtil _pathUtil;
        private IFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _pathUtil = new PathUtil(new PathWrap(), new DirectoryWrap());
        }

        [TearDown]
        public void CleanUp()
        {
            TempFileHelper.CleanUp();
        }

        [TestCase(@"A:\Test.txt")]
        [TestCase(@"Z:\Test.txt")]
        [TestCase(@"X:\Test\Folder\With\Many\Sub\Folders\test.txt")]
        [TestCase(@"\\TestServer\SomeFolder\Test.txt")]
        [TestCase(@"a:\Test.txt")]
        [TestCase(@"z:\Test.txt")]
        [TestCase(@"x:\Test\Folder\With\Many\Sub\Folders\test.txt")]
        public void TestValidRootedPath(string validPath)
        {
            Assert.IsTrue(_pathUtil.IsValidRootedPath(validPath), "Expected '" + validPath + "' to be a valid path");
        }

        [TestCase("text.txt")]
        [TestCase(@":@!|")]
        [TestCase(@"\Test\test.txt")]
        [TestCase(@":\test.txt")]
        [TestCase(@"_:\Test.txt")]
        [TestCase("")]
        [TestCase("a")]
        [TestCase("C:MyDir")]
        [TestCase(@"C:\Mydir:")]
        public void TestInvalidRootedPath(string invalidPath)
        {
            Assert.IsFalse(_pathUtil.IsValidRootedPath(invalidPath), "Expected '" + invalidPath + "' to be an invalid path");
        }

        [Test]
        public void CheckWritability_WithWritableFolder_ReturnsTrue()
        {
            var tmpFolder = TempFileHelper.CreateTempFolder("CheckWritability");
            Assert.IsTrue(_pathUtil.CheckWritability(tmpFolder));
        }

        [Test]
        public void Ellipsis_FolderWithoutFile_ThrowsException()
        {
            const string folder = @"C:\folder\";

            Assert.Throws<ArgumentException>(() => _pathUtil.EllipsisForTooLongPath(folder));
        }

        [Test]
        public void Ellipsis_Given300CharFile_ReturnsShortenedString()
        {
            var file = @"C:\folder\" + new string('a', 293) + ".txt";

            var shortFile = _pathUtil.EllipsisForTooLongPath(file);

            Assert.AreNotEqual(file, shortFile);
            StringAssert.Contains(_pathUtil.ELLIPSIS, shortFile);
            Assert.IsTrue(shortFile.Length <= _pathUtil.MAX_PATH);
        }

        [Test]
        public void Ellipsis_Given300CharFileWithoutExtension_ReturnsShortenedString()
        {
            var file = @"C:\" + new string('a', 297);

            var shortFile = _pathUtil.EllipsisForTooLongPath(file);

            StringAssert.Contains(_pathUtil.ELLIPSIS, shortFile);
            Assert.IsTrue(shortFile.Length <= _pathUtil.MAX_PATH, "Shortened file still is too long");
        }

        [Test]
        public void Ellipsis_Given300CharFolderWithFile_ThrowsException()
        {
            var folder = @"C:\" + new string('a', 300) + "\\";
            var file = folder + "test.txt";

            Assert.Throws<ArgumentException>(() => _pathUtil.EllipsisForTooLongPath(file));
        }

        [Test]
        public void Ellipsis_GivenEmptyString_ReturnsEmptyString()
        {
            Assert.AreEqual("", _pathUtil.EllipsisForTooLongPath(""));
        }

        [Test]
        public void Ellipsis_GivenNull_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _pathUtil.EllipsisForTooLongPath(null));
        }

        [Test]
        public void Ellipsis_GivenShortFile_ReturnsSameString()
        {
            const string file = @"C:\Test\abc.txt";
            Assert.AreEqual(file, _pathUtil.EllipsisForTooLongPath(file));
        }

        [Test]
        public void EllipsisWithLength_Given300CharFile_ReturnsShortenedString()
        {
            var file = @"C:\" + new string('a', 293) + ".txt";

            var shortFile = _pathUtil.EllipsisForFilename(file, 200);

            Assert.AreNotEqual(file, shortFile);
            StringAssert.Contains(_pathUtil.ELLIPSIS, shortFile);
            Assert.IsTrue(shortFile.Length <= 200);
        }

        [Test]
        public void EllipsisWithLength_GivenLength261_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => _pathUtil.EllipsisForFilename(@"C:\test.txt", _pathUtil.MAX_PATH + 1));
        }

        [Test]
        public void EllipsisWithLength_GivenLength9_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => _pathUtil.EllipsisForFilename(@"C:\test.txt", 9));
        }

        [Test]
        public void IsValidRootedPath_PathTooLong_ReturnsFalse()
        {
            var path = "C:\\" + string.Join(string.Empty, _fixture.CreateMany<string>(10)) + ".pdf";

            var result = _pathUtil.IsValidRootedPath(path);

            Assert.IsFalse(result, "Expected '" + path + "' to be too long.");
        }

        [Test]
        public void IsValidRootedPath_PathWithIllegalChars_ReturnsFalse()
        {
            var path = "C:\\||**..**.pdf";

            var result = _pathUtil.IsValidRootedPath(path);

            Assert.IsFalse(result, "Expected '" + path + "' to contain illegal characters.");
        }

        [Test]
        public void IsValidRootedPath_PathCausesNotSupportedException_ReturnsFalse()
        {
            var path = "C:\\OhNo:ThisPahtIsNotSupported.pdf";

            var result = _pathUtil.IsValidRootedPath(path);

            Assert.IsFalse(result, "Expected '" + path + "' to cause a NotSupportedException.");
        }

        [Test]
        public void IsValidRootedPath_PathStartsWithCorrectSyntaxAndLetter_ReturnsTrue()
        {
            var path = "C:\\ThisPahtIsCorrect.pdf";

            var result = _pathUtil.IsValidRootedPath(path);

            Assert.IsTrue(result, "Expected '" + path + "' to be valid.");
        }
    }
}
