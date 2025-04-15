namespace pdfforge.CustomScriptAction
{
    public interface ICustomScriptLoader
    {
        string ScriptFolder { get; }

        LoadScriptResult LoadScriptWithValidation(string scriptFile, bool enableDebugging = false);

        LoadScriptResult ReLoadScriptWithValidation(string scriptFile);
    }
}
