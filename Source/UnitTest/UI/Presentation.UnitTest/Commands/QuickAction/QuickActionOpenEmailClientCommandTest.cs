using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Interface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Services.JobHistory;
using pdfforge.PDFCreator.UI.Presentation.Commands.QuickActions;
using System;
using System.Collections.Generic;

namespace Presentation.UnitTest.Commands.QuickAction
{
    [TestFixture]
    public class QuickActionOpenEmailClientCommandTest
    {
        private ConversionProfile _profile;
        private Job _job;
        private IEMailClientAction _emailClientAction;
        private List<string> _fileList;
        private IMailSignatureHelper _mailSignatureHelper;

        private const string SignatureText = "Signature!";

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
            _fileList.Add("abc");
            _fileList.Add("def");
            _fileList.Add("ghi");

            _emailClientAction = Substitute.For<IEMailClientAction>();
            _mailSignatureHelper = Substitute.For<IMailSignatureHelper>();
            _mailSignatureHelper.ComposeMailSignature().Returns(SignatureText);
        }

        private QuickActionOpenMailClientCommand build()
        {
            return new QuickActionOpenMailClientCommand(new UnitTestTranslationUpdater(), _emailClientAction, _mailSignatureHelper);
        }

        private bool ListsAreEqual(IEnumerable<string> list1, IEnumerable<string> list2)
        {
            CollectionAssert.AreEqual(list1, list2);
            return true;
        }

        [Test]
        public void CreateCommand_CheckCanExecute_AlwaysReturnsTrue()
        {
            var command = build();
            Assert.IsTrue(command.CanExecute(null));
        }

        [Test]
        public void SendValidJobIntoCommand_ExecuteCommand_RunsAction()
        {
            var command = build();
            _job.OutputFiles = _fileList;

            command.Execute(_job);
            _emailClientAction.
                Received(1).
                Process("",
                        "",
                        false,
                        true,
                        SignatureText,
                        "",
                        "",
                        "",
                        true,
                        Arg.Is<IEnumerable<string>>(enumerable => ListsAreEqual(_fileList, enumerable)));
        }

        [Test]
        public void SendValidHistoricJobIntoCommand_ExecuteCommand_RunsAction()
        {
            var command = build();
            var historicFiles = new List<HistoricFile>
            {
                new HistoricFile("C:\\kartoffel.pdf", "kartoffel.pdf", "C:\\", "wtf1"),
                new HistoricFile("C:\\salat.pdf", "salat", "C:\\", "wtf2"),
                new HistoricFile("C:\\Marvin.pdf", "Marvin.pdf", "C:\\", "wtf3")
            };

            var historicJob = new HistoricJob(historicFiles, OutputFormat.Pdf, DateTime.Now, new Metadata(), 3, false);

            var equivalentList = new List<string>();
            foreach (var historicFile in historicJob.HistoricFiles)
                equivalentList.Add(historicFile.Path);

            command.Execute(historicJob);
            _emailClientAction.
                Received(1).
                Process("",
                        "",
                        false,
                        true,
                        SignatureText,
                        "",
                        "",
                        "",
                        true,
                        Arg.Is<IEnumerable<string>>(enumerable => ListsAreEqual(equivalentList, enumerable)));
        }

        [Test]
        public void SendValidPath_ExecuteCommand_RunsAction()
        {
            var command = build();
            var path = "C:\\Kartoffel.pdf";
            var equivalentList = new List<string>() { path };

            command.Execute(path);

            _emailClientAction.
                Received(1).
                Process("",
                    "",
                    false,
                    true,
                    SignatureText,
                    "",
                    "",
                    "",
                    true,
                    Arg.Is<IEnumerable<string>>(enumerable => ListsAreEqual(equivalentList, enumerable)));
        }

        [Test]
        public void SendInvalidObject_ExecuteAction_ThrowsNotSupportedError()
        {
            var command = build();
            Assert.Throws<NotSupportedException>(() => command.Execute(new Object()));
        }
    }
}
