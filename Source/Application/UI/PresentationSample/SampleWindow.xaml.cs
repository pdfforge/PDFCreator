using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using System.Collections.ObjectModel;
using System.Windows;

namespace PresentationSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<ConversionProfileWrapper> Profiles => new ObservableCollection<ConversionProfileWrapper> {
                new ConversionProfileWrapper(new ConversionProfile { Name = "Profile One" }),
                new ConversionProfileWrapper(new ConversionProfile { Name = "Profile Two" }),
                new ConversionProfileWrapper(new ConversionProfile { Name = "Profile Three" })
            };


        public MainWindow()
        {
            // dummy reference to force GongSolutions.Wpf.DragDrop to be copied to bin folder
            var t = typeof(GongSolutions.Wpf.DragDrop.DragDrop);
            // same for MahApps IconPacks
            var t2 = typeof(MahApps.Metro.IconPacks.MaterialDesignExtension);

            InitializeComponent();
        }
    }
}
