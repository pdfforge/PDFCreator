using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using pdfforge.PDFCreator.Core.Services;

namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    public static class TransposerHelper
    {
        public static void Register(FrameworkElement element, IMountable mountable)
        {
            if (!(element?.DataContext is IMountable))
                throw new InvalidOperationException("The DataContext of the given element must inherit from IMountable");

            element.Loaded += (sender, args) => mountable.MountView();
            element.Unloaded += (sender, args) => mountable.UnmountView();
        }

        public static void Register(FrameworkElement element, IMountableAsync mountable)
        {
            if (!(element?.DataContext is IMountableAsync))
                throw new InvalidOperationException("The DataContext of the given element must inherit from IMountableAsync");

            element.Loaded += async (sender, args) => await mountable.MountViewAsync();
            element.Unloaded += async (sender, args) => await mountable.UnmountViewAsync();
        }
        public static void Register(FrameworkContentElement element, IMountable mountable)
        {
            if (!(element?.DataContext is IMountable))
                throw new InvalidOperationException("The DataContext of the given element must inherit from IMountable");

            element.Loaded += (sender, args) => mountable.MountView();
            element.Unloaded += (sender, args) => mountable.UnmountView();
        }

        public static void Register(FrameworkContentElement element, IMountableAsync mountable)
        {
            if (!(element?.DataContext is IMountableAsync))
                throw new InvalidOperationException("The DataContext of the given element must inherit from IMountableAsync");

            element.Loaded += async (sender, args) => await mountable.MountViewAsync();
            element.Unloaded += async (sender, args) => await mountable.UnmountViewAsync();
        }
    }
}
