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
        public bool IsServer { get; private set; }

        private readonly ITokenButtonFunctionProvider _buttonFunctionProvider;
        private readonly ITokenHelper _tokenHelper;
        private readonly ITokenViewModelFactory _tokenViewModelFactory;
        private readonly ErrorCodeInterpreter _errorCodeInterpreter;
        private readonly IProfileChecker _profileChecker;
        private readonly IActionManager _actionManager;

        public TokenViewModel<ConversionProfile> FileNameViewModel { get; private set; }
        public TokenViewModel<ConversionProfile> FolderViewModel { get; private set; }
        public bool AllowSkipPrintDialog { get; }

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
        }

        public override void MountView()
        {
            FileNameViewModel.MountView();
            FolderViewModel.MountView();
            RaisePropertyChanged(nameof(FileNameViewModel));
            RaisePropertyChanged(nameof(FolderViewModel));
            CurrentProfile.PropertyChanged += StatusChanged;
            CurrentProfile.AutoSave.PropertyChanged += StatusChanged;

            base.MountView();
        }

        public override void UnmountView()
        {
            base.UnmountView();

            FileNameViewModel.UnmountView();
            FolderViewModel.UnmountView();
            CurrentProfile.PropertyChanged -= StatusChanged;
            CurrentProfile.AutoSave.PropertyChanged -= StatusChanged;
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
            }
        }

        protected override void OnCurrentProfileChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            base.OnCurrentProfileChanged(sender, propertyChangedEventArgs);
            RaisePropertyChanged(nameof(TemporarySaveFiles));
        }
    }
}
