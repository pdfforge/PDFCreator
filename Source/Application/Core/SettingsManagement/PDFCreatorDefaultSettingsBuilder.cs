﻿using pdfforge.DataStorage;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.SettingsManagement.DefaultSettings;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public class PDFCreatorDefaultSettingsBuilder : DefaultSettingsBuilderBase
    {
        private readonly IActionOrderHelper _actionOrderHelper;

        public bool WithEmailSignature { get; set; } = true;
        public EncryptionLevel EncryptionLevel { get; set; }

        public PDFCreatorDefaultSettingsBuilder(IActionOrderHelper actionOrderHelper)
        {
            _actionOrderHelper = actionOrderHelper;
        }

        /// <summary>
        ///     Create an empty settings class with the proper registry storage attached
        /// </summary>
        /// <returns>An empty settings object</returns>
        public override ISettings CreateEmptySettings()
        {
            var settings = new PdfCreatorSettings();
            return settings;
        }

        public override IEditionSettings CreateDefaultSettings(ISettings currentSettings)
        {
            var pdfCreatorSettings = (PdfCreatorSettings)currentSettings;
            var defaultSettings = (PdfCreatorSettings)CreateDefaultSettings(
                pdfCreatorSettings.CreatorAppSettings.PrimaryPrinter,
                pdfCreatorSettings.ApplicationSettings.Language
                );

            if (pdfCreatorSettings.ApplicationSettings.PrinterMappings?.Count > 0)
            {
                foreach (var printerMapping in pdfCreatorSettings.ApplicationSettings.PrinterMappings)
                {
                    defaultSettings.ApplicationSettings.PrinterMappings.Add(new PrinterMapping(printerMapping.PrinterName, ProfileGuids.DEFAULT_PROFILE_GUID));
                }
            }
            else defaultSettings.ApplicationSettings.PrinterMappings.Add(new PrinterMapping("PDFCreator", ProfileGuids.DEFAULT_PROFILE_GUID));

            if (pdfCreatorSettings.DefaultViewers.Count > 0)
            {
                foreach (var defaultViewer in pdfCreatorSettings.DefaultViewers)
                {
                    defaultSettings.DefaultViewers.Add(new DefaultViewer() { OutputFormat = defaultViewer.OutputFormat });
                }
            }

            return defaultSettings;
        }

        /// <summary>
        ///     Creates a settings object with default settings and profiles
        /// </summary>
        /// <returns>The initialized settings object</returns>
        public override IEditionSettings CreateDefaultSettings(string primaryPrinter, string defaultLanguage)
        {
            var settings = (PdfCreatorSettings)CreateEmptySettings();

            AddPrimaryPrinter(primaryPrinter, settings);
            AddDefaultTimeServer(settings);
            AddDefaultTitleReplacements(settings);
            AddLanguage(defaultLanguage, settings);
            AddDefaultProfiles(settings);
            AddLastUsedProfileGuid(settings);

            return settings;
        }

        private void AddDefaultTitleReplacements(PdfCreatorSettings settings)
        {
            settings.ApplicationSettings.TitleReplacement = CreateDefaultTitleReplacements();
        }

        private static void AddLastUsedProfileGuid(PdfCreatorSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.CreatorAppSettings.LastUsedProfileGuid))
                settings.CreatorAppSettings.LastUsedProfileGuid = ProfileGuids.DEFAULT_PROFILE_GUID;
        }

        private static void AddLanguage(string defaultLanguage, PdfCreatorSettings settings)
        {
            settings.ApplicationSettings.Language = defaultLanguage;
        }

        private static void AddPrimaryPrinter(string primaryPrinter, PdfCreatorSettings settings)
        {
            settings.CreatorAppSettings.PrimaryPrinter = primaryPrinter;
        }

        private void AddDefaultProfiles(PdfCreatorSettings settings)
        {
            settings.ConversionProfiles.Add(CreateDefaultProfile());
            settings.ConversionProfiles.Add(CreateHighCompressionProfile());
            settings.ConversionProfiles.Add(CreateSecuredPdfProfile());
            settings.ConversionProfiles.Add(CreateHighQualityProfile());
            settings.ConversionProfiles.Add(CreateJpegProfile());
            settings.ConversionProfiles.Add(CreatePdfaProfile());
            settings.ConversionProfiles.Add(CreatePngProfile());
            settings.ConversionProfiles.Add(CreatePrintProfile());
            settings.ConversionProfiles.Add(CreateTiffProfile());

            settings.SortConversionProfiles();
        }

        private ConversionProfile CreateSecuredPdfProfile()
        {
            var profile = new ConversionProfile();
            profile.Name = "Secured PDF";
            profile.Guid = ProfileGuids.SECURED_PDF_PROFILE_GUID;

            profile.OutputFormat = OutputFormat.Pdf;
            _actionOrderHelper.EnsureValidOrder(profile.ActionOrder);
            profile.PdfSettings.Security.Enabled = true;
            profile.ActionOrder.Add(profile.PdfSettings.Security.GetType().Name);
            profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Aes256Bit;
            profile.PdfSettings.Security.RequireUserPassword = true;

            profile.PdfSettings.Security.AllowToCopyContent = false;
            profile.PdfSettings.Security.AllowPrinting = false;
            profile.PdfSettings.Security.AllowScreenReader = false;
            profile.PdfSettings.Security.AllowToEditAssembly = false;
            profile.PdfSettings.Security.AllowToEditTheDocument = false;
            profile.PdfSettings.Security.AllowToFillForms = false;

            SetDefaultProperties(profile, true);
            return profile;
        }

        private ConversionProfile CreateTiffProfile()
        {
            var tiffProfile = new ConversionProfile();
            tiffProfile.Name = "TIFF (multipage graphic file)";
            tiffProfile.Guid = ProfileGuids.TIFF_PROFILE_GUID;

            tiffProfile.OutputFormat = OutputFormat.Tif;
            tiffProfile.TiffSettings.Dpi = 150;
            tiffProfile.TiffSettings.Color = TiffColor.Color24Bit;

            SetDefaultProperties(tiffProfile, true);
            return tiffProfile;
        }

        private ConversionProfile CreatePrintProfile()
        {
            var printProfile = new ConversionProfile();
            printProfile.Name = "Print after saving";
            printProfile.Guid = ProfileGuids.PRINT_PROFILE_GUID;

            printProfile.Printing.Enabled = true;
            printProfile.ActionOrder.Add(printProfile.Printing.GetType().Name);
            _actionOrderHelper.EnsureValidOrder(printProfile.ActionOrder);
            printProfile.Printing.SelectPrinter = SelectPrinter.ShowDialog;

            SetDefaultProperties(printProfile, true);
            return printProfile;
        }

        private ConversionProfile CreatePngProfile()
        {
            var pngProfile = new ConversionProfile();
            pngProfile.Name = "PNG (graphic file)";
            pngProfile.Guid = ProfileGuids.PNG_PROFILE_GUID;

            pngProfile.OutputFormat = OutputFormat.Png;
            pngProfile.PngSettings.Dpi = 150;
            pngProfile.PngSettings.Color = PngColor.Color24Bit;

            SetDefaultProperties(pngProfile, true);
            return pngProfile;
        }

        private ConversionProfile CreatePdfaProfile()
        {
            var pdfaProfile = new ConversionProfile();
            pdfaProfile.Name = "PDF/A (long term preservation)";
            pdfaProfile.Guid = ProfileGuids.PDFA_PROFILE_GUID;

            pdfaProfile.OutputFormat = OutputFormat.PdfA2B;
            pdfaProfile.PdfSettings.CompressColorAndGray.Enabled = true;
            pdfaProfile.PdfSettings.CompressColorAndGray.Compression = CompressionColorAndGray.Automatic;
            pdfaProfile.PdfSettings.CompressMonochrome.Enabled = true;
            pdfaProfile.PdfSettings.CompressMonochrome.Compression = CompressionMonochrome.CcittFaxEncoding;

            SetDefaultProperties(pdfaProfile, true);
            return pdfaProfile;
        }

        private ConversionProfile CreateJpegProfile()
        {
            var jpegProfile = new ConversionProfile();
            jpegProfile.Name = "JPEG (graphic file)";
            jpegProfile.Guid = ProfileGuids.JPEG_PROFILE_GUID;

            jpegProfile.OutputFormat = OutputFormat.Jpeg;
            jpegProfile.JpegSettings.Dpi = 150;
            jpegProfile.JpegSettings.Color = JpegColor.Color24Bit;
            jpegProfile.JpegSettings.Quality = 75;

            SetDefaultProperties(jpegProfile, true);
            return jpegProfile;
        }

        private ConversionProfile CreateHighQualityProfile()
        {
            var highQualityProfile = new ConversionProfile();
            highQualityProfile.Name = "High Quality (large files)";
            highQualityProfile.Guid = ProfileGuids.HIGH_QUALITY_PROFILE_GUID;

            highQualityProfile.OutputFormat = OutputFormat.Pdf;
            highQualityProfile.PdfSettings.CompressColorAndGray.Enabled = true;
            highQualityProfile.PdfSettings.CompressColorAndGray.Compression = CompressionColorAndGray.Zip;
            highQualityProfile.PdfSettings.CompressMonochrome.Enabled = true;
            highQualityProfile.PdfSettings.CompressMonochrome.Compression = CompressionMonochrome.Zip;

            highQualityProfile.JpegSettings.Dpi = 300;
            highQualityProfile.JpegSettings.Quality = 100;
            highQualityProfile.JpegSettings.Color = JpegColor.Color24Bit;

            highQualityProfile.PngSettings.Dpi = 300;
            highQualityProfile.PngSettings.Color = PngColor.Color32BitTransp;

            highQualityProfile.TiffSettings.Dpi = 300;
            highQualityProfile.TiffSettings.Color = TiffColor.Color24Bit;

            SetDefaultProperties(highQualityProfile, true);
            return highQualityProfile;
        }

        private ConversionProfile CreateHighCompressionProfile()
        {
            var highCompressionProfile = new ConversionProfile();
            highCompressionProfile.Name = "High Compression (small files)";
            highCompressionProfile.Guid = ProfileGuids.HIGH_COMPRESSION_PROFILE_GUID;

            highCompressionProfile.OutputFormat = OutputFormat.Pdf;
            highCompressionProfile.PdfSettings.CompressColorAndGray.Enabled = true;
            highCompressionProfile.PdfSettings.CompressColorAndGray.Compression = CompressionColorAndGray.JpegMaximum;

            highCompressionProfile.PdfSettings.CompressMonochrome.Enabled = true;
            highCompressionProfile.PdfSettings.CompressMonochrome.Compression = CompressionMonochrome.RunLengthEncoding;

            highCompressionProfile.JpegSettings.Dpi = 100;
            highCompressionProfile.JpegSettings.Color = JpegColor.Color24Bit;
            highCompressionProfile.JpegSettings.Quality = 50;

            highCompressionProfile.PngSettings.Dpi = 100;
            highCompressionProfile.PngSettings.Color = PngColor.Color24Bit;

            highCompressionProfile.TiffSettings.Dpi = 100;
            highCompressionProfile.TiffSettings.Color = TiffColor.Color24Bit;

            SetDefaultProperties(highCompressionProfile, true);
            return highCompressionProfile;
        }

        // can be used for all profiles
        protected override void SetDefaultProperties(ConversionProfile profile, bool isDeletable)
        {
            base.SetDefaultProperties(profile, isDeletable);
            profile.OpenViewer.Enabled = true;
            profile.ActionOrder.Add(profile.OpenViewer.GetType().Name);
            profile.OpenViewer.OpenWithPdfArchitect = true;
            profile.PdfSettings.Security.EncryptionLevel = EncryptionLevel;
            profile.EmailClientSettings.AddSignature = WithEmailSignature;
            profile.EmailSmtpSettings.AddSignature = WithEmailSignature;
            profile.EmailWebSettings.AddSignature = WithEmailSignature;
        }

        public override ConversionProfile CreateDefaultProfile()
        {
            var defaultProfile = new ConversionProfile();
            defaultProfile.Name = "<Default Profile>";
            defaultProfile.Guid = ProfileGuids.DEFAULT_PROFILE_GUID;
            SetDefaultProperties(defaultProfile, false);
            return defaultProfile;
        }
    }
}
