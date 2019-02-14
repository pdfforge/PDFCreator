using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Windows;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using System.Collections.Generic;

namespace Presentation.UnitTest.Windows
{
    [TestFixture]
    public class ManagePrintJobsEventTests
    {
        [SetUp]
        public void Setup()
        {
            _jobList = new List<JobInfo> { new JobInfo(), new JobInfo(), new JobInfo() };
            _jobInfoQueue = Substitute.For<IJobInfoQueue>();
            _dragAndDrop = new DragAndDropEventHandler(Substitute.For<IFileConversionAssistant>());
            _jobInfoManager = Substitute.For<IJobInfoManager>();
            _dispatcher = new InvokeImmediatelyDispatcher();
            _jobInfoQueue.JobInfos.Returns(_jobList);
            _eventWasRaised = false;
            _jobInfoStub = new JobInfo();

            _model = new ManagePrintJobsViewModel(_jobInfoQueue, _dragAndDrop, _jobInfoManager, _dispatcher, new DesignTimeTranslationUpdater(), new DesignTimeApplicationNameProvider());
        }

        private IJobInfoQueue _jobInfoQueue;
        private IList<JobInfo> _jobList;
        private bool _eventWasRaised;
        private ManagePrintJobsViewModel _model;
        private DragAndDropEventHandler _dragAndDrop;
        private IJobInfoManager _jobInfoManager;
        private IDispatcher _dispatcher;
        private JobInfo _jobInfoStub;

        [Test]
        public void DeleteJobCommandWithOneJobQueue_NewJobIsAdded_RaisesCanExecuteChanged()
        {
            _model.DeleteJobCommand.CanExecuteChanged += delegate { _eventWasRaised = true; };

            _jobInfoQueue.OnNewJobInfo += Raise.EventWith(null, new NewJobInfoEventArgs(_jobInfoStub));

            Assert.IsTrue(_eventWasRaised);
        }

        [Test]
        public void DeleteJobCommandWithTwoJobsInQueue_CurrentItemChanges_CommandRaisesCanExecuteChanged()
        {
            _model = new ManagePrintJobsViewModel(_jobInfoQueue, _dragAndDrop, _jobInfoManager, _dispatcher, new DesignTimeTranslationUpdater(), new DesignTimeApplicationNameProvider());
            _model.DeleteJobCommand.CanExecuteChanged += delegate { _eventWasRaised = true; };

            _jobInfoQueue.OnNewJobInfo += Raise.EventWith(null, new NewJobInfoEventArgs(_jobInfoStub));

            Assert.IsTrue(_eventWasRaised);
        }

        [Test]
        public void MergeAllJobsCommandWithOneJobQueue_NewJobIsAdded_RaisesCanExecuteChanged()
        {
            _model.MergeAllJobsCommand.CanExecuteChanged += delegate { _eventWasRaised = true; };

            _jobInfoQueue.OnNewJobInfo += Raise.EventWith(null, new NewJobInfoEventArgs(_jobInfoStub));

            Assert.IsTrue(_eventWasRaised);
        }

        [Test]
        public void MergeAllJobsCommand_WhenExecuted_RaisesSelectedPrintJobChanged()
        {
            _jobInfoQueue.JobInfos.Add(new JobInfo());
            _model.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_model.SelectedPrintJob))
                    _eventWasRaised = true;
            };

            _model.MergeAllJobsCommand.Execute(null);

            Assert.IsTrue(_eventWasRaised);
        }

        [Test]
        public void MergeJobsCommandWithOneJobQueue_NewJobIsAdded_RaisesCanExecuteChanged()
        {
            _model.MergeJobsCommand.CanExecuteChanged += delegate { _eventWasRaised = true; };

            _jobInfoQueue.OnNewJobInfo += Raise.EventWith(null, new NewJobInfoEventArgs(_jobInfoStub));

            Assert.IsTrue(_eventWasRaised);
        }

        [Test]
        public void ViewModel_RaiseRefreshView_RaisesDeleteJobCanExecuteChanged()
        {
            _model.DeleteJobCommand.CanExecuteChanged += delegate { _eventWasRaised = true; };

            _model.RaiseRefreshView();

            Assert.IsTrue(_eventWasRaised);
        }

        [Test]
        public void ViewModel_RaiseRefreshView_RaisesMergeAllJobsCanExecuteChanged()
        {
            _model.MergeAllJobsCommand.CanExecuteChanged += delegate { _eventWasRaised = true; };

            _model.RaiseRefreshView();

            Assert.IsTrue(_eventWasRaised);
        }

        [Test]
        public void ViewModel_RaiseRefreshView_RaisesMergeJobsCanExecuteChanged()
        {
            _model.MergeJobsCommand.CanExecuteChanged += delegate { _eventWasRaised = true; };

            _model.RaiseRefreshView();

            Assert.IsTrue(_eventWasRaised);
        }
    }
}
