using NLog;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities;
using SystemInterface;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants
{
    public interface IRepairSpoolFolderAssistant
    {
        void TryRepairSpoolPath();

        void DisplayRepairFailedMessage();
    }

    public class RepairSpoolFolderAssistant : IRepairSpoolFolderAssistant
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IAssemblyHelper _assemblyHelper;
        private readonly IEnvironment _environment;
        private readonly IFile _file;
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly IPath _path;
        private readonly IShellExecuteHelper _shellExecuteHelper;
        private readonly string _tempFolder;
        private ApplicationTranslation _translation;

        public RepairSpoolFolderAssistant(IInteractionInvoker interactionInvoker, ITranslationUpdater translationUpdater,
            ISpoolerProvider spoolerProvider, IShellExecuteHelper shellExecuteHelper, IPath path, IFile file,
            IEnvironment environment, IAssemblyHelper assemblyHelper)
        {
            _interactionInvoker = interactionInvoker;
            translationUpdater.RegisterAndSetTranslation(tf => _translation = tf.UpdateOrCreateTranslation(_translation));
            _shellExecuteHelper = shellExecuteHelper;
            _path = path;
            _file = file;
            _environment = environment;
            _assemblyHelper = assemblyHelper;

            _tempFolder = _path.GetFullPath(_path.Combine(spoolerProvider.SpoolFolder, ".."));
        }

        public void TryRepairSpoolPath()
        {
            Logger.Error(
                "The spool folder is not accessible due to a permission problem. PDFCreator will not work this way");

            var username = _environment.UserName;

            Logger.Debug("UserName is {0}", username);

            var title = _translation.SpoolFolderAccessDenied;
            var message = _translation.GetSpoolFolderAskToRepairMessage(_tempFolder);

            Logger.Debug("Asking to start repair..");
            if (ShowMessage(message, title, MessageOptions.YesNo, MessageIcon.Exclamation) == MessageResponse.Yes)
            {
                var repairToolPath = _assemblyHelper.GetAssemblyDirectory();
                repairToolPath = _path.Combine(repairToolPath, "RepairFolderPermissions.exe");

                var repairToolParameters = $"\"{username}\" \"{_tempFolder}\"";

                Logger.Debug("RepairTool path is: {0}", repairToolPath);
                Logger.Debug("Parameters: {0}", repairToolParameters);

                if (!_file.Exists(repairToolPath))
                {
                    Logger.Error("RepairFolderPermissions.exe does not exist!");
                    title = _translation.RepairToolNotFound;
                    message = _translation.GetSetupFileMissingMessage(_path.GetFileName(repairToolPath));

                    ShowMessage(message, title, MessageOptions.Ok, MessageIcon.Error);
                    return;
                }

                Logger.Debug("Starting RepairTool...");
                var result = _shellExecuteHelper.RunAsAdmin(repairToolPath, repairToolParameters);
                Logger.Debug("Done: {0}", result);
            }
        }

        public void DisplayRepairFailedMessage()
        {
            var title = _translation.SpoolFolderAccessDenied;
            var message = _translation.GetSpoolFolderUnableToRepairMessage(_tempFolder);

            ShowMessage(message, title, MessageOptions.Ok, MessageIcon.Exclamation);
        }

        private MessageResponse ShowMessage(string message, string title, MessageOptions buttons, MessageIcon icon)
        {
            var interaction = new MessageInteraction(message, title, buttons, icon);
            _interactionInvoker.Invoke(interaction);
            return interaction.Response;
        }
    }
}
