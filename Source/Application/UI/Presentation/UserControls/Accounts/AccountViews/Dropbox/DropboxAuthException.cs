using System;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews
{
    public class DropboxAuthException : Exception
    {
        public DropboxAuthException(string message) : base(message)
        {
        }
    }

    public class DropboxAccessDeniedException : DropboxAuthException
    {
        public DropboxAccessDeniedException(string message) : base(message)
        {
        }
    }

    public class DropboxLocalPortBlockedException : DropboxAuthException
    {
        public DropboxLocalPortBlockedException(string message) : base(message)
        {
        }
    }
}
