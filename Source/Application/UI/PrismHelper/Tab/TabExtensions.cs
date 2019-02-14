using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.ServiceLocator;
using pdfforge.PDFCreator.UI.Presentation.Controls.Tab;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Gpo;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper;
using Prism.Regions;
using System;
using System.Windows;
using System.Windows.Controls;
using Translatable;

namespace pdfforge.PDFCreator.UI.PrismHelper.Tab
{
    public static class TabExtensions
    {
        private static readonly Thickness ContentPadding = new Thickness(30, 30, 30, 0);

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
                var tab = new TabItem();
                tab.Name = typeof(TView).Name;
                var viewModel = whitelistedServiceLocator.GetInstance<TModel>();
                HelpProvider.SetHelpTopic(tab, helpTopic);

                var content = new ContentControl();
                content.SetValue(RegionManager.RegionNameProperty, contentRegionName);
                content.IsTabStop = false;

                var contentContainer = new Grid();
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

                var contentRegion = new ItemsControl();
                contentRegion.SetValue(RegionManager.RegionNameProperty, contentRegionName);
                contentRegion.Padding = ContentPadding;
                contentRegion.IsTabStop = false;
                content.Children.Add(contentRegion);

                var header = new IconTabHeader();
                tab.Header = header;
                header.DataContext = viewModel;

                AddGpoBehavior(tab, content, viewModel);
                tab.Content = content;
                return tab;
            });
        }

        public static void RegisterMasterTab<T>(this IRegionManager regionManager,
            string subTabRegionName,
            string contentRegionName,
            string tabRegion,
            IWhitelistedServiceLocator whitelistedServiceLocator) where T : class, ITabViewModel, IWhitelisted
        {
            regionManager.RegisterViewWithRegion(tabRegion, () =>
            {
                ITabViewModel viewModel = whitelistedServiceLocator.GetInstance<T>();
                var tab = new TabItem();
                var masterIconTabHeader = new MasterIconTabHeader(subTabRegionName);
                var contentContainer = new Grid();
                var tabContent = new ContentControl();
                tabContent.Padding = ContentPadding;
                tabContent.IsTabStop = false;

                tab.Header = masterIconTabHeader;
                tabContent.SetValue(RegionManager.RegionNameProperty, contentRegionName);
                tabContent.DataContext = viewModel;

                masterIconTabHeader.DataContext = viewModel;
                contentContainer.Children.Add(tabContent);
                AddGpoBehavior(tab, contentContainer, viewModel);
                tab.Content = contentContainer;

                return tab;
            });
        }

        public static void RegisterSubTab<TView, TModel, TTranslation>(this IRegionManager regionManager, string tabItemRegion, string contentRegion, Func<TTranslation, string> titleId, Func<ConversionProfile, IProfileSetting> setting, IWhitelistedServiceLocator whitelistedServiceLocator, Func<ConversionProfile, bool> hasNotSupportedFeatures, bool isFirst = false)
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

                model.Init<TTranslation>(titleId, setting, new PrismNavigationValueObject(contentRegion, typeof(TView).Name, () =>
                {
                    var listBox = subItem.Parent as ListBox;
                    if (listBox != null)
                    {
                        listBox.SelectedItem = subItem;
                    }
                }), hasNotSupportedFeatures);

                subItem.DataContext = model;

                subItem.IsActiveChanged += (sender, args) =>
                {
                    if (subItem.IsActive)
                    {
                        regionManager.RequestNavigate(contentRegion, typeof(TView).Name);
                    }
                };

                return subItem;
            });
        }
    }
}
