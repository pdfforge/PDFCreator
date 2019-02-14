using CSScriptLibrary;
using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Utilities;
using System;
using SystemInterface.IO;

namespace pdfforge.CustomScriptAction
{
    public class CsScriptLoader : ICustomScriptLoader
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IFile _file;

        public static string CsScriptsFolderName = "CS-Scripts";
        public string ScriptFolder { get; set; }

        public CsScriptLoader(IFile file, IAssemblyHelper assemblyHelper)
        {
            _file = file;

            var assemblyDir = assemblyHelper.GetAssemblyDirectory();
            CSScript.GlobalSettings.AddSearchDir(assemblyDir);

            ScriptFolder = PathSafe.Combine(assemblyDir, CsScriptsFolderName);
            CSScript.GlobalSettings.AddSearchDir(ScriptFolder);

            _logger.Debug($"CsScriptLoaderInitialized with following SearchDirs: {CSScript.GlobalSettings.SearchDirs}");
        }

        public LoadScriptResult ReLoadScriptWithValidation(string scriptFile)
        {
            CSScript.CacheEnabled = false;
            var result = LoadScriptWithValidation(scriptFile);
            CSScript.CacheEnabled = true;
            return result;
        }

        public LoadScriptResult LoadScriptWithValidation(string scriptFilename)
        {
            var actionResult = new ActionResult();

            if (string.IsNullOrWhiteSpace(scriptFilename))
            {
                actionResult.Add(ErrorCode.CustomScript_NoScriptFileSpecified);
                return new LoadScriptResult(actionResult, null, "");
            }

            var scriptFile = PathSafe.Combine(ScriptFolder, scriptFilename);

            if (!_file.Exists(scriptFile))
            {
                actionResult.Add(ErrorCode.CustomScript_FileDoesNotExistInScriptFolder);
                return new LoadScriptResult(actionResult, null, "");
            }

            return LoadScript(scriptFile);
        }

        private LoadScriptResult LoadScript(string scriptFile)
        {
            try
            {
                var script = CSScript.CodeDomEvaluator.LoadFile<IPDFCreatorScript>(scriptFile);
                if (script == null)
                    return new LoadScriptResult(new ActionResult(ErrorCode.CustomScript_ErrorDuringCompilation), null, "");
                return new LoadScriptResult(new ActionResult(), script, "");
            }
            catch (Exception exception)
            {
                return new LoadScriptResult(new ActionResult(ErrorCode.CustomScript_ErrorDuringCompilation), null, exception.Message);
            }
        }
    }
}
