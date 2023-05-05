using pdfforge.PDFCreator.Utilities.IO;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Jobs.JobInfo
{
    public interface ISourceFileInfoDuplicator
    {
        SourceFileInfo Duplicate(SourceFileInfo sfi, string duplicateFolder, string profileGuid);

        SourceFileInfo Move(SourceFileInfo sfi, string moveFolder, string profileGuid);

        SourceFileInfo DuplicateProperties(SourceFileInfo oldSfi, string profileGuid);
    }

    public class SourceFileInfoDuplicator : ISourceFileInfoDuplicator
    {
        private readonly IUniqueFilenameFactory _uniqueFilenameFactory;
        private readonly IFile _file;

        public SourceFileInfoDuplicator(IUniqueFilenameFactory uniqueFilenameFactory, IFile file)
        {
            _uniqueFilenameFactory = uniqueFilenameFactory;
            _file = file;
        }

        public SourceFileInfo Duplicate(SourceFileInfo sfi, string duplicateFolder, string profileGuid)
        {
            var newSfi = DuplicateProperties(sfi, profileGuid);
            var duplicateFilePath = GetDestinationPath(sfi.Filename, duplicateFolder);

            _file.Copy(sfi.Filename, duplicateFilePath);

            newSfi.Filename = duplicateFilePath;
            return newSfi;
        }

        public SourceFileInfo Move(SourceFileInfo sfi, string moveFolder, string profileGuid)
        {
            var newSfi = DuplicateProperties(sfi, profileGuid);
            var movedFilePath = GetDestinationPath(sfi.Filename, moveFolder);

            _file.Move(sfi.Filename, movedFilePath);

            newSfi.Filename = movedFilePath;
            return newSfi;
        }

        public SourceFileInfo DuplicateProperties(SourceFileInfo oldSfi, string profileGuid)
        {
            var newSfi = new SourceFileInfo
            {
                Filename = oldSfi.Filename,
                SessionId = oldSfi.SessionId,
                WinStation = oldSfi.WinStation,
                Author = oldSfi.Author,
                ClientComputer = oldSfi.ClientComputer,
                PrinterName = oldSfi.PrinterName,
                JobCounter = oldSfi.JobCounter,
                JobId = oldSfi.JobId,
                DocumentTitle = oldSfi.DocumentTitle,
                OriginalFilePath = oldSfi.OriginalFilePath,
                PrintedAt = oldSfi.PrintedAt,
                Type = oldSfi.Type,
                TotalPages = oldSfi.TotalPages,
                Copies = oldSfi.Copies,
                UserTokenEvaluated = oldSfi.UserTokenEvaluated,
                UserToken = oldSfi.UserToken,
                PrinterParameter = profileGuid == null
                    ? oldSfi.PrinterParameter
                    : "",
                ProfileParameter = profileGuid ?? oldSfi.ProfileParameter,
                OutputFileParameter = oldSfi.OutputFileParameter
            };

            return newSfi;
        }

        private string GetDestinationPath(string filename, string destinationFolder)
        {
            var destinationFilename = PathSafe.GetFileNameWithoutExtension(filename);
            var extension = PathSafe.GetExtension(filename);
            var destinationPath = PathSafe.Combine(destinationFolder, destinationFilename) + extension;
            return _uniqueFilenameFactory.Build(destinationPath).CreateUniqueFileName();
        }
    }
}
