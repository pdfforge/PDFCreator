using pdfforge.DataStorage;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction.DialogInteractions;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.SettingsManagementInterface;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using pdfforge.Obsidian.Trigger;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants
{
    public interface IIniSettingsAssistant
    {
        Task<bool> LoadIniSettings();
        Task SyncPrinterMappingWithInstalledPrintersQuery(ObservableCollection<PrinterMapping> printerMappings);
        void SaveIniSettings(bool removePasswords);
    }

    public abstract class IniSettingsAssistantBase : IIniSettingsAssistant
    {
        private LoadSettingsTranslation Translation { get; set; }

        private readonly IPrinterProvider _printerProvider;
        private readonly IUacAssistant _uacAssistant;
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly IInteractionRequest _interactionRequest;
        private readonly IDataStorageFactory _dataStorageFactory;
        
        protected IniSettingsAssistantBase(
            IPrinterProvider printerProvider,
            IUacAssistant uacAssistant,
            IInteractionInvoker interactionInvoker,
            IInteractionRequest interactionRequest,
            IDataStorageFactory dataStorageFactory,
            ITranslationUpdater translationUpdater)
        {
            _printerProvider = printerProvider;
            _uacAssistant = uacAssistant;
            _interactionInvoker = interactionInvoker;
            _interactionRequest = interactionRequest;
            _dataStorageFactory = dataStorageFactory;
            translationUpdater.RegisterAndSetTranslation(tf => Translation = tf.UpdateOrCreateTranslation(Translation));
        }

        public async Task<bool> LoadIniSettings()
        {
            var fileName = QueryLoadFileName();
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            var overwriteSettings = await QueryOverwriteSettings();
            if (!overwriteSettings)
                return false;

            return await DoLoadIniSettings(fileName);
        }

        protected abstract Task<bool> DoLoadIniSettings(string fileName);

        protected abstract ISettings GetSettingsCopy();

        protected abstract string ProductName { get; }

        public void SaveIniSettings(bool removePasswords)
        {
            var settings = GetSettingsCopy();

            var suggestedFilename = Translation.FormatSettingsFileName(ProductName);

            if (removePasswords && SettingsHelper.CountPasswords(settings) > 0)
            {
                SettingsHelper.ReplacePasswords(settings, "<removed during export>");
                suggestedFilename += $" ({Translation.ReplacedPasswords})";
            }

            var fileName = QuerySaveFileName(suggestedFilename);
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            var iniStorage = _dataStorageFactory.BuildIniStorage(fileName);

            settings.SaveData(iniStorage);
        }

        private string QueryLoadFileName()
        {
            var interaction = new OpenFileInteraction();

            interaction.Filter = Translation.IniFileFilter;

            _interactionInvoker.Invoke(interaction);

            return interaction.Success ? interaction.FileName : "";
        }

        private string QuerySaveFileName(string suggestedFilename)
        {
            suggestedFilename = PathSafe.ChangeExtension(suggestedFilename, ".ini");

            var interaction = new SaveFileInteraction();
            interaction.Filter = Translation.IniFileFilter;
            interaction.FileName = suggestedFilename;

            _interactionInvoker.Invoke(interaction);

            return interaction.Success ? interaction.FileName : "";
        }

        protected async Task DisplayInvalidSettingsWarning()
        {
            var message = Translation.InvalidSettingsWarning;
            var caption = Translation.InvalidSettings;

            var interaction = new MessageInteraction(message, caption, MessageOptions.Ok, MessageIcon.Error);
            await _interactionRequest.RaiseAsync(interaction);
        }

        public async Task SyncPrinterMappingWithInstalledPrintersQuery(ObservableCollection<PrinterMapping> printerMappings)
        {
            await QueryAndDeleteUnusedPrinters(printerMappings);
            await QueryAndAddMissingPrinters(printerMappings);
        }

        private async Task<bool> QueryAndDeleteUnusedPrinters(ObservableCollection<PrinterMapping> printerMappings)
        {
            var installedPrinters = _printerProvider.GetPDFCreatorPrinters();
            var usedPrinters = printerMappings.Select(pm => pm.PrinterName).ToList();
            var unusedPrinters = installedPrinters
                .Where(p => !usedPrinters.Contains(p))
                .Distinct()
                .ToList();

            if (unusedPrinters.Any())
            {
                var text = Translation.AskDeleteUnusedPrinters + "\n\n" + string.Join("\n", unusedPrinters);
                var interaction = new MessageInteraction(text, Translation.UnusedPrinters, MessageOptions.YesNoUac, MessageIcon.Question);

                await _interactionRequest.RaiseAsync(interaction);

                if (interaction.Response == MessageResponse.Yes)
                {
                    _uacAssistant.DeletePrinter(unusedPrinters.ToArray());
                }
            }

            return true;
        }

        private async Task<bool> QueryAndAddMissingPrinters(ObservableCollection<PrinterMapping> printerMappings)
        {
            var missingPrinters = GetMissingPrinters(printerMappings);
            if (!missingPrinters.Any())
                return true;
            
            var text = Translation.AskAddMissingPrinters + "\n\n" + string.Join("\n", missingPrinters);
            var interaction = new MessageInteraction(text, Translation.MissingPrinters, MessageOptions.YesNoUac, MessageIcon.Question);

            await _interactionRequest.RaiseAsync(interaction);

            if (interaction.Response == MessageResponse.Yes)
            {
                await _uacAssistant.AddPrinters(missingPrinters.ToArray());
            }

            //Remove orphaned printers cause user declined or if printer could not be installed  
            missingPrinters = GetMissingPrinters(printerMappings);
            IList<PrinterMapping> orphanedPrinterMappings = printerMappings
                .Where(pm => missingPrinters.Contains(pm.PrinterName))
                .ToList();
            foreach (var orphanedPrinterMapping in orphanedPrinterMappings)
            {
                printerMappings.Remove(orphanedPrinterMapping);
            }

            return true;
        }

        private IList<string> GetMissingPrinters(IEnumerable<PrinterMapping> printerMappings)
        {
            var installedPrinters = _printerProvider.GetPDFCreatorPrinters();

            var missingPrinters = printerMappings
                .Select(pm => pm.PrinterName)
                .Where(p => !installedPrinters.Contains(p))
                .Distinct()
                .ToList();

            return missingPrinters;
        }

        private async Task<bool> QueryOverwriteSettings()
        {
            var message = Translation.LoadSettingsFromFileWarning;
            var caption = Translation.OverwriteAllSettings;

            var interaction = new MessageInteraction(message, caption, MessageOptions.YesNo, MessageIcon.Warning);
            await _interactionRequest.RaiseAsync(interaction);

            return interaction.Response == MessageResponse.Yes;
        }
    }
}
