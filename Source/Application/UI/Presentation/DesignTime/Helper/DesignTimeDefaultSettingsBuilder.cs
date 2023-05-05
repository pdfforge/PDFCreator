using pdfforge.DataStorage;
using pdfforge.DataStorage.Storage;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement.DefaultSettings;
using System.Collections.ObjectModel;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper
{
    public class DesignTimeDefaultSettingsBuilder : IDefaultSettingsBuilder
    {
        public ISettings CreateEmptySettings()
        {
            return new DesignTimeSetting(true, true);
        }

        public IEditionSettings CreateDefaultSettings(ISettings currentSettings)
        {
            return new DesignTimeSetting(true, true);
        }

        public IEditionSettings CreateDefaultSettings(string primaryPrinter, string defaultLanguage)
        {
            return new DesignTimeSetting(true, true);
        }

        public ObservableCollection<TitleReplacement> CreateDefaultTitleReplacements()
        {
            return new ObservableCollection<TitleReplacement>();
        }

        public ConversionProfile CreateDefaultProfile()
        {
            return new ConversionProfile();
        }
    }

    public class DesignTimeSetting : ISettings, IEditionSettings
    {
        private readonly bool _canLoad;
        private readonly bool _canSave;

        public DesignTimeSetting(bool canLoad, bool canSave)
        {
            _canLoad = canLoad;
            _canSave = canSave;
        }

        public bool LoadData(IStorage storage)
        {
            return _canLoad;
        }

        public bool SaveData(IStorage storage)
        {
            return _canSave;
        }

        public void ReadValues(Data data, string path = "")
        {
        }

        public Data StoreValues(string path = "")
        {
            return Data.CreateDataStorage();
        }

        public ApplicationSettings ApplicationSettings { get; set; }
    }
}
