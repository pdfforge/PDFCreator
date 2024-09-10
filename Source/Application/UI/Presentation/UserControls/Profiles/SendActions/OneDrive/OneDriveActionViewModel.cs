using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement.DefaultSettings;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews.Microsoft;
using System.ComponentModel;
using pdfforge.PDFCreator.Conversion.Actions.Actions.OneDrive;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.OneDrive
{
    public class OneDriveActionViewModel : ActionViewModelBase<OneDriveAction, OneDriveTranslation>
    {
        public OneDriveActionViewModel(IActionLocator actionLocator, ErrorCodeInterpreter errorCodeInterpreter, ITranslationUpdater translationUpdater,
            ICurrentSettingsProvider currentSettingsProvider, IDispatcher dispatcher, IDefaultSettingsBuilder defaultSettingsBuilder, IActionOrderHelper actionOrderHelper, ITokenViewModelFactory tokenViewModelFactory) :
            base(actionLocator, errorCodeInterpreter, translationUpdater, currentSettingsProvider, dispatcher, defaultSettingsBuilder, actionOrderHelper)
        {
            translationUpdater.RegisterAndSetTranslation(tf =>
            {
                SharedFolderTokenViewModel = tokenViewModelFactory
                    .BuilderWithSelectedProfile()
                    .WithSelector(p => p.OneDriveSettings.SharedFolder)
                    .WithDefaultTokenReplacerPreview(th => th.GetTokenListForDirectory())
                    .Build();
            });
        }

        public TokenViewModel<ConversionProfile> SharedFolderTokenViewModel { get; set; }

        public bool CreateShareLink
        {
            get => CurrentProfile != null && CurrentProfile.OneDriveSettings.CreateShareLink;
            set
            {
                if (value == CurrentProfile.OneDriveSettings.CreateShareLink) 
                    return;

                CurrentProfile.OneDriveSettings.CreateShareLink = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowLink
        {
            get => CurrentProfile != null && CurrentProfile.OneDriveSettings.ShowShareLink;
            set
            {
                if (value == CurrentProfile.OneDriveSettings.ShowShareLink) return;
 
                CurrentProfile.OneDriveSettings.ShowShareLink = value;
                RaisePropertyChanged();
            }
        }

        public bool OpenUploadedFile
        {
            get => CurrentProfile != null && CurrentProfile.OneDriveSettings.OpenUploadedFile;
            set
            {
                if (value == CurrentProfile.OneDriveSettings.OpenUploadedFile) return;

                CurrentProfile.OneDriveSettings.OpenUploadedFile = value;
                RaisePropertyChanged();
            }
        }

        public bool EnsureUniqueOneDriveFilenames
        {
            get => CurrentProfile != null && CurrentProfile.OneDriveSettings.EnsureUniqueFilenames;
            set
            {
                if (value == CurrentProfile.OneDriveSettings.EnsureUniqueFilenames) return;
                CurrentProfile.OneDriveSettings.EnsureUniqueFilenames = value;
                RaisePropertyChanged();
            }
        }

        protected override string SettingsPreviewString
        {
            get
            {
                // TODO: We currently only support a single microsoft account. GetMicrosoftAccount should be modified when that changes.
                var microsoftAccount = Accounts.GetMicrosoftAccount(CurrentProfile);
                return microsoftAccount != null ? microsoftAccount.AccountInfo : string.Empty;
            }
        }

        public override void MountView()
        {
            SharedFolderTokenViewModel.MountView();

            base.MountView();
        }

        public override void UnmountView()
        {
            base.UnmountView();
            SharedFolderTokenViewModel.UnmountView();
        }

        protected override void OnCurrentProfileChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            base.OnCurrentProfileChanged(sender, propertyChangedEventArgs);

            SharedFolderTokenViewModel.RaiseTextChanged();
        }
    }
}
