using pdfforge.PDFCreator.Core.ServiceLocator;
using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Misc
{
    public partial class TokenHintPanel : UserControl
    {
        public static readonly DependencyProperty TextWithTokenProperty = DependencyProperty.Register(
            "TextWithToken", typeof(string), typeof(TokenHintPanel), new PropertyMetadata("", TextWithTokenChanged));

        private static void TextWithTokenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = d as TokenHintPanel;
            panel?.RaiseTextChanged();
        }

        public string TextWithToken
        {
            get { return GetValue(TextWithTokenProperty) as string; }
            set { SetValue(TextWithTokenProperty, value); }
        }

        private void RaiseTextChanged()
        {
            var tokenHintPanelViewModel = DataContext as TokenHintPanelViewModel;
            tokenHintPanelViewModel?.OnTextChanged(GetValue(TextWithTokenProperty).ToString());
        }

        public TokenHintPanel()
        {
            // Initialize TokenHelper with empty Translator to avoid exception in converter
            if (RestrictedServiceLocator.IsLocationProviderSet)
            {
                DataContext = RestrictedServiceLocator.Current.GetInstance<TokenHintPanelViewModel>();
            }
            InitializeComponent();
        }
    }
}
