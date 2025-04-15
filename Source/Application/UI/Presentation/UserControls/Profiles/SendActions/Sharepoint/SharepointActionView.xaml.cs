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
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Helper;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.Sharepoint
{
    /// <summary>
    /// Interaction logic for SharepointActionView.xaml
    /// </summary>
    public partial class SharepointActionView : IActionView
    {
        private readonly IActionViewModel _vm;

        public SharepointActionView(SharepointActionViewModel viewModel)
        {
            _vm = viewModel;
            DataContext = viewModel;
            //TransposerHelper.Register(this, _vm);
            InitializeComponent();
        }

        public IActionViewModel ViewModel => _vm;

    }
}
