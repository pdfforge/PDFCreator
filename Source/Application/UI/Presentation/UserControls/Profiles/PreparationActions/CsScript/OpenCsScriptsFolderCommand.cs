using pdfforge.CustomScriptAction;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities.Process;
using System;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.PreparationActions.CsScript
{
    public class OpenCsScriptsFolderCommand : TranslatableCommandBase<CsScriptTranslation>
    {
        private readonly string _scriptFolder;
        private readonly IProcessStarter _processStarter;
        private readonly IDirectory _directory;
        private readonly IInteractionRequest _interactionRequest;

        public OpenCsScriptsFolderCommand(
            ITranslationUpdater translationUpdater,
            ICustomScriptLoader customScriptLoader,
            IProcessStarter processStarter,
            IDirectory directory,
            IInteractionRequest interactionRequest)
            : base(translationUpdater)
        {
            _scriptFolder = customScriptLoader.ScriptFolder;
            _processStarter = processStarter;
            _directory = directory;
            _interactionRequest = interactionRequest;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            if (!_directory.Exists(_scriptFolder))
            {
                NotifyUser();
                return;
            }

            string args = $"/e, \"{_scriptFolder}\"";
            _processStarter.Start("explorer", args);
        }

        private void NotifyUser()
        {
            var title = Translation.CsScriptDisplayName;
            var text = Translation.CsScriptsFolderDoesNotExist;
            text += Environment.NewLine;
            text += Translation.GetFormattedLicenseEnsureCsScriptsFolder(_scriptFolder);

            var interaction = new MessageInteraction(text, title, MessageOptions.Ok, MessageIcon.Error);
            _interactionRequest.Raise(interaction);
        }
    }
}
