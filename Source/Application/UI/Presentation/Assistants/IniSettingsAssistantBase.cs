using pdfforge.DataStorage;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction.DialogInteractions;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants
{
    public interface IIniSettingsAssistant
    {
        bool LoadIniSettings();

        void SaveIniSettings(bool removePasswords);
    }

    public abstract class IniSettingsAssistantBase : IIniSettingsAssistant
    {
        protected readonly IInteractionInvoker InteractionInvoker;
        protected readonly IDataStorageFactory DataStorageFactory;
        private readonly EditionHelper _editionHelper;
        protected LoadSettingsTranslation Translation;

        protected IniSettingsAssistantBase(
            IInteractionInvoker interactionInvoker,
            IDataStorageFactory dataStorageFactory,
            ITranslationUpdater translationUpdater,
            EditionHelper editionHelper)
        {
            InteractionInvoker = interactionInvoker;
            DataStorageFactory = dataStorageFactory;
            _editionHelper = editionHelper;

            translationUpdater.RegisterAndSetTranslation(tf => Translation = tf.UpdateOrCreateTranslation(Translation));
        }

        public abstract bool LoadIniSettings();

        protected abstract ISettings GetSettingsCopy();

        public void SaveIniSettings(bool removePasswords)
        {
            var settings = GetSettingsCopy();

            var productName = _editionHelper.IsServer
                ? "PDFCreator Server"
                : "PDFCreator";

            var suggestedFilename = Translation.FormatSettingsFileName(productName);

            if (removePasswords && SettingsHelper.CountPasswords(settings) > 0)
            {
                SettingsHelper.ReplacePasswords(settings, "<removed during export>");
                suggestedFilename += $" ({Translation.ReplacedPasswords})";
            }

            var fileName = QuerySaveFileName(suggestedFilename);
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            var iniStorage = DataStorageFactory.BuildIniStorage(fileName);

            settings.SaveData(iniStorage);
        }

        protected void DisplayInvalidSettingsWarning()
        {
            var message = Translation.InvalidSettingsWarning;
            var caption = Translation.InvalidSettings;

            var interaction = new MessageInteraction(message, caption, MessageOptions.OK, MessageIcon.Error);
            InteractionInvoker.Invoke(interaction);
        }

        protected bool QueryOverwriteSettings()
        {
            var message = Translation.LoadSettingsFromFileWarning;
            var caption = Translation.OverwriteAllSettings;

            var interaction = new MessageInteraction(message, caption, MessageOptions.YesNo, MessageIcon.Warning);
            InteractionInvoker.Invoke(interaction);

            return interaction.Response == MessageResponse.Yes;
        }

        protected string QueryLoadFileName()
        {
            var interaction = new OpenFileInteraction();

            interaction.Filter = Translation.IniFileFilter;
            interaction.FileName = "PDFCreator.ini";

            InteractionInvoker.Invoke(interaction);

            return interaction.Success ? interaction.FileName : "";
        }

        private string QuerySaveFileName(string suggestedFilename)
        {
            suggestedFilename = PathSafe.ChangeExtension(suggestedFilename, ".ini");

            var interaction = new SaveFileInteraction();
            interaction.Filter = Translation.IniFileFilter;
            interaction.FileName = suggestedFilename;

            InteractionInvoker.Invoke(interaction);

            return interaction.Success ? interaction.FileName : "";
        }
    }
}
