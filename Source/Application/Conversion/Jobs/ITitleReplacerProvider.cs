using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Conversion.Jobs
{
    public interface ITitleReplacerProvider
    {
        TitleReplacer BuildTitleReplacer();
    }

    public class SettingsTitleReplacerProvider : ITitleReplacerProvider
    {
        private readonly ISettingsProvider _settingsProvider;

        public SettingsTitleReplacerProvider(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;
        }

        public TitleReplacer BuildTitleReplacer()
        {
            var titleReplacer = new TitleReplacer();
            titleReplacer.AddReplacements(_settingsProvider.Settings.ApplicationSettings.TitleReplacement);
            return titleReplacer;
        }
    }

    public class LocalTitleReplacerProvider : ITitleReplacerProvider
    {
        private readonly IEnumerable<TitleReplacement> _titleReplacements;

        public LocalTitleReplacerProvider(IEnumerable<TitleReplacement> titleReplacements)
        {
            _titleReplacements = titleReplacements;
        }

        public TitleReplacer BuildTitleReplacer()
        {
            var titleReplacer = new TitleReplacer();
            titleReplacer.AddReplacements(_titleReplacements);
            return titleReplacer;
        }
    }
}
