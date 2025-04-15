using NLog;
using pdfforge.PDFCreator.Utilities.Messages;
using SystemInterface;
using SystemInterface.IO;
using Translatable;

namespace pdfforge.PDFCreator.Utilities.Spool
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
        private readonly IMessageHelper _messageHelper;
        private readonly IEnvironment _environment;
        private readonly IFile _file;
        private readonly ITranslationFactory _translationFactory;
        private readonly IPath _path;
        private readonly IShellExecuteHelper _shellExecuteHelper;
        private readonly string _tempFolder;
        private SpoolApplicationTranslation _translation;

        public RepairSpoolFolderAssistant(ITranslationFactory translationFactory, ISpoolerProvider spoolerProvider, 
            IShellExecuteHelper shellExecuteHelper, IPath path, IFile file,
            IEnvironment environment, IAssemblyHelper assemblyHelper, IMessageHelper messageHelper)
        {
            _translationFactory = translationFactory;
            _translation = _translationFactory.CreateTranslation<SpoolApplicationTranslation>();
            _shellExecuteHelper = shellExecuteHelper;
            _path = path;
            _file = file;
            _environment = environment;
            _assemblyHelper = assemblyHelper;
            _messageHelper = messageHelper;

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
            var shouldRepairResponse = _messageHelper.ShowMessage(message, title, MessageOptions.YesNo, MessageIcon.Exclamation, MessageResponse.Yes);
            if (shouldRepairResponse == MessageResponse.Yes)
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

                    _messageHelper.ShowMessage(message, title, MessageOptions.Ok, MessageIcon.Error, MessageResponse.Ok);
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

            _messageHelper.ShowMessage(message, title, MessageOptions.Ok, MessageIcon.Exclamation, MessageResponse.Ok);
        }
    }
}
