using NSubstitute;
using NUnit.Framework;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Core.DirectConversion;
using pdfforge.PDFCreator.Core.Printing.Printing;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SystemInterface.IO;

namespace Presentation.UnitTest.Assistant
{
    [TestFixture]
    internal class FileConversionAssistantTest
    {
        private FileConversionAssistant _fileConversionHandler;
        private IDirectConversion _directConversion;
        private IPrintFileHelper _printFileHelper;
        private IFile _fileWrap;
        private List<string> _droppedFiles;
        private IDirectory _directoryWrap;
        private IInteractionInvoker _interactionInvoker;
        private IStoredParametersManager _storedParametersManager;

        [SetUp]
        public void Setup()
        {
            _directConversion = Substitute.For<IDirectConversion>();
            _printFileHelper = Substitute.For<IPrintFileHelper>();
            _fileWrap = Substitute.For<IFile>();
            _directoryWrap = Substitute.For<IDirectory>();
            _interactionInvoker = Substitute.For<IInteractionInvoker>();
            _storedParametersManager = Substitute.For<IStoredParametersManager>();

            _fileConversionHandler = new FileConversionAssistant(_directConversion,
                _printFileHelper, _fileWrap, _directoryWrap, _interactionInvoker,
                new UnitTestTranslationUpdater(), _storedParametersManager);

            _droppedFiles = new List<string> { "file1", "file2", "file3" };
        }

        [Test]
        public void HandleDroppedFiles_Null_DoNothing()
        {
            _fileConversionHandler.HandleFileList(null);

            Assert.AreEqual(0, _directConversion.ReceivedCalls().Count());
            Assert.AreEqual(0, _printFileHelper.ReceivedCalls().Count());
            Assert.AreEqual(0, _fileWrap.ReceivedCalls().Count());
        }

        [Test]
        public void HandleDroppedFiles_EmptyList_DoNothing()
        {
            var droppedFiles = new List<string>();

            _fileConversionHandler.HandleFileList(droppedFiles);

            Assert.AreEqual(0, _directConversion.ReceivedCalls().Count());
            Assert.AreEqual(0, _printFileHelper.ReceivedCalls().Count());
            Assert.AreEqual(0, _fileWrap.ReceivedCalls().Count());
        }

        [Test]
        public void HandleDroppedFiles_ListWithExistingAndNonExistingPrintableFiles_ExistingFilesGetPrinted()
        {
            _fileWrap.Exists(_droppedFiles[0]).Returns(true);
            _fileWrap.Exists(_droppedFiles[1]).Returns(false);
            _fileWrap.Exists(_droppedFiles[2]).Returns(true);

            _directConversion.CanConvertDirectly("").ReturnsForAnyArgs(false);
            _printFileHelper.AddFiles(null).ReturnsForAnyArgs(true);
            _fileConversionHandler.HandleFileList(_droppedFiles);
            Assert.AreEqual(2, _directConversion.ReceivedCalls().Count());

            _printFileHelper.Received(1).AddFiles(Arg.Is<List<string>>(x => (x.Count == 2) && x[0].Equals(_droppedFiles[0]) && x[1].Equals(_droppedFiles[2])));
            _printFileHelper.Received(1).PrintAll();
        }

        [Test]
        public void HandleDroppedFiles_ListWithExistingAndNotExitstingDirectConversionFiles_ExistingFilesGetConverteDirectly()
        {
            _fileWrap.Exists(_droppedFiles[0]).Returns(true);
            _fileWrap.Exists(_droppedFiles[1]).Returns(false);
            _fileWrap.Exists(_droppedFiles[2]).Returns(true);
            _directConversion.CanConvertDirectly("").ReturnsForAnyArgs(true);

            _fileConversionHandler.HandleFileList(_droppedFiles);

            Assert.AreEqual(0, _printFileHelper.ReceivedCalls().Count());
            _directConversion.Received(1).CanConvertDirectly(_droppedFiles[0]);
            _directConversion.Received(1).CanConvertDirectly(_droppedFiles[2]);
            _directConversion.Received(1).ConvertDirectly(_droppedFiles[0]);
            _directConversion.Received(1).ConvertDirectly(_droppedFiles[2]);
        }

        [Test]
        public void HandleDroppedFiles_ListWithExistingDirectConversionFiles_GetConverteDirectly()
        {
            _fileWrap.Exists("").ReturnsForAnyArgs(true);
            _directConversion.CanConvertDirectly("").ReturnsForAnyArgs(true);

            _fileConversionHandler.HandleFileList(_droppedFiles);

            Assert.AreEqual(0, _printFileHelper.ReceivedCalls().Count());
            _directConversion.Received(1).CanConvertDirectly(_droppedFiles[0]);
            _directConversion.Received(1).CanConvertDirectly(_droppedFiles[1]);
            _directConversion.Received(1).CanConvertDirectly(_droppedFiles[2]);
            _directConversion.Received(1).ConvertDirectly(_droppedFiles[0]);
            _directConversion.Received(1).ConvertDirectly(_droppedFiles[1]);
            _directConversion.Received(1).ConvertDirectly(_droppedFiles[2]);
        }

