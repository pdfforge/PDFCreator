using System;
using System.Windows.Markup;
using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.UI.Presentation.Styles.OutputFormat
{
    internal class GetOutputFormatDescription : MarkupExtension
    {
        public Conversion.Settings.Enums.OutputFormat Value { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Value.GetDescription();
        }
    }
}
