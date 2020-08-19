using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.Services.JobHistory
{
    public class JobHistoryActiveRecord : IJobHistoryActiveRecord
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

        private int Capacity => _settingsProvider.Settings.ApplicationSettings.JobHistory.Capacity;

        private readonly ISettingsProvider _settingsProvider;
        private readonly IJobHistoryStorage _jobHistoryStorage;
        private readonly ITempFolderProvider _tempFolderProvider;
        private readonly IFile _file;
        private readonly IHashUtil _hashUtil;
        private readonly IGpoSettings _gpoSettings;

        public JobHistoryActiveRecord(ISettingsProvider settingsProvider, IJobHistoryStorage jobHistoryStorage,
            ITempFolderProvider tempFolderProvider, IFile file, IHashUtil hashUtil, IGpoSettings gpoSettings)
        {
            History = new List<HistoricJob>();
            _settingsProvider = settingsProvider;
            _jobHistoryStorage = jobHistoryStorage;
            _tempFolderProvider = tempFolderProvider;
            _file = file;
            _hashUtil = hashUtil;
            _gpoSettings = gpoSettings;
            _settingsProvider.SettingsChanged += (sender, args) => UpdateCapacity();
        }

        /// <param name="force">Set true to save history even is history is disabled (e.g. for deleting history)</param>
        public void Save(bool force = false)
        {
            if (!force && !HistoryEnabled)
                return;

            lock (this)
            {
                _jobHistoryStorage.Save(History);
            }
        }

        public void Delete()
        {
            History.Clear();
            Save(true); //force saving empty list even if history is disabled
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
            if (!HistoryEnabled)
                return;

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

        public bool HistoryEnabled
        {
            set => _settingsProvider.Settings.ApplicationSettings.JobHistory.Enabled = value;
            get
            {
                if (_gpoSettings.DisableHistory)
                    return false;
                return _settingsProvider.Settings.ApplicationSettings.JobHistory.Enabled;
            }
        }

        private bool IsTemp(Job job)
        {
            return job.OutputFileTemplate.StartsWith(_tempFolderProvider.TempFolder);
        }

        public void Add(Job job)
        {
            if (!HistoryEnabled)
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
