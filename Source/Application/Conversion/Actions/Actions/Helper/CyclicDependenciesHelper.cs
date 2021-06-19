using pdfforge.PDFCreator.Conversion.Settings;
using System.Collections.Generic;
using System.Linq;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions.Helper
{
    public static class CyclicDependenciesHelper
    {
        public static (bool, IList<string>) HasCyclicDependency(ConversionProfile profile, IList<PrinterMapping> printerMappings, IList<ConversionProfile> allProfiles, IList<string> referencedProfiles = null)
        {
            referencedProfiles = referencedProfiles ?? new List<string>();

            if (referencedProfiles.Contains(profile.Guid))
            {
                return (true, referencedProfiles);
            }

            referencedProfiles.Add(profile.Guid);

            if (profile.ForwardToFurtherProfile.Enabled)
            {
                var forwardProfile = SettingsHelper.GetProfileByGuid(allProfiles, profile.ForwardToFurtherProfile.ProfileGuid);

                if (forwardProfile == null)
                {
                    // There is no circular dependency. The missing profile will be displayed in the check for the forwarded profile.
                    return (false, referencedProfiles);
                }

                return HasCyclicDependency(forwardProfile, printerMappings, allProfiles, referencedProfiles);
            }

            if (profile.Printing.Enabled)
            {
                var printerMapping = printerMappings.FirstOrDefault(mapping => mapping.PrinterName == profile.Printing.PrinterName);
                if (printerMapping?.ProfileGuid != null)
                {
                    var printedProfile = SettingsHelper.GetProfileByGuid(allProfiles, printerMapping.ProfileGuid);
                    if (printedProfile != null)
                        return HasCyclicDependency(printedProfile, printerMappings, allProfiles, referencedProfiles);
                }
            }

            return (false, referencedProfiles);
        }
    }
}
