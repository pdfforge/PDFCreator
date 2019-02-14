using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;

namespace pdfforge.CustomScriptAction
{
    public interface IPDFCreatorScript
    {
        ScriptResult PreConversion(Job job, Logger logger);

        ScriptResult PostConversion(Job job, Logger logger);
    }
}
