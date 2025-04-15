using System;
using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.Controls
{
    public class UniformWrapPanel : WrapPanel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            double maxChildWidth = 0;
            double maxChildHeight = 0;

            foreach (UIElement child in InternalChildren)
            {
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                maxChildWidth = Math.Max(maxChildWidth, child.DesiredSize.Width);
                maxChildHeight = Math.Max(maxChildHeight, child.DesiredSize.Height);
            }

            ItemWidth = maxChildWidth;
            ItemHeight = maxChildHeight;

            return base.MeasureOverride(availableSize);
        }
    }
}