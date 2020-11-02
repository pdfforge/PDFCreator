using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeCommandBuilderProvider : ICommandBuilderProvider
    {
        public IMacroCommandBuilder ProvideBuilder(ICommandLocator commandLocator)
        {
            return new DesignTimeCommandBuilder();
        }
    }
}
