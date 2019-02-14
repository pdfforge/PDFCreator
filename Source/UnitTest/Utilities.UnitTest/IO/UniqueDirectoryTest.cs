using NUnit.Framework;
using pdfforge.PDFCreator.Utilities.IO;
using Rhino.Mocks;
using System;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Utilities.UnitTest.IO
{
    [TestFixture]
    public class UniqueDirectoryTest
    {
        [Test]
        public void EnsureUniqueDirectory_GivenExisting_ReturnsUniquifiedDirectory()
        {
            const string path = @"C:\TestFolder\FileNameAsSubFolder.xx";
            const string expectedPath = @"C:\TestFolder\FileNameAsSubFolder.xx_2";

            var directoryWrap = MockRepository.GenerateStub<IDirectory>();
            directoryWrap.Stub(x => x.Exists(path)).Return(false);
            var fileWrap = MockRepository.GenerateStub<IFile>();
            fileWrap.Stub(x => x.Exists(path)).Return(true);

            var uniqueDirectory = new UniqueDirectory(path, directoryWrap, fileWrap);

            Assert.AreEqual(expectedPath, uniqueDirectory.MakeUniqueDirectory());
        }

        [Test]
        public void EnsureUniqueDirectory_GivenExistingDirectory_ReturnsUniquifiedDirectory()
        {
            const string path = @"C:\TestFolder\MySubFolder";
            const string expectedPath = @"C:\TestFolder\MySubFolder_2";

            var directoryWrap = MockRepository.GenerateStub<IDirectory>();
            directoryWrap.Stub(x => x.Exists(path)).Return(true);
            var fileWrap = MockRepository.GenerateStub<IFile>();
            directoryWrap.Stub(x => x.Exists(path)).Return(false);

            var uniqueDirectory = new UniqueDirectory(path, directoryWrap, fileWrap);

            Assert.AreEqual(expectedPath, uniqueDirectory.MakeUniqueDirectory());
        }

        [Test]
        public void EnsureUniqueDirectory_GivenNonExistingDirectory_ReturnsSameDirectory()
        {
            const string path = @"C:\TestFolder\MySubFolder";

            var directoryWrap = MockRepository.GenerateStub<IDirectory>();
            directoryWrap.Stub(x => x.Exists(path)).Return(false);
            var fileWrap = MockRepository.GenerateStub<IFile>();
            directoryWrap.Stub(x => x.Exists(path)).Return(false);

            var uniqueDirectory = new UniqueDirectory(path, directoryWrap, fileWrap);

            Assert.AreEqual(path, uniqueDirectory.MakeUniqueDirectory());
        }

        [Test]
        public void EnsureUniqueDirectory_GivenNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new UniqueDirectory(null));
        }

        [Test]
        public void EnsureUniqueDirectory_GivenEmptyString_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new UniqueDirectory(""));
        }
    }
}
