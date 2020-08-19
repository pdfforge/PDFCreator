using pdfforge.PDFCreator.Utilities.IO;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Jobs.JobInfo
{
    public interface ISourceFileInfoDuplicator
    {
        SourceFileInfo Duplicate(SourceFileInfo sfi, string duplicateFolder, string profileGuid);
    }

    public class SourceFileInfoDuplicator : ISourceFileInfoDuplicator
    {
        private readonly IUniqueFilePathBuilder _uniqueFilePathBuilder;
        private readonly IFile _file;

        public SourceFileInfoDuplicator(IUniqueFilePathBuilder uniqueFilePathBuilder, IFile file)
        {
            _uniqueFilePathBuilder = uniqueFilePathBuilder;
            _file = file;
        }

        public SourceFileInfo Duplicate(SourceFileInfo sfi, string duplicateFolder, string profileGuid)
        {
            var duplicate = new SourceFileInfo();

            duplicate.SessionId = sfi.SessionId;
            duplicate.WinStation = sfi.WinStation;
            duplicate.Author = sfi.Author;
            duplicate.ClientComputer = sfi.ClientComputer;
            duplicate.PrinterName = sfi.PrinterName;
            duplicate.JobCounter = sfi.JobCounter;
            duplicate.JobId = sfi.JobId;
            duplicate.DocumentTitle = sfi.DocumentTitle;
            duplicate.OriginalFilePath = sfi.OriginalFilePath;
            duplicate.Type = sfi.Type;
            duplicate.TotalPages = sfi.TotalPages;
            duplicate.Copies = sfi.Copies;
            duplicate.UserTokenEvaluated = sfi.UserTokenEvaluated;
            duplicate.UserToken = sfi.UserToken;
            duplicate.OutputFileParameter = sfi.OutputFileParameter;

            duplicate.PrinterParameter = profileGuid == null ? sfi.PrinterParameter : "";
            duplicate.ProfileParameter = profileGuid ?? sfi.ProfileParameter;

            var duplicateFilename = PathSafe.GetFileNameWithoutExtension(sfi.Filename);
            var duplicateFilePath = PathSafe.Combine(duplicateFolder, duplicateFilename);
            duplicateFilePath = _uniqueFilePathBuilder.Build(duplicateFilePath).Unique;
            _file.Copy(sfi.Filename, duplicateFilePath);
            duplicate.Filename = duplicateFilePath;

            return duplicate;
        }
    }
}
