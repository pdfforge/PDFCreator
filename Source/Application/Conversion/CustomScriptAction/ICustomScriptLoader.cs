namespace pdfforge.CustomScriptAction
{
    public interface ICustomScriptLoader
    {
        string ScriptFolder { get; }

        LoadScriptResult LoadScriptWithValidation(string scriptFile);

        LoadScriptResult ReLoadScriptWithValidation(string scriptFile);
    }
}
