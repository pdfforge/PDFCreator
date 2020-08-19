using System;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Utilities
{
    public interface IProgramDataDirectoryHelper
    {
        string GetDir();
    }

    public class ProgramDataDirectoryHelper : IProgramDataDirectoryHelper
    {
        private readonly string _applicationName;

        public ProgramDataDirectoryHelper(string applicationName)
        {
            _applicationName = applicationName;
        }

        public string GetDir()
        {
            var pdfforgeProgramData = @"%ProgramData%\pdfforge\";
            var applicationProgramData = PathSafe.Combine(pdfforgeProgramData, _applicationName);

            return Environment.ExpandEnvironmentVariables(applicationProgramData);
        }
    }
}
