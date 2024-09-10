using GongSolutions.Wpf.DragDrop;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Core.JobInfoQueue;
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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using NaturalSort.Extension;

namespace pdfforge.PDFCreator.UI.Presentation.Windows
{
    public class ManagePrintJobsViewModel : OverlayViewModelBase<ManagePrintJobsInteraction, ManagePrintJobsWindowTranslation>
    {
        private readonly DragAndDropEventHandler _dragAndDrop;
        private readonly IJobInfoManager _jobInfoManager;
        private readonly IDispatcher _dispatcher;
        private readonly ApplicationNameProvider _applicationNameProvider;
        private readonly IVersionHelper _versionHelper;
        private readonly IJobInfoQueue _jobInfoQueue;
        private readonly ObservableCollection<JobInfo> _jobInfos;
        private Helper.SynchronizedCollection<JobInfo> _synchronizedJobs;
        public IDropTarget CustomDropHandler { get; } = new CustomDropEventHandler();

        public ManagePrintJobsViewModel(IJobInfoQueue jobInfoQueue, DragAndDropEventHandler dragAndDrop, IJobInfoManager jobInfoManager,
            IDispatcher dispatcher, ITranslationUpdater translationUpdater, ApplicationNameProvider applicationNameProvider,
            IVersionHelper versionHelper, ICommandLocator commandLocator)
            : base(translationUpdater)
        {
            _jobInfoQueue = jobInfoQueue;
            _dragAndDrop = dragAndDrop;
            _jobInfoManager = jobInfoManager;
            _dispatcher = dispatcher;
            _applicationNameProvider = applicationNameProvider;
            _versionHelper = versionHelper;
            _jobInfoQueue.OnNewJobInfo += OnNewJobInfo;

            ConvertFileCommand = commandLocator.GetCommand<SelectFileViaDialogAndConvertCommand>();
            DeleteJobCommand = new DelegateCommand(DeleteJobExecute);
            MergeJobsCommand = new DelegateCommand(ExecuteMergeJobs, CanExecuteMergeJobs);
            MergeAllJobsCommand = new DelegateCommand(ExecuteMergeAllJobs, HasMoreThanOneJob);
            SelectUnSelectAllJobsCommand = new DelegateCommand(SelectUnSelectAllJobsExecute);
            WindowClosedCommand = new DelegateCommand(OnWindowClosed);
            WindowActivatedCommand = new DelegateCommand(OnWindowActivated);
            DragEnterCommand = new DelegateCommand<DragEventArgs>(OnDragEnter);
            DropCommand = new DelegateCommand<DragEventArgs>(OnDrop);
            KeyDownCommand = new DelegateCommand<KeyEventArgs>(OnKeyDown);

            SortCommand = new DelegateCommand(SortCommandExecute, HasMoreThanOneJob);
            SetupSortMenuItems();

            _synchronizedJobs = new Helper.SynchronizedCollection<JobInfo>(_jobInfoQueue.JobInfos);
            _jobInfos = _synchronizedJobs.ObservableCollection;
            DisplayedJobInfo = _jobInfos.FirstOrDefault();
            JobInfos = new CollectionView(_jobInfos);

            JobListSelectionChangedCommand = new DelegateCommand(JobListSelectionChangedExecute);
        }

        private void SetupSortMenuItems()
        {
            SortMenuItems = new List<MenuItem>
            {
                new MenuItem { Header = Translation.IdAscending, CommandParameter = MergeSortingEnum.IdAscending },
                new MenuItem { Header = Translation.IdDescending, CommandParameter = MergeSortingEnum.IdDescending },
                new MenuItem { Header = Translation.NameAscending, CommandParameter = MergeSortingEnum.NameAscending },
                new MenuItem { Header = Translation.NameDescending, CommandParameter = MergeSortingEnum.NameDescending },
                new MenuItem { Header = Translation.DateAscending, CommandParameter = MergeSortingEnum.DateAscending },
                new MenuItem { Header = Translation.DateDescending, CommandParameter = MergeSortingEnum.DateDescending }
            };
        }

        private void SortCommandExecute(object parameter)
        {
            var list = _jobInfos.ToList();

            switch ((MergeSortingEnum)parameter)
            {
                case MergeSortingEnum.IdAscending:
                    list = list.OrderBy(info => info.SourceFiles[0].JobCounter).ToList();
                    break;

                case MergeSortingEnum.IdDescending:
                    list = list.OrderByDescending(info => info.SourceFiles[0].JobCounter).ToList();
                    break;

                case MergeSortingEnum.NameAscending:
                    list = list.OrderBy(info => info.Metadata.PrintJobName, StringComparison.OrdinalIgnoreCase.WithNaturalSort()).ToList();
                    break;

                case MergeSortingEnum.NameDescending:
                    list = list.OrderByDescending(info => info.Metadata.PrintJobName, StringComparison.OrdinalIgnoreCase.WithNaturalSort()).ToList();
                    break;

                case MergeSortingEnum.DateAscending:
                    list = list.OrderBy(info => info.PrintDateTime).ToList();
                    break;

                case MergeSortingEnum.DateDescending:
                    list = list.OrderByDescending(info => info.PrintDateTime).ToList();
                    break;
            }

            _jobInfos.Clear();

            foreach (var jobInfo in list)
            {
                _jobInfos.Add(jobInfo);
            }

            JobInfos.Refresh();
        }

