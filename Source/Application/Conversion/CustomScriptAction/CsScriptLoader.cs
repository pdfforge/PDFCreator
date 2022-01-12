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
        public string ScriptFolder { get; }

        public CsScriptLoader(IFile file, IProgramDataDirectoryHelper programDataDirectoryHelper, IAssemblyHelper assemblyHelper)
        {
            _file = file;

            var assemblyDir = assemblyHelper.GetAssemblyDirectory();
            CSScript.GlobalSettings.AddSearchDir(assemblyDir);

            var programData = programDataDirectoryHelper.GetDir();
            ScriptFolder = PathSafe.Combine(programData, CsScriptsFolderName);
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
            var time = DateTime.Now;
            try
            {
                var script = CSScript.CodeDomEvaluator.LoadFile<IPDFCreatorScript>(scriptFile);
                if (script == null)
                    return new LoadScriptResult(new ActionResult(ErrorCode.CustomScript_ErrorDuringCompilation), null, "");

                var compileTime = DateTime.Now - time;
                _logger.Trace($"It took {compileTime} to compile the cs-script.");

                return new LoadScriptResult(new ActionResult(), script, "");
            }
            catch (Exception exception)
            {
                return new LoadScriptResult(new ActionResult(ErrorCode.CustomScript_ErrorDuringCompilation), null, exception.Message);
            }
        }
    }
}
