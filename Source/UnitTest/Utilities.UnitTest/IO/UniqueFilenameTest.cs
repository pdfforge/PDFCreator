using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Utilities.IO;
using System;
using System.IO;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Utilities.UnitTest.IO
{
    [TestFixture]
    public class UniqueFilenameTest
    {
        [SetUp]
        public void SetUp()
        {
            _directory = Substitute.For<IDirectory>();
            _file = Substitute.For<IFile>();
            _pathUtil = Substitute.For<IPathUtil>();
            _pathUtil.MAX_PATH.Returns(259);
        }

        private IDirectory _directory;
        private IFile _file;
        private IPathUtil _pathUtil;

        [Test]
        public void UniqueFile_ExistingFilePathWithMaximumPathLength_UniqueFilenameCallsEllpipsisForTooLongPath()
        {
            const string dir245Chars = @"C:\ThisIsAVeryLongFileNameBecauseItHasMoreThan150CharactersAndToReachThatIHaveToWriteALotMoreTextThanICanThinkAboutRightNowIfYouReadThisUpToHereIOwnYouALittleSnackAndIStillNeedAFewMoreCharactersLetsSimplyCountOneTwoThreeFourFiveSixSevenEightNine";
            const string fileName13Chars = "File12345.pdf";
            //Combine adds a "\" so the result is the max path lengh of 260
            var tooLongPath = PathSafe.Combine(dir245Chars, fileName13Chars);

            _file.Exists("").ReturnsForAnyArgs(x => true, x => false);
            _directory.Exists("").ReturnsForAnyArgs(false);

            var uniqueFilename = new UniqueFilename(tooLongPath, _directory, _file, _pathUtil);

            Assert.Throws<PathTooLongException>(() => uniqueFilename.CreateUniqueFileName());
        }

        [Test]
        public void UniqueFile_GivenEmptyString_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new UniqueFilename("", _directory, _file, _pathUtil).CreateUniqueFileName());
        }

        [Test]
        public void UniqueFile_GivenExistingDirectory_ReturnsUniquifiedFile()
        {
            const string filename = @"C:\test";
            const string expectedFilename = @"C:\test_2";

            _file.Exists(filename).Returns(false);
            _file.Exists(expectedFilename).Returns(false);
            _directory.Exists(filename).Returns(true);
            _directory.Exists(expectedFilename).Returns(false);

            var uniqueFilename = new UniqueFilename(filename, _directory, _file, _pathUtil);

            Assert.AreEqual(expectedFilename, uniqueFilename.CreateUniqueFileName());
        }

        [Test]
        public void UniqueFile_GivenExistingDirectoryOnSecondCall_AppendixGetsContinued()
        {
            const string filename = @"C:\test";
            const string firstUniquifiedFile = @"C:\test_2";
            const string expectedFilename = @"C:\test_3";

            _file.Exists(filename).Returns(x => true, x => false);
            _file.Exists(firstUniquifiedFile).Returns(x => false, x => true);
            _directory.Exists(filename).Returns(x => true, x => false);
            _directory.Exists(firstUniquifiedFile).Returns(x => false, x => true);

            var uniqueFilename = new UniqueFilename(filename, _directory, _file, _pathUtil);
            uniqueFilename.CreateUniqueFileName();

            _file.Exists(firstUniquifiedFile).Returns(x => false, x => true);
            _file.Exists(expectedFilename).Returns(x => false, x => true);
            _directory.Exists(firstUniquifiedFile).Returns(x => true, x => false);
            _directory.Exists(expectedFilename).Returns(x => false, x => true);

            Assert.AreEqual(expectedFilename, uniqueFilename.CreateUniqueFileName());
        }

        [Test]
        public void UniqueFile_GivenExistingFile_LastUniqueFilenamePropertyIsUniquifiedFile()
        {
            const string filename = @"C:\test.txt";
            const string expectedFilename = @"C:\test_2.txt";

            _file.Exists(filename).Returns(true);
            _file.Exists(expectedFilename).Returns(false);
            _directory.Exists("").ReturnsForAnyArgs(false);

            var uniqueFilename = new UniqueFilename(filename, _directory, _file, _pathUtil);

            uniqueFilename.CreateUniqueFileName();
            Assert.AreEqual(expectedFilename, uniqueFilename.LastUniqueFilename);
        }

        [Test]
        public void UniqueFile_GivenExistingFile_ReturnsUniquifiedFile()
        {
            const string filename = @"C:\test.txt";
            const string expectedFilename = @"C:\test_2.txt";

            _file.Exists(filename).Returns(true);
            _file.Exists(expectedFilename).Returns(false);
            _directory.Exists("").ReturnsForAnyArgs(false);

            var uniqueFilename = new UniqueFilename(filename, _directory, _file, _pathUtil);

            Assert.AreEqual(expectedFilename, uniqueFilename.CreateUniqueFileName());
        }

        [Test]
        public void UniqueFile_GivenExistingFilenameOnSecondCall_AppendixGetsContinued()
        {
            const string filename = @"C:\test.jpg";
            const string firstUniquifiedFile = @"C:\test_2.jpg";
            const string expectedFilename = @"C:\test_3.jpg";

            _file.Exists(filename).Returns(true);
            _file.Exists(expectedFilename).Returns(false);
            _directory.Exists("").ReturnsForAnyArgs(false);

            var uniqueFilename = new UniqueFilename(filename, _directory, _file, _pathUtil);
            uniqueFilename.CreateUniqueFileName();

            _file.Exists(firstUniquifiedFile).Returns(true);
            _file.Exists(expectedFilename).Returns(false);

            Assert.AreEqual(expectedFilename, uniqueFilename.CreateUniqueFileName());
        }

        [Test]
        public void UniqueFile_GivenExistingFileWithoutExtension_ReturnsUniquifiedFile()
        {
            const string filename = @"C:\test";
            const string expectedFilename = @"C:\test_2";

            _file.Exists(filename).Returns(true);
            _file.Exists(expectedFilename).Returns(false);
            _directory.Exists("").ReturnsForAnyArgs(false);

            var uniqueFilename = new UniqueFilename(filename, _directory, _file, _pathUtil);

            Assert.AreEqual(expectedFilename, uniqueFilename.CreateUniqueFileName());
        }

        [Test]
        public void UniqueFile_GivenNonexistingFile_ReturnsSameFile()
        {
            const string filename = @"C:\test.txt";

            _file.Exists(filename).Returns(false);
            _directory.Exists("").ReturnsForAnyArgs(false);

            var uniqueFilename = new UniqueFilename(filename, _directory, _file, _pathUtil);

            Assert.AreEqual(filename, uniqueFilename.CreateUniqueFileName());
        }

        [Test]
        public void UniqueFile_GivenNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new UniqueFilename(null, _directory, _file, _pathUtil));
        }

        [Test]
        public void UniqueFileName_TestInitialising()
        {
            const string filename = @"C:\test.txt";
            var uniqueFilename = new UniqueFilename(filename, _directory, _file, _pathUtil);

            Assert.AreEqual(filename, uniqueFilename.OriginalFilename, "OriginalFilename is not setted file.");
            Assert.AreEqual(filename, uniqueFilename.LastUniqueFilename, "LastUniqueFilename is not setted file.");
        }
    }
}
