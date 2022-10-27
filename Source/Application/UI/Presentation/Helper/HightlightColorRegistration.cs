using pdfforge.PDFCreator.Core.ServiceLocator;
using System.Windows;
using System.Windows.Media;

namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    public class HighlightColorRegistration : IWhitelisted
    {
        private readonly Color _highlightColor;

        public const string HighlightColorName = "ApplicationHightlightColor";

        public HighlightColorRegistration(Color highlightColor)
        {
            _highlightColor = highlightColor;
        }

        public void RegisterHighlightColorResource(FrameworkElement frameworkElement)
        {
            frameworkElement.Resources[HighlightColorName] = new SolidColorBrush(_highlightColor);
        }
    }
}
