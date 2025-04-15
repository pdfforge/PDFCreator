using System;
using System.Collections.Generic;
using pdfforge.PDFCreator.Utilities.Update;

namespace pdfforge.PDFCreator.Core.Services.Update
{
    public interface IApplicationVersion
    {
        Version Version { get; }
        string DownloadUrl { get; }
        string FileHash { get; }
        List<ReleaseInfo> ReleaseInfos { get; }
    }

    public class ApplicationVersion : IApplicationVersion
    {
        public ApplicationVersion(Version version, string downloadUrl, string fileHash, List<ReleaseInfo> versionInfos)
        {
            Version = version;
            DownloadUrl = downloadUrl;
            FileHash = fileHash;
            ReleaseInfos = versionInfos ?? new List<ReleaseInfo>();
        }

        public Version Version { get; }
        public string DownloadUrl { get; }
        public string FileHash { get; }
        public List<ReleaseInfo> ReleaseInfos { get; }
    }
}
