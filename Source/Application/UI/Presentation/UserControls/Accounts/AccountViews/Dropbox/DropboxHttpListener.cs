using NLog;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews.Dropbox;
using pdfforge.PDFCreator.Utilities.Process;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews
{
    public interface IDropboxHttpListener
    {
        Task<string> StartAsync(string authUrl, int port);
    }

    public class DropboxHttpListener : IDropboxHttpListener
    {
        private readonly IProcessStarter _processStarter;
        private readonly HttpListener _httpListener;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly DropboxTranslation _translation;

        public DropboxHttpListener(ITranslationFactory translationFactory, IProcessStarter processStarter)
        {
            _processStarter = processStarter;
            _translation = translationFactory.CreateTranslation<DropboxTranslation>();
            _httpListener = new HttpListener();
        }

        public async Task<string> StartAsync(string authUrl, int port)
        {
            try
            {
                _httpListener.Prefixes.Clear();
                _httpListener.Prefixes.Add($"http://{IPAddress.Loopback}:{port}/");
                _httpListener.Start();

                _logger.Debug($"DropboxHttpListener startet listening on port {port}.");

                // TODO The IWebLinkLauncher currently does not work here as it escapes special chars a second time
                _processStarter.Start(authUrl);
                var request = await GetCodeFromRequestAsync();
                return request;
            }
            catch (HttpListenerException)
            {
                throw new DropboxLocalPortBlockedException("HttpListenerException was thrown.");
            }
            catch (DropboxAuthException ex)
            {
                _logger.Error(ex, "A DropboxAuthException was thrown while awaiting DropboxHttpListener request.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occured while awaiting DropboxHttpListener request.");

                return null;
            }
        }

        private string BuildResponsePage(string text)
        {
            var responseHtml = DropboxResources.ResponseHtml;
            if (!responseHtml.Contains("%%TEXT%%"))
                throw new DropboxAuthException("The HTML does not contain a placeholder for the response text!");

            return responseHtml.Replace("%%TEXT%%", WebUtility.HtmlEncode(text));
        }

        private async Task<string> GetCodeFromRequestAsync()
        {
            var context = await _httpListener.GetContextAsync();

            var responseText = context.Request.QueryString.Get("error") != null ? _translation.DropboxYouCanCloseWindow : _translation.DropboxReturnToApp;
            var responseString = BuildResponsePage(responseText);

            var buffer = Encoding.UTF8.GetBytes(responseString);
            context.Response.ContentLength64 = buffer.Length;
            var responseOutput = context.Response.OutputStream;
            await responseOutput.WriteAsync(buffer, 0, buffer.Length);

            responseOutput.Close();
            _httpListener.Stop();

            if (context.Response.StatusCode == (int)HttpStatusCode.OK)
            {
                if (context.Request.QueryString.Get("error") != null)
                    throw new DropboxAccessDeniedException($"{context.Request.QueryString.Get("error_description")}");

                return context.Request.QueryString.Get("code");
            }
            else
                throw new DropboxAuthException($"An error occured while retrieving code from Dropbox.");
        }
    }
}
