namespace pdfforge.PDFCreator.Core.Services.Update
{
    public class UpdateInformationProvider
    {
        public UpdateInformationProvider(string updateInfoUrl, string sectionName, string changeLogUrl)
        {
            ChangeLogUrl = changeLogUrl;
            UpdateInfoUrl = updateInfoUrl;
            SectionName = sectionName;
        }

        public string UpdateInfoUrl { get; }
        public string SectionName { get; }
        public string ChangeLogUrl { get; }
    }
}
