using System.Collections.Generic;
using System.Linq;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Utilities
{
    public interface ISplitDocumentFilePathHelper
    {
        string GetSplitDocumentFilePath(string originalFilePath);
    }

    public class SplitDocumentFilePathHelper : ISplitDocumentFilePathHelper
    {
        public string GetSplitDocumentFilePath(string originalFilePath)
        {
            var fileName = PathSafe.GetFileNameWithoutExtension(originalFilePath);
            var directory = PathSafe.GetDirectoryName(originalFilePath);
            var extension = PathSafe.GetExtension(originalFilePath);
            var fileNameSplit = fileName.Split('_').ToList();
            var number = 2;
            var removeLastElement = false;
            if (int.TryParse(fileNameSplit.Last(), out var parseNumber))
            {
                // Handle numbers lower than 2 as string
                // Numbers lower than 2 are always defined by the user
                if (parseNumber > 1)
                {
                    number = ++parseNumber;
                    removeLastElement = true;
                }
            }

            fileName = RejoinFileNameSplit(fileNameSplit, removeLastElement) + "_" + number + extension;

            return PathSafe.Combine(directory, fileName);
        }

        private static string RejoinFileNameSplit(List<string> fileNameSplit, bool removeLastElement)
        {
            if (fileNameSplit.Count == 1)
                return fileNameSplit.First();

            if (removeLastElement)
                fileNameSplit = fileNameSplit.GetRange(0, fileNameSplit.Count - 1);

            var joinedString = string.Join("_", fileNameSplit);

            return joinedString;
        }
    }
}
