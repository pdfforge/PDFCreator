using NSubstitute;
using NUnit.Framework;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Interface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.JobHistory;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Commands.QuickActions;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Presentation.UnitTest.Commands.QuickAction
{
    [TestFixture]
    public class QuickActionOpenWithDefaultCommandTest
    {
        private IDefaultViewerAction _defaultViewerAction;
        private ICommandLocator _commandLocator;
        private ISettingsProvider _settingsProvider;
        private IInteractionInvoker _interactionInvoker;
        private ConversionProfile _profile;
        private Job _job;
        private List<string> _fileList;
        private IFileAssoc _fileAssoc;

        [SetUp]
        public void Setup()
        {
            _profile = new ConversionProfile();

            _job = new Job(null, null, null)
            {
                Profile = _profile,
                OutputFiles = new List<string> { "FirstFile.pdf" }
            };

            _fileList = new List<string>();
            _fileList.Add("C:\\Kartoffel.pdf");
            _fileList.Add("C:\\Salat.jpg");
            _fileList.Add("C:\\Marvin.tiff");
            _fileAssoc = Substitute.For<IFileAssoc>();
            _defaultViewerAction = Substitute.For<IDefaultViewerAction>();
            _commandLocator = Substitute.For<ICommandLocator>();
            _settingsProvider = Substitute.For<ISettingsProvider>();
            var pdfCreatorSettings = new PdfCreatorSettings();
            pdfCreatorSettings.ApplicationSettings = new ApplicationSettings();
            pdfCreatorSettings.DefaultViewers.Add(new DefaultViewer { IsActive = true, OutputFormat = OutputFormat.Pdf });
            _settingsProvider.Settings.Returns(pdfCreatorSettings);
            _interactionInvoker = Substitute.For<IInteractionInvoker>();
        }

        private QuickActionOpenWithDefaultCommand BuildCommand()
        {
            return new QuickActionOpenWithDefaultCommand(
                new UnitTestTranslationUpdater(),
                _defaultViewerAction,
                _fileAssoc,
                _commandLocator,
                _settingsProvider,
                _interactionInvoker);
        }

        [Test]
        public void CreateCommand_CallCanExecute_ReturnsTrue()
        {
            var command = BuildCommand();
            Assert.IsTrue(command.CanExecute(new Object()));
        }

        [Test]
        public void SendValidPDFJob_CallExecute_CallOpenOutputFile()
        {
            var command = BuildCommand();
            var defaultViewerByOutputFormat = _settingsProvider.Settings.GetDefaultViewerByOutputFormat(OutputFormat.Pdf);
            defaultViewerByOutputFormat.IsActive = true;
            _job.OutputFiles.Add(_fileList.First());
            _fileAssoc.HasOpen(Arg.Any<string>()).Returns(false);
            _defaultViewerAction.OpenOutputFile(_job.OutputFiles.First()).Returns(new ActionResult());
            command.Execute(_job);

            _defaultViewerAction.Received(1).OpenOutputFile(_job.OutputFiles.First());
        }

        [Test]
        public void SendValidPDFPath_CallExecute_CallOpenOutputFile()
        {
            var command = BuildCommand();
            var defaultViewerByOutputFormat = _settingsProvider.Settings.GetDefaultViewerByOutputFormat(OutputFormat.Pdf);
            defaultViewerByOutputFormat.IsActive = true;

            _fileAssoc.HasOpen(Arg.Any<string>()).Returns(false);

            var targetPath = _fileList.ElementAt(1);

            _defaultViewerAction.OpenOutputFile(targetPath).Returns(new ActionResult());
            command.Execute(targetPath);

            _defaultViewerAction.Received(1).OpenOutputFile(targetPath);
        }

        [Test]
        public void SendValidPDFButNoViewerFound_CallExecute_TryCallQuickActionOpenWithPdfArchitectCommand()
        {
            var command = BuildCommand();
            var defaultViewerByOutputFormat = _settingsProvider.Settings.GetDefaultViewerByOutputFormat(OutputFormat.Pdf);
            defaultViewerByOutputFormat.IsActive = false;

            _fileAssoc.HasOpen(Arg.Any<string>()).Returns(false);

            var targetPath = _fileList.ElementAt(0);

            _defaultViewerAction.OpenOutputFile(targetPath).Returns(new ActionResult());
            var dummyCommand = Substitute.For<ICommand>();
            _commandLocator.GetCommand<QuickActionOpenWithPdfArchitectCommand>().Returns(dummyCommand);
            command.Execute(targetPath);

            _commandLocator.Received(1).GetCommand<QuickActionOpenWithPdfArchitectCommand>();
        }

        [Test]
        public void SendValidPDFPathNonPDF_CallExecute_CallOpenOutputFile()
        {
            var command = BuildCommand();
            var defaultViewerByOutputFormat = _settingsProvider.Settings.GetDefaultViewerByOutputFormat(OutputFormat.Pdf);
            defaultViewerByOutputFormat.IsActive = true;

            _fileAssoc.HasOpen(Arg.Any<string>()).Returns(false);

            _defaultViewerAction.OpenOutputFile(_fileList.First()).Returns(new ActionResult());
            command.Execute(_fileList.First());

            _defaultViewerAction.Received(1).OpenOutputFile(_fileList.First());
        }

        [Test]
        public void SendValidPDFJobHistory_CallExecute_CallOpenOutputFile()
        {
            var command = BuildCommand();
            var historicFiles = new List<HistoricFile>
            {
                new HistoricFile("C:\\kartoffel.pdf", "kartoffel.pdf", "C:\\", "wtf1"),
                new HistoricFile("C:\\salat.pdf", "salat", "C:\\", "wtf2"),
                new HistoricFile("C:\\Marvin.pdf", "Marvin.pdf", "C:\\", "wtf3")
            };

            var historicJob = new HistoricJob(historicFiles, OutputFormat.Pdf, DateTime.Now, new Metadata(), 3, false);

            var defaultViewerByOutputFormat = _settingsProvider.Settings.GetDefaultViewerByOutputFormat(OutputFormat.Pdf);
            defaultViewerByOutputFormat.IsActive = true;

            _fileAssoc.HasOpen(Arg.Any<string>()).Returns(false);

            _defaultViewerAction.OpenOutputFile(historicFiles.First().Path).Returns(new ActionResult());
            command.Execute(historicJob);

            _defaultViewerAction.Received(1).OpenOutputFile(historicFiles.First().Path);
        }

        [Test]
        public void SendInvalidObject_CallExecute_ThrowsNotSupportedException()
        {
            var command = BuildCommand();

            Assert.Throws<NotSupportedException>(() => command.Execute(new Object()));
        }

        [Test]
        public void SetupDefaultViewerNotFoundError_RunExecute_TryToShowMessage()
        {
            var command = BuildCommand();

            var defaultViewerByOutputFormat = _settingsProvider.Settings.GetDefaultViewerByOutputFormat(OutputFormat.Pdf);
            defaultViewerByOutputFormat.IsActive = true;

            var actionResult = new ActionResult();
            actionResult.Add(ErrorCode.DefaultViewer_Not_Found);
            _defaultViewerAction.OpenOutputFile(_fileList.First()).Returns(actionResult);
            command.Execute(_fileList.First());
            _interactionInvoker.Received(1).Invoke(Arg.Any<MessageInteraction>());
        }
    }
}
