using pdfforge.DataStorage;
using pdfforge.PDFCreator.Utilities;
using System;

namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public class CreatorSettingsUpgrader : SettingsUpgrader
    {
        private readonly IFontHelper _fontHelper;

        public CreatorSettingsUpgrader(Data settingsData, IFontHelper fontHelper) : base(settingsData)
        {
            _fontHelper = fontHelper;
            VersionSettingPaths = new string[] { @"CreatorAppSettings\SettingsVersion", @"ApplicationSettings\SettingsVersion", @"ApplicationProperties\SettingsVersion" };
        }

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

            var profilePath = "ConversionProfiles";
            var applicationPath = "ApplicationSettings";

            ExtractTimeServerAccounts(profilePath, applicationPath);
            ExtractFtpAccounts(profilePath, applicationPath);
            ExtractSmtpAccounts(profilePath, applicationPath);
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
                var ttfFile = _fontHelper.GetFontFilename(fontFamily) ?? "arial.ttf";
                Data.SetValue(path + @"Stamping\FontFile", ttfFile);
            }, "ConversionProfiles");

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
            },
                "ConversionProfiles");
        }
    }
}
