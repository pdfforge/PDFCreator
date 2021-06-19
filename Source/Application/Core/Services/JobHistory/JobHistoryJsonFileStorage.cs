using Newtonsoft.Json;
using NLog;
using pdfforge.PDFCreator.Core.SettingsManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using SystemInterface;
using SystemInterface.IO;
using pdfforge.PDFCreator.Core.SettingsManagement.Helper;

namespace pdfforge.PDFCreator.Core.Services.JobHistory
{
    public class JobHistoryJsonFileStorage : IJobHistoryStorage
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IFile _file;
        private readonly IAppDataProvider _appDataProvider;
        private readonly IEnvironment _environment;

        private readonly string _savePath;

        public JobHistoryJsonFileStorage(IEnvironment environment, IFile file, IAppDataProvider appDataProvider)
        {
            _environment = environment;
            _file = file;
            _appDataProvider = appDataProvider;

            var historyDir = _appDataProvider.LocalAppDataFolder;

            _savePath = PathSafe.Combine(historyDir, "PDFCreatorHistory.json");
        }

        public List<HistoricJob> Load()
        {
            var oldHistoryDir = PathSafe.Combine(_environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PDFCreator");
            var oldSavePath = PathSafe.Combine(oldHistoryDir, "PDFCreatorHistory.json");

            if (!_file.Exists(_savePath))
            {
                if (_file.Exists(oldSavePath))
                {
                    _file.Copy(oldSavePath, _savePath);
                    _logger.Debug($"Migrated job history from '{oldSavePath}' to '{_savePath}'.");
                }
                else
                {
                    return new List<HistoricJob>();
                }
            }

            return ReadHistoryJobsFile();
        }

        private List<HistoricJob> ReadHistoryJobsFile()
        {
            try
            {
                var value = _file.ReadAllText(_savePath);
                return JsonConvert.DeserializeObject<List<HistoricJob>>(value)
                    .Where(j => j != null)
                    .ToList();
            }
            catch (Exception)
            {
                return new List<HistoricJob>();
            }
        }

        public void Save(IList<HistoricJob> history)
        {
            try
            {
                var serializedHistory = JsonConvert.SerializeObject(history);
                _file.WriteAllText(_savePath, serializedHistory);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while saving the job history file.");
            }
        }
    }
}
