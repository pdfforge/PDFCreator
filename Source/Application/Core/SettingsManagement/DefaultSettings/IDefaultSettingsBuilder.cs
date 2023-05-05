using pdfforge.DataStorage;
using pdfforge.PDFCreator.Conversion.Settings;
using System.Collections.ObjectModel;

namespace pdfforge.PDFCreator.Core.SettingsManagement.DefaultSettings
{
    public interface IDefaultSettingsBuilder
    {
        /// <summary>
        ///     Create an empty settings class with the proper registry storage attached
        /// </summary>
        /// <returns>An empty settings object</returns>
        ISettings CreateEmptySettings();

        IEditionSettings CreateDefaultSettings(ISettings currentSettings);

        /// <summary>
        ///     Creates a settings object with default settings and profiles
        /// </summary>
        /// <returns>The initialized settings object</returns>
        IEditionSettings CreateDefaultSettings(string primaryPrinter, string defaultLanguage);

        ObservableCollection<TitleReplacement> CreateDefaultTitleReplacements();

        ConversionProfile CreateDefaultProfile();
    }
}
