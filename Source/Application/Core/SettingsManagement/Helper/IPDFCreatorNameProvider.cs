using System;
using System.IO;
using System.Linq;
using SystemInterface.IO;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.Core.SettingsManagement.Helper
{
    public interface IPDFCreatorNameProvider
    {
        string GetExeName();

        string GetExePath();

        string GetPortApplicationPath();
    }

    public class PDFCreatorNameProvider : IPDFCreatorNameProvider
    {
        private readonly IAssemblyHelper _assemblyHelper;
        private readonly IDirectory _directory;

        public PDFCreatorNameProvider(IAssemblyHelper assemblyHelper, IDirectory directory)
        {
            _assemblyHelper = assemblyHelper;
            _directory = directory;
        }

        public string GetExePath()
        {
            return _assemblyHelper.GetAssemblyDirectory() + "\\" + GetExeName();
        }

        public string GetPortApplicationPath()
        {
            return GetExePath();
        }

        public string GetExeName()
        {
            var assemblyDirectory = _assemblyHelper.GetAssemblyDirectory();

            // Get files that start with PDFCreator, end with exe and have only one dot (to exclude .vshost.exe and PDFCreator.LicenseService.exe)
            var candidates = _directory.EnumerateFiles(assemblyDirectory, "PDFCreator*.exe")
                .Select(x => new FileInfo(x))
                .Where(file => file.Name.Count(c => c == '.') == 1)
                .ToList();

            if (candidates.Count() != 1)
                throw new ApplicationException("The assembly directory contains more or less than one PDFCreator*.exe");

            return candidates.First().Name;
        }
    }
}
