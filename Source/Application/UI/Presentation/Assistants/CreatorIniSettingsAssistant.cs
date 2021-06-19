using pdfforge.DataStorage;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System.Collections.Generic;
using System.Linq;
using pdfforge.PDFCreator.Core.SettingsManagement.Helper;
using pdfforge.PDFCreator.Core.SettingsManagement.SettingsLoading;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants
{
    public class CreatorIniSettingsAssistant : IniSettingsAssistantBase
    {
        private readonly IIniSettingsLoader _iniSettingsLoader;
        private readonly IPrinterProvider _printerProvider;
        private readonly IUacAssistant _uacAssistant;
        private readonly IActionOrderChecker _actionOrderChecker;
        private readonly ISettingsManager _settingsManager;
        private readonly ISettingsProvider _settingsProvider;

        public CreatorIniSettingsAssistant
            (
            IInteractionInvoker interactionInvoker,
            ITranslationUpdater translationUpdater,
            ISettingsManager settingsManager,
            IDataStorageFactory dataStorageFactory,
            IIniSettingsLoader iniSettingsLoader,
            IPrinterProvider printerProvider,
            IUacAssistant uacAssistant,
            IActionOrderChecker actionOrderChecker,
            EditionHelper editionHelper)
            : base(interactionInvoker, dataStorageFactory, translationUpdater, editionHelper)
        {
            _settingsManager = settingsManager;
            _settingsProvider = settingsManager.GetSettingsProvider();
            _iniSettingsLoader = iniSettingsLoader;
            _printerProvider = printerProvider;
            _uacAssistant = uacAssistant;
            _actionOrderChecker = actionOrderChecker;
        }

        public override bool LoadIniSettings()
        {
            var fileName = QueryLoadFileName();
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            var overwriteSettings = QueryOverwriteSettings();
            if (!overwriteSettings)
                return false;

            if (_iniSettingsLoader.LoadIniSettings(fileName) is PdfCreatorSettings settings)
            {
                if (!_settingsProvider.CheckValidSettings(settings))
                {
                    DisplayInvalidSettingsWarning();
                    return false;
                }

                var missingPrinters = FindMissingPrinters(settings.ApplicationSettings.PrinterMappings);

                var unusedPrinters = GetUnusedPrinters(settings.ApplicationSettings.PrinterMappings);
                if (unusedPrinters.Any())
                    QueryAndDeleteUnusedPrinters(unusedPrinters);

                if (missingPrinters.Any())
                    QueryAndAddMissingPrinters(missingPrinters);

                _actionOrderChecker.Check(settings.ConversionProfiles);

                foreach (var profile in settings.ConversionProfiles)
                {
                    profile.Properties.IsShared = false;
                }

                _settingsManager.ApplyAndSaveSettings(settings);
            }

            return true;
        }

        protected override ISettings GetSettingsCopy()
        {
            return _settingsProvider.Settings.Copy();
        }

        private List<string> GetUnusedPrinters(IEnumerable<PrinterMapping> loadedPrinterMappings)
        {
            var list = loadedPrinterMappings.Select(mapping => mapping.PrinterName).ToList();
            var installedPrinters = _printerProvider.GetPDFCreatorPrinters();
            var unusedPrinters = new List<string>();

            foreach (var printer in installedPrinters)
            {
                if (!list.Contains(printer))
                    unusedPrinters.Add(printer);
            }

            return unusedPrinters;
        }

        protected void QueryAndDeleteUnusedPrinters(List<string> unusedPrinters)
        {
            var text = Translation.AskDeleteUnusedPrinters + "\n\n" + string.Join("\n", unusedPrinters);

            var interaction = new MessageInteraction(text, Translation.UnusedPrinters, MessageOptions.YesNo, MessageIcon.Question);
            InteractionInvoker.Invoke(interaction);

            if (interaction.Response == MessageResponse.Yes)
            {
                _uacAssistant.DeletePrinter(unusedPrinters.ToArray());
            }
        }

        private List<string> FindMissingPrinters(IEnumerable<PrinterMapping> printerMappings)
        {
            var installedPrinters = _printerProvider.GetPDFCreatorPrinters();

            return printerMappings
                .Select(pm => pm.PrinterName)
                .Where(p => !installedPrinters.Contains(p))
                .Distinct()
                .ToList();
        }

        protected void QueryAndAddMissingPrinters(List<string> missingPrinters)
        {
            var text = Translation.AskAddMissingPrinters + "\n\n" + string.Join("\n", missingPrinters);

            var interaction = new MessageInteraction(text, Translation.MissingPrinters, MessageOptions.YesNo, MessageIcon.Question);
            InteractionInvoker.Invoke(interaction);

            if (interaction.Response == MessageResponse.Yes)
            {
                _uacAssistant.AddPrinters(missingPrinters.ToArray());
            }
        }
    }
}