        [Test]
        public void HandleDroppedFiles_ListWithExistingPrintableFiles_PrintFileHelperAddFilesAndPrintAllGetCalled()
        {
            _fileWrap.Exists("").ReturnsForAnyArgs(true);
            _directConversion.CanConvertDirectly("").ReturnsForAnyArgs(false);
            _printFileHelper.AddFiles(null).ReturnsForAnyArgs(true);
            _fileConversionHandler.HandleFileList(_droppedFiles);
            Assert.AreEqual(3, _directConversion.ReceivedCalls().Count());

            _printFileHelper.Received(1).AddFiles(Arg.Is<List<string>>(x => x.SequenceEqual(_droppedFiles)));
            _printFileHelper.Received(1).PrintAll();
        }

        [Test]
        public void HandleDroppedFiles_ListWithExistingPrintableFiles_SavesFirstFileAsOriginalFilePath()
        {
            _fileWrap.Exists("").ReturnsForAnyArgs(true);
            _directConversion.CanConvertDirectly("").ReturnsForAnyArgs(false);
            _printFileHelper.AddFiles(null).ReturnsForAnyArgs(true);

            _fileConversionHandler.HandleFileList(_droppedFiles);

            _storedParametersManager.Received(1).SaveParameterSettings("", "", _droppedFiles.First());
        }

        [Test]
        public void HandleDroppedFiles_ListWithExistingPrintableFiles_PrintFileHelperAddFilesReturnsFalse_PrintAllDoeNotGetCalled()
        {
            _fileWrap.Exists("").ReturnsForAnyArgs(true);
            _directConversion.CanConvertDirectly("").ReturnsForAnyArgs(false);
            _printFileHelper.AddFiles(null).ReturnsForAnyArgs(false);
            _fileConversionHandler.HandleFileList(_droppedFiles);
            Assert.AreEqual(3, _directConversion.ReceivedCalls().Count());

            _printFileHelper.Received(1).AddFiles(Arg.Is<List<string>>(x => x.SequenceEqual(_droppedFiles)));
            _printFileHelper.DidNotReceive().PrintAll();
        }

        [Test]
        public void HandleDroppedFiles_ListWithNonExistingFiles_DoNothing()
        {
            _fileWrap.Exists("").ReturnsForAnyArgs(false);
            _fileConversionHandler.HandleFileList(_droppedFiles);
            Assert.AreEqual(0, _directConversion.ReceivedCalls().Count());
            Assert.AreEqual(0, _printFileHelper.ReceivedCalls().Count());
        }

        [Test]
        public void HandleFileList_WithDirectory_AddsAllFilesFromDirectory()
        {
            var dir = @"X:\test";
            var file = Path.Combine(dir, "test.docx");

            _directoryWrap.Exists(dir).Returns(true);
            _directoryWrap.GetFiles(dir).Returns(new[] { file });

            _fileConversionHandler.HandleFileList(new[] { dir });

            _printFileHelper.Received(1).AddFiles(Arg.Is<IEnumerable<string>>(list => list.Contains(file)));
        }

        [Test]
        public void HandleFileList_ManyFiles_TriggersMessageInteraction()
        {
            _droppedFiles.Clear();

            _interactionInvoker.Invoke(Arg.Do<MessageInteraction>(i =>
                {
                    i.Response = MessageResponse.Yes;
                    var translation = new FileConversionAssistantTranslation();
                    Assert.AreEqual(translation.GetFormattedMoreThanXFilesQuestion(_droppedFiles.Count), i.Text);
                }
            ));

            for (int i = 0; i <= 100; i++)
            {
                _droppedFiles.Add("file" + i);
            }

            _fileWrap.Exists(Arg.Any<string>()).Returns(true);

            _fileConversionHandler.HandleFileList(_droppedFiles);

            _interactionInvoker.Received(1).Invoke(Arg.Any<MessageInteraction>());
            _printFileHelper.Received(1).AddFiles(Arg.Is<IEnumerable<string>>(list => list.Count() == _droppedFiles.Count));
        }

        [Test]
        public void HandleFileList_FewFiles_DoesNotTriggersMessageInteraction()
        {
            _fileWrap.Exists(Arg.Any<string>()).Returns(true);

            _fileConversionHandler.HandleFileList(_droppedFiles);

            _interactionInvoker.DidNotReceive().Invoke(Arg.Any<MessageInteraction>());
            _printFileHelper.Received(1).AddFiles(Arg.Is<IEnumerable<string>>(list => list.Count() == _droppedFiles.Count));
        }
    }
}
