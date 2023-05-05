using pdfforge.DataStorage;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System;
using System.Collections.ObjectModel;

namespace pdfforge.PDFCreator.Core.SettingsManagement.DefaultSettings
{
    public abstract class DefaultSettingsBuilderBase : IDefaultSettingsBuilder
    {
        public abstract ISettings CreateEmptySettings();

        public abstract IEditionSettings CreateDefaultSettings(ISettings currentSettings);

        public abstract IEditionSettings CreateDefaultSettings(string primaryPrinter, string defaultLanguage);

        public abstract ConversionProfile CreateDefaultProfile();

        public ObservableCollection<TitleReplacement> CreateDefaultTitleReplacements()
        {
            var startReplacements = new[]
            {
                "Microsoft Word - ",
                "Microsoft PowerPoint - ",
                "Microsoft Excel - ",
                "*"
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

        protected virtual void SetDefaultProperties(ConversionProfile profile, bool isDeletable)
        {
            profile.Properties.Renamable = false;
            profile.Properties.Deletable = isDeletable;
        }

        protected void AddDefaultLanguage(string defaultLanguage, IEditionSettings defaultSettings)
        {
            defaultSettings.ApplicationSettings.Language = defaultLanguage;
        }

        protected void AddDefaultTitleReplacements(IEditionSettings defaultSettings)
        {
            defaultSettings.ApplicationSettings.TitleReplacement = CreateDefaultTitleReplacements();
        }

        protected void AddDefaultTimeServer(IEditionSettings settings)
        {
            settings.ApplicationSettings.Accounts.TimeServerAccounts.Add(new TimeServerAccount
            {
                AccountId = Guid.NewGuid().ToString()
            });

            settings.ApplicationSettings.Accounts.TimeServerAccounts.Add(new TimeServerAccount
            {
                AccountId = Guid.NewGuid().ToString(),
                Url = "http://timestamp.globalsign.com/scripts/timestamp.dll"
            });

            settings.ApplicationSettings.Accounts.TimeServerAccounts.Add(new TimeServerAccount
            {
                AccountId = Guid.NewGuid().ToString(),
                Url = "http://timestamp.digicert.com"
            });
        }
    }
}
