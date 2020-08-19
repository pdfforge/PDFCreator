using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Misc
{
    public partial class WarningIconControl : UserControl
    {
        public static readonly DependencyProperty ToolTipTextDependencyProperty =
            DependencyProperty.Register("ToolTipText", typeof(object), typeof(WarningIconControl), new PropertyMetadata(""));

        public static readonly DependencyProperty ApplicationCommandDependencyProperty =
            DependencyProperty.Register("ApplicationCommand", typeof(object), typeof(WarningIconControl), new PropertyMetadata("ApplicationCommands.Help"));

        public WarningIconControl()
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
