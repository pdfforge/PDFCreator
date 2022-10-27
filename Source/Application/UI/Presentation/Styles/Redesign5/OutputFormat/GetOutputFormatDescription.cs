using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System;
using System.Windows.Markup;

namespace pdfforge.PDFCreator.UI.Presentation.Styles.Redesign5.SelectOutputFormatButton
{
    internal class GetOutputFormatDescription : MarkupExtension
    {
        public OutputFormat Value { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Value.GetDescription();
        }
    }
}
