using System;
using System.Linq;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General
{
    public class HotStandbySettingsViewModel : AGeneralSettingsItemControlModel
    {
        public ICurrentSettings<CreatorAppSettings> ApplicationSettingsProvider { get; }

        public bool StandbyIsEditable => GpoSettings.HotStandbyMinutes == null;

        public StandbySetting StandbySetting
        {
            get
            {
                if (GpoSettings.HotStandbyMinutes != null)
                    return GetSetting(GpoSettings.HotStandbyMinutes.Value);

                return GetSetting(ApplicationSettingsProvider.Settings.HotStandbyMinutes);
            }
            set
            {
                if (GpoSettings.HotStandbyMinutes != null)
                    return;
                
                ApplicationSettingsProvider.Settings.HotStandbyMinutes = (int) value;
            }
        }

        private StandbySetting GetSetting(int minutes)
        {
            foreach (var setting in Enum.GetValues(typeof(StandbySetting)).Cast<StandbySetting>())
            {
                if ((int) setting == minutes)
                    return setting;
            }

            return StandbySetting.Medium;
        }

        public HotStandbySettingsViewModel(
            ICurrentSettings<CreatorAppSettings> applicationSettingsProvider,
            ITranslationUpdater translationUpdater,
            ICurrentSettingsProvider settingsProvider,
            IGpoSettings gpoSettings) : base(translationUpdater, settingsProvider, gpoSettings)
        {
            ApplicationSettingsProvider = applicationSettingsProvider;
        }
    }

    [Translatable]

    public enum StandbySetting
    {
        [Translation("No standby")]
        Disabled = 0,
        [Translation("30 minutes")]
        Short = 30,
        [Translation("2 hours")]
        Medium = 120,
        [Translation("1 day")]
        Long = 60 * 24,
        [Translation("No limit")]
        Infinite = 60 * 24 * 365
    }


}
