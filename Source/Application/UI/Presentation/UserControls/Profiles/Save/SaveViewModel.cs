using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.UI.Presentation.Controls;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities;
using System;
using System.ComponentModel;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public class SaveViewModel : ProfileUserControlViewModel<SaveViewTranslation>, IStatusHintViewModel
    {
        public bool HideStatusInOverlay => false;

        private readonly ITokenButtonFunctionProvider _buttonFunctionProvider;
        private readonly ITokenHelper _tokenHelper;
        private readonly ITokenViewModelFactory _tokenViewModelFactory;
        private readonly ErrorCodeInterpreter _errorCodeInterpreter;
        private readonly IProfileChecker _profileChecker;
        private readonly IActionManager _actionManager;

        public TokenViewModel<ConversionProfile> FileNameViewModel { get; private set; }
        public TokenViewModel<ConversionProfile> FolderViewModel { get; private set; }

        public bool IsServer { get; private set; }
        public bool AllowNotifications { get; }
        public bool AllowSkipPrintDialog { get; }
        public bool ShowUserTokenUsedTargetFolderWarning { get; set; }

        public string StatusText
        {
            get
            {
                var actionStatus = DetermineActionStatus();
                HasWarning = actionStatus.HasWarning;
                RaisePropertyChanged(nameof(HasWarning));
                return actionStatus.StatusText;
            }
        }

        public bool HasWarning { get; private set; }

        private (bool HasWarning, string StatusText) DetermineActionStatus()
        {
            if (CurrentProfile == null)
                return (false, "");

            var result = _profileChecker.CheckFileNameAndTargetDirectory(CurrentProfile);
            if (!result)
                return (true, _errorCodeInterpreter.GetFirstErrorText(result, false));

            if (CurrentProfile.SaveFileTemporary && !_actionManager.HasSendActions(CurrentProfile))
                return (true, Translation.NoSendActionEnabledHintInfo);

            return (false, "");
        }

        public bool AutoSaveEnabled
        {
            get
            {
                if (CurrentProfile != null)
                    return CurrentProfile.AutoSave.Enabled || IsServer;
                else
                    return IsServer;
            }
            set
            {
                CurrentProfile.AutoSave.Enabled = value || IsServer;
                RaisePropertyChanged(nameof(AutoSaveEnabled));
                RaisePropertyChanged(nameof(StatusText));
            }
        }

        public SaveViewModel(ITokenButtonFunctionProvider buttonFunctionProvider, ISelectedProfileProvider selectedProfileProvider,
            ITranslationUpdater translationUpdater, EditionHelper editionHelper, ITokenHelper tokenHelper,
            ITokenViewModelFactory tokenViewModelFactory, IDispatcher dispatcher, ErrorCodeInterpreter errorCodeInterpreter,
            IProfileChecker profileChecker, IActionManager actionManager)
            : base(translationUpdater, selectedProfileProvider, dispatcher)
        {
            IsServer = editionHelper.IsServer;
            AllowSkipPrintDialog = !editionHelper.IsFreeEdition;
            AllowNotifications = !editionHelper.IsFreeEdition;

            _buttonFunctionProvider = buttonFunctionProvider;
            _tokenHelper = tokenHelper;
            _tokenViewModelFactory = tokenViewModelFactory;
            _errorCodeInterpreter = errorCodeInterpreter;
            _profileChecker = profileChecker;
            _actionManager = actionManager;

            translationUpdater?.RegisterAndSetTranslation(tf => SetTokenViewModels());

            void RaiseHasNoSendActionForSavingTempOnly(object s, EventArgs e)
            {
                RaisePropertyChanged(nameof(HasNoSendActionForSavingTempOnly));
            }

            CurrentProfileChanged += (sender, args) =>
            {
                RaiseHasNoSendActionForSavingTempOnly(sender, args);
                CurrentProfile.PropertyChanged += RaiseHasNoSendActionForSavingTempOnly;
            };

            if (CurrentProfile != null)
                CurrentProfile.PropertyChanged += RaiseHasNoSendActionForSavingTempOnly;
        }

        private void SetTokenViewModels()
        {
            var builder = _tokenViewModelFactory
                .BuilderWithSelectedProfile();

            var tokenReplacer = _tokenHelper.TokenReplacerWithPlaceHolders;

            FileNameViewModel = builder
                .WithTokenList(th => th.GetTokenListForFilename())
                .WithTokenCustomPreview(s => ValidName.MakeValidFileName(tokenReplacer.ReplaceTokens(s)))
                .WithSelector(p => p.FileNameTemplate)
                .Build();

            FolderViewModel = builder
                    .WithTokenList(th => th.GetTokenListForDirectory())
                    .WithTokenCustomPreview(s => ValidName.MakeValidFolderName(tokenReplacer.ReplaceTokens(s)))
                    .WithSelector(p => p.TargetDirectory)
                    .WithButtonCommand(_buttonFunctionProvider.GetBrowseFolderFunction(Translation.SelectTargetDirectory))
                    .Build();

            RaisePropertyChanged(nameof(FileNameViewModel));
            RaisePropertyChanged(nameof(FolderViewModel));
            RaisePropertyChanged(nameof(HasNoSendActionForSavingTempOnly));
            IsUserTokenUsedTargetFolderServer();
            RaisePropertyChanged(nameof(ShowUserTokenUsedTargetFolderWarning));
        }

        private void IsUserTokenUsedTargetFolderServer()
        {
            ShowUserTokenUsedTargetFolderWarning = false;
            if (IsServer && !TemporarySaveFiles)
            {
                var token = FileNameViewModel.CurrentValue?.TargetDirectory;
                var isUserTokenUsedTargetFolder = _tokenHelper.TokenWarningCheck(token, CurrentProfile);
                ShowUserTokenUsedTargetFolderWarning = isUserTokenUsedTargetFolder == TokenWarningCheckResult.RequiresEnablingUserTokens;
            }
            RaisePropertyChanged(nameof(ShowUserTokenUsedTargetFolderWarning));
        }

        public override void MountView()
        {
            FileNameViewModel.MountView();
            FolderViewModel.MountView();
            RaisePropertyChanged(nameof(FileNameViewModel));
            RaisePropertyChanged(nameof(FolderViewModel));
            FolderViewModel.TextChanged += FolderViewModelOnTextChanged;
            CurrentProfile.PropertyChanged += StatusChanged;
            base.MountView();
        }

        private void FolderViewModelOnTextChanged(object sender, EventArgs e)
        {
            IsUserTokenUsedTargetFolderServer();
        }

        public override void UnmountView()
        {
            base.UnmountView();
            FileNameViewModel.UnmountView();
            FolderViewModel.UnmountView();

            FolderViewModel.TextChanged -= FolderViewModelOnTextChanged;

            CurrentProfile.PropertyChanged -= StatusChanged;
        }

        private void StatusChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(StatusText));
        }

        public bool HasNoSendActionForSavingTempOnly
        {
            get
            {
                if (CurrentProfile == null)
                    return false;
                return CurrentProfile.SaveFileTemporary && !_actionManager.HasSendActions(CurrentProfile);
            }
        }

        public bool TemporarySaveFiles
        {
            get => CurrentProfile != null && CurrentProfile.SaveFileTemporary;
            set
            {
                CurrentProfile.SaveFileTemporary = value;
                RaisePropertyChanged(nameof(HasNoSendActionForSavingTempOnly));
                IsUserTokenUsedTargetFolderServer();
            }
        }

        public bool ShowAllNotifications
        {
            get => CurrentProfile != null && CurrentProfile.ShowAllNotifications && AllowNotifications;
            set => CurrentProfile.ShowAllNotifications = value;
        }

        protected override void OnCurrentProfileChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            base.OnCurrentProfileChanged(sender, propertyChangedEventArgs);
            RaisePropertyChanged(nameof(TemporarySaveFiles));
            RaisePropertyChanged(nameof(ShowAllNotifications));
        }
    }
}
