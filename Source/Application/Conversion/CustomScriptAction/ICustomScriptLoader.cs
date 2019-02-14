namespace pdfforge.CustomScriptAction
{
    public interface ICustomScriptLoader
    {
        string ScriptFolder { get; set; }

        LoadScriptResult LoadScriptWithValidation(string scriptFile);

        LoadScriptResult ReLoadScriptWithValidation(string scriptFile);
    }
}
