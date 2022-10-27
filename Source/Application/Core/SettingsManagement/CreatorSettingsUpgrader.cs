using pdfforge.DataStorage;
using pdfforge.PDFCreator.Core.SettingsManagement.SettingsLoading;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public class CreatorSettingsUpgrader : SettingsUpgrader
    {
        private readonly IFontHelper _fontHelper;

        public CreatorSettingsUpgrader(Data settingsData, IFontHelper fontHelper) : base(settingsData)
        {
            _fontHelper = fontHelper;
        }

        protected override string ProfilePath => "ConversionProfiles";
        public override string SettingsVersionPath => @"CreatorAppSettings\SettingsVersion";
        protected override string[] VersionSettingPaths => new string[] { SettingsVersionPath, @"ApplicationSettings\SettingsVersion", @"ApplicationProperties\SettingsVersion" };

        protected override void AddUpgradeMethods()
        {
            base.AddUpgradeMethods();

            UpgradeMethods.Add(UpgradeV0ToV1);
            UpgradeMethods.Add(UpgradeV1ToV2);
            UpgradeMethods.Add(UpgradeV2ToV3);
            UpgradeMethods.Add(UpgradeV3ToV4);
            UpgradeMethods.Add(UpgradeV4ToV5);
            UpgradeMethods.Add(UpgradeV5ToV6);
            UpgradeMethods.Add(UpgradeV6ToV7);
            UpgradeMethods.Add(UpgradeV7ToV8);
            UpgradeMethods.Add(UpgradeV8ToV9);
            UpgradeMethods.Add(UpgradeV9ToV10);
            UpgradeMethods.Add(UpgradeV10ToV11);
            UpgradeMethods.Add(UpgradeV11ToV12);
            UpgradeMethods.Add(UpgradeV12ToV13);
        }

        private void UpgradeV0ToV1()
        {
            MoveSettingInAllProfiles("DefaultFormat", "OutputFormat");
            MapSettingInAllProfiles("PdfSettings\\Security\\" + "EncryptionLevel", MapEncryptionNamesV1);
            ApplyNewSettingInAllProfiles("TitleTemplate", "<PrintJobName>");
            ApplyNewSettingInAllProfiles("AuthorTemplate", "<PrintJobAuthor>");

            Data.SetValue(SettingsVersionPath, "1");
        }

        private void UpgradeV1ToV2()
        {
            MoveSettingInAllProfiles(@"CoverPage\AddBackground", @"BackgroundPage\OnCover");
            MoveSettingInAllProfiles(@"AttachmentPage\AddBackground", @"BackgroundPage\OnAttachment");
            MoveValue(@"ApplicationSettings\LastUsedProfilGuid", @"ApplicationSettings\LastUsedProfileGuid");
            Data.SetValue(SettingsVersionPath, "2");
        }

        private void UpgradeV2ToV3()
        {
            MapSettingInAllProfiles(@"OutputFormat", MapOutputformatPdfA_V3);
            Data.SetValue(SettingsVersionPath, "3");
        }

        private void UpgradeV3ToV4()
        {
            Data.SetValue(SettingsVersionPath, "4");
            MapSettingInAllProfiles(@"TiffSettings\Color", MapTiffColorBlackWhite_V4);
        }

        private void UpgradeV4ToV5()
        {
            Data.SetValue(SettingsVersionPath, "5");

            AddReplacementTypeToTitleReplacements("ApplicationSettings\\TitleReplacement");
        }

        private void UpgradeV5ToV6()
        {
            Data.SetValue(SettingsVersionPath, "6");

            MoveSettingInAllProfiles(@"EmailClient\AddSignature", @"EmailClientSettings\AddSignature");
            MoveSettingInAllProfiles(@"EmailClient\Content", @"EmailClientSettings\Content");
            MoveSettingInAllProfiles(@"EmailClient\Enabled", @"EmailClientSettings\Enabled");
            MoveSettingInAllProfiles(@"EmailClient\Recipients", @"EmailClientSettings\Recipients");
            MoveSettingInAllProfiles(@"EmailClient\Subject", @"EmailClientSettings\Subject");

            MoveSettingInAllProfiles(@"EmailSmtp\Address", @"EmailSmtpSettings\Address");
            MoveSettingInAllProfiles(@"EmailSmtp\AddSignature", @"EmailSmtpSettings\AddSignature");
            MoveSettingInAllProfiles(@"EmailSmtp\Content", @"EmailSmtpSettings\Content");
            MoveSettingInAllProfiles(@"EmailSmtp\Enabled", @"EmailSmtpSettings\Enabled");
            MoveSettingInAllProfiles(@"EmailSmtp\Password", @"EmailSmtpSettings\Password");
            MoveSettingInAllProfiles(@"EmailSmtp\Port", @"EmailSmtpSettings\Port");
            MoveSettingInAllProfiles(@"EmailSmtp\Recipients", @"EmailSmtpSettings\Recipients");
            MoveSettingInAllProfiles(@"EmailSmtp\SameTextAsClientMail", @"EmailSmtpSettings\SameTextAsClientMail");
            MoveSettingInAllProfiles(@"EmailSmtp\Server", @"EmailSmtpSettings\Server");
            MoveSettingInAllProfiles(@"EmailSmtp\Ssl", @"EmailSmtpSettings\Ssl");
            MoveSettingInAllProfiles(@"EmailSmtp\Subject", @"EmailSmtpSettings\Subject");
            MoveSettingInAllProfiles(@"EmailSmtp\UserName", @"EmailSmtpSettings\UserName");
        }

        private void UpgradeV6ToV7()
        {
            Data.SetValue(SettingsVersionPath, "7");

            ApplyV7TargetFolder();
            UpgradeToImprovedRegexTitleReplacements(@"ApplicationSettings\TitleReplacement");

            var applicationPath = "ApplicationSettings";

            ExtractTimeServerAccounts(applicationPath);
            ExtractFtpAccounts(applicationPath);
            ExtractSmtpAccounts(applicationPath);
        }

        private void UpgradeV7ToV8()
        {
            MoveValue(@"ApplicationProperties\NextUpdate", @"ApplicationSettings\NextUpdate");
            MoveSection(@"ApplicationProperties", @"CreatorAppSettings");
            Data.SetValue(SettingsVersionPath, "8");
        }

        private void UpgradeV8ToV9()
        {
            ForAllProfiles((path, i) =>
            {
                var fontFamily = Data.GetValue(path + @"Stamping\FontName");
                var ttfFile = _fontHelper.GetFontFilename(fontFamily) ?? FontHelper.DEFAULT_FONT_FILE;
                Data.SetValue(path + @"Stamping\FontFile", ttfFile);
            });

            Data.SetValue(SettingsVersionPath, "9");
        }

        private string MapTiffColorBlackWhite_V4(string s)
        {
            if (s.Equals("BlackWhite", StringComparison.OrdinalIgnoreCase))
                return "BlackWhiteG4Fax";
            return s;
        }

        private string MapOutputformatPdfA_V3(string s)
        {
            if (s.Equals("PdfA", StringComparison.OrdinalIgnoreCase))
                return "PdfA2B";
            return s;
        }

        private string MapEncryptionNamesV1(string s)
        {
            switch (s)
            {
                case "Low40Bit":
                    return "Rc40Bit";

                case "Medium128Bit":
                    return "Rc128Bit";

                case "High128BitAes":
                    return "Aes128Bit";
            }

            return "Rc128Bit";
        }

        private void ApplyV7TargetFolder()
        {
            ForAllProfiles((s, i) =>
            {
                if (GetBool(Data.GetValue(s + @"AutoSave\Enabled")) == true)
                    Data.SetValue(s + "TargetDirectory", Data.GetValue(s + @"AutoSave\TargetDirectory"));
                else
                    Data.SetValue(s + "TargetDirectory", Data.GetValue(s + @"SaveDialog\Folder"));
            });
        }

        private void UpgradeV9ToV10()
        {
            ForAllProfiles
            (
                (profileOffset, i) =>
                {
                    MigrateValueToList(profileOffset, "AttachmentPage", "File", "Files");
                    MigrateValueToList(profileOffset, "CoverPage", "File", "Files");

                    // Migrate BackgroundOnCover and BackgroundOnAttachment
                    var orderedList = GetOrderedList(profileOffset, GetList(profileOffset, "ActionOrder"));

                    MigrateOpenViewer(profileOffset, orderedList);

                    SetList(profileOffset, orderedList, "ActionOrder");
                }
            );

            Data.SetValue(SettingsVersionPath, "10");
        }

        private void UpgradeV10ToV11()
        {
            ForAllProfiles
            (
                (s, i) =>
                {
                    //EmailFormat for Smtp
                    if (GetBool(Data.GetValue(s + @"EmailSmtpSettings\Html")) == true)
                        Data.SetValue(s + @"EmailSmtpSettings\Format", "Html");
                    else
                        Data.SetValue(s + @"EmailSmtpSettings\Format", "Text");
                    Data.RemoveValue(s + @"EmailSmtpSettings\Html");

                    //EmailFormat for Client
                    if (GetBool(Data.GetValue(s + @"EmailClientSettings\Html")) == true)
                        Data.SetValue(s + @"EmailClientSettings\Format", "Html");
                    else
                        Data.SetValue(s + @"EmailClientSettings\Format", "Auto");
                    Data.RemoveValue(s + @"EmailClientSettings\Html");
                }
            );

            Data.SetValue(SettingsVersionPath, "11");
        }

        private void UpgradeV11ToV12()
        {
            MoveSettingInAllProfiles("UserTokens\\Seperator", "UserTokens\\Separator");
            Data.SetValue(SettingsVersionPath, "12");
        }

        private void UpgradeV12ToV13()
        {
            ForAllProfiles
            (
                (s, i) =>
                {
                    string displaySignature;
                    if (GetBool(Data.GetValue(s + @"PdfSettings\Signature\DisplaySignatureInDocument")) == true)
                    {
                        displaySignature = GetBool(Data.GetValue(s + @"PdfSettings\Signature\DisplayOnlyImage")) == true ? "ImageOnly" : "ImageAndText";
                    }
                    else
                    {
                        displaySignature = "NoDisplay";
                    }
                    Data.SetValue(s + @"PdfSettings\Signature\DisplaySignature", displaySignature);
                    Data.RemoveValue(s + @"PdfSettings\Signature\DisplayOnlyImage");
                    Data.RemoveValue(s + @"PdfSettings\Signature\DisplaySignatureInDocument");
                }
            );
            Data.SetValue(SettingsVersionPath, "13");
        }

        protected IList<string> GetActionOrder(string profileOffset)
        {
            var list = new List<string>();

            int.TryParse(Data.GetValue(profileOffset + @"ActionOrder\numClasses"), out var actionCount);
            for (int i = 0; i < actionCount; i++)
            {
                list.Add(Data.GetValue($@"{profileOffset}ActionOrder\{i}\ActionOrder"));
            }
            return list;
        }
    }
}
