using pdfforge.DataStorage;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System;

namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public class CreatorDefaultSettingsBuilder : BasicDefaultSettingsBuilder
    {
        /// <summary>
        ///     Create an empty settings class with the proper registry storage attached
        /// </summary>
        /// <returns>An empty settings object</returns>
        public override ISettings CreateEmptySettings()
        {
            var settings = new PdfCreatorSettings();
            return settings;
        }

        public override ISettings CreateDefaultSettings(ISettings currentSettings)
        {
            var pdfCreatorSettings = (PdfCreatorSettings)currentSettings;
            return CreateDefaultSettings(
                pdfCreatorSettings.CreatorAppSettings.PrimaryPrinter,
                pdfCreatorSettings.ApplicationSettings.Language
                );
        }

        /// <summary>
        ///     Creates a settings object with default settings and profiles
        /// </summary>
        /// <returns>The initialized settings object</returns>
        public override ISettings CreateDefaultSettings(string primaryPrinter, string defaultLanguage)
        {
            var settings = CreateEmptySettings();

            InstallDefaultApplicationSettings(primaryPrinter, defaultLanguage, (PdfCreatorSettings)settings);

            InstallDefaultProfileSet((PdfCreatorSettings)settings);

            return settings;
        }

        private void InstallDefaultApplicationSettings(string primaryPrinter, string defaultLanguage, PdfCreatorSettings settings)
        {
            settings.CreatorAppSettings.PrimaryPrinter = primaryPrinter;

            settings.ApplicationSettings.TitleReplacement = CreateDefaultTitleReplacements();

            settings.ApplicationSettings.Language = defaultLanguage;

            InstallDefaultTimeServer(settings);

            if (string.IsNullOrWhiteSpace(settings.CreatorAppSettings.LastUsedProfileGuid))
                settings.CreatorAppSettings.LastUsedProfileGuid = ProfileGuids.DEFAULT_PROFILE_GUID;
        }

        private void InstallDefaultTimeServer(PdfCreatorSettings settings)
        {
            settings.ApplicationSettings.Accounts.TimeServerAccounts.Add(new TimeServerAccount
            {
                AccountId = Guid.NewGuid().ToString()
            });

            settings.ApplicationSettings.Accounts.TimeServerAccounts.Add(new TimeServerAccount
            {
                AccountId = Guid.NewGuid().ToString(),
                Url = "http://timestamp.globalsign.com/scripts/timestamp.dll"
            });

            settings.ApplicationSettings.Accounts.TimeServerAccounts.Add(new TimeServerAccount
            {
                AccountId = Guid.NewGuid().ToString(),
                Url = "http://timestamp.digicert.com"
            });
        }

        private void InstallDefaultProfileSet(PdfCreatorSettings settings)
        {
            settings.ConversionProfiles.Add(CreateDefaultProfile());
            settings.ConversionProfiles.Add(CreateHighCompressionProfile());
            settings.ConversionProfiles.Add(CreateHighQualityProfile());
            settings.ConversionProfiles.Add(CreateJpegProfile());
            settings.ConversionProfiles.Add(CreatePdfaProfile());
            settings.ConversionProfiles.Add(CreatePngProfile());
            settings.ConversionProfiles.Add(CreatePrintProfile());
            settings.ConversionProfiles.Add(CreateTiffProfile());
            settings.SortConversionProfiles();
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
    }
}
