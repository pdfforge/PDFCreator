using System.Collections.Generic;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions.Dropbox
{
    public class DropboxAppData
    {
        public readonly string AppKey;

        public List<int> PortsList { get; } = new List<int>
        {
           38330, 13193, 25262, 5163, 8849
        };

        public DropboxAppData(string appKey)
        {
            AppKey = appKey;
        }
    }
}
