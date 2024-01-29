using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.Shared;
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

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.Shared
{
    /// <summary>
    /// Interaction logic for SaveButtonControl.xaml
    /// </summary>
    public partial class SaveButtonControl : UserControl
    {
        public SaveButtonControl(SettingControlsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}