using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using System;

namespace pdfforge.CustomScriptAction
{
    public interface ICustomScriptHandler
    {
        ActionResult ExecutePreConversion(Job job);

        ActionResult ExecutePostConversion(Job job);
    }

    public class CustomScriptHandler : ICustomScriptHandler
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ICustomScriptLoader _customScriptLoader;

        public CustomScriptHandler(ICustomScriptLoader customScriptLoader)
        {
            _customScriptLoader = customScriptLoader;
        }

        public ActionResult ExecutePreConversion(Job job)
        {
            var scriptFilename = job.Profile.CustomScript.ScriptFilename;

            var loadScriptResult = _customScriptLoader.LoadScriptWithValidation(scriptFilename);
            if (!loadScriptResult.Result)
            {
                _logger.Error($"Exception during compilation of CustomScript: {loadScriptResult.ExceptionMessage}");
                return loadScriptResult.Result;
            }

            _logger.Trace($"Loaded CustomScript: {scriptFilename}");

            try
            {
                var loadedScript = loadScriptResult.Script;
                var scriptResult = loadedScript.PreConversion(job, _logger);
                switch (scriptResult)
                {
                    case ScriptResult.Abort:
                        _logger.Error("Executing CustomScript PreConversion returned ScriptResult Abort");
                        return new ActionResult(ErrorCode.CustomScriptPreConversion_ScriptResultAbort);

                    case ScriptResult.Success:
                    default:
                        return new ActionResult();
                }
            }
            catch (Exception e)
            {
                _logger.Error($"Exception during execution of CustomScript PreConversion:\r\n{e.Message}");
                return new ActionResult(ErrorCode.CustomScriptPreConversion_Exception);
            }
        }

        public ActionResult ExecutePostConversion(Job job)
        {
            var scriptFilename = job.Profile.CustomScript.ScriptFilename;

            var loadScriptResult = _customScriptLoader.LoadScriptWithValidation(scriptFilename);
            if (!loadScriptResult.Result)
            {
                _logger.Error($"Exception during compilation of CustomScript: {loadScriptResult.ExceptionMessage}");
                return loadScriptResult.Result;
            }

            _logger.Trace($"Loaded CustomScript: {scriptFilename}");

            try
            {
                var scriptResult = loadScriptResult.Script.PostConversion(job, _logger);
                switch (scriptResult)
                {
                    case ScriptResult.Abort:
                        _logger.Error("Executing CustomScript PostConversion returned ScriptResult Abort");
                        return new ActionResult(ErrorCode.CustomScriptPostConversion_ScriptResultAbort);

                    case ScriptResult.Success:
                    default:
                        return new ActionResult();
                }
            }
            catch (Exception e)
            {
                _logger.Error($"Exception during execution of CustomScript PostConversion:\r\n{e.Message}");
                return new ActionResult(ErrorCode.CustomScriptPostConversion_Exception);
            }
        }
    }
}
