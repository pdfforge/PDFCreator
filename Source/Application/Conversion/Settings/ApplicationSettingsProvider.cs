using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Conversion.Settings
{
    public class ApplicationSettingsProvider :IAppSettingsProvider
    {
        public ApplicationSettings Settings => GetSettings();

        private Func<ApplicationSettings> GetSettings;

        public ApplicationSettingsProvider(Func<ApplicationSettings> settingsFunc)
        {
            GetSettings = settingsFunc;
        }
    }

    public interface IAppSettingsProvider
    {
        ApplicationSettings Settings { get; }
    }
}
