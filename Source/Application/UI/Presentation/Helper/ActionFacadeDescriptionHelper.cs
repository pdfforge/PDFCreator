using pdfforge.PDFCreator.Conversion.Actions;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SecureTab.Encrypt;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.OpenFile;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.Print;
using pdfforge.PDFCreator.Utilities.Tokens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    public interface IActionFacadeDescriptionHelper
    {
        string GetDescription(IProfileSetting actionFacade);
    }

    public class ActionFacadeDescriptionHelper : IActionFacadeDescriptionHelper
    {
        private readonly ICurrentSettings<Accounts> _accountSettingsProvider;
        private readonly ISelectedProfileProvider _selectedProfileProvider;
        private readonly IScriptActionHelper _scriptActionHelper;
        private readonly ITranslationFactory _translationFactory;
        private readonly ICurrentSettings<ObservableCollection<ConversionProfile>> _profilesProvider;

        public TokenReplacer TokenReplacer { get; }

        private readonly Dictionary<Type, Func<IProfileSetting, string>> _descriptions;

        public ActionFacadeDescriptionHelper(ICurrentSettings<Accounts> accountSettingsProvider, ISelectedProfileProvider selectedProfileProvider,
            IScriptActionHelper scriptActionHelper, ITokenHelper tokenHelper, ITranslationFactory translationFactory,
            ICurrentSettings<ObservableCollection<ConversionProfile>> profilesProvider)
        {
            _accountSettingsProvider = accountSettingsProvider;
            _selectedProfileProvider = selectedProfileProvider;
            _translationFactory = translationFactory;
            _profilesProvider = profilesProvider;
            _scriptActionHelper = scriptActionHelper;

            if (tokenHelper != null)
            {
                TokenReplacer = tokenHelper.TokenReplacerWithPlaceHolders;
            }

            _descriptions = new Dictionary<Type, Func<IProfileSetting, string>>
            {
                {typeof(CoverPage), GetCoverActionDescription},
                {typeof(BackgroundPage), GetBackgroundActionDescription},
                {typeof(AttachmentPage), GetAttachmentActionDescription},
                {typeof(Stamping), GetStampActionDescription},
                {typeof(Printing), GetPrintActionDescription},
                {typeof(Ftp), GetFtpActionDescription},
                {typeof(EmailSmtpSettings), GetSmtpActionDescription},
                {typeof(DropboxSettings), GetDropboxActionDescription},
                {typeof(HttpSettings), GetHttpActionDescription},
                {typeof(Signature), GetSignatureActionDescription },
                {typeof(EmailClientSettings), GetEmailActionDescription },
                {typeof(Security), GetEncryptionActionDescription },
                {typeof(Scripting), GetSecurityActionDescription },
                {typeof(CustomScript), GetCustomScriptDescription },
                {typeof(UserTokens), GetUserTokenDescription },
                {typeof(ForwardToFurtherProfile), GetForwardToFurtherProfileDescription },
                {typeof(Watermark), GetWatermarkActionDescription },
                {typeof(OpenViewer), GetOpenViewerDescription}
            };
        }

        private string GetOpenViewerDescription(IProfileSetting profileSetting)
        {
            var openViewerActionTranslation = _translationFactory.CreateTranslation<OpenViewerActionTranslation>();
            if (profileSetting is OpenViewer openViewerSetting && openViewerSetting.OpenWithPdfArchitect)
            {
                return openViewerActionTranslation.OpenWithPdfArchitect;
            }

            return openViewerActionTranslation.OpenWithDefault;
        }

        public string GetDescription(IProfileSetting actionFacade)
        {
            if (_descriptions.TryGetValue(actionFacade.GetType(), out Func<IProfileSetting, string> descriptionFunc))
            {
                return descriptionFunc.Invoke(actionFacade);
            }
            return "";
        }

        private string GetSecurityActionDescription(IProfileSetting arg)
        {
            var scriptingSettings = _selectedProfileProvider.SelectedProfile.Scripting;
            var desc = _scriptActionHelper.GetPreview(scriptingSettings.ScriptFile, scriptingSettings.ParameterString, TokenReplacer);

            return desc;
        }

        private string GetEncryptionActionDescription(IProfileSetting arg)
        {
            return _translationFactory.CreateTranslation<EncryptUserControlTranslation>().GetEncryptionName(_selectedProfileProvider.SelectedProfile.PdfSettings.Security.EncryptionLevel);
        }

        private string GetEmailActionDescription(IProfileSetting arg)
        {
            return _selectedProfileProvider.SelectedProfile.EmailClientSettings.Recipients;
        }

        private string GetSignatureActionDescription(IProfileSetting arg)
        {
            return _selectedProfileProvider.SelectedProfile.PdfSettings.Signature.CertificateFile;
        }

        private string GetCoverActionDescription(IProfileSetting arg)
        {
            return _selectedProfileProvider.SelectedProfile.CoverPage.Files.DefaultIfEmpty()
                .Aggregate((current, next) => $"{current}\n{next}");
        }

        private string GetBackgroundActionDescription(IProfileSetting arg)
        {
            return _selectedProfileProvider.SelectedProfile.BackgroundPage.File;
        }

        private string GetAttachmentActionDescription(IProfileSetting arg)
        {
            return _selectedProfileProvider.SelectedProfile.AttachmentPage.Files.DefaultIfEmpty()
                .Aggregate((current, next) => $"{current}\n{next}");
        }

        private string GetStampActionDescription(IProfileSetting arg)
        {
            return _selectedProfileProvider.SelectedProfile.Stamping.StampText;
        }

        private string GetPrintActionDescription(IProfileSetting arg)
        {
            return _translationFactory.CreateTranslation<PrintTabTranslation>()
                .GetPrinterText(_selectedProfileProvider.SelectedProfile.Printing.SelectPrinter, _selectedProfileProvider.SelectedProfile.Printing.PrinterName);
        }

        private string GetFtpActionDescription(IProfileSetting setting)
        {
            var ftpAccount = _accountSettingsProvider.Settings.FtpAccounts.FirstOrDefault(x => x.AccountId == (setting as Ftp)?.AccountId);
            if (ftpAccount != null)
            {
                var connectionType = _translationFactory.CreateTranslation<FtpActionTranslation>()
                    .FormatFtpConnectionName(ftpAccount.Server, ftpAccount.FtpConnectionType);
                return connectionType;
            }

            return string.Empty;
        }

        private string GetDropboxActionDescription(IProfileSetting setting)
        {
            var dropboxAccount = _accountSettingsProvider.Settings.DropboxAccounts.FirstOrDefault(x => x.AccountId == (setting as DropboxSettings)?.AccountId);
            if (dropboxAccount != null)
            {
                return dropboxAccount.AccountInfo;
            }

            return string.Empty;
        }

        private string GetSmtpActionDescription(IProfileSetting setting)
        {
            var smtpAccount = _accountSettingsProvider.Settings.SmtpAccounts.FirstOrDefault(x => x.AccountId == (setting as EmailSmtpSettings)?.AccountId);
            if (smtpAccount != null)
            {
                return smtpAccount.Address;
            }

            return string.Empty;
        }

        private string GetHttpActionDescription(IProfileSetting setting)
        {
            var httpAccount = _accountSettingsProvider.Settings.HttpAccounts.FirstOrDefault(x => x.AccountId == (setting as HttpSettings)?.AccountId);
            if (httpAccount != null)
            {
                return httpAccount.Url;
            }

            return string.Empty;
        }

        private string GetCustomScriptDescription(IProfileSetting setting)
        {
            return _selectedProfileProvider.SelectedProfile.CustomScript.ScriptFilename;
        }

        private string GetUserTokenDescription(IProfileSetting setting)
        {
            switch (_selectedProfileProvider.SelectedProfile.UserTokens.Seperator)
            {
                case UserTokenSeperator.SquareBrackets:
                    return "[[[ ]]]";

                case UserTokenSeperator.AngleBrackets:
                    return "<<< >>>";

                case UserTokenSeperator.CurlyBrackets:
                    return "{{{ }}}";

                case UserTokenSeperator.RoundBrackets:
                    return "((( )))";

                default:
                    return "";
            }
        }

        private string GetForwardToFurtherProfileDescription(IProfileSetting setting)
        {
            var forwardProfileGuid = _selectedProfileProvider.SelectedProfile.ForwardToFurtherProfile.ProfileGuid;
            var profile = _profilesProvider.Settings.FirstOrDefault(p => p.Guid == forwardProfileGuid);
            if (profile != null)
                return profile.Name;

            return string.Empty;
        }

        private string GetWatermarkActionDescription(IProfileSetting arg)
        {
            return _selectedProfileProvider.SelectedProfile.Watermark.File;
        }
    }
}
