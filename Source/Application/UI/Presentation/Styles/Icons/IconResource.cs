using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.Styles.Icons
{
    internal static class IconResource
    {
        private static readonly IEnumerable<ResourceDictionary> IconResources = new List<ResourceDictionary>
        {
            new()
            {
                Source = new Uri("pack://application:,,,/PDFCreator.Presentation;component/Styles/Icons/AccountIcons.xaml",
                    UriKind.RelativeOrAbsolute)
            }
        };

        public static ContentControl TryFindResource(string iconName)
        {
            foreach (var resource in IconResources)
            {
                if (resource[iconName] is FrameworkElement icon)
                {
                    return new ContentControl { Content = icon, IsTabStop = false };
                }
            }
            return null;
        }
    }
}
