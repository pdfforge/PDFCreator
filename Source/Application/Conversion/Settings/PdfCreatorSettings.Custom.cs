using System;
using System.Collections.ObjectModel;
using System.Linq;
using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.Conversion.Settings
{
    partial class PdfCreatorSettings
    {
        public PdfCreatorSettings CopyAndPreserveApplicationSettings()
        {
            var copy = Copy();

            copy.CreatorAppSettings = CreatorAppSettings;
            copy.ApplicationSettings = ApplicationSettings;

            return copy;
        }

        /// <summary>
        ///     Function that returns a profile from the inner Conversionprofiles(list) by a given guid.
        /// </summary>
        /// <param name="guid">Guid to look for</param>
        /// <returns>(First) Conversionprofile with the given guid. Returns null, if no profile with given guid exists.</returns>
        public ConversionProfile GetProfileByGuid(string guid)
        {
            return SettingsHelper.GetProfileByGuid(ConversionProfiles, guid);
        }

        /// <summary>
        ///     Function that returns a profile from the inner Conversionprofiles(list) by a given name.
        /// </summary>
        /// <param name="name">Profilename</param>
        /// <returns>(First) Conversionprofile with the given name. Returns null, if no profile with given name exists.</returns>
        public ConversionProfile GetProfileByName(string name)
        {
            return SettingsHelper.GetProfileByName(ConversionProfiles, name);
        }

        /// <summary>
        ///     Function that returns a profile from the inner Conversionprofiles(list) by a given name or guid.
        /// </summary>
        /// <param name="nameOrGuid">Profilename or Guid</param>
        /// <returns>The first Conversionprofile with the given name or first with Guid. Returns null, if no profile with given name/guid exists.</returns>

        public ConversionProfile GetProfileByNameOrGuid(string nameOrGuid)
        {
            //Ignore empty names 
            if (string.IsNullOrWhiteSpace(nameOrGuid))
                return null;

            var profile = GetProfileByName(nameOrGuid) ?? GetProfileByGuid(nameOrGuid);
            return profile;
        }

        public ConversionProfile GetProfileByMappedPrinter(string printerName)
        {
            //Ignore empty names
            if (string.IsNullOrWhiteSpace(printerName))
                return null;

            foreach (var mapping in ApplicationSettings.PrinterMappings)
            {
                if (mapping.PrinterName.Equals(printerName, StringComparison.OrdinalIgnoreCase))
                {
                    var profile = GetProfileByGuid(mapping.ProfileGuid);

                    if (mapping.ProfileGuid == ProfileGuids.LAST_USED_PROFILE_GUID)
                        profile = GetLastUsedProfile();

                    if (profile != null)
                        return profile;
                }
            }
            return null;
        }

        /// <summary>
        ///     Function that returns the last used profile, according to the LastUsedProfileGuid of the ApplicationSettings.
        ///     If the Conversionprofiles(list) does not contain a profile with the LastUsedProfileGuid (because it was deleted)
        ///     or the last guid is null the function will null.
        /// </summary>
        /// <returns>Returns last used profile. Returns null if ConversionProfiles is empty or no last profile is known.</returns>
        public ConversionProfile GetLastUsedProfile()
        {
            if (CreatorAppSettings.LastUsedProfileGuid == null)
                return null;

            return GetProfileByGuid(CreatorAppSettings.LastUsedProfileGuid);
        }

        /// <summary>
        ///     Sorts the inner list "ConversionProfiles", firstly considering their properties and then the alphabetical order
        ///     temporary > default > other
        /// </summary>
        public void SortConversionProfiles()
        {
            //((List<ConversionProfile>)ConversionProfiles).Sort(CompareTemporaryFirstDefaultSecond);
            var profiles = ConversionProfiles.ToList();
            profiles.Sort(new ProfileSorter().Compare);
            ConversionProfiles = new ObservableCollection<ConversionProfile>(profiles);
        }
        
        public DefaultViewer GetDefaultViewerByOutputFormat(OutputFormat format)
        {
            //Same default viewer for all pdf types
            if (format == OutputFormat.PdfA1B || format == OutputFormat.PdfA2B || format == OutputFormat.PdfX)
                format = OutputFormat.Pdf;

            foreach (var defaultViewer in DefaultViewers)
            {
                if (defaultViewer.OutputFormat == format)
                {
                    return defaultViewer;
                }
            }

            return new DefaultViewer
            {
                IsActive = false,
                OutputFormat = format
            };
        }

        public static OutputFormat[] GetDefaultViewerFormats()
        {
            return new[]
            {
                OutputFormat.Pdf,
                OutputFormat.Png,
                OutputFormat.Jpeg,
                OutputFormat.Tif,
                OutputFormat.Txt,
            };
        }

        public ObservableCollection<DefaultViewer> DefaultViewerList
        {
            get
            {
                var viewers = GetDefaultViewerFormats().Select(GetDefaultViewerByOutputFormat);

                return new ObservableCollection<DefaultViewer>(viewers);
            }
            set { throw new InvalidOperationException("It's not possible to set the list of default viewers");}
        }
    }
}