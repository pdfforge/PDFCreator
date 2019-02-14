using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.Services.JobHistory
{
    public class JobHistoryManager : IJobHistoryManager
    {
        private IList<HistoricJob> _history;

        public IList<HistoricJob> History
        {
            get { return _history; }
            set
            {
                _history = value;
                RaiseHistoryChanged();
            }
        }

        public event EventHandler HistoryChanged;

        private bool Enabled => _settingsProvider.Settings.ApplicationSettings.JobHistory.Enabled;

        private int Capacity => _settingsProvider.Settings.ApplicationSettings.JobHistory.Capacity;

        private readonly ISettingsProvider _settingsProvider;
        private readonly IJobHistoryStorage _jobHistoryStorage;
        private readonly ITempFolderProvider _tempFolderProvider;
        private readonly IFile _file;
        private readonly IHashUtil _hashUtil;

        public JobHistoryManager(ISettingsProvider settingsProvider, IJobHistoryStorage jobHistroryStorage,
            ITempFolderProvider tempFolderProvider, IFile file, IHashUtil hashUtil)
        {
            History = new List<HistoricJob>();
            _settingsProvider = settingsProvider;
            _jobHistoryStorage = jobHistroryStorage;
            _tempFolderProvider = tempFolderProvider;
            _file = file;
            _hashUtil = hashUtil;
            _settingsProvider.SettingsChanged += (sender, args) => UpdateCapacity();
        }

        public void Save()
        {
            lock (this)
            {
                _jobHistoryStorage.Save(History);
            }
        }

        public void Clear()
        {
            History.Clear();
            Save();
            RaiseHistoryChanged();
        }

        private void UpdateCapacity()
        {
            if (History.Count > Capacity)
            {
                History = History.Skip(Math.Max(0, History.Count - Capacity)).ToList();
                RaiseHistoryChanged();
            }
        }

        public async Task Load()
        {
            lock (this)
            {
                History = _jobHistoryStorage.Load();
            }

            await Refresh();
            UpdateCapacity();
        }

        public async Task Refresh()
        {
            await Task.Run(() =>
            {
                Parallel.ForEach(History.ToArray(), CheckAndMaybeRemoveHistoricJob);
            });

            RaiseHistoryChanged();
        }

        private void CheckAndMaybeRemoveHistoricJob(HistoricJob job)
        {
            var count = job.HistoricFiles.Count;
            job.HistoricFiles = job.HistoricFiles.Where(hf => _file.Exists(hf.Path)).ToList();

            if (!count.Equals(job.HistoricFiles.Count))
                job.ChangedSinceCreation = true;

            if (job.HistoricFiles.Count == 0)
                lock (this)
                    History.Remove(job);
        }

        public void Remove(HistoricJob historicJob)
        {
            History.Remove(historicJob);
            RaiseHistoryChanged();
        }

        private bool IsTemp(Job job)
        {
            return job.OutputFileTemplate.StartsWith(_tempFolderProvider.TempFolder);
        }

        public void Add(Job job)
        {
            if (!Enabled)
                return;

            if (job.OutputFiles.Count <= 0)
                return;

            if (IsTemp(job))
                return;

            if (History.Count >= Capacity - 1)
                History.Remove(History.Last());

            History.Insert(0, TransformToHistoricJob(job));

            RaiseHistoryChanged();
        }

        public HistoricJob TransformToHistoricJob(Job job)
        {
            return new HistoricJob(CreateHistoricFiles(job), job.Profile.OutputFormat, job.JobInfo.PrintDateTime, job.JobInfo.Metadata, job.NumberOfPages, false);
        }

        private IList<HistoricFile> CreateHistoricFiles(Job job)
        {
            var historicFiles = new List<HistoricFile>();

            foreach (var file in job.OutputFiles)
            {
                var fileName = PathSafe.GetFileName(file);
                var directory = PathSafe.GetDirectoryName(file);
                var hash = BuildHash(file);
                var historicFile = new HistoricFile(file, fileName, directory, hash);
                historicFiles.Add(historicFile);
            }

            return historicFiles;
        }

        private string BuildHash(string filepath)
        {
            try
            {
                return _hashUtil.CalculateFileMd5(filepath);
            }
            catch
            {
                return "";
            }
        }

        private void RaiseHistoryChanged()
        {
            HistoryChanged?.Invoke(this, new EventArgs());
        }
    }
}
