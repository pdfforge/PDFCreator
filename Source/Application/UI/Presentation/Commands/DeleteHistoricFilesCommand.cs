using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Core.Services.JobHistory;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemInterface.IO;
using pdfforge.PDFCreator.Conversion.Jobs;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class DeleteHistoricFilesCommand : TranslatableViewModelBase<DeleteFilesTranslation>, IWaitableCommand
    {
        private readonly IFile _file;
        private readonly IInteractionRequest _interactionRequest;

        public DeleteHistoricFilesCommand(IFile file, IInteractionRequest interactionRequest, ITranslationUpdater translationUpdater)
            : base(translationUpdater)
        {
            _file = file;
            _interactionRequest = interactionRequest;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter is HistoricJob historicJob)
            {
                DeleteFilesWithQuery(historicJob.HistoricFiles);
                return;
            }
            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Error));
        }

        private void DeleteFilesWithQuery(IList<HistoricFile> files)
        {
            var interaction = BuildDeleteFilesInteraction(files);
            _interactionRequest.Raise(interaction, i => Callback(i, files));
        }

        private void Callback(MessageInteraction interaction, IList<HistoricFile> files)
        {
            if (interaction.Response != MessageResponse.Yes)
            {
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Cancel));
                return;
            }

            DoDeleteFiles(files);

            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
        }

        private MessageInteraction BuildDeleteFilesInteraction(IList<HistoricFile> files)
        {
            const int maxDisplayFiles = 5;
            var title = Translation.GetDeleteFilesTitle(files.Count);
            var message = Translation.GetAreYouSureYouWantToDeleteFilesMessage(files.Count);
            foreach (var historicFile in files.Take(maxDisplayFiles))
            {
                message += "\r\n" + historicFile.Path;
            }

            var remainingFiles = files.Skip(maxDisplayFiles).Count();
            if (remainingFiles > 0)
            {
                message += "\r\n" + Translation.GetAndXMoreMessage(remainingFiles);
            }

            return new MessageInteraction(message, title, MessageOptions.YesNo, MessageIcon.Question);
        }

        private void DoDeleteFiles(IList<HistoricFile> files)
        {
            var notDeletedFiles = new List<HistoricFile>();
            foreach (var historicFile in files)
            {
                try
                {
                    if (_file.Exists(historicFile.Path))
                        _file.Delete(historicFile.Path);
                }
                catch
                {
                    notDeletedFiles.Add(historicFile);
                }
            }

            if (notDeletedFiles.Count > 0)
                NotifyUserAboutNotDeletedFiles(notDeletedFiles);
        }

        private void NotifyUserAboutNotDeletedFiles(IList<HistoricFile> notDeletedFiles)
        {
            var title = Translation.ErrorDuringDeletionTitle;
            var message = Translation.GetCouldNotDeleteTheFollowingFilesMessage(notDeletedFiles.Count);
            foreach (var historicFile in notDeletedFiles)
            {
                message += "\r\n" + historicFile.Path;
            }
            var interaction = new MessageInteraction(message, title, MessageOptions.Ok, MessageIcon.Error);
            _interactionRequest.Raise(interaction);
        }

#pragma warning disable CS0067

        public event EventHandler CanExecuteChanged;

#pragma warning restore CS0067

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;
    }
}
