using pdfforge.PDFCreator.Conversion.Settings;
using System.Collections.Generic;
using System;

namespace pdfforge.PDFCreator.Core.SettingsManagementInterface
{
    public class LanguageChangedEventArgs : EventArgs
    {
        private readonly ApplicationSettings _appSettings;
        private readonly IEnumerable<ConversionProfile> _profiles;

        public LanguageChangedEventArgs(ApplicationSettings appSettings, IEnumerable<ConversionProfile> profiles)
        {
            _appSettings = appSettings;
            _profiles = profiles;
        }

        public ApplicationSettings AppSettings => _appSettings;

        public IEnumerable<ConversionProfile> Profiles => _profiles;
    }
}
