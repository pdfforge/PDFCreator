using JamesWright.SimpleHttp;
using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Net.Sockets;
using System.Threading;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.Jobs.Actions
{
    [TestFixture]
    public class HttpActionTest
    {
        private int _port;

        [SetUp]
        public void SetUp()
        {
            _account = new HttpAccount();

            _account.Url = GetUrl();
            _account.IsBasicAuthentication = false;
            _account.AccountId = "1";

            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder("HttpRequestTest");

            _th.GenerateGsJob_WithSetOutput(TestFile.PDFCreatorTestpage_GS9_19_PDF);

            _th.Job.Accounts.HttpAccounts.Add(_account);

            _th.Job.Profile.HttpSettings.Enabled = true;
            _th.Job.Profile.HttpSettings.AccountId = _account.AccountId;

            _httpAction = new HttpAction();
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
        }

        private TestHelper _th;
        private HttpAction _httpAction;
        private HttpAccount _account;
        private Thread _serverThread;

        [OneTimeSetUpAttribute]
        public void InitFixture()
        {
            _port = FreeTcpPort();
            _serverThread = new Thread(ServerStart);
            _serverThread.Start();
        }

        [OneTimeTearDown]
        public void DestroyFixture()
        {
            _serverThread.Abort();
            _serverThread.Join();
        }

        [TestCase("fileOutputTestJpeg/", TestFile.PDFCreatorTestpageJPG)]
        [TestCase("fileOutputTestTIF/", TestFile.PDFCreatorTestpageTIF)]
        [TestCase("fileOutputTestPNG/", TestFile.PDFCreatorTestpagePNG)]
        [TestCase("fileOutputTestTXT/", TestFile.PDFCreatorTestpageTXT)]
        [TestCase("fileOutputTestPDF/", TestFile.PDFCreatorTestpage_GS9_19_PDF)]
        public void SetupDifferentOutputType_SendToHttp_ActionResultSucessTrue(string address, TestFile file)
        {
            _account.Url = GetUrl() + address;
            _th.GenerateGsJob_WithSetOutput(file, "file1");
            _th.AddOutputFileToJob(file, "file2");
            _th.AddOutputFileToJob(file, "file3");
            _th.Job.Accounts.HttpAccounts.Add(_account);

            var actionResult = _httpAction.ProcessJob(_th.Job);

            Assert.IsTrue(actionResult);
        }

        [Test]
        public void HaveAuthenication_SendRequestToUrlWithAuthenication_ActionSuccessTrue()
        {
            _account.Url = GetUrl() + "authTest1/";
            _th.Job.Passwords.HttpPassword = "swordfish";
            _account.UserName = "admin";
            _account.IsBasicAuthentication = true;

            var actionResult = _httpAction.ProcessJob(_th.Job);
            Assert.IsTrue(actionResult.IsSuccess);
        }

        [Test]
        public void HaveNoAuthenication_SendRequestToUrlWithAuthenication_ActionReturnsLoginError()
        {
            _account.Url = GetUrl() + "authTest1/";
            var actionResult = _httpAction.ProcessJob(_th.Job);
            Assert.IsTrue(actionResult.Contains(ErrorCode.PasswordAction_Login_Error));
        }

        [Test]
        public void JobReady_RequestFileUpload_FilesAreSendSuccessfully()
        {
            _account.Url = GetUrl();
            var actionResult = _httpAction.ProcessJob(_th.Job);
            Assert.IsTrue(actionResult);
        }

        [Test]
        public void JobReady_RequestFileUploadWithAuthenication_FilesAreSendSuccessfully()
        {
            _th.Job.Passwords.HttpPassword = "potato";
            _account.IsBasicAuthentication = true;
            _account.UserName = "max";
            _account.Url = GetUrl();
            var actionResult = _httpAction.ProcessJob(_th.Job);
            Assert.IsTrue(actionResult);
        }

        private string GetUrl()
        {
            return $"http://localhost:{_port}/";
        }

        #region Server Code

        private void ServerStart()
        {
            var app = new HttpItegrationServer();

            app.Post("/", async (req, res) =>
            {
                res.Content = "";
                res.ContentType = "plain/text";
                await res.SendAsync();
            });

            app.Post("/authTest1/", ProcessAuthentication);
            app.Post("/fileOutputTestJpeg/", (req, res) => ProcessOutputTest(req, res, MediaTypeNames.Image.Jpeg));
            app.Post("/fileOutputTestPDF/", (req, res) => ProcessOutputTest(req, res, MediaTypeNames.Application.Pdf));
            app.Post("/fileOutputTestPNG/", (req, res) => ProcessOutputTest(req, res, "image/png"));
            app.Post("/fileOutputTestTIF/", (req, res) => ProcessOutputTest(req, res, MediaTypeNames.Image.Tiff));
            app.Post("/fileOutputTestTXT/", (req, res) => ProcessOutputTest(req, res, MediaTypeNames.Text.Plain));

            app.Start(_port.ToString());
        }

        private async void ProcessOutputTest(Request req, Response res, string mimeType)
        {
            if (!ProcessOutputTestFiles(req, mimeType))
            {
                res.httpListenerResponse.StatusCode = (int)HttpStatusCode.Conflict;
                res.httpListenerResponse.Close();
            }
            else
            {
                res.Content = "great success";
                res.ContentType = MediaTypeNames.Text.Plain;
                await res.SendAsync();
            }
        }

        private bool ProcessOutputTestFiles(Request req, string mimeType)
        {
            var streamContent = new StreamContent(req.HttpRequest.InputStream, (int)req.HttpRequest.ContentLength64);
            streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(req.HttpRequest.ContentType);

            var provider = streamContent.ReadAsMultipartAsync().Result;

            foreach (var httpContent in provider.Contents)
            {
                if (httpContent.Headers.ContentType.MediaType != mimeType)
                {
                    return false;
                }
            }
            return true;
        }

        private async void ProcessAuthentication(Request req, Response res)
        {
            if (!req.HttpRequest.Headers.AllKeys.Contains(HttpRequestHeader.Authorization.ToString()))
            {
                res.httpListenerResponse.StatusCode = (int)HttpStatusCode.Unauthorized;
                res.httpListenerResponse.Close();
            }
            else
            {
                res.Content = req.HttpRequest.Headers.AllKeys.ToString();
                res.ContentType = MediaTypeNames.Text.Plain;
                await res.SendAsync();
            }
        }

        private int FreeTcpPort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

        #endregion Server Code
    }
}
