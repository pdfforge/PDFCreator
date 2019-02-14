using System;
using System.Windows;
using System.Windows.Input;

namespace Banners
{
    internal static class WpfHelper
    {
        internal static void RegisterOpen(UIElement owner, Action<string> executeHandler)
        {
            var binding = new CommandBinding(ApplicationCommands.Open, (sender, e) =>
            {
                var parameter = e.Parameter as string;

                if (IsUrl(parameter))
                    executeHandler(parameter);

                e.Handled = true;
            }, HandleCanExecute);

            owner.CommandBindings.Add(binding);
        }

        private static bool IsUrl(string parameter)
        {
            if (string.IsNullOrEmpty(parameter))
                return false;

            return parameter.StartsWith("http") && parameter.Contains("://");
        }

        private static void HandleCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var parameter = e.Parameter as string;

            e.CanExecute = IsUrl(parameter);
            e.Handled = true;
        }
    }
}
