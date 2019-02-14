using System.Collections.Generic;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.Wrapper;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General
{
    public class DefaultPrinterSettingsViewModel : AGeneralSettingsItemControlModel
    {
        public ICurrentSettings<CreatorAppSettings> CreatorSettingsProvider { get; }
        private readonly AskSwitchPrinter _switchPrinterAsk;
        private readonly AskSwitchPrinter _switchPrinterYes;

        public DefaultPrinterSettingsViewModel(ITranslationUpdater translationUpdater, ICurrentSettingsProvider settingsProvider, IGpoSettings gpoSettings, ICurrentSettings<CreatorAppSettings> applicationSettingsProvider) : base(translationUpdater, settingsProvider, gpoSettings)
        {
            CreatorSettingsProvider = applicationSettingsProvider;

            _switchPrinterAsk = new AskSwitchPrinter(Translation.Ask, true);
            _switchPrinterYes = new AskSwitchPrinter(Translation.Yes, false);
            translationUpdater.RegisterAndSetTranslation(tf =>
            {
                _switchPrinterAsk.Name = Translation.Ask;
                _switchPrinterYes.Name = Translation.Yes;
                
            });
        }

        public IEnumerable<AskSwitchPrinter> AskSwitchPrinterValues
        {
            get
            {
                return new List<AskSwitchPrinter>
                {
                    _switchPrinterAsk,
                    _switchPrinterYes
                };
            }
        }
    }
}
