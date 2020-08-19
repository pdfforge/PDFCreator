using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.Utilities.Web
{
    public interface IWebLinkLauncher
    {
        /// <summary>
        /// Launch an URL in the main web browser
        /// </summary>
        /// <param name="url">The URL that will be opened</param>
        void Launch(string url);
    }

    public class WebLinkLauncher : IWebLinkLauncher
    {
        private readonly IProcessStarter _processStarter;
        private readonly TrackingParameters _trackingParameters;

        public WebLinkLauncher(TrackingParameters trackingParameters, IProcessStarter processStarter)
        {
            _processStarter = processStarter;
            _trackingParameters = trackingParameters;
        }

        public void Launch(string url)
        {
            try
            {
                var urlWithParams = _trackingParameters.CleanUpParamsAndAppendToUrl(url);
                _processStarter.Start(urlWithParams);
            }
            catch
            {
                // ignore exceptions
            }
        }
    }
}
