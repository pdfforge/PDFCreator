using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Signature
{
    /// <summary>
    /// Interaction logic for SignaturePasswordOverlayView.xaml
    /// </summary>
    public partial class SignaturePasswordOverlayView : UserControl
    {
        public SignaturePasswordOverlayView(SignaturePasswordOverlayViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
