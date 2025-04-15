using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Utilities;
using System;
using CSScripting;
using CSScriptLib;
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
            var result = LoadScriptWithValidationInternal(scriptFile, false);
            return result;
        }


        public LoadScriptResult LoadScriptWithValidation(string scriptFilename, bool enableDebugging = false) => LoadScriptWithValidationInternal(scriptFilename, enableDebugging: enableDebugging);

        private LoadScriptResult LoadScriptWithValidationInternal(string scriptFilename, bool withCaching = true, bool enableDebugging = false)
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

            return LoadScript(scriptFile, withCaching, enableDebugging);
        }

        private LoadScriptResult LoadScript(string scriptFile, bool withCaching = true, bool enableDebugging = false)
        {
            var time = DateTime.Now;
            try
            {
                    var script = CSScript.RoslynEvaluator.With(config =>
                    {
                        config.IsCachingEnabled = !enableDebugging && withCaching;
                        config.DebugBuild = enableDebugging;
                    }).LoadFile<IPDFCreatorScript>(scriptFile);
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
            return null;
        }
    }
}
