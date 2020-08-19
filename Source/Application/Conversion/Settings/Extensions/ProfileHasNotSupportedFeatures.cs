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
                   || profile.HasNotSupportedConvert()
                   || profile.HasNotSupportedSendSettings();
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
            return false;
        }

        public static bool HasNotSupportedSecure(this ConversionProfile profile)
        {
            return profile.HasNotSupportedEncryption()
                   || profile.HasNotSupportedSignature();
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

        public static bool HasNotSupportedSendSettings(this ConversionProfile profile)
        {
            var hasSendSettings = profile.EmailClientSettings.Enabled
                                  || profile.DropboxSettings.Enabled
                                  || profile.Ftp.Enabled
                                  || profile.HttpSettings.Enabled
                                  || profile.Printing.Enabled
                                  || profile.EmailSmtpSettings.Enabled;



            return !hasSendSettings && profile.SaveFileTemporary;
        }

        public static void MountRaiseConditionsForNotSupportedFeatureSections(this ConversionProfile profile, PropertyChangedEventHandler onPropertyChanged)
        {
            profile.PropertyChanged += onPropertyChanged;
            profile.BackgroundPage.PropertyChanged += onPropertyChanged;
            profile.PdfSettings.PropertyChanged += onPropertyChanged;
            profile.PdfSettings.Security.PropertyChanged += onPropertyChanged;
            profile.PdfSettings.Signature.PropertyChanged += onPropertyChanged;  
            
            profile.Ftp.PropertyChanged += onPropertyChanged;          
            profile.EmailSmtpSettings.PropertyChanged += onPropertyChanged;          
            profile.DropboxSettings.PropertyChanged += onPropertyChanged;
            profile.Printing.PropertyChanged += onPropertyChanged;
            profile.HttpSettings.PropertyChanged += onPropertyChanged;
            profile.EmailClientSettings.PropertyChanged += onPropertyChanged;
        }

        public static void UnMountRaiseConditionsForNotSupportedFeatureSections(this ConversionProfile profile, PropertyChangedEventHandler onPropertyChanged)
        {
            profile.PropertyChanged -= onPropertyChanged;
            profile.BackgroundPage.PropertyChanged -= onPropertyChanged;
            profile.PdfSettings.PropertyChanged -= onPropertyChanged;
            profile.PdfSettings.Security.PropertyChanged -= onPropertyChanged;
            profile.PdfSettings.Signature.PropertyChanged -= onPropertyChanged;  
            
            profile.Ftp.PropertyChanged -= onPropertyChanged;          
            profile.EmailSmtpSettings.PropertyChanged -= onPropertyChanged;          
            profile.DropboxSettings.PropertyChanged -= onPropertyChanged;
            profile.Printing.PropertyChanged -= onPropertyChanged;
            profile.HttpSettings.PropertyChanged -= onPropertyChanged;
            profile.EmailClientSettings.PropertyChanged -= onPropertyChanged;
        }
    }
}
