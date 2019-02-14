using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.Windows
{
    public class ManagePrintJobsViewModel : OverlayViewModelBase<ManagePrintJobsInteraction, ManagePrintJobsWindowTranslation>
    {
        private readonly DragAndDropEventHandler _dragAndDrop;
        private readonly IJobInfoManager _jobInfoManager;
        private readonly IDispatcher _dispatcher;
        private readonly ApplicationNameProvider _applicationNameProvider;
        private readonly IJobInfoQueue _jobInfoQueue;
        private readonly ObservableCollection<JobInfo> _jobInfos;

        public ManagePrintJobsViewModel(IJobInfoQueue jobInfoQueue, DragAndDropEventHandler dragAndDrop, IJobInfoManager jobInfoManager, IDispatcher dispatcher, ITranslationUpdater translationUpdater, ApplicationNameProvider applicationNameProvider)
            : base(translationUpdater)
        {
            _jobInfoQueue = jobInfoQueue;
            _dragAndDrop = dragAndDrop;
            _jobInfoManager = jobInfoManager;
            _dispatcher = dispatcher;
            _applicationNameProvider = applicationNameProvider;
            _jobInfoQueue.OnNewJobInfo += OnNewJobInfo;

            ListSelectionChangedCommand = new DelegateCommand(ListSelectionChanged);
            DeleteJobCommand = new DelegateCommand(ExecuteDeleteJob);
            MergeJobsCommand = new DelegateCommand(ExecuteMergeJobs, CanExecuteMergeJobs);
            MergeAllJobsCommand = new DelegateCommand(ExecuteMergeAllJobs, CanExecuteMergeAllJobs);
            WindowClosedCommand = new DelegateCommand(OnWindowClosed);
            WindowActivatedCommand = new DelegateCommand(OnWindowActivated);
            DragEnterCommand = new DelegateCommand<DragEventArgs>(OnDragEnter);
            DropCommand = new DelegateCommand<DragEventArgs>(OnDrop);
            KeyDownCommand = new DelegateCommand<KeyEventArgs>(OnKeyDown);

            var synchronizedJobs = new Helper.SynchronizedCollection<JobInfo>(_jobInfoQueue.JobInfos);
            _jobInfos = synchronizedJobs.ObservableCollection;
            JobInfos = new CollectionView(_jobInfos);
            JobListSelectionChanged = new DelegateCommand(ListItemChange);
        }

        private void ListItemChange(object obj)
        {
            RaiseRefreshView();
            RaisePropertyChanged(nameof(SelectedPrintJob));
        }

        public JobInfo SelectedPrintJob
        {
            get { return (JobInfo)JobInfos.CurrentItem; }
        }

        public CollectionView JobInfos { get; }
        public DelegateCommand ListSelectionChangedCommand { get; }
        public DelegateCommand DeleteJobCommand { get; }
        public DelegateCommand MergeJobsCommand { get; }
        public DelegateCommand MergeAllJobsCommand { get; }
        public DelegateCommand WindowClosedCommand { get; }
        public DelegateCommand WindowActivatedCommand { get; }
        public DelegateCommand<DragEventArgs> DragEnterCommand { get; }
        public DelegateCommand<DragEventArgs> DropCommand { get; }
        public DelegateCommand<KeyEventArgs> KeyDownCommand { get; }

        public DelegateCommand JobListSelectionChanged { get; set; }

        private void ListSelectionChanged(object obj)
        {
            DeleteJobCommand.RaiseCanExecuteChanged();
            MergeJobsCommand.RaiseCanExecuteChanged();
        }

        private void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                FinishInteraction();
        }

        private void OnWindowActivated(object obj)
        {
            RaiseRefreshView();
        }

        private void OnDrop(DragEventArgs e)
        {
            _dragAndDrop.HandleDropEvent(e);
        }

        private void OnDragEnter(DragEventArgs e)
        {
            _dragAndDrop.HandleDragEnter(e);
        }

        private void OnWindowClosed(object obj)
        {
            _jobInfoQueue.OnNewJobInfo -= OnNewJobInfo;
        }

        private void OnNewJobInfo(object sender, NewJobInfoEventArgs e)
        {
            Action<JobInfo> addMethod = AddJobInfo;
            _dispatcher.BeginInvoke(addMethod, e.JobInfo);
        }

        private void AddJobInfo(JobInfo jobInfo)
        {
            _jobInfos.Add(jobInfo);

            if (JobInfos.CurrentItem == null)
                JobInfos.MoveCurrentToFirst();

            RaiseRefreshView();
        }

        public void RaiseRefreshView()
        {
            RaiseRefreshView(false);
        }

        private void RaiseRefreshView(bool refreshCollectionView)
        {
            DeleteJobCommand.RaiseCanExecuteChanged();
            MergeJobsCommand.RaiseCanExecuteChanged();
            MergeAllJobsCommand.RaiseCanExecuteChanged();

            if (refreshCollectionView)
                JobInfos.Refresh();
        }

        private void ExecuteDeleteJob(object o)
        {
            var jobInfo = o as JobInfo;
            var position = JobInfos.CurrentPosition;

            if (jobInfo == null)
                return;

            _jobInfos.Remove(jobInfo);
            _jobInfoQueue.Remove(jobInfo, true);

            if (_jobInfos.Count > 0)
                JobInfos.MoveCurrentToPosition(Math.Max(0, position - 1));

            RaiseRefreshView();
        }

        private void ExecuteMergeJobs(object o)
        {
            if (!CanExecuteMergeJobs(o))
                throw new InvalidOperationException("CanExecute is false");

            var jobObjects = o as IEnumerable<object>;
            if (jobObjects == null)
                return;

            var jobs = jobObjects.ToList();
            var first = (JobInfo)jobs.First();

            foreach (var jobObject in jobs.Skip(1))
            {
                var job = (JobInfo)jobObject;
                if (job.JobType != first.JobType)
                    continue;

                _jobInfoManager.Merge(first, job);
                _jobInfos.Remove(job);
                _jobInfoQueue.Remove(job, false);
            }

            _jobInfoManager.SaveToInfFile(first);

            RaiseRefreshView(true);
        }

        private bool CanExecuteMergeJobs(object o)
        {
            var jobs = o as IEnumerable<object>;
            return jobs != null && jobs.Count() > 1;
        }

        private void ExecuteMergeAllJobs(object o)
        {
            ExecuteMergeJobs(_jobInfos);
            RaisePropertyChanged(nameof(SelectedPrintJob));
        }

        private bool CanExecuteMergeAllJobs(object o)
        {
            return _jobInfos.Count > 1;
        }

        public override string Title => _applicationNameProvider.ApplicationName;
    }
}
