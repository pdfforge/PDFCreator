using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Misc
{
    public partial class InfoIconControl : UserControl
    {
        public static readonly DependencyProperty ToolTipTextDependencyProperty =
            DependencyProperty.Register("ToolTipText", typeof(object), typeof(InfoIconControl), new PropertyMetadata(""));

        public static readonly DependencyProperty ApplicationCommandDependencyProperty =
            DependencyProperty.Register("ApplicationCommand", typeof(object), typeof(InfoIconControl), new PropertyMetadata("ApplicationCommands.Help"));

        public InfoIconControl()
        {
            InitializeComponent();
        }

        public object ToolTipText
        {
            get { return (string)GetValue(ToolTipTextDependencyProperty); }
            set { SetValue(ToolTipTextDependencyProperty, value); }
        }

        public object ApplicationCommand
        {
            get { return GetValue(ApplicationCommandDependencyProperty); }
            set { SetValue(ApplicationCommandDependencyProperty, value); }
        }
    }
}
