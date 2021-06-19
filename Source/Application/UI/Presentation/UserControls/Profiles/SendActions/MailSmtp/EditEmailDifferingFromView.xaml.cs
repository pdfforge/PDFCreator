using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.MailSmtp
{
    /// <summary>
    /// Interaction logic for EditEmailSenderView.xaml
    /// </summary>
    public partial class EditEmailDifferingFromView : UserControl
    {
        public EditEmailDifferingFromView(EditEmailDifferingFromViewModel editEmailDifferingFromViewModel)
        {
            DataContext = editEmailDifferingFromViewModel;
            InitializeComponent();
        }
    }
}
