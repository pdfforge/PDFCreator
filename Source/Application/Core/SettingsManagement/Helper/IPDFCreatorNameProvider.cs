using pdfforge.PDFCreator.Utilities;
using System;
using System.IO;
using System.Linq;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.SettingsManagement.Helper
{
    public interface IPDFCreatorNameProvider
    {
        string GetExeName();

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

        private string GetApplicationPath()
        {
            return _assemblyHelper.GetAssemblyDirectory();
        }

        public string GetPortApplicationPath()
        {
            var candidates = _directory.EnumerateFiles(GetApplicationPath(), "PDFCreator-cli.exe").ToList();

            if (candidates.Count == 1)
                return candidates.Single();

            return GetApplicationPath() + "\\" + GetExeName(); ;
        }

        public string GetExeName()
        {
            // Get files that start with PDFCreator, end with exe and have only one dot (to exclude .vshost.exe and PDFCreator.LicenseService.exe)
            var candidates = _directory.EnumerateFiles(GetApplicationPath(), "PDFCreator*.exe")
                .Select(x => new FileInfo(x))
                .Where(file => file.Extension == ".exe")
                .Where(file => file.Name.Count(c => c == '.') == 1)
                .Where(file => !file.Name.Contains("-cli"))
                .ToList();

            if (candidates.Count != 1)
                throw new ApplicationException("The assembly directory contains more or less than one PDFCreator*.exe");

            return candidates.First().Name;
        }
    }
}
