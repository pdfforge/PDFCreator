using System;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Core.Services.Update
{
    public interface IApplicationVersion
    {
        Version Version { get; }
        string DownloadUrl { get; }
        string FileHash { get; }
        List<Release> VersionInfos { get; }
    }

    public class ApplicationVersion : IApplicationVersion
    {
        public ApplicationVersion(Version version, string downloadUrl, string fileHash, List<Release> versionInfos)
        {
            Version = version;
            DownloadUrl = downloadUrl;
            FileHash = fileHash;
            VersionInfos = versionInfos ?? new List<Release>();
        }

        public Version Version { get; }
        public string DownloadUrl { get; }
        public string FileHash { get; }
        public List<Release> VersionInfos { get; }
    }
}
