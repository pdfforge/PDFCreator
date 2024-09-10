using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SelectFiles
{
    public class SelectFilesUserControlViewModel : ProfileUserControlViewModel<SelectFileTranslation>
    {
        public ICommand AddFileCommand { get; private set; }
        public ICommand EditFileCommand { get; private set; }
        public ICommand RemoveFileCommand { get; private set; }

        private readonly IInteractionRequest _interactionRequest;
        private readonly Func<string> _getSelectFileInteractionTitle;
        private readonly Func<string> _getSelectAddFileButtonText;
        private readonly Func<ConversionProfile, List<string>> _profileToFileListFunction;
        private readonly List<string> _tokens;
        private readonly string _filter;

        public SelectFilesUserControlViewModel(
            ITranslationUpdater translationUpdater,
            ISelectedProfileProvider selectedProfileProvider,
            IDispatcher dispatcher,
            IInteractionRequest interactionRequest,
            Func<string> getSelectFileInteractionTitle,
            Func<string> getSelectAddFileButtonText,
            Func<ConversionProfile, List<string>> profileToFileListFunction,
            List<string> tokens,
            string filter)
            : base(translationUpdater, selectedProfileProvider, dispatcher)
        {
            _interactionRequest = interactionRequest;
            _getSelectFileInteractionTitle = getSelectFileInteractionTitle;
            _profileToFileListFunction = profileToFileListFunction;
            _getSelectAddFileButtonText = getSelectAddFileButtonText;
            _tokens = tokens;
            _filter = filter;

            AddFileCommand = new AsyncCommand(AddFileExecute);
            EditFileCommand = new AsyncCommand(EditFileExecute);
            RemoveFileCommand = new AsyncCommand(RemoveFileExecute);
        }

        private async Task AddFileExecute(object obj)
        {
            var interaction = new SelectFileInteraction(_getSelectFileInteractionTitle(), "", false, _tokens, _filter);
            var interactionResult = await _interactionRequest.RaiseAsync(interaction);

            if (interactionResult.Result != SelectFileInteractionResult.Apply)
                return;

            AddFilePath(interactionResult.File);

            RaisePropertyChanged(nameof(FileList));
        }

        private void AddFilePath(string filePath, int index = -1)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return;
            if (_fileList.Collection.Contains(filePath))
                return;
            if (index < 0 || index >= FileList.Count)
            {
                FileList.Add(filePath);
            }
            else
            {
                FileList.Insert(index, filePath);
            }
        }

        private async Task EditFileExecute(object obj)
        {
            var originalFile = obj as string;
            var originalIndex = FileList.IndexOf(originalFile);
            var interaction = new SelectFileInteraction(_getSelectFileInteractionTitle(), originalFile, true, _tokens, _filter);
            var interactionResult = await _interactionRequest.RaiseAsync(interaction);

            if (interactionResult.Result != SelectFileInteractionResult.Apply
                && interactionResult.Result != SelectFileInteractionResult.Remove)
                return;

            FileList.Remove(originalFile);

            if (interactionResult.Result == SelectFileInteractionResult.Apply)
                AddFilePath(interactionResult.File, originalIndex);

            RaisePropertyChanged(nameof(FileList));
        }

        private async Task RemoveFileExecute(object obj)
        {
            var file = obj as string;
            _ = FileList.Remove(file);
            RaisePropertyChanged(nameof(FileList));
            await Task.CompletedTask;
        }

        private Helper.SynchronizedCollection<string> _fileList;

        public ObservableCollection<string> FileList
        {
            get
            {
                _fileList = new Helper.SynchronizedCollection<string>(_profileToFileListFunction(CurrentProfile));
                return _fileList.ObservableCollection;
            }
        }

        public string ButtonText
        {
            get
            {
                var text = Translation.AddFile;
                if (_getSelectAddFileButtonText != null)
                {
                    text = _getSelectAddFileButtonText();
                }
                return text;
            }
        }
    }
}
