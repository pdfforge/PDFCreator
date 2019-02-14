using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.Core.Controller
{
    public class TestPageCreator
    {
        private readonly ApplicationNameProvider _applicationNameProvider;
        private readonly IVersionHelper _versionHelper;
        private readonly IOsHelper _osHelper;

        public TestPageCreator(ApplicationNameProvider applicationNameProvider, IVersionHelper versionHelper, IOsHelper osHelper)
        {
            _applicationNameProvider = applicationNameProvider;
            _versionHelper = versionHelper;
            _osHelper = osHelper;
        }

        public string GetTestFileContent()
        {
            var sb = new StringBuilder(Testpage.TestPage);
            sb.Replace("[INFOEDITION]", _applicationNameProvider.EditionName.ToUpper());
            sb.Replace("[INFOTITLE]", "PDFCreator " + _versionHelper.ApplicationVersion);
            sb.Replace("[INFODATE]", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString());
            sb.Replace("[INFOAUTHORS]", "pdfforge");
            sb.Replace("[INFOHOMEPAGE]", Urls.PdfforgeWebsiteUrl);
            sb.Replace("[INFOPDFCREATOR]", "PDFCreator " + _versionHelper.ApplicationVersion);

            sb.Replace("[INFOCOMPUTER]", Environment.MachineName);
            sb.Replace("[INFOWINDOWS]", _osHelper.GetWindowsVersion());
            sb.Replace("[INFO64BIT]", _osHelper.Is64BitOperatingSystem.ToString());

            return sb.ToString();
        }

        public string GetInfFileContent(string psFilePath)
        {
            var sb = new StringBuilder();

            sb.AppendLine("[0]");
            sb.AppendLine("SessionId=" + Process.GetCurrentProcess().SessionId);
            sb.AppendLine("WinStation=Console");
            sb.AppendLine("UserName=" + Environment.UserName);
            sb.AppendLine("ClientComputer=" + Environment.MachineName);
            sb.AppendLine("SpoolFileName=" + psFilePath);
            sb.AppendLine("PrinterName=PDFCreator");
            sb.AppendLine("JobId=1");
            sb.AppendLine("TotalPages=1");
            sb.AppendLine("Copies=1");
            sb.AppendLine("DocumentTitle=PDFCreator Testpage");
            sb.AppendLine("");

            return sb.ToString();
        }
    }
}