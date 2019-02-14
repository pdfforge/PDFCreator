using System.Collections.Generic;
using System.Linq;

namespace pdfforge.PDFCreator.Conversion.Settings
{
    public static class SettingsHelper
    {
        /// <summary>
        ///     Function that returns a profile from the inner Conversionprofiles(list) by a given guid.
        /// </summary>
        /// <param name="profiles">The list of profile to search for the given guid</param>
        /// <param name="guid">Guid to look for</param>
        /// <returns>(First) Conversionprofile with the given guid. Returns null, if no profile with given guid exists.</returns>
        public static ConversionProfile GetProfileByGuid(IEnumerable<ConversionProfile> profiles, string guid)
        {
            return profiles.FirstOrDefault(p => p.Guid == guid);
        }

        /// <summary>
        ///     Function that returns a profile from the inner Conversionprofiles(list) by a given name.
        /// </summary>
        /// <param name="profiles">The list of profile to search for the given name</param>
        /// <param name="name">Profilename</param>
        /// <returns>(First) Conversionprofile with the given name. Returns null, if no profile with given name exists.</returns>
        public static ConversionProfile GetProfileByName(IEnumerable<ConversionProfile> profiles, string name)
        {
            return profiles.FirstOrDefault(p => p.Name == name);
        }
    }
}