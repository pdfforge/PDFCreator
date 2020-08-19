using pdfforge.PDFCreator.Utilities.Tokens;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Conversion.Actions
{
    public interface IScriptActionHelper
    {
        string ComposeScriptPath(string path, TokenReplacer tokenReplacer);

        string ComposeScriptParameters(string parameterString, IList<string> outputFiles, TokenReplacer tokenReplacer);

        string GetPreview(string scriptPath, string additionalParams, TokenReplacer tokenReplacer);
    }
}
