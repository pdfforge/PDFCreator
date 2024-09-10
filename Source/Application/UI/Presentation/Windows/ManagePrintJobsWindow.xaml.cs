using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using System.Windows.Media;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using ListBox = System.Windows.Controls.ListBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;


namespace pdfforge.PDFCreator.UI.Presentation.Windows
{
    public partial class ManagePrintJobsWindow
    {
        public ManagePrintJobsWindow(ManagePrintJobsViewModel viewModel)
        {
            viewModel.ResetLastSelectedItem = () => _lastSelectedItem = null;
            DataContext = viewModel;
            InitializeComponent();

            // dummy reference to force GongSolutions.Wpf.DragDrop to be copied into bin folder
            var t = typeof(GongSolutions.Wpf.DragDrop.DragDrop);
        }

        private void ManagePrintJobsWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            AccessKeyHelper.SetAccessKeys(sender);
        }

        private void ListBoxItem_MouseLeave_ShowFirstItemsDocumentInfo(object sender, MouseEventArgs e)
        {
            if (sender is ListBoxItem)
            {
                if (DataContext is ManagePrintJobsViewModel viewModel)
                {
                    viewModel.SetDisplayedJobInfoToFirst();
                }
            }
        }

        private void ListBoxItem_MouseEnter_ShowCurrentItemsDocumentInfo(object sender, MouseEventArgs e)
        {
            if (sender is ListBoxItem item)
            {
                if (DataContext is ManagePrintJobsViewModel viewModel)
                {
                    viewModel.DisplayedJobInfo = item.DataContext as JobInfo;
                }
            }
        }

        private bool IsAncestorOfTypeButton(object obj)
        {
            var element = obj as DependencyObject;

            while (element != null)
            {
                if (element is Button)
                {
                    return true;
                }

                try
                {
                    element = VisualTreeHelper.GetParent(element);
                } catch {
                    // this might throw a System.InvalidOperationException when "wild clicking" between the elements
                    // and hitting an element that is not part of (was removed from) visual tree 
                    return true;
                }
            }

            return false;
        }

        private ListBoxItem _lastSelectedItem;
        private bool _isFirstClick = true;
      
        private void ListBox_PreviewMouseDown_HandleShiftSelection(object sender, MouseButtonEventArgs e)
        {
            //Do not handle selection of listbox items if inner button was clicked
            if (IsAncestorOfTypeButton(e.OriginalSource))
                return;

            if (sender is ListBox listBox)
            {
                var item = ItemsControl.ContainerFromElement(listBox, e.OriginalSource as DependencyObject) as ListBoxItem;
                if (item != null)
                {
                    if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                    {
                        //this is before it gets toggled, so we don't store unselected items
                        if (item is { IsSelected: false })
                            _lastSelectedItem = item;
                        else if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                            _lastSelectedItem = null;

                        return;
                    }

                    //always select item (or keep selection)
                    item.IsSelected = true;

                    //Handle first click with Shift, as the first job may already be selected when the window is opened
                    if (_isFirstClick && _lastSelectedItem == null)
                        _lastSelectedItem = GetFirstSelectedItem(listBox);
                    _isFirstClick = false;

                    if (_lastSelectedItem != null)
                    {
                        // Get indices of the last selected item and the current item
                        var startIndex = listBox.ItemContainerGenerator.IndexFromContainer(_lastSelectedItem);
                        var endIndex = listBox.ItemContainerGenerator.IndexFromContainer(item);

                        if (startIndex > endIndex)
                            (startIndex, endIndex) = (endIndex, startIndex);

                        for (int i = startIndex; i <= endIndex; i++)
                        {
                            (listBox.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem).IsSelected = true;
                        }
                    }

                    _lastSelectedItem = item;

                    // Prevent default behavior
                    e.Handled = true;
                }
            }
        }
        
        private ListBoxItem GetFirstSelectedItem(ListBox listBox)
        {
            if (listBox.SelectedItems.Count > 0)
            {
                var firstSelectedItem = listBox.SelectedItems[0];
                return listBox.ItemContainerGenerator.ContainerFromItem(firstSelectedItem) as ListBoxItem;
            }
            return null;
        }
    }
}