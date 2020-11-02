using MahApps.Metro.SimpleChildWindow.Utils;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.ServiceLocator;
using pdfforge.PDFCreator.UI.Presentation.Controls.Tab;
using pdfforge.PDFCreator.UI.Presentation.Events;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Gpo;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper;
using Prism.Events;
using Prism.Regions;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Translatable;

namespace pdfforge.PDFCreator.UI.PrismHelper.Tab
{
    public static class TabExtensions
    {
        private static void AddGpoBehavior(TabItem tab, Grid content, ITabViewModel viewModel)
        {
            if (viewModel.BlockedByGPO)
            {
                var contentControl = new GPOLockLayer();
                contentControl.IsLockedByGpo = viewModel.BlockedByGPO;
                contentControl.DataContext = viewModel;
                content.Children.Add(contentControl);
            }

            if (viewModel.HiddenByGPO)
            {
                tab.Visibility = Visibility.Collapsed;
            }
        }

        public static void RegisterSimpleTab<TView, TModel>(this IRegionManager regionManager, string tabControlRegion, string contentRegionName, HelpTopic helpTopic, IWhitelistedServiceLocator whitelistedServiceLocator)
            where TView : class
            where TModel : class, ITabViewModel, IWhitelisted
        {
            regionManager.RegisterViewWithRegion(contentRegionName, typeof(TView));
            regionManager.RegisterViewWithRegion(tabControlRegion, () =>
            {
                var tab = new TabItem
                {
                    Name = typeof(TView).Name
                };

                var viewModel = whitelistedServiceLocator.GetInstance<TModel>();
                HelpProvider.SetHelpTopic(tab, helpTopic);

                var content = new ContentControl();
                content.SetValue(RegionManager.RegionNameProperty, contentRegionName);
                content.IsTabStop = false;
                content.SetResourceReference(FrameworkElement.StyleProperty, "TabUserControl");

                var contentContainer = new Grid();
                contentContainer.Background = new SolidColorBrush(Colors.White);
                contentContainer.Children.Add(content);
                AddGpoBehavior(tab, contentContainer, viewModel);

                var header = new IconTabHeader();
                tab.Header = header;
                header.DataContext = viewModel;

                tab.Content = contentContainer;

                return tab;
            });
        }

        public static void RegisterMultiContentTab<T>(this IRegionManager regionManager, string tabControlRegion, string contentRegionName, HelpTopic helpTopic, IWhitelistedServiceLocator whitelistedServiceLocator) where T : class, ITabViewModel, IWhitelisted
        {
            regionManager.RegisterViewWithRegion(tabControlRegion, () =>
            {
                var tab = new TabItem();
                HelpProvider.SetHelpTopic(tab, helpTopic);
                var viewModel = whitelistedServiceLocator.GetInstance<T>();
                var content = new Grid();
                content.Background = Brushes.White;

                var contentRegion = new ItemsControl();
                contentRegion.SetResourceReference(FrameworkElement.StyleProperty, "TabUserControl");
                contentRegion.SetValue(RegionManager.RegionNameProperty, contentRegionName);
                contentRegion.IsTabStop = false;
                contentRegion.Background = new SolidColorBrush(Colors.White);
                content.Children.Add(contentRegion);

                var header = new IconTabHeader();
                tab.Header = header;
                header.DataContext = viewModel;

                AddGpoBehavior(tab, content, viewModel);
                tab.Content = content;

                TransposerHelper.Register(header, viewModel);
                return tab;
            });
        }

        public static void RegisterMasterTab<T>(this IRegionManager regionManager,
            string subTabRegionName,
            string contentRegionName,
            string tabRegion,
            IWhitelistedServiceLocator whitelistedServiceLocator,
            IEventAggregator eventAggregator) where T : class, ITabViewModel, IWhitelisted
        {
            regionManager.RegisterViewWithRegion(tabRegion, () =>
            {
                ITabViewModel viewModel = whitelistedServiceLocator.GetInstance<T>();

                var tab = new TabItem();
                var masterIconTabHeader = new MasterIconTabHeader(subTabRegionName);
                var contentContainer = new Grid();
                var tabContent = new ContentControl { IsTabStop = false };
                tabContent.SetResourceReference(FrameworkElement.StyleProperty, "TabUserControl");

                tab.Header = masterIconTabHeader;
                tabContent.SetValue(RegionManager.RegionNameProperty, contentRegionName);
                tabContent.DataContext = viewModel;

                tabContent.Loaded += (sender, args) =>
                {
                    var helpTopic = HelpTopic.ProfileSettings;
                    var dependencyObjects = (args.OriginalSource as ContentControl).GetChildObjects();

                    if (dependencyObjects.First() is DependencyObject view)
                        helpTopic = (HelpTopic)view.GetValue(HelpProvider.HelpTopicProperty);

                    eventAggregator.GetEvent<SetProfileTabHelpTopicEvent>().Publish(helpTopic);
                };

                masterIconTabHeader.DataContext = viewModel;
                contentContainer.Background = new SolidColorBrush(Colors.White);
                contentContainer.Children.Add(tabContent);
                AddGpoBehavior(tab, contentContainer, viewModel);
                tab.Content = contentContainer;

                TransposerHelper.Register(masterIconTabHeader, viewModel);
                TransposerHelper.Register(tabContent, viewModel);

                return tab;
            });
        }

        public static void RegisterSubTab<TView, TModel, TTranslation>(this IRegionManager regionManager, string tabItemRegion, string contentRegion, Func<TTranslation, string> titleId, Func<ConversionProfile, IProfileSetting> setting, IWhitelistedServiceLocator whitelistedServiceLocator, Func<ConversionProfile, bool> hasNotSupportedFeatures, IEventAggregator eventAggregator, bool isFirst = false)
            where TView : class
            where TModel : class, IWhitelisted, ISubTabViewModel
            where TTranslation : ITranslatable, new()
        {
            if (isFirst)
            {
                regionManager.RegisterViewWithRegion(contentRegion, typeof(TView));
            }

            regionManager.RegisterViewWithRegion(tabItemRegion, () =>
            {
                var subItem = new SubTabItem();
                var model = whitelistedServiceLocator.GetInstance<TModel>();

                InitSubItemModel<TView, TModel, TTranslation>(contentRegion, titleId, setting, hasNotSupportedFeatures, model, subItem);

                subItem.DataContext = model;

                SetupSubItemIsActiveChanged<TView>(regionManager, contentRegion, eventAggregator, subItem);

                subItem.Initialized += (sender, args) => model.MountView();
                TransposerHelper.Register(subItem, model);

                return subItem;
            });
        }

        private static void InitSubItemModel<TView, TModel, TTranslation>(string contentRegion, Func<TTranslation, string> titleId, Func<ConversionProfile, IProfileSetting> setting, Func<ConversionProfile, bool> hasNotSupportedFeatures, TModel model, SubTabItem subItem) where TView : class where TModel : class, IWhitelisted, ISubTabViewModel where TTranslation : ITranslatable, new()
        {
            model.Init<TTranslation>(titleId, setting, new PrismNavigationValueObject(contentRegion, typeof(TView).Name, () =>
            {
                var listBox = subItem.Parent as ListBox;
                if (listBox != null && listBox.Items.Count == 0)
                {
                    listBox.SelectedItem = subItem;
                }
            }), hasNotSupportedFeatures);
        }

        private static void SetupSubItemIsActiveChanged<TView>(IRegionManager regionManager, string contentRegion, IEventAggregator eventAggregator, SubTabItem subItem) where TView : class
        {
            subItem.IsActiveChanged += (sender, args) =>
            {
                if (!subItem.IsActive)
                    return;

                var helpTopic = HelpTopic.ProfileSettings;

                regionManager.RequestNavigate(contentRegion, typeof(TView).Name, result =>
                {
                    if (result.Context.NavigationService == null)
                        return;

                    var regionActiveViews = result.Context.NavigationService.Region.ActiveViews.ToList();
                    if (regionActiveViews.First() is DependencyObject view)
                        helpTopic = (HelpTopic)view.GetValue(HelpProvider.HelpTopicProperty);
                });

                eventAggregator.GetEvent<SetProfileTabHelpTopicEvent>().Publish(helpTopic);
            };
        }
    }
}
