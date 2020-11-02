using System;
using System.Linq;

namespace pdfforge.PDFCreator.Conversion.Settings
{
    public partial class  CoverPage : IProfileSetting
    {
        [Obsolete("'File' has been replaced with 'Files'.")]
        public string File
        {
            get => Files.FirstOrDefault() ?? "";
            set
            {
                Files.Clear();
                Files.Add(value); }
        }
    }
}

