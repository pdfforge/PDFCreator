using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        private static bool IsPasswordProperty(PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType != typeof(string))
                return false;

            if (propertyInfo.Name.EndsWith("Password") || propertyInfo.Name.EndsWith("AccessToken") || propertyInfo.Name.EndsWith("RefreshToken"))
                return true;

            return false;
        }

        private static void TraversePasswordSettings(object settings, Action<PropertyInfo, object> action)
        {
            var settingsNamespace = typeof(PdfCreatorSettings).Namespace;
            
            foreach (var propertyInfo in settings.GetType().GetProperties())
            {
                if (IsPasswordProperty(propertyInfo))
                {
                    action(propertyInfo, settings);
                }

                if (propertyInfo.PropertyType.Namespace.StartsWith(settingsNamespace))
                {
                    var value = propertyInfo.GetValue(settings);
                    TraversePasswordSettings(value, action);
                }

                if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    var list = (IEnumerable)propertyInfo.GetValue(settings);
                    foreach (var obj in list)
                    {
                        TraversePasswordSettings(obj, action);
                    }
                }
            }
        }

        /// <summary>
        /// Replace all passwords that are set with the given value. Empty passwords will not be changed.
        /// </summary>
        /// <param name="settings">The settings object to inspect</param>
        /// <param name="replaceWith">The text to insert in the password field</param>
        public static void ReplacePasswords(object settings, string replaceWith)
        {
            TraversePasswordSettings(settings, (propertyInfo, obj) =>
            {
                var value = propertyInfo.GetValue(obj) as string;
                if (!string.IsNullOrEmpty(value))
                    propertyInfo.SetValue(obj, replaceWith);
            });
        }

        /// <summary>
        /// Count the number of password fields that contain a non-empty value
        /// </summary>
        /// <param name="settings">The settings object to inspect</param>
        /// <returns>The number of password fields that were found to contain a password</returns>
        public static int CountPasswords(object settings)
        {
            var passwordsFound = 0;
            TraversePasswordSettings(settings, (propertyInfo, obj) =>
            {
                if (propertyInfo.GetValue(obj) as string != "")
                    passwordsFound++;
            });
            return passwordsFound;
        }
    }
}