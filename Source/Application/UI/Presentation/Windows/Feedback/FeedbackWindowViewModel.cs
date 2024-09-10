using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System.Windows.Data;
using NLog;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Interactions.Feedback;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Feedback;
using DelegateCommand = Prism.Commands.DelegateCommand;

namespace pdfforge.PDFCreator.UI.Presentation.Windows.Feedback
{
    public class FeedbackWindowViewModel : OverlayViewModelBase<FeedbackInteraction, FeedbackWindowTranslation>
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IFeedbackSender _feedbackSender;
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;

        public FeedbackWindowViewModel(ITranslationUpdater translationUpdater, IFeedbackSender feedbackSender, 
            IInteractionInvoker interactionInvoker, IOpenFileInteractionHelper openFileInteractionHelper) : base(translationUpdater)
        {
            _feedbackSender = feedbackSender;
            _interactionInvoker = interactionInvoker;
            _openFileInteractionHelper = openFileInteractionHelper;

            SendFeedbackCommand = new DelegateCommand(SendFeedbackCommandExecute, SendFeedbackCommandCanExecute);
            SendFeedbackCommand.ObservesProperty(() => FeedbackText);
            SendFeedbackCommand.ObservesProperty(() => FileIsAttached);
            SendFeedbackCommand.ObservesProperty(() => FileSizeLimitExceeded);
            SendFeedbackCommand.ObservesProperty(() => IsSending);
            SendFeedbackCommand.ObservesProperty(() => IsItemSelected);

            AttachFileCommand = new DelegateCommand(AttachFileCommandExecute);
            RemoveFileCommand = new DelegateCommand<string>(RemoveFileCommandExecute);

            FeedbackTypeChangedCommand = new DelegateCommand(FeedbackTypeChangedExecute);

            _feedbackTypes = new Dictionary<FeedbackType, string>
            {
                { FeedbackType.None, Translation.PleaseSelectFeedbackType },
                { FeedbackType.PositiveFeedback, Translation.PositiveFeedback },
                { FeedbackType.ReportIssue, Translation.ReportIssue },
                { FeedbackType.FeatureSuggestion, Translation.FeatureSuggestion }
            };

            TotalMb = 0;
        }

        public override string Title => Translation.ShareYourFeedback;

        private FeedbackType _selectedType = FeedbackType.None;
        private string _feedbackText = "";
        private string _email = "";
        private bool _isSending;

        public FeedbackType SelectedType
        {
            get => _selectedType;
            set
            {
                if (value == _selectedType) return;
                _selectedType = value;
                RaisePropertyChanged(nameof(SelectedType));
                RaisePropertyChanged(nameof(SelectedTypeString));
            }
        }

        public string SelectedTypeString => GetFeedbackTypeString(SelectedType);
        private string GetFeedbackTypeString(FeedbackType feedbackType)
        {
            return FeedbackTypes[feedbackType];
        }

        private Dictionary<FeedbackType, string> _feedbackTypes;
        public Dictionary<FeedbackType, string> FeedbackTypes
        {
            get => _feedbackTypes;
            set
            {
                // FeedbackTypes property is only changed when an item is removed
                // Checking for number of elements in list is enough
                if (_feedbackTypes.Count == value.Count) return;
                _feedbackTypes = value;
                RaisePropertyChanged(nameof(FeedbackTypes));
            }
        }

        public string FeedbackText
        {
            get => _feedbackText;
            set
            {
                if (value == _feedbackText) return;
                _feedbackText = value;
                RaisePropertyChanged(nameof(FeedbackText));
                RaisePropertyChanged(nameof(CharacterCount));
            }
        }

        public bool IsItemSelected { get; set; }
        public ICommand FeedbackTypeChangedCommand { get; }
        private void FeedbackTypeChangedExecute()
        {
            if (IsItemSelected)
                return;

            IsItemSelected = true;
            RaisePropertyChanged(nameof(IsItemSelected));

            FeedbackTypes = FeedbackTypes
                .Where(t => t.Key != FeedbackType.None)
                .ToDictionary(t => t.Key, t => t.Value);
        }

        public string CharacterCount => $"{FeedbackText.Length}/800";

        public CompositeCollection UploadedFiles { get; } = new();
        public int NumUploadedFiles => UploadedFiles.Count;

        public double TotalMb { get; set; }
        public string MbUsed => $"{TotalMb:F2}/5 MB";
        public bool FileSizeLimitExceeded => TotalMb > 5.0;

        public bool FileIsAttached => NumUploadedFiles > 0;

        public string Email
        {
            get => _email;
            set
            {
                if (value == _email) return;
                _email = value;
                RaisePropertyChanged(nameof(Email));
            }
        }

        public DelegateCommand SendFeedbackCommand { get; }

        private bool SendFeedbackCommandCanExecute()
        {
            if (!IsItemSelected)
                return false;

            if (IsSending)
                return false;

            if (FileSizeLimitExceeded)
                return false;

            return FeedbackText.Length > 3 || FileIsAttached;
        }

        public bool IsSending
        {
            get => _isSending;
            private set
            {
                _isSending = value;
                RaisePropertyChanged(nameof(IsSending));
            }
        }

        private async void SendFeedbackCommandExecute()
        {
            IsSending = true;

            using var content = _feedbackSender.GetFormDataContent(FeedbackText, SelectedType, UploadedFiles, GetFeedbackTypeString(SelectedType));
            using var response = await _feedbackSender.SendFeedbackAsync(content);

            IsSending = false;
            FinishInteraction();

            if (response.IsSuccessStatusCode)
            {
                var successInteraction = new FeedbackSentInteraction(SelectedType);
                _interactionInvoker.Invoke(successInteraction);
            }
            else
            {
                _logger.Error($"An error occured when attempting to submit feedback. StatusCode='{response.StatusCode}'");
                var failedInteraction = new MessageInteraction(Translation.ErrorMessage, Translation.Error, MessageOptions.Ok, MessageIcon.Error);
                _interactionInvoker.Invoke(failedInteraction);
            }
        }

        public DelegateCommand<string> RemoveFileCommand { get; }

        private void RemoveFileCommandExecute(object parameter)
        {
            var filePath = parameter as string;
            if (string.IsNullOrEmpty(filePath))
                return;

            UpdateUploadedFiles(filePath, remove: true);
        }

        public ICommand AttachFileCommand { get; }

        private void AttachFileCommandExecute()
        {
            var title = Translation.AttachFile;
            var filter = Translation.PdfFiles + @" (*.pdf)|*.pdf|" 
                                + Translation.ImageFiles + @" (*.bmp, *.jpg, *.gif, *.png, *.tif, *.tiff)|*.bmp;*.jpg;*.gif;*.png;*.tif;*.tiff|"
                                + Translation.VideoFiles + @" (*.mp4, *.mov, *.avi, *.wvm, *.webm, *.flv)|*.mp4;*.mov;*.avi;*.wvm;*.webm;*.flv|"
                                + Translation.AllFiles + " " + @"(*.*)|*.*";

            var interactionResult = _openFileInteractionHelper.StartOpenMultipleFilesInteraction("", title, filter);
            interactionResult.MatchSome(filePaths =>
            {
                foreach (var filePath in filePaths)
                {
                    if (UploadedFiles.Contains(filePath))
                    {
                        _logger.Error($"The file with path '{filePath}' is already attached! Ignoring file and continuing.");
                        continue;
                    }

                    UpdateUploadedFiles(filePath);
                }
            });
        }


        private void UpdateUploadedFiles(string filePath, bool remove = false)
        {
            var length = new FileInfo(filePath).Length;
            var sizeInMb = (double)length / (1024 * 1024);
            if (remove)
            {
                UploadedFiles.Remove(filePath);
                TotalMb -= sizeInMb;
            }
            else
            {
                UploadedFiles.Add(filePath);
                TotalMb += sizeInMb;
            }

            RaisePropertyChanged(nameof(UploadedFiles));
            RaisePropertyChanged(nameof(NumUploadedFiles));
            RaisePropertyChanged(nameof(FileIsAttached));
            RaisePropertyChanged(nameof(TotalMb));
            RaisePropertyChanged(nameof(MbUsed));
            RaisePropertyChanged(nameof(FileSizeLimitExceeded));
        }
    }
}
