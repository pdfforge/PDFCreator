using MahApps.Metro.Controls;
using pdfforge.PDFCreator.UI.Presentation.Help;
using Prism.Regions;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    /// <summary>
    /// Interaction logic for WorkflowEditorOverlayView.xaml
    /// </summary>
    public partial class WorkflowEditorOverlayView : UserControl
    {
        private readonly IRegionManager _regionManager;
        private static bool _isRegistered;

        public WorkflowEditorOverlayView(WorkflowEditorOverlayViewModel vm, IRegionManager regionManager)
        {
            _regionManager = regionManager;
            DataContext = vm;
            InitializeComponent();
            // Prism doesn't want to register the region properly so we do it by hand
            if (!_isRegistered)
            {
                Prism.Regions.RegionManager.SetRegionManager(ContentControl, _regionManager);
                _isRegistered = true;
            }

            vm.UpdateHelpTopic = () =>
            {
                var dependencyObject = ContentControl.GetChildObjects().FirstOrDefault();
                if (dependencyObject == null)
                    return;

                var valueSource = DependencyPropertyHelper.GetValueSource(dependencyObject, HelpProvider.HelpTopicProperty);
                if (valueSource.BaseValueSource != BaseValueSource.Default)
                {
                    SetValue(HelpProvider.HelpTopicProperty, dependencyObject.GetValue(HelpProvider.HelpTopicProperty));
                }
            };
        }

        private void WorkflowEditorOverlayView_OnUnloaded(object sender, RoutedEventArgs e)
        {
            // remove region from prism to prevent collision
            _regionManager.Regions.Remove(RegionNames.ProfileWorkflowEditorOverlayRegion);
            _isRegistered = false;
        }

        private void WorkflowEditorOverlayView_OnLoaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
