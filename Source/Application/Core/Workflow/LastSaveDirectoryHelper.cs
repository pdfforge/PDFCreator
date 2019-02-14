using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.SettingsManagement;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.Workflow
{
    public interface ILastSaveDirectoryHelper
    {
        bool IsEnabled(Job job);

        string GetDirectory();

        void Save(Job job);
    }

    public class LastSaveDirectoryHelper : ILastSaveDirectoryHelper
    {
        private readonly ISettingsProvider _settingsProvider;
        private readonly ISettingsManager _settingsManager;
        private readonly ITempFolderProvider _tempFolderProvider;

        public LastSaveDirectoryHelper(ISettingsProvider settingsProvider, ISettingsManager settingsManager, ITempFolderProvider tempFolderProvider)
        {
            _settingsProvider = settingsProvider;
            _settingsManager = settingsManager;
            _tempFolderProvider = tempFolderProvider;
        }

        public bool IsEnabled(Job job)
        {
            return string.IsNullOrWhiteSpace(job.Profile.TargetDirectory);
        }

        private bool IsTemp(string directory)
        {
            return directory.StartsWith(_tempFolderProvider.TempFolder);
        }

        public string GetDirectory()
        {
            return _settingsProvider.Settings.CreatorAppSettings.LastSaveDirectory;
        }

        public void Save(Job job)
        {
            if (!IsEnabled(job))
                return;

            var directory = PathSafe.GetDirectoryName(job.OutputFileTemplate);

            if (IsTemp(directory))
                return;

            _settingsProvider.Settings.CreatorAppSettings.LastSaveDirectory = directory;
            _settingsManager.SaveCurrentSettings();
        }
    }
}
