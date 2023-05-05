using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.WorkflowEditor;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.WorkflowEditor.Commands;
using pdfforge.PDFCreator.Utilities;
using Prism.Events;
using System.ComponentModel;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public class SaveOutputFormatMetadataViewModel : ProfileUserControlViewModel<SaveOutputFormatMetadataTranslation>, IMountable
    {
        private readonly IActionManager _actionManager;
        private readonly IProfileChecker _profileChecker;
        private readonly ErrorCodeInterpreter _errorCodeInterpreter;
        private readonly OutputFormatHelper _formatHelper = new OutputFormatHelper();
        public bool IsServer { get; private set; }
        public bool IsFreeEdition { get; private set; }

        public ICommand SetMetaDataCommand { get; set; }
        public ICommand SetSaveCommand { get; set; }
        public ICommand SetOutputFormatCommand { get; set; }

        public SaveOutputFormatMetadataViewModel(ISelectedProfileProvider selectedProfileProvider,
            ITranslationUpdater translationUpdater,
            IEventAggregator eventAggregator,
            ICommandLocator commandLocator,
            IWorkflowEditorSubViewProvider viewProvider,
            ICommandBuilderProvider commandBuilderProvider,
            IDispatcher dispatcher,
            EditionHelper editionHelper,
            IActionManager actionManager,
            IProfileChecker profileChecker,
            ErrorCodeInterpreter errorCodeInterpreter
        ) : base(translationUpdater, selectedProfileProvider, dispatcher)
        {
            _actionManager = actionManager;
            _profileChecker = profileChecker;
            _errorCodeInterpreter = errorCodeInterpreter;
            IsServer = editionHelper.IsServer;
            IsFreeEdition = editionHelper.IsFreeEdition;

            var updateSettingsPreviewsCommand = new DelegateCommand(o => UpdateSettingsPreviews());

            SetMetaDataCommand = commandBuilderProvider.ProvideBuilder(commandLocator)
                .AddInitializedCommand<WorkflowEditorCommand>(c => c.Initialize(viewProvider.MetaDataOverlay, t => t.MetaData))
                .AddCommand(updateSettingsPreviewsCommand)
                .Build();

            SetSaveCommand = commandBuilderProvider.ProvideBuilder(commandLocator)
                .AddInitializedCommand<WorkflowEditorCommand>(c => c.Initialize(viewProvider.SaveOverlay, t => t.Save))
                .AddCommand(updateSettingsPreviewsCommand)
                .Build();

            SetOutputFormatCommand = commandBuilderProvider.ProvideBuilder(commandLocator)
                .AddInitializedCommand<WorkflowEditorCommand>(c => c.Initialize(viewProvider.OutputFormatOverlay, t => t.OutputFormat))
                .AddCommand(updateSettingsPreviewsCommand)
                .Build();

            selectedProfileProvider.SelectedProfileChanged += SelectedProfileOnPropertyChanged;

            eventAggregator.GetEvent<WorkflowSettingsChanged>().Subscribe(UpdateSettingsPreviews);
            eventAggregator.GetEvent<ActionAddedToWorkflowEvent>().Subscribe(UpdateSettingsPreviews);
            eventAggregator.GetEvent<ActionRemovedFromWorkflowEvent>().Subscribe(UpdateSettingsPreviews);
        }

        #region Save

        public bool AutoSaveEnabled => CurrentProfile != null && CurrentProfile.AutoSave.Enabled || IsServer;

        public string Filename
        {
            get
            {
                var (hasWarning, filenameText) = DetermineFilenameStatus();
                FilenameHasWarning = hasWarning;
                RaisePropertyChanged(nameof(FilenameHasWarning));
                return filenameText;
            }
        }

        public bool FilenameHasWarning { get; private set; }

        private (bool HasWarning, string FilenameText) DetermineFilenameStatus()
        {
            if (CurrentProfile == null)
                return (false, "");

            var result = _profileChecker.CheckFileName(CurrentProfile);
            if (!result)
            {
                var errorMessage = _errorCodeInterpreter.GetFirstErrorText(result, false);
                return (true, errorMessage);
            }

            return (false, _formatHelper.EnsureValidExtension(CurrentProfile.FileNameTemplate, CurrentProfile.OutputFormat));
        }

        public string Directory
        {
            get
            {
                var (hasWarning, directoryText) = DetermineDirectoryStatus();
                DirectoryHasWarning = hasWarning;
                RaisePropertyChanged(nameof(DirectoryHasWarning));
                return directoryText;
            }
        }

        public bool DirectoryHasWarning { get; private set; }

        private (bool HasWarning, string DirectoryText) DetermineDirectoryStatus()
        {
            if (CurrentProfile == null)
                return (false, "");

            if (CurrentProfile.SaveFileTemporary)
            {
                if (_actionManager.HasSendActions(CurrentProfile))
                    return (false, Translation.SaveOnlyTemporary);
                return (true, Translation.SaveOnlyTemporary);
            }

            if (!string.IsNullOrEmpty(CurrentProfile.TargetDirectory))
            {
                var result = _profileChecker.CheckTargetDirectory(CurrentProfile);
                if (!result)
                {
                    var errorMessage = _errorCodeInterpreter.GetFirstErrorText(result, false);
                    return (true, errorMessage);
                }
                return (false, CurrentProfile.TargetDirectory);
            }

            if (CurrentProfile.AutoSave.Enabled)
                return (true, Translation.MissingDirectory);

            return (false, Translation.LastUsedDirectory);
        }

        public string ExistingFileBehavior
        {
            get
            {
                var (hasWarning, existingFileBehaviorText) = DetermineExistingFileBehaviorStatus();
                ExistingFileBehaviorHasWarning = hasWarning;
                RaisePropertyChanged(nameof(ExistingFileBehaviorHasWarning));
                return existingFileBehaviorText;
            }
        }

        public bool ExistingFileBehaviorHasWarning { get; private set; }

        private (bool HasWarning, string ExistingFileText) DetermineExistingFileBehaviorStatus()
        {
            if (CurrentProfile == null || !CurrentProfile.AutoSave.Enabled)
                return (false, "");

            // AutoSave is enabled

            if (CurrentProfile.AutoSave.ExistingFileBehaviour == AutoSaveExistingFileBehaviour.EnsureUniqueFilenames)
                return (false, Translation.BehaviorUnique);

            if (CurrentProfile.AutoSave.ExistingFileBehaviour == AutoSaveExistingFileBehaviour.Merge)
                return !CurrentProfile.OutputFormat.IsPdf() ? (true, Translation.NonPdfAutoMerge) : (false, Translation.BehaviorMerge);

            if (CurrentProfile.AutoSave.ExistingFileBehaviour == AutoSaveExistingFileBehaviour.MergeBeforeModifyActions)
                return !CurrentProfile.OutputFormat.IsPdf() ? (true, Translation.NonPdfAutoMerge) : (false, Translation.BehaviorMergeBeforeModifyActions);

            return (false, Translation.BehaviorOverwrite);
        }

        public bool SkipPrintDialog => CurrentProfile != null && CurrentProfile.SkipPrintDialog;

        public bool EnsureUniqueFilenames => CurrentProfile != null && CurrentProfile.AutoSave.ExistingFileBehaviour == AutoSaveExistingFileBehaviour.EnsureUniqueFilenames;

        public bool AllowTrayNotifications => !IsFreeEdition && !IsServer;

        public bool ShowTrayNotification => CurrentProfile != null && CurrentProfile.ShowAllNotifications;

        public bool SkipSendFailures => CurrentProfile != null && CurrentProfile.SkipSendFailures;

        #endregion Save

        #region OutputFormat

        public string OutputFormatString => CurrentProfile == null ? "" : CurrentProfile.OutputFormat.GetDescription();

        public string ResolutionCompressionLabel
        {
            get
            {
                if (CurrentProfile == null)
                    return "";

                if (!CurrentProfile.OutputFormat.IsPdf())
                    return Translation.ResolutionLabel;

                return Translation.CompressionLabel;
            }
        }

        public string Colors
        {
            get
            {
                if (CurrentProfile == null)
                    return "";

                try
                {
                    if (CurrentProfile.OutputFormat.IsPdf())
                    {
                        if (CurrentProfile.OutputFormat == OutputFormat.PdfX
                            && CurrentProfile.PdfSettings.ColorModel == ColorModel.Rgb)
                            return Translation.PdfColorValues[(int)ColorModel.Cmyk].Translation;

                        return Translation.PdfColorValues[(int)CurrentProfile.PdfSettings.ColorModel].Translation;
                    }

                    switch (CurrentProfile.OutputFormat)
                    {
                        case OutputFormat.Jpeg:
                            return Translation.JpegColorValues[(int)CurrentProfile.JpegSettings.Color].Translation;

                        case OutputFormat.Png:
                            return Translation.PngColorValues[(int)CurrentProfile.PngSettings.Color].Translation;

                        case OutputFormat.Tif:
                            return Translation.TiffColorValues[(int)CurrentProfile.TiffSettings.Color].Translation;

                        case OutputFormat.Txt:
                            return "./.";
                    }
                }
                catch { }

                return "";
            }
        }

        public string ResolutionCompression
        {
            get
            {
                if (CurrentProfile == null)
                    return "";

                if (CurrentProfile.OutputFormat.IsPdf())
                    return Translation.CompressionValues[(int)CurrentProfile.PdfSettings.CompressColorAndGray.Compression].Translation;

                switch (CurrentProfile.OutputFormat)
                {
                    case OutputFormat.Jpeg:
                        return CurrentProfile.JpegSettings.Dpi.ToString();

                    case OutputFormat.Png:
                        return CurrentProfile.PngSettings.Dpi.ToString();

                    case OutputFormat.Tif:
                        return CurrentProfile.TiffSettings.Dpi.ToString();

                    case OutputFormat.Txt:
                        return "./.";
                }

                return "";
            }
        }

        #endregion OutputFormat

        #region Metadata

        public string TitleTemplate => CurrentProfile == null ? "" : CurrentProfile.TitleTemplate;
        public string AuthorTemplate => CurrentProfile == null ? "" : CurrentProfile.AuthorTemplate;
        public string SubjectTemplate => CurrentProfile == null ? "" : CurrentProfile.SubjectTemplate;
        public string KeywordTemplate => CurrentProfile == null ? "" : CurrentProfile.KeywordTemplate;

        #endregion Metadata

        private void SelectedProfileOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateSettingsPreviews();
        }

        public override void MountView()
        {
            UpdateSettingsPreviews();
        }

        private void UpdateSettingsPreviews()
        {
            RaisePropertyChanged(nameof(CurrentProfile));

            RaisePropertyChanged(nameof(AutoSaveEnabled));
            RaisePropertyChanged(nameof(Filename));
            RaisePropertyChanged(nameof(Directory));
            RaisePropertyChanged(nameof(DirectoryHasWarning));
            RaisePropertyChanged(nameof(SkipPrintDialog));
            RaisePropertyChanged(nameof(SkipSendFailures));
            RaisePropertyChanged(nameof(EnsureUniqueFilenames));
            RaisePropertyChanged(nameof(ShowTrayNotification));

            RaisePropertyChanged(nameof(OutputFormatString));
            RaisePropertyChanged(nameof(Colors));
            RaisePropertyChanged(nameof(ResolutionCompressionLabel));
            RaisePropertyChanged(nameof(ResolutionCompression));

            RaisePropertyChanged(nameof(TitleTemplate));
            RaisePropertyChanged(nameof(AuthorTemplate));
            RaisePropertyChanged(nameof(SubjectTemplate));
            RaisePropertyChanged(nameof(KeywordTemplate));
            RaisePropertyChanged(nameof(ExistingFileBehavior));
        }
    }
}
