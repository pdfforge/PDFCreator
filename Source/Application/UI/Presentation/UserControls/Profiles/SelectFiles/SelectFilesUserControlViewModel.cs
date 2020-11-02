using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SelectFiles
{
    public class SelectFilesUserControlViewModel : ProfileUserControlViewModel<SelectFileTranslation>
    {
        public ICommand AddAdditionalAttachmentCommand { get; private set; }
        public ICommand EditAdditionalAttachmentCommand { get; private set; }

        private readonly IInteractionRequest _interactionRequest;
        private readonly Func<string> _getSelectFileInteractionTitle;
        private readonly Func<ConversionProfile, List<string>> _getFileList;
        private readonly List<string> _tokens;
        private readonly string _filter;

        public SelectFilesUserControlViewModel(
            ITranslationUpdater translationUpdater,
            ISelectedProfileProvider selectedProfileProvider,
            IDispatcher dispatcher,
            IInteractionRequest interactionRequest,
            Func<string> getSelectFileInteractionTitle,
            Func<ConversionProfile, List<string>> getFileList,
            List<string> tokens,
            string filter)
            : base(translationUpdater, selectedProfileProvider, dispatcher)
        {
            _interactionRequest = interactionRequest;
            _getSelectFileInteractionTitle = getSelectFileInteractionTitle;
            _getFileList = getFileList;
            _tokens = tokens;
            _filter = filter;

            AddAdditionalAttachmentCommand = new AsyncCommand(AddAdditionalAttachmentExecute);
            EditAdditionalAttachmentCommand = new AsyncCommand(EditAdditionalAttachmentExecute);
        }

        private async Task AddAdditionalAttachmentExecute(object obj)
        {
            var interaction = new SelectFileInteraction(_getSelectFileInteractionTitle(), "", false, _tokens, _filter);
            var interactionResult = await _interactionRequest.RaiseAsync(interaction);

            if (interactionResult.Result != SelectFileInteractionResult.Apply)
                return;

            AddFilePath(interactionResult.File);

            RaisePropertyChanged(nameof(FileListDictionary));
            RaisePropertyChanged(nameof(FileList));
        }

        private void AddFilePath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return;
            if (FileListDictionary.ContainsKey(filePath))
                return;
            FileList.Add(filePath);
        }

        private async Task EditAdditionalAttachmentExecute(object obj)
        {
            var originalFile = obj as string;
            var interaction = new SelectFileInteraction(_getSelectFileInteractionTitle(), originalFile, true, _tokens, _filter);
            var interactionResult = await _interactionRequest.RaiseAsync(interaction);

            if (interactionResult.Result != SelectFileInteractionResult.Apply
                && interactionResult.Result != SelectFileInteractionResult.Remove)
                return;

            FileList.Remove(originalFile);

            if (interactionResult.Result == SelectFileInteractionResult.Apply)
                AddFilePath(interactionResult.File);

            RaisePropertyChanged(nameof(FileListDictionary));
            RaisePropertyChanged(nameof(FileList));
        }

        protected List<string> FileList => _getFileList(CurrentProfile) ?? new List<string>();

        public Dictionary<string, string> FileListDictionary
        {
            get
            {
                var fileDict = new Dictionary<string, string>();
                FileList.ForEach(e =>
                {
                    if (!fileDict.ContainsKey(e))
                    {
                        fileDict.Add(
                            e,
                            PathSafe.GetFileName(e));
                    }
                });

                return fileDict;
            }
        }
    }
}
