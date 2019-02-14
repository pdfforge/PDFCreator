using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.DesignTime;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;
using pdfforge.PDFCreator.Utilities.Threading;
using System;
using System.Windows.Input;
using Translatable;

namespace Presentation.UnitTest.UserControls
{
    [TestFixture]
    public class DropboxShareLinkViewModelTest
    {
        private DropboxShareLinkStepViewModel _viewModel;
        private ICommand _urlOpenCommand;
        private ICommand _copyToClipboardCommand;

        [SetUp]
        public void Setup()
        {
            var translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());
            var commandLocator = Substitute.For<ICommandLocator>();

            _urlOpenCommand = Substitute.For<ICommand>();
            commandLocator.GetCommand<UrlOpenCommand>().Returns(_urlOpenCommand);

            _copyToClipboardCommand = Substitute.For<ICommand>();
            commandLocator.GetCommand<CopyToClipboardCommand>().Returns(_copyToClipboardCommand);

            _viewModel = new DropboxShareLinkStepViewModel(translationUpdater, commandLocator);
        }

        [Test]
        public void Initialise_UrlOpenCommand_SetByCommandLocator()
        {
            Assert.AreSame(_urlOpenCommand, _viewModel.UrlOpenCommand);
        }

        [Test]
        public void Initialise_CopyToClipboardCommand_SetByCommandLocator()
        {
            Assert.AreSame(_copyToClipboardCommand, _viewModel.CopyToClipboardCommand);
        }

        [Test]
        public void ExecuteWorkflowStep_SetsShareUrl()
        {
            var shareUrl = "hpp://www.shareUrl.test";
            var job = new Job(new JobInfo(), new ConversionProfile(), new Accounts());
            job.ShareLinks.DropboxShareUrl = shareUrl;

            _viewModel.ExecuteWorkflowStep(job);

            Assert.AreEqual(shareUrl, _viewModel.ShareUrl);
        }

        [Test]
        public void ExecuteWorkflowStep_RaisesShareUrlPropertyChanged()
        {
            var wasCalled = false;
            _viewModel.PropertyChanged += (sender, args) => wasCalled = args.PropertyName == nameof(_viewModel.ShareUrl);
            var job = new Job(new JobInfo(), new ConversionProfile(), new Accounts());

            _viewModel.ExecuteWorkflowStep(job);

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void OkCommandExecute_CallsFinishInteraction()
        {
            var sender = new object();
            var eventArgs = new EventArgs();
            _viewModel.StepFinished += (s, args) =>
            {
                sender = s;
                eventArgs = args;
            };

            _viewModel.OkCommand.Execute(null);

            Assert.AreEqual(_viewModel, sender);
            Assert.AreEqual(EventArgs.Empty, eventArgs);
        }

        [Test]
        public void DesignTimeViewModel_IsNewable()
        {
            var dtvm = new DesignTimeDropboxShareLinkStepViewModel();
            Assert.IsNotNull(dtvm);
        }
    }
}
