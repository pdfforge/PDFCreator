using System;
using System.ComponentModel;
using pdfforge.PDFCreator.Conversion.Settings.Enums;

// ReSharper disable once CheckNamespace
namespace pdfforge.PDFCreator.Conversion.Settings.ProfileHasNotSupportedFeaturesExtension
{
    public static class ProfileNotSupportedFeatures
    {
        public static bool HasNotSupportedFeatures(this ConversionProfile profile)
        {
            return profile.HasNotSupportedMetadata()
                   || profile.HasNotSupportedModify()
                   || profile.HasNotSupportedSecure()
                   || profile.HasNotSupportedConvert();
        }

        public static bool HasNotSupportedMetadata(this ConversionProfile profile)
        {
            var hasMetadata = !string.IsNullOrWhiteSpace(profile.TitleTemplate)
                              || !string.IsNullOrWhiteSpace(profile.AuthorTemplate)
                              || !string.IsNullOrWhiteSpace(profile.SubjectTemplate)
                              || !string.IsNullOrWhiteSpace(profile.KeywordTemplate);

            return hasMetadata && !profile.OutputFormat.IsPdf();
        }

        public static bool HasNotSupportedModify(this ConversionProfile profile)
        {
            return profile.HasNotSupportedBackground();
        }

        public static bool HasNotSupportedBackground(this ConversionProfile profile)
        {
            return profile.BackgroundPage.Enabled && !profile.OutputFormat.IsPdf();
        }

        public static bool HasNotSupportedSecure(this ConversionProfile profile)
        {
            return profile.HasNotSupportedEncryption()
                   || profile.HasNotSupportedSignature();
        }

        public static bool HasNotSupportedEncryption(this ConversionProfile profile)
        {
            return profile.PdfSettings.Security.Enabled && profile.OutputFormat != OutputFormat.Pdf;
        }

        public static bool HasNotSupportedSignature(this ConversionProfile profile)
        {
            return profile.PdfSettings.Signature.Enabled && !profile.OutputFormat.IsPdf();
        }

        public static bool HasNotSupportedConvert(this ConversionProfile profile)
        {
            return profile.HasNotSupportedColorModel();
        }

        public static bool HasNotSupportedColorModel(this ConversionProfile profile)
        {
            return (profile.OutputFormat == OutputFormat.PdfX) && (profile.PdfSettings.ColorModel == ColorModel.Rgb);
        }

        public static void SetRaiseConditionsForNotSupportedFeatureSections(this ConversionProfile profile, PropertyChangedEventHandler onPropertyChanged)
        {
            profile.PropertyChanged += onPropertyChanged;
            profile.BackgroundPage.PropertyChanged += onPropertyChanged;
            profile.PdfSettings.PropertyChanged += onPropertyChanged;
            profile.PdfSettings.Security.PropertyChanged += onPropertyChanged;
            profile.PdfSettings.Signature.PropertyChanged += onPropertyChanged;
        }
    }
}
