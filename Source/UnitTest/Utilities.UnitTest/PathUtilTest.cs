using NSubstitute;
using NUnit.Framework;
using System;
using System.IO;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Utilities.UnitTest
{
    [TestFixture]
    public class PathUtilTest
    {
        private PathUtil _pathUtil;
        private IPath _path;
        private IDirectory _directory;

        [SetUp]
        public void SetUp()
        {
            _path = Substitute.For<IPath>();
            _directory = Substitute.For<IDirectory>();
            _pathUtil = new PathUtil(_path, _directory);
        }

        [Test]
        public void EllipsisForPath_PathisNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _pathUtil.EllipsisForFilename(null, _pathUtil.MAX_PATH));
        }

        [Test]
        public void EllipsisForPath_PathIsDirectory_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _pathUtil.EllipsisForFilename("Directory\\", _pathUtil.MAX_PATH));
        }

        [Test]
        public void EllipsisForPath_MaxLengthShorterThan10_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _pathUtil.EllipsisForFilename("Directory\\Filename.ext", 9));
        }

        [Test]
        public void EllipsisForPath_MaxLengthBiggerThanmaxpath_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _pathUtil.EllipsisForFilename("Directory\\Filename.ext", _pathUtil.MAX_PATH + 1));
        }

        [Test]
        public void EllipsisForPath_PathIsShorterThanMaxLength_ReturnsOriginalFilePath()
        {
            var filePath = "Directory\\Filename.ext";
            var length = filePath.Length;

            var filePathWithEllipsis = _pathUtil.EllipsisForFilename(filePath, length + 1);

            Assert.AreEqual(filePath, filePathWithEllipsis);
        }

        [Test]
        public void EllipsisForPath_PathLengthEqualsMaxLength_ReturnsOriginalFilePath()
        {
            var filePath = "Directory\\Filename.ext";
            var length = filePath.Length;

            var filePathWithEllipsis = _pathUtil.EllipsisForFilename(filePath, length);

            Assert.AreEqual(filePath, filePathWithEllipsis);
        }

        [Test]
        public void EllipsisForPath_TooLongPathButNotToBigToKeepDirectoryWithUsefullFilename_FileNameGetsEllipsis()
        {
            var directory = @"\\Very\Very\Long\Directory\Name\Without\Backslash";
            var extension = ".ext";
            var file = "Filename That Needs An Ellipsis Because It Is Too Long For Directory" + extension;
            var path = Path.Combine(directory, file);
            var maxLength = path.Length - file.Length + extension.Length + _pathUtil.ELLIPSIS.Length + 6; //enough space to shorten the filename to an useful length

            var pathWithEllipsis = _pathUtil.EllipsisForFilename(path, maxLength);

            Assert.AreEqual(maxLength, pathWithEllipsis.Length, "Shortend path is too long");
            Assert.AreEqual(directory, Path.GetDirectoryName(pathWithEllipsis), "Directory was changed");
            Assert.AreEqual(Path.GetExtension(path), Path.GetExtension(pathWithEllipsis), "Extension was changed");
            StringAssert.Contains(_pathUtil.ELLIPSIS, Path.GetFileName(pathWithEllipsis), "Shortend file does not contain Ellipsis");
        }

        [Test]
        public void EllipsisForPath_TooLongPathToKeepDirectoryWithUsefullFilename_ThrowException()
        {
            var directory = @"\\Very\Very\Long\Directory\Name\Without\Backslash";
            var extension = ".ext";
            var file = "Filename That Needs An Ellipsis Because It Is Too Long For Directory" + extension;
            var path = Path.Combine(directory, file);
            var maxLength = path.Length - file.Length + extension.Length + 5; //Not enough space to shorten the filename to an useful length

            Assert.Throws<ArgumentException>(() => _pathUtil.EllipsisForFilename(path, maxLength));
        }
    }
}
