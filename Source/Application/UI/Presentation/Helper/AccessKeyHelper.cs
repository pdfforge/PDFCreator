using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using NLog.LayoutRenderers.Wrappers;

namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    public static class AccessKeyHelper
    {
        private static readonly List<string> InvalidAccessKeys = new() { "-", "." };

        public static void SetAccessKeys(object sender)
        {
            if (sender is DependencyObject dependencyObject)
            {
                var assignedAccessKeys = new List<string>();
                var accessKeyContentControls = new List<ContentControl>();

                var allContentControls = FindVisualChildren<Button, Label>(dependencyObject).ToList();
                foreach (var contentControl in allContentControls)
                {
                    if (NeedsAccessKey(contentControl, assignedAccessKeys))
                        accessKeyContentControls.Add(contentControl);
                }

                foreach (var accessKeyContentControl in accessKeyContentControls)
                {
                    DoSetAccessKey(accessKeyContentControl, assignedAccessKeys);
                }
            }
        }

        private static IEnumerable<ContentControl> FindVisualChildren<T1, T2>(DependencyObject dependencyObject) where T1 : ContentControl where T2 : ContentControl
        {
            if (dependencyObject == null) yield return (ContentControl)Enumerable.Empty<ContentControl>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
            {
                DependencyObject ithChild = VisualTreeHelper.GetChild(dependencyObject, i);
                if (ithChild == null) 
                    continue;
                if (ithChild is T1 || ithChild is T2) 
                    yield return (ContentControl)ithChild;
                foreach (var childOfChild in FindVisualChildren<T1, T2>(ithChild)) yield return childOfChild;
            }
        }

        private static bool NeedsAccessKey(ContentControl contentControl, ICollection<string> assignedAccessKeys)
        {
            var contentText = "";

            //ignore labels without target
            if (contentControl is Label { Target: null })
                return false;

            if (contentControl.Content is AccessText accessText)
            {
                contentText = accessText.Text;
            }
            else if (contentControl.Content is string contentString)
            {
                contentText = contentString;
            }

            if (string.IsNullOrEmpty(contentText))
                return false;

            //Keep intended access key if it's not used already
            var underScoreIndex = contentText.IndexOf('_');
            if (underScoreIndex >= 0 && underScoreIndex < contentText.Length - 1)
            {
                var accessKey = contentText[underScoreIndex + 1].ToString();
                if (!assignedAccessKeys.Contains(accessKey))
                {
                    assignedAccessKeys.Add(contentText[underScoreIndex + 1].ToString());
                    return false;
                }
            }

            return true;
        }

        private static void DoSetAccessKey(ContentControl contentControl, ICollection<string> assignedAccessKeys)
        {
            if (contentControl.Content is AccessText accessText)
            {
                accessText.Text = DetermineAccessKey(accessText.Text, assignedAccessKeys);
            }
            else if (contentControl.Content is string contentString)
            {
                contentControl.Content = DetermineAccessKey(contentString, assignedAccessKeys);
            }
        }

        private static string DetermineAccessKey(string contentString, ICollection<string> assignedAccessKeys)
        {
            contentString = contentString.Replace("_", "");
            for (var index = 0; index < contentString.Length; index++)
            {
                var accessKey = contentString[index].ToString().ToLower(CultureInfo.InvariantCulture);
                if (InvalidAccessKeys.Contains(accessKey))
                    continue;
                if (assignedAccessKeys.Contains(accessKey))
                    continue;

                assignedAccessKeys.Add(accessKey);
                contentString = contentString.Insert(index, "_");
                break;
            }
            return contentString;
        }
    }
}