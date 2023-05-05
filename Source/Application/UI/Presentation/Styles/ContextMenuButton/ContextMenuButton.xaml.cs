using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.IconPacks;

namespace pdfforge.PDFCreator.UI.Presentation.Styles.ContextMenuButton
{
    /// <summary>
    /// Interaction logic for ContextMenuButton.xaml
    /// </summary>
    public partial class ContextMenuButton : Button
    {
        public ContextMenuButton()
        {
            InitializeComponent();
        }

        private bool _wasLoaded = false;

        private void ContextMenuButton_OnLoaded(object sender, RoutedEventArgs e)
        {
            //View gets loaded twice...
            if (_wasLoaded)
                return;
            _wasLoaded = true;

            if (MenuItemsCommand != null)
            {
                MenuItemsCommand.CanExecuteChanged += (sender, args) =>
                {
                    this.IsEnabled = MenuItemsCommand.CanExecute(MenuItemsCommandParameter);
                };
            }

            //Add DropDownArrow to current Content
            if (ShowArrow)
            {
                HorizontalContentAlignment = HorizontalAlignment.Stretch;
                var dockPanel = new DockPanel
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    LastChildFill = true
                };

                var dropArrow = new PackIconMaterialDesign
                {
                    Kind = PackIconMaterialDesignKind.ArrowDropDown,
                    Width = 9,
                    Foreground = Foreground,
                    Margin = new Thickness(1, 0, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                DockPanel.SetDock(dropArrow, Dock.Right);
                dockPanel.Children.Add(dropArrow);

                if (Content != null)
                {
                    var content = new ContentControl
                    {
                        Content = Content,
                        Margin = new Thickness(0, 0, 3, 0),
                        HorizontalAlignment = HorizontalAlignment.Center,
                    };
                    DockPanel.SetDock(content, Dock.Left);
                    dockPanel.Children.Add(content);
                }

                Content = dockPanel;
            }
        }

        private void ContextMenuButton_OnClick(object sender, RoutedEventArgs e)
        {
            //Suppress exception in case context menu was not set
            if (ContextMenu == null)
                return;

            //Animation has to be removed, else setting IsOpen has no effect
            ContextMenu.BeginAnimation(ContextMenu.IsOpenProperty, null);
            ContextMenu.PlacementTarget = this;

            switch (ContextMenuPosition)
            {
                case ContextMenuPosition.BottomLeft:
                    ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Left;
                    ContextMenu.VerticalOffset = Height;
                    ContextMenu.HorizontalOffset = Width;
                    break;

                default:
                case ContextMenuPosition.BottomRight:
                    ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                    break;
            }

            ContextMenu.IsOpen = true;

            //This is how a general command/parameter can be set for all items
            //It must be set after ContextMenu was opened / was loaded...
            foreach (MenuItem menuItem in ContextMenu.Items)
            {
                if (menuItem.Command == null)
                    menuItem.Command = MenuItemsCommand;

                if (menuItem.CommandParameter == null)
                    menuItem.CommandParameter = MenuItemsCommandParameter;
            }
        }

        public static readonly DependencyProperty IconForegroundProperty =
            DependencyProperty.Register(nameof(IconForeground), typeof(Brush), typeof(ContextMenuButton));

        public Brush IconForeground
        {
            get => (Brush)GetValue(IconForegroundProperty);
            set => SetValue(IconForegroundProperty, value);
        }

        public static readonly DependencyProperty ContextMenuPositionProperty =
            DependencyProperty.Register(nameof(ContextMenuPosition), typeof(ContextMenuPosition),
                typeof(ContextMenuButton), new PropertyMetadata(ContextMenuPosition.BottomRight));

        public ContextMenuPosition ContextMenuPosition
        {
            get => (ContextMenuPosition)GetValue(ContextMenuPositionProperty);
            set => SetValue(ContextMenuPositionProperty, value);
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable<MenuItem>),
                typeof(ContextMenuButton), new PropertyMetadata(ItemsSourceChanged));

        private static void ItemsSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var contextMenuButton = sender as ContextMenuButton;
            if (contextMenuButton != null)
                contextMenuButton.OnItemsSourceChanged();
        }

        private void OnItemsSourceChanged()
        {
            if (ItemsSource != null)
                this.ContextMenu = new ContextMenu { ItemsSource = ItemsSource };
        }

        public IEnumerable<MenuItem> ItemsSource
        {
            get => (IEnumerable<MenuItem>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly DependencyProperty ShowArrowProperty =
            DependencyProperty.Register(nameof(ShowArrow), typeof(bool),
                typeof(ContextMenuButton), new PropertyMetadata(true));

        public bool ShowArrow
        {
            get => (bool)GetValue(ShowArrowProperty);
            set => SetValue(ShowArrowProperty, value);
        }

        public static readonly DependencyProperty MenuItemsCommandProperty =
            DependencyProperty.Register(nameof(MenuItemsCommand), typeof(ICommand),
                typeof(ContextMenuButton));

        public ICommand MenuItemsCommand
        {
            get => (ICommand)GetValue(MenuItemsCommandProperty);
            set => SetValue(MenuItemsCommandProperty, value);
        }

        public static readonly DependencyProperty MenuItemsCommandParameterProperty =
            DependencyProperty.Register(nameof(MenuItemsCommandParameter), typeof(object),
                typeof(ContextMenuButton));

        public object MenuItemsCommandParameter
        {
            get => (object)GetValue(MenuItemsCommandParameterProperty);
            set => SetValue(MenuItemsCommandParameterProperty, value);
        }
    }

    public enum ContextMenuPosition
    {
        BottomRight,
        BottomLeft
    }
}
