using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Utilities.Web;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class UrlOpenCommand : IInitializedCommand<string>
    {
        private readonly IWebLinkLauncher _webLinkLauncher;
        protected string Url { get; set; }

        public UrlOpenCommand(IWebLinkLauncher webLinkLauncher)
        {
            _webLinkLauncher = webLinkLauncher;
        }

        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var url = parameter as string ?? Url;

            try
            {
                _webLinkLauncher.Launch(url);
            }
            catch
            {
                // ignored
            }
        }

        public void Init(string parameter)
        {
            Url = parameter;
        }

#pragma warning disable CS0067

        public event EventHandler CanExecuteChanged;

#pragma warning restore CS0067
    }
}
