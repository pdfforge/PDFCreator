using pdfforge.PDFCreator.Conversion.Settings.ProfileHasNotSupportedFeaturesExtension;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Advanced;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Advanced.ForwardToOtherProfile;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Advanced.Script;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Advanced.UserToken;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab.Attachment;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab.Background;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab.Cover;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab.Stamp;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SecureTab;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SecureTab.Encrypt;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SecureTab.Sign;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.Dropbox;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.FTP;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.HTTP;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.MailClient;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.MailSmtp;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.Print;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs.ConvertTabs;
using pdfforge.PDFCreator.UI.PrismHelper.Tab;

namespace pdfforge.PDFCreator.Editions.EditionBase.CreatorTab
{
    public class ProfileSettingsTabs : TabRegion, IProfileSettingsTabs
    {
        public ProfileSettingsTabs() : base(RegionNames.ProfileTabContentRegion)
        {
            Add(new SimpleTab<SaveTab, SaveTabViewModel>(RegionNames.SaveTabContentRegion, HelpTopic.ProfileSave));
            Add(new MultiTab<ConvertTabViewModel>(RegionNames.ConvertTabContentRegion, HelpTopic.Convert, typeof(OutputFormatTab), typeof(ConvertPdfView), typeof(ConvertJpgView), typeof(ConvertPngView), typeof(ConvertTiffView), typeof(ConvertTextView)));
            Add(new SimpleTab<MetadataTab, MetadataViewModel>(RegionNames.MetadataTabContentRegion, HelpTopic.ProfileMetadata));

            var modifyTab = new MasterTab<ModifyMasterTabViewModel>(RegionNames.ModifyMasterTabItemsRegion, RegionNames.ModifyMasterTabContentRegion);
            modifyTab.AddSubTab(new SubTab<CoverUserControl, ProfileModifyTranslation>(t => t.Cover, p => p.CoverPage));
            modifyTab.AddSubTab(new SubTab<BackgroundUserControl, ProfileModifyTranslation>(t => t.Background, p => p.BackgroundPage));
            modifyTab.AddSubTab(new SubTab<WatermarkView, ProfileModifyTranslation>(t => t.Watermark, p => p.Watermark));
            modifyTab.AddSubTab(new SubTab<AttachmentUserControl, ProfileModifyTranslation>(t => t.Attachment, p => p.AttachmentPage));
            modifyTab.AddSubTab(new SubTab<StampUserControl, ProfileModifyTranslation>(t => t.Stamp, p => p.Stamping));
            Add(modifyTab);

            var sendTab = new MasterTab<SendMasterTabViewModel>(RegionNames.SendMasterTabItemsRegion, RegionNames.SendMasterTabContentRegion);
            sendTab.AddSubTab(new SubTab<FTPActionUserControl, ProfileSendSubTabTranslation>(t => t.Ftp, p => p.Ftp));
            sendTab.AddSubTab(new SubTab<MailClientUserControl, MailClientTabTranslation>(t => t.Email, p => p.EmailClientSettings));
            sendTab.AddSubTab(new SubTab<HttpActionUserControl, HttpTranslation>(t => t.HttpSubTabTitle, p => p.HttpSettings));
            sendTab.AddSubTab(new SubTab<SmtpActionUserControl, SmtpTranslation>(t => t.SmtpSubTabTitle, p => p.EmailSmtpSettings));
            sendTab.AddSubTab(new SubTab<DropboxUserControl, DropboxTranslation>(t => t.Dropbox, p => p.DropboxSettings));
            sendTab.AddSubTab(new SubTab<PrintUserControl, PrintTabTranslation>(t => t.Print, p => p.Printing));
            Add(sendTab);

            var secureTab = new MasterTab<SecureMasterTabViewModel>(RegionNames.SecureMasterTabItemsRegion, RegionNames.SecureMasterTabContentRegion);
            secureTab.AddSubTab(new SubTab<EncryptUserControl, ProfileSecureTranslation>(t => t.Encryption, p => p.PdfSettings.Security, p => p.HasNotSupportedEncryption()));
            secureTab.AddSubTab(new SubTab<SignatureUserControl, ProfileSecureTranslation>(t => t.Signature, p => p.PdfSettings.Signature, p => p.HasNotSupportedSignature()));
            Add(secureTab);

            var advancedTab = new MasterTab<AdvancedMasterTabViewModel>(RegionNames.AdvancedMasterTabItemsRegion, RegionNames.AdvancedMasterTabContentRegion);
            advancedTab.AddSubTab(new SubTab<ScriptUserControl, ProfileAdvancedTranslation>(t => t.Script, p => p.Scripting));
            advancedTab.AddSubTab(new SubTab<ForwardToFurtherProfileView, ProfileAdvancedTranslation>(t => t.Forward, p => p.ForwardToFurtherProfile));
            advancedTab.AddSubTab(new SubTab<UserTokenUserControl, ProfileAdvancedTranslation>(t => t.UserToken, p => p.UserTokens));
            Add(advancedTab);
        }
    }

    public interface IProfileSettingsTabs : ITabRegion
    {
    }
}