        private void JobListSelectionChangedExecute(object obj)
        {
            MergeJobsCommand.RaiseCanExecuteChanged();
            MergeAllJobsCommand.RaiseCanExecuteChanged();
            
            if (!_suppressUncheckOfSelectUnselectAllOnSelectedChanged)
            {
                SelectUnselectAll = false;
                RaisePropertyChanged(nameof(SelectUnselectAll));
            }
        }

        private JobInfo _displayedJobInfo;

        public JobInfo DisplayedJobInfo
        {
            set
            {
                _displayedJobInfo = value;
                RaisePropertyChanged(nameof(DisplayedJobInfo));
            }
            get { return _displayedJobInfo; }
        }

        public CollectionView JobInfos { get; private set; }
        public ICommand ConvertFileCommand { get; set; }
        public DelegateCommand DeleteJobCommand { get; }
        public DelegateCommand MergeJobsCommand { get; }
        public DelegateCommand MergeAllJobsCommand { get; }
        public DelegateCommand SelectUnSelectAllJobsCommand { get; }
        public DelegateCommand WindowClosedCommand { get; }
        public DelegateCommand WindowActivatedCommand { get; }

        public IEnumerable<MenuItem> SortMenuItems { get; private set; }

        public DelegateCommand SortCommand { get; }
        public DelegateCommand<DragEventArgs> DragEnterCommand { get; }
        public DelegateCommand<DragEventArgs> DropCommand { get; }
        public DelegateCommand<KeyEventArgs> KeyDownCommand { get; }

        public DelegateCommand JobListSelectionChangedCommand { get; set; }

        private void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                FinishInteraction();
        }

        private void OnWindowActivated(object obj)
        {
            MergeJobsCommand.RaiseCanExecuteChanged();
            MergeAllJobsCommand.RaiseCanExecuteChanged();
            SortCommand.RaiseCanExecuteChanged();
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
            if (_jobInfos.Contains(jobInfo))
                return;

            _synchronizedJobs.SuspendUpdates();

            var nextJob = _jobInfos.FirstOrDefault(j => j.PrintDateTime > jobInfo.PrintDateTime);

            var targetPosition = nextJob == null
                ? _jobInfos.Count
                : _jobInfos.IndexOf(nextJob);

            _jobInfos.Insert(targetPosition, jobInfo);

            _synchronizedJobs.ResumeUpdates();

            if (JobInfos.CurrentItem == null)
                JobInfos.MoveCurrentToFirst();

            MergeJobsCommand.RaiseCanExecuteChanged();
            MergeAllJobsCommand.RaiseCanExecuteChanged();
            SortCommand.RaiseCanExecuteChanged();

            SelectUnselectAll = false;
            RaisePropertyChanged(nameof(SelectUnselectAll));
        }

        private void DeleteJobExecute(object o)
        {
            //var position = JobInfos.CurrentPosition;

            if (o is not JobInfo jobInfo)
                return;

            _jobInfos.Remove(jobInfo);
            _jobInfoQueue.Remove(jobInfo, true);

            //if (_jobInfos.Count > 0)
            //    JobInfos.MoveCurrentToPosition(Math.Max(0, position - 1));

            MergeJobsCommand.RaiseCanExecuteChanged();
            MergeAllJobsCommand.RaiseCanExecuteChanged();
            SortCommand.RaiseCanExecuteChanged();
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
                _jobInfoManager.DeleteInf(job);
            }

            _jobInfoManager.SaveToInfFile(first);

            MergeJobsCommand.RaiseCanExecuteChanged();
            MergeAllJobsCommand.RaiseCanExecuteChanged();
            SortCommand.RaiseCanExecuteChanged();

            JobInfos.Refresh();
        }


        public bool SelectUnselectAll { get; set; } = false;

        private bool _suppressUncheckOfSelectUnselectAllOnSelectedChanged = false;

        private void SelectUnSelectAllJobsExecute(object o)
        {
            if (o is not ListBox listBox) 
                return;

            _suppressUncheckOfSelectUnselectAllOnSelectedChanged = true;
            if (SelectUnselectAll)
                listBox.SelectAll();
            else
                listBox.UnselectAll();

            ResetLastSelectedItem?.Invoke();

            _suppressUncheckOfSelectUnselectAllOnSelectedChanged = false;
        }

        public Action ResetLastSelectedItem { get; set; }

        private bool CanExecuteMergeJobs(object o)
        {
            var jobs = o as IEnumerable<object>;
            return jobs != null && jobs.Count() > 1;
        }

        private void ExecuteMergeAllJobs(object o)
        {
            ExecuteMergeJobs(_jobInfos);
            SetDisplayedJobInfoToFirst();
        }

        private bool HasMoreThanOneJob(object o)
        {
            return _jobInfos.Count > 1;
        }

        public void SetDisplayedJobInfoToFirst()
        {
            DisplayedJobInfo = _jobInfos.FirstOrDefault();
        }
        
        public override string Title => _applicationNameProvider.ApplicationNameWithEdition + " " + _versionHelper.FormatWithThreeDigits();
    }

    public enum MergeSortingEnum
    {
        IdAscending,
        IdDescending,
        NameAscending,
        NameDescending,
        DateAscending,
        DateDescending
    }
}
