using System;
using System.Collections.Generic;
using System.Windows.Input;
using NLog;
using SystemInterface.IO;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Logging;
using pdfforge.PDFCreator.Utilities.Messages;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Commands.QuickActions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings
{
    public class LoggingSettingViewModel : ADebugSettingsItemControlModel
    {
        private readonly IFile _fileWrap;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        public ICurrentSettings<ApplicationSettings> ApplicationSettings { get; }
        private readonly IInteractionInvoker _invoker;
        private readonly ICommand _quickActionOpenExplorerLocationCommand;

        public LoggingSettingViewModel(IInteractionInvoker invoker, IFile fileWrap,
            ITranslationUpdater translationUpdater, IGpoSettings gpoSettings, ICommandLocator commandLocator,ICurrentSettings<ApplicationSettings> applicationSettings)
            : base(translationUpdater, gpoSettings)
        {
            _fileWrap = fileWrap;
            ApplicationSettings = applicationSettings;
            _quickActionOpenExplorerLocationCommand = commandLocator.GetCommand<QuickActionOpenExplorerLocationCommand>();
            _invoker = invoker;

            ShowLogFileCommand = new DelegateCommand(ShowLogFileExecute);
            ClearLogFileCommand = new DelegateCommand(ClearLogFileExecute);
        }

        public ICommand ShowLogFileCommand { get; }
        public ICommand ClearLogFileCommand { get; }

        public IEnumerable<LoggingLevel> LoggingValues => Enum.GetValues(typeof(LoggingLevel)) as LoggingLevel[];

        private void ClearLogFileExecute(object o)
        {
            var success = LoggingHelper.ClearLogFile();

            if (!success)
                _logger.Warn("Could not clear the log file!");
        }

        private void ShowLogFileExecute(object o)
        {
            if (_fileWrap.Exists(LoggingHelper.LogFile))
            {
                _quickActionOpenExplorerLocationCommand.Execute(LoggingHelper.LogFile);
            }
            else
            {
                var caption = Translation.NoLogFile;
                var message = Translation.NoLogFileAvailable;

                var interaction = new MessageInteraction(message, caption, MessageOptions.Ok, MessageIcon.Warning);
                _invoker.Invoke(interaction);
            }
        }
    }
}