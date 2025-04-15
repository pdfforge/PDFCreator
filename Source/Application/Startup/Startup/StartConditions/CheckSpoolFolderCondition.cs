using System.Diagnostics;
using NLog;
using pdfforge.PDFCreator.Core.Startup.Translations;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.Utilities.Spool;
using Translatable;

namespace pdfforge.PDFCreator.Core.Startup.StartConditions
{
    public class CheckSpoolFolderCondition : IStartupCondition
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IRepairSpoolFolderAssistant _repairSpoolFolderAssistant;
        private readonly StartupApplicationTranslation _translation;
        private readonly ISpoolerProvider _spoolerProvider;
        private readonly ISpoolFolderAccess _spoolFolderAccess;

        public bool CanRequestUserInteraction => false;

        public CheckSpoolFolderCondition(ISpoolFolderAccess spoolFolderAccess, IRepairSpoolFolderAssistant repairSpoolFolderAssistant, ITranslationFactory translationFactory, ISpoolerProvider spoolerProvider)
        {
            _spoolFolderAccess = spoolFolderAccess;
            _repairSpoolFolderAssistant = repairSpoolFolderAssistant;
            _translation = translationFactory.CreateTranslation<StartupApplicationTranslation>();
            _spoolerProvider = spoolerProvider;
        }

        public StartupConditionResult Check()
        {
            if (_spoolFolderAccess.CanAccess())
                return StartupConditionResult.BuildSuccess();

            _repairSpoolFolderAssistant.TryRepairSpoolPath();

            _logger.Debug("Now we'll check again, if the spool folder is not accessible");

            if (!_spoolFolderAccess.CanAccess())
            {
                _logger.Info("The spool folder could not be repaired.");
                _repairSpoolFolderAssistant.DisplayRepairFailedMessage();
                var message = _translation.GetSpoolFolderUnableToRepairMessage(_spoolerProvider.SpoolFolder);
                return StartupConditionResult.BuildErrorWithMessage((int)ExitCode.SpoolFolderInaccessible, message);
            }
            _logger.Info("The spool folder was repaired successfully.");
            return StartupConditionResult.BuildSuccess();
        }
    }
}
