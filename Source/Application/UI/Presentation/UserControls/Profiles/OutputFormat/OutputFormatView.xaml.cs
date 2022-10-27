using pdfforge.PDFCreator.UI.Presentation.Helper;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    /// <summary>
    /// Interaction logic for OutputFormatUserControl.xaml
    /// </summary>
    public partial class OutputFormatView : UserControl
    {
        private readonly IRegionManager _regionManager;
        private static bool RegionViewsInit;

        public OutputFormatView(OutputFormatViewModel viewModel, IRegionManager regionManager)
        {
            DataContext = viewModel;
            TransposerHelper.Register(this, viewModel);
            _regionManager = regionManager;
            InitializeComponent();

            // Prism doesn't want to register the region properly so we do it by hand
            Prism.Regions.RegionManager.SetRegionManager(UserControl, regionManager);

            if (!RegionViewsInit)
            {
                IList<Type> convertViews = new List<Type> { typeof(OutputFormatJpgView), typeof(OutputFormatPngView), typeof(OutputFormatTiffView), typeof(OutputFormatTextView), typeof(OutputFormatPdfView) };
                foreach (var convertView in convertViews)
                {
                    try
                    {
                        regionManager?.RegisterViewWithRegion(RegionNames.OutputFormatOverlayContentRegion, convertView);
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }
                RegionViewsInit = true;
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // remove region from prism to prevent collision
            _regionManager.Regions.Remove(RegionNames.OutputFormatOverlayContentRegion);
        }
    }

    /*
    public class OutputFormatUserControlViewModel : IStatusHintViewModel
    {
        public string StatusText => "";
        public bool HasWarning => false;
        public bool HideStatusInOverlay => false;
    }
    */
}
