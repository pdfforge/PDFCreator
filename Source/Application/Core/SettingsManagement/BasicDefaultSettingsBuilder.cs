using pdfforge.DataStorage;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System.Collections.ObjectModel;

namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public abstract class BasicDefaultSettingsBuilder : IDefaultSettingsBuilder
    {
        public abstract ISettings CreateEmptySettings();

        public abstract ISettings CreateDefaultSettings(ISettings currentSettings);

        public abstract ISettings CreateDefaultSettings(string primaryPrinter, string defaultLanguage);

        public ObservableCollection<TitleReplacement> CreateDefaultTitleReplacements()
        {
            var startReplacements = new[]
            {
                "Microsoft Word - ",
                "Microsoft PowerPoint - ",
                "Microsoft Excel - "
            };

            var endReplacements = new[]
            {
                ".xps",
                ".xml",
                ".xltx",
                ".xltm",
                ".xlt",
                ".xlsx",
                ".xlsm",
                ".xlsb",
                ".xls",
                ".xlam",
                ".xla",
                ".wmf",
                ".txt - Editor",
                ".txt - Notepad",
                ".txt",
                ".tiff",
                ".tif",
                ".thmx",
                ".slk",
                ".rtf",
                ".prn",
                ".pptx",
                ".pptm",
                ".ppt",
                ".ppsx",
                ".ppsm",
                ".pps",
                ".ppam",
                ".ppa",
                ".potx",
                ".potm",
                ".pot",
                ".png",
                ".pdf",
                ".odt",
                ".ods",
                ".odp",
                ".mhtml",
                ".mht",
                ".jpg",
                ".jpeg",
                ".html",
                ".htm",
                ".emf",
                ".dotx",
                ".dotm",
                ".dot",
                ".docx",
                ".docm",
                ".doc",
                ".dif",
                ".csv",
                ".bmp",
                " - Editor",
                " - Notepad"
            };

            var titleReplacements = new ObservableCollection<TitleReplacement>();

            foreach (var replacement in startReplacements)
            {
                titleReplacements.Add(new TitleReplacement(ReplacementType.Start, replacement, ""));
            }

            foreach (var replacement in endReplacements)
            {
                titleReplacements.Add(new TitleReplacement(ReplacementType.End, replacement, ""));
            }

            return titleReplacements;
        }

        public ConversionProfile CreateDefaultProfile()
        {
            var defaultProfile = new ConversionProfile();
            defaultProfile.Name = "<Default Profile>";
            defaultProfile.Guid = ProfileGuids.DEFAULT_PROFILE_GUID;

            SetDefaultProperties(defaultProfile, false);
            return defaultProfile;
        }

        protected void SetDefaultProperties(ConversionProfile profile, bool isDeletable)
        {
            profile.Properties.Renamable = false;
            profile.Properties.Deletable = isDeletable;
        }
    }
}
