using System;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings
{
   
    public partial class AddActionUserControl : UserControl
    {
        public AddActionUserControl(AddActionViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
