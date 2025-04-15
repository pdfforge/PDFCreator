using System.Collections.Generic;
using System.Security.Policy;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions.Helper.Sharepoint;

public class GetAllDrivesForSiteRequest
{
    public static string RequestURL(SharepointSite site)
    {
        return $"https://graph.microsoft.com/v1.0/sites/{site.Id}/drives?search=*";
    }

    public List<SharepointDrive> value { get; set; }
}
