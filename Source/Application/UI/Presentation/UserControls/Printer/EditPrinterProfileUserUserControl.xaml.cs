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
using pdfforge.DataStorage;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Printer
{
    /// <summary>
    /// Interaction logic for EditPrinterProfileUserUserControl.xaml
    /// </summary>
    public partial class EditPrinterProfileUserUserControl : UserControl
    {
        public EditPrinterProfileUserUserControl(EditPrinterProfileViewModel dataContext)
        {
            DataContext = dataContext;
            InitializeComponent();
        }
    }
}
