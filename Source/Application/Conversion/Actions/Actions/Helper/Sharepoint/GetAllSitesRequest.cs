using System.Collections.Generic;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions.Helper.Sharepoint;

public class GetAllSitesRequest
{
    public static string RequestURL()
    {
        return "https://graph.microsoft.com/v1.0/sites?search=*";
    }

    public List<SharepointSite> value { get; set; }
}
