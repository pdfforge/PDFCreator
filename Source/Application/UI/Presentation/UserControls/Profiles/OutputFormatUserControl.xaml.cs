using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs.ConvertTabs;
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
    public partial class OutputFormatUserControl : UserControl
    {
        private readonly IRegionManager _regionManager;
        private static bool RegionViewsInit;

        public OutputFormatUserControl(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            InitializeComponent();

            // Prism doesn't want to register the region properly so we do it by hand
            Prism.Regions.RegionManager.SetRegionManager(UserControl, regionManager);

            if (!RegionViewsInit)
            {
                IList<Type> convertViews = new List<Type> { typeof(OutputFormatTab), typeof(ConvertJpgView), typeof(ConvertPngView), typeof(ConvertTiffView), typeof(ConvertTextView), typeof(ConvertPdfView) };
                foreach (var convertView in convertViews)
                {
                    try
                    {
                        regionManager?.RegisterViewWithRegion(RegionNames.ConvertTabOverlayContentRegion, convertView);
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
            _regionManager.Regions.Remove(RegionNames.ConvertTabOverlayContentRegion);
        }
    }
}
