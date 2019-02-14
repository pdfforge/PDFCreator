using System;
using System.Windows;
using System.Windows.Input;

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
    }

    public interface IMountable
    {
        void MountView();

        void UnmountView();
    }

    public interface IMountableCommand : IMountable, ICommand
    {
    }
}
