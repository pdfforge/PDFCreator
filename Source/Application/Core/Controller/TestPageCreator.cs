using pdfforge.PDFCreator.Core.Controller.TestPage;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Diagnostics;
using System.Text;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.Controller
{
    public interface ITestPageCreator
    {
        string CreateTestPage(string tempFolderPath);
    }

    public class TestPageCreator : ITestPageCreator
    {
        private readonly ApplicationNameProvider _applicationNameProvider;
        private readonly IVersionHelper _versionHelper;
        private readonly IOsHelper _osHelper;
        private readonly IFile _file;

        public TestPageCreator(ApplicationNameProvider applicationNameProvider, IVersionHelper versionHelper, IOsHelper osHelper, IFile file)
        {
            _applicationNameProvider = applicationNameProvider;
            _versionHelper = versionHelper;
            _osHelper = osHelper;
            _file = file;
        }

        public string GetTestFileContent()
        {
            var sb = new StringBuilder(Testpage.TestPage);
            sb.Replace("[INFOEDITION]", _applicationNameProvider.EditionName.ToUpper());
            sb.Replace("[CURRENTYEAR]", DateTime.Now.Year.ToString());
            sb.Replace("[INFOTITLE]", "PDFCreator " + _versionHelper.ApplicationVersion);
            sb.Replace("[INFODATE]", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString());
            sb.Replace("[INFOAUTHORS]", "Avanquest pdfforge");
            sb.Replace("[INFOHOMEPAGE]", Urls.PdfforgeWebsiteUrl);
            sb.Replace("[INFOPDFCREATOR]", "PDFCreator " + _versionHelper.ApplicationVersion);

            sb.Replace("[INFOCOMPUTER]", Environment.MachineName);
            sb.Replace("[INFOWINDOWS]", _osHelper.GetWindowsVersion());
            sb.Replace("[INFO64BIT]", _osHelper.Is64BitOperatingSystem.ToString());
            sb.Replace("[USERTOKEN]", "[[[TestUserToken:User Token Value]]]");

            return sb.ToString();
        }

        public static string GetInfFileContent(string psFilePath)
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

        public string CreateTestPage(string tempFolderPath)
        {
            var psFilePath = WritePsFile(tempFolderPath);
            var infFilePath = WriteInfFile(tempFolderPath, psFilePath);
            return infFilePath;
        }

        private string WritePsFile(string tempFolderPath)
        {
            var psFileContent = GetTestFileContent();
            var psFilePath = PathSafe.Combine(tempFolderPath, "testPage.ps");
            _file.WriteAllText(psFilePath, psFileContent);
            return psFilePath;
        }

        private string WriteInfFile(string tempFolderPath, string psFilePath)
        {
            var infFileContent = GetInfFileContent(psFilePath);
            var infFilePath = PathSafe.Combine(tempFolderPath, "testPage.inf");
            _file.WriteAllText(infFilePath, infFileContent, Encoding.Unicode);
            return infFilePath;
        }
    }
}
