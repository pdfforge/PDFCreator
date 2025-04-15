﻿using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement.DefaultSettings;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System.ComponentModel;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Encryption
{
    public class EncryptionActionViewModel : ActionViewModelBase<EncryptionAction, EncryptionActionViewTranslation>
    {
        private readonly IInteractionRequest _interactionRequest;
        private readonly EditionHelper _editionHelper;

        public EncryptionActionViewModel
            (ITranslationUpdater translationUpdater, ICurrentSettingsProvider currentSettingsProvider, IDispatcher dispatcher, EditionHelper editionHelper,
            IActionLocator actionLocator, ErrorCodeInterpreter errorCodeInterpreter,
            IInteractionRequest interactionRequest, IDefaultSettingsBuilder defaultSettingsBuilder, IActionOrderHelper actionOrderHelper)
        : base(actionLocator, errorCodeInterpreter, translationUpdater, currentSettingsProvider, dispatcher, defaultSettingsBuilder, actionOrderHelper)
        {
            _interactionRequest = interactionRequest;
            _editionHelper = editionHelper;

            SecurityPasswordCommand = new DelegateCommand(SecurityPasswordExecute);
        }

        public DelegateCommand SecurityPasswordCommand { get; }

        public bool SupportsHighLevelEncryption => !_editionHelper.IsFreeEdition;

        public bool LowEncryptionEnabled
        {
            get { return CurrentProfile?.PdfSettings.Security.EncryptionLevel == EncryptionLevel.Rc128Bit; }
            set
            {
                if (value)
                    CurrentProfile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Rc128Bit;
                RaisePropertyChangedForEncryptionProperties();
            }
        }

        public bool MediumEncryptionEnabled
        {
            get { return CurrentProfile?.PdfSettings.Security.EncryptionLevel == EncryptionLevel.Aes128Bit; }
            set
            {
                if (value)
                    CurrentProfile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Aes128Bit;
                RaisePropertyChangedForEncryptionProperties();
            }
        }

        public bool HighEncryptionEnabled
        {
            get { return CurrentProfile?.PdfSettings.Security.EncryptionLevel == EncryptionLevel.Aes256Bit; }
            set
            {
                if (value)
                    CurrentProfile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Aes256Bit;
                RaisePropertyChangedForEncryptionProperties();
            }
        }

        public bool ExtendedPermissonsEnabled
        {
            get
            {
                if (CurrentProfile == null)
                    return false;
                return CurrentProfile.PdfSettings.Security.EncryptionLevel != EncryptionLevel.Rc40Bit;
            }
        }

        public bool RestrictLowQualityPrintingEnabled
        {
            get
            {
                if (CurrentProfile == null)
                    return false;
                return CurrentProfile.PdfSettings.Security.RestrictPrintingToLowQuality && ExtendedPermissonsEnabled;
            }
            set { CurrentProfile.PdfSettings.Security.RestrictPrintingToLowQuality = value; }
        }

        public bool AllowFillFormsEnabled
        {
            get
            {
                if (CurrentProfile == null)
                    return false;
                return CurrentProfile.PdfSettings.Security.AllowToFillForms || !ExtendedPermissonsEnabled;
            }
            set { CurrentProfile.PdfSettings.Security.AllowToFillForms = value; }
        }

        public bool AllowScreenReadersEnabled
        {
            get
            {
                if (CurrentProfile == null)
                    return false;
                return CurrentProfile.PdfSettings.Security.AllowScreenReader || !ExtendedPermissonsEnabled;
            }
            set { CurrentProfile.PdfSettings.Security.AllowScreenReader = value; }
        }

        public bool AllowEditingAssemblyEnabled
        {
            get
            {
                if (CurrentProfile == null)
                    return false;
                return CurrentProfile.PdfSettings.Security.AllowToEditAssembly || !ExtendedPermissonsEnabled;
            }

            set { CurrentProfile.PdfSettings.Security.AllowToEditAssembly = value; }
        }

        private void SecurityPasswordExecute(object obj)
        {
            var askUserPassword = CurrentProfile.PdfSettings.Security.RequireUserPassword;

            // ReSharper disable once UseObjectOrCollectionInitializer
            var interaction = new EncryptionPasswordInteraction(false, true, askUserPassword);
            interaction.OwnerPassword = CurrentProfile.PdfSettings.Security.OwnerPassword;
            interaction.UserPassword = CurrentProfile.PdfSettings.Security.UserPassword;
            interaction.IsAutoSaveMode = CurrentProfile.AutoSave.Enabled;


            _interactionRequest.Raise(interaction, EncryptionPasswordsCallback);
        }

        private void EncryptionPasswordsCallback(EncryptionPasswordInteraction interaction)
        {
            switch (interaction.Response)
            {
                case PasswordResult.StorePassword:
                    CurrentProfile.PdfSettings.Security.OwnerPassword = interaction.OwnerPassword;
                    CurrentProfile.PdfSettings.Security.UserPassword = interaction.UserPassword;
                    break;

                case PasswordResult.RemovePassword:
                    CurrentProfile.PdfSettings.Security.UserPassword = "";
                    CurrentProfile.PdfSettings.Security.OwnerPassword = "";
                    break;
            }

            RaisePropertyChanged(nameof(ShowPasswordHint));
        }

        protected override void OnCurrentProfileChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            base.OnCurrentProfileChanged(sender, propertyChangedEventArgs);
            RaisePropertyChangedForEncryptionProperties();
            RaisePropertyChanged(nameof(RequireUserPassword));
            RaisePropertyChanged(nameof(ShowPasswordHint));
        }

        public bool RequireUserPassword
        {
            get
            {
                return CurrentProfile != null && CurrentProfile.PdfSettings.Security.RequireUserPassword;
            }

            set
            {
                CurrentProfile.PdfSettings.Security.RequireUserPassword = value;
                RaisePropertyChanged(nameof(ShowPasswordHint));
            }
        }

        public bool ShowPasswordHint
        {
            get
            {
                if (CurrentProfile == null)
                    return false;

                if (!CurrentProfile.AutoSave.Enabled)
                    return false;

                if (CurrentProfile.PdfSettings.Security.RequireUserPassword
                    && string.IsNullOrWhiteSpace(CurrentProfile.PdfSettings.Security.UserPassword))
                    return true;

                return string.IsNullOrWhiteSpace(CurrentProfile.PdfSettings.Security.OwnerPassword);
            }
        }

        private void RaisePropertyChangedForEncryptionProperties()
        {
            RaisePropertyChanged(nameof(LowEncryptionEnabled));
            RaisePropertyChanged(nameof(MediumEncryptionEnabled));
            RaisePropertyChanged(nameof(HighEncryptionEnabled));
            RaisePropertyChanged(nameof(ExtendedPermissonsEnabled));
            RaisePropertyChanged(nameof(RestrictLowQualityPrintingEnabled));
            RaisePropertyChanged(nameof(AllowFillFormsEnabled));
            RaisePropertyChanged(nameof(AllowScreenReadersEnabled));
            RaisePropertyChanged(nameof(AllowEditingAssemblyEnabled));
        }

        protected override string SettingsPreviewString => Translation.GetEncryptionName(CurrentProfile.PdfSettings.Security.EncryptionLevel);
    }
}
