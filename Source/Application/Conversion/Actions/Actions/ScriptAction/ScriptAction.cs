using NLog;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;
using pdfforge.PDFCreator.Utilities.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SystemInterface.Diagnostics;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions
{
    /// <summary>
    ///     Executes a script or executable after the conversion process.
    ///     The script receives the full paths to all created files and a string with user-configurable parameters as arguments
    /// </summary>
    public class ScriptAction : ActionBase<Scripting>, IPostConversionAction, IScriptActionHelper
    {
        private readonly IFile _file;
        private readonly IPathUtil _pathUtil;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPath _path;
        private readonly IProcessStarter _processStarter;

        public ScriptAction(IPath path, IProcessStarter processStarter, IFile file, IPathUtil pathUtil)
            : base(p => p.Scripting)
        {
            _path = path;
            _processStarter = processStarter;
            _file = file;
            _pathUtil = pathUtil;
        }

        /// <summary>
        ///     Calls the script
        /// </summary>
        /// <param name="job">The current job</param>
        /// <param name="processor"></param>
        /// <returns>An ActionResult to determine the success and a list of errors</returns>
        protected override ActionResult DoProcessJob(Job job, IPdfProcessor processor)
        {
            _logger.Debug("Launched Script-Action");

            var settings = new CurrentCheckSettings(job.AvailableProfiles, job.PrinterMappings, job.Accounts);
            var actionResult = Check(job.Profile, settings, CheckLevel.RunningJob);
            if (!actionResult)
                return actionResult;

            var scriptFile = job.Profile.Scripting.ScriptFile;
            _logger.Debug("Script-File: " + scriptFile);

            IProcess process = _processStarter.CreateProcess(scriptFile);

            var parameters = ComposeScriptParameters(job.Profile.Scripting.ParameterString, job.OutputFiles, job.TokenReplacer);

            process.StartInfo.Arguments = parameters;
            if (job.Profile.Scripting.Visible == false)
                process.StartInfo.ProcessStartInfoInstance.WindowStyle = ProcessWindowStyle.Hidden;
            _logger.Debug("Script-Parameters: " + parameters);

            var scriptDir = PathSafe.GetDirectoryName(scriptFile);
            if (scriptDir != null)
                process.StartInfo.WorkingDirectory = scriptDir;

            _logger.Debug("Script-Working Directory: " + scriptDir);

            process.EnableRaisingEvents = true;
            process.Exited += (sender, args) => process.Close();

            try
            {
                _logger.Debug("Launching script...");
                process.Start();

                if (job.Profile.Scripting.WaitForScript)
                {
                    _logger.Debug("Waiting for script to end");
                    process.WaitForExit();
                    _logger.Debug("Script execution ended");
                }
                else
                {
                    _logger.Debug("The script is executed in the background");
                }

                return new ActionResult();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception while running the script file \"" + scriptFile);
                return new ActionResult(ErrorCode.RunProgram_GenericError);
            }
        }

        public override void ApplyPreSpecifiedTokens(Job job)
        {
            job.Profile.Scripting.ScriptFile = ComposeScriptPath(job.Profile.Scripting.ScriptFile, job.TokenReplacer);
        }

        public override bool IsRestricted(ConversionProfile profile)
        {
            return false;
        }

        protected override void ApplyActionSpecificRestrictions(Job job)
        { }

        public string ComposeScriptPath(string path, TokenReplacer tokenReplacer)
        {
            var scriptPath = tokenReplacer.ReplaceTokens(path);
            try
            {
                scriptPath = _path.GetFullPath(scriptPath);
            }
            catch
            {
                //Check will deal with it later
            }

            return scriptPath;
        }

        public string GetPreview(string scriptPath, string additionalParams, TokenReplacer tokenReplacer)
        {
            if (string.IsNullOrEmpty(scriptPath) || (scriptPath.Trim().Length == 0))
                return "";

            var scriptCall = PathSafe.GetFileName(ComposeScriptPath(scriptPath, tokenReplacer));

            if (!string.IsNullOrEmpty(additionalParams))
            {
                scriptCall += " " + ComposeScriptParameters(additionalParams, new[] { @"C:\File1.pdf", @"C:\File2.pdf" }, tokenReplacer);
            }
            else
            {
                scriptCall += @" C:\File1.pdf C:\File2.pdf";
            }

            return scriptCall;
        }

        public string ComposeScriptParameters(string parameterString, IList<string> outputFiles, TokenReplacer tokenReplacer)
        {
            var composedParameters = new StringBuilder();

            composedParameters.Append(tokenReplacer.ReplaceTokens(parameterString) + " ");
            composedParameters.Append(string.Join(" ", outputFiles.Select(s => "\"" + _path.GetFullPath(s) + "\"")));

            return composedParameters.ToString();
        }

        /// <summary>
        ///     Check for valid profile
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="accounts">accounts are not used here, but are required for common ICheckable</param>
        /// <param name="checkLevel"></param>
        /// <returns>ActionResult</returns>
        public override ActionResult Check(ConversionProfile profile, CurrentCheckSettings settings, CheckLevel checkLevel)
        {
            if (!IsEnabled(profile))
                return new ActionResult();

            var isJobLevelCheck = checkLevel == CheckLevel.RunningJob;

            if (string.IsNullOrWhiteSpace(profile.Scripting.ScriptFile))
                return new ActionResult(ErrorCode.RunProgram_NoScriptFileSpecified);

            if (!isJobLevelCheck && !profile.UserTokens.Enabled && TokenIdentifier.ContainsUserToken(profile.Scripting.ScriptFile))
                return new ActionResult(ErrorCode.RunProgram_ScriptFile_RequiresUserTokens);

            //Skip further check for tokens
            if (!isJobLevelCheck && TokenIdentifier.ContainsTokens(profile.Scripting.ScriptFile))
                return new ActionResult();

            var pathUtilStatus = _pathUtil.IsValidRootedPathWithResponse(profile.Scripting.ScriptFile);
            switch (pathUtilStatus)
            {
                case PathUtilStatus.InvalidRootedPath:
                    return new ActionResult(ErrorCode.RunProgram_InvalidRootedPath);

                case PathUtilStatus.PathTooLongEx:
                    return new ActionResult(ErrorCode.RunProgram_PathTooLong);

                case PathUtilStatus.NotSupportedEx:
                    return new ActionResult(ErrorCode.RunProgram_InvalidRootedPath);

                case PathUtilStatus.ArgumentEx:
                    return new ActionResult(ErrorCode.RunProgram_IllegalCharacters);
            }

            //Skip check for network path
            if (!isJobLevelCheck && profile.Scripting.ScriptFile.StartsWith(@"\\"))
                return new ActionResult();

            if (!_file.Exists(profile.Scripting.ScriptFile))
            {
                _logger.Error("The script file \"" + profile.Scripting.ScriptFile + "\" does not exist.");
                return new ActionResult(ErrorCode.RunProgram_FileDoesNotExist);
            }

            return new ActionResult();
        }
    }
}
