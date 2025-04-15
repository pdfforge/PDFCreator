using System.Text.RegularExpressions;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.EmailCollection;

public static class EmailValidator
{
    private static readonly Regex EmailRegex = new(@"^(?![-.])(?!.*[-.]{2})[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    public static bool IsValidEmail(string email)
    {
        return !string.IsNullOrWhiteSpace(email) && EmailRegex.IsMatch(email);
    }
}
