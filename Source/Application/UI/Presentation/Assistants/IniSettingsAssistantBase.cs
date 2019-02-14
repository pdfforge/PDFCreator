using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction.DialogInteractions;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants
{
    public abstract class IniSettingsAssistantBase : IIniSettingsAssistant
    {
        protected readonly IInteractionInvoker InteractionInvoker;
        protected readonly IDataStorageFactory DataStorageFactory;
        protected LoadSettingsTranslation Translation;

        protected IniSettingsAssistantBase(
            IInteractionInvoker interactionInvoker,
            IDataStorageFactory dataStorageFactory,
        ITranslationUpdater translationUpdater)
        {
            InteractionInvoker = interactionInvoker;
            this.DataStorageFactory = dataStorageFactory;

            translationUpdater.RegisterAndSetTranslation(tf => Translation = tf.UpdateOrCreateTranslation(Translation));
        }

        public abstract bool LoadIniSettings();

        public abstract void SaveIniSettings();

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

        protected string QuerySaveFileName()
        {
            var interaction = new SaveFileInteraction();
            interaction.Filter = Translation.IniFileFilter;
            interaction.FileName = "PDFCreator.ini";

            InteractionInvoker.Invoke(interaction);

            return interaction.Success ? interaction.FileName : "";
        }
    }
}
