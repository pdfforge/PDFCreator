using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.Core.DirectConversion
{
    public class AppStartParameters
    {
        //INITIALIZE WITH EMPTY STRINGS

        public string Printer { get; set; } = "";

        public string OutputFile { get; set; } = "";

        public string Profile { get; set; } = "";

        public bool ManagePrintJobs { get; set; }

        public bool Merge { get; set; }

        public bool Silent { get; set; }
        public PageSize PageSize { get; set; } = PageSize.Automatic;
        public PageOrientation PageOrientation { get; set; } = PageOrientation.Automatic;
    }
}
