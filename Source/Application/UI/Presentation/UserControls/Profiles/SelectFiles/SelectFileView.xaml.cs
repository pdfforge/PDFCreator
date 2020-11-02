using Prism.Regions;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SelectFiles
{
    /// <summary>
    /// Interaction logic for EditMailAttachment.xaml
    /// </summary>
    public partial class SelectFileView : UserControl, IRegionMemberLifetime
    {
        public SelectFileView(SelectFileViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }

        public bool KeepAlive { get; } = true;
    }
}
