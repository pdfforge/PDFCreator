using System;
using System.Linq;

namespace pdfforge.SetupHelper.dotnet
{
    public class DotNetVersion
    {
        public int Major { get; private set; }
        public int Minor { get; private set; }
        public int Build { get; private set; }
        public int Revision { get; private set; }

        public DotNetVersion(string dotNetVersion)
        {
            SetVersionFromString(dotNetVersion);
        }

        public DotNetVersion(int major = 0, int minor = 0, int build = 0, int revision = 0)
        {
            Major = major;
            Minor = minor;
            Build = build;
            Revision = revision;
        }

        public string Name
        {
            get
            {
                var name = $"{Major}.{Minor}.{Build}.{Revision}";

                return name;
            }
        }

        private void SetVersionFromString(string version)
        {
            if (string.IsNullOrWhiteSpace(version)) return;
            var versionArray = version.Split(new[] { '.' }, 4);
            versionArray = versionArray.Select(s =>
            {
                var indexOfSeparator = s.IndexOf("-", StringComparison.Ordinal);
                return indexOfSeparator > 0 ? s[..indexOfSeparator] : s;

            }).ToArray();
            Major = int.Parse(versionArray[0]);
            if (versionArray.Length >= 2)
                Minor = int.Parse(versionArray[1]);
            if (versionArray.Length >= 3)
                Build = int.Parse(versionArray[2]);
            if (versionArray.Length == 4)
                Revision = int.Parse(versionArray[3]);
        }

    }
}