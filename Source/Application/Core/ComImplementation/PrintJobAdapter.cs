using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.JobInfoQueue;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.UI.COM;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using SystemInterface.IO;
using pdfforge.PDFCreator.Core.SettingsManagement.Helper;

namespace pdfforge.PDFCreator.Core.ComImplementation
{
    public class PrintJobAdapter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IDirectory _directory;
        private readonly IPathUtil _pathUtil;
        private readonly IActionOrderChecker _actionOrderChecker;
        private readonly ErrorCodeInterpreter _errorCodeInterpreter;
        private readonly IJobInfoQueue _jobInfoQueue;
        private readonly OutputFormatHelper _outputFormatHelper = new OutputFormatHelper();

        private readonly ISettingsProvider _settingsProvider;
        private readonly ThreadPool _threadPool;

        private readonly List<string> _unaccessibleItems = new List<string>
        {
            "autosave",
            "properties",
            "skipprintdialog",
            "savedialog",
            "openviewer"
        };

        private readonly IComWorkflowFactory _workflowFactory;

        public PrintJobAdapter(ISettingsProvider settingsProvider, IComWorkflowFactory workflowFactory, ThreadPool threadPool, IJobInfoQueue jobInfoQueue, ErrorCodeInterpreter errorCodeInterpreter, IDirectory directory, IPathUtil pathUtil, IActionOrderChecker actionOrderChecker)
        {
            _settingsProvider = settingsProvider;
            _workflowFactory = workflowFactory;
            _threadPool = threadPool;
            _jobInfoQueue = jobInfoQueue;
            _errorCodeInterpreter = errorCodeInterpreter;
            _directory = directory;
            _pathUtil = pathUtil;
            _actionOrderChecker = actionOrderChecker;
        }

        public Job Job { get; set; }

        public bool IsFinished { get; private set; }
        public bool IsSuccessful { get; private set; }

        public event EventHandler JobFinished;

        public void SetProfileByGuid(string profileGuid)
        {
            Logger.Trace("COM: Setting the job profile: {0}", profileGuid);
            var profile = _settingsProvider.Settings.GetProfileByGuid(profileGuid);

            if (profile == null)
                throw new COMException($"A profile with the GUID '{profileGuid}' does not exist!");

            Job.Profile = profile.Copy();
        }

        public void ConvertTo(string fullFileName)
        {
            if (!_pathUtil.IsValidRootedPath(fullFileName))
                throw new ArgumentException("Invalid path", nameof(fullFileName));

            Logger.Trace("COM: Starting the conversion process.");
            DoConversion(Job, DetermineOutputFilename(fullFileName));
        }

        public void ConvertToAsync(string fullFileName)
        {
            CreateThread(DetermineOutputFilename(fullFileName));
        }

        public void SetProfileSetting(string name, string value)
        {
            var valueReflector = new ValueReflector();

            if (HasAccess(name) && valueReflector.HasProperty(Job.Profile, name))
            {
                valueReflector.SetPropertyValue(Job.Profile, name, value);
            }
            else
            {
                throw new COMException($"The property '{name}' does not exist!");
            }
        }

        public void SetProfileListSetting(string name, IEnumerable<string> value)
        {
            var valueReflector = new ValueReflector();

            if (HasAccess(name) && valueReflector.HasProperty(Job.Profile, name))
            {
                valueReflector.SetListPropertyValue(Job.Profile, name, value);
            }
            else
            {
                throw new COMException($"The property '{name}' does not exist!");
            }
        }

        public string GetProfileSetting(string name)
        {
            var valueReflector = new ValueReflector();

            if (HasAccess(name) && valueReflector.HasProperty(Job.Profile, name))
            {
                return valueReflector.GetPropertyValue(Job.Profile, name);
            }

            throw new COMException($"The property '{name}' does not exist!");
        }

        public string[] GetProfileListSetting(string name)
        {
            var valueReflector = new ValueReflector();

            if (HasAccess(name) && valueReflector.HasProperty(Job.Profile, name))
            {
                return valueReflector.GetPropertyListValue(Job.Profile, name).ToArray();
            }

            throw new COMException($"The property '{name}' does not exist!");
        }

        /// <summary>
        ///     Starts a new asynchronous thread
        /// </summary>
        private void CreateThread(string filename)
        {
            Logger.Trace("COM: Removing jobinfo from queue.");
            _jobInfoQueue.Remove(Job.JobInfo);

            Logger.Trace("COM: Creating new asynchronous thread.");
            var thread = new SynchronizedThread(() => DoConversion(Job, filename));

            thread.Name = "ConversionThread";

            Logger.Trace("COM: Adding the new thread to the thread pool.");
            _threadPool.AddThread(thread);
        }

        /// <summary>
        ///     Does the actual conversion process of the job
        /// </summary>
        private void DoConversion(Job job, string targetFilename)
        {
            DisableIrrelevantProfileSettings(job.Profile);

            try
            {
                if (job.JobInfo.SourceFiles.Count == 0)
                {
                    Logger.Info("COM: JobInfo has no source files and will be skipped");
                    return;
                }

                Logger.Trace("COM: Creating workflow");
                var workflow = _workflowFactory.BuildWorkflow(targetFilename);

                Logger.Trace("COM: Running workflow");
                var workflowResult = workflow.RunWorkflow(job);

                if (workflowResult.State == WorkflowResultState.Error)
                {
                    var errorCode = ErrorCode.Conversion_UnknownError;
                    if (workflow.LastError != null)
                        errorCode = workflow.LastError.Value;

                    throw new COMException(_errorCodeInterpreter.GetErrorText(errorCode, true));
                }

                if (workflowResult.State == WorkflowResultState.AbortedByUser)
                {
                    Logger.Info("COM: The job '{0}' was aborted by the user.",
                        job.JobInfo.Metadata.Title);
                }

                if (workflowResult.State == WorkflowResultState.Finished)
                {
                    IsSuccessful = true;
                }
            }
            catch (Exception e)
            {
                throw new COMException(e.Message);
            }
            finally
            {
                Logger.Trace("COM: Removing jobinfo from the queue.");
                _jobInfoQueue.Remove(job.JobInfo, true);

                IsFinished = true;

                JobFinished?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Disabling unnecessary profile settings for COM
        /// </summary>
        private void DisableIrrelevantProfileSettings(ConversionProfile profile)
        {
            profile.AutoSave.Enabled = false;
            profile.AutoSave.EnsureUniqueFilenames = false;

            profile.OpenViewer.Enabled = false;
        }

        /// <summary>
        ///     Checks if the property name is accessible through COM
        /// </summary>
        /// <param name="propertyName">The property to check for</param>
        /// <returns>True, if the property can be accessed otherwise false</returns>
        private bool HasAccess(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return false;

            return !_unaccessibleItems.Any(item => propertyName.ToLowerInvariant().StartsWith(item));
        }

        /// <summary>
        ///     Sets the location where the job should be converted to and the jobs full name
        /// </summary>
        /// <param name="fileName">Specifies the location and the name of the converted file</param>
        private string DetermineOutputFilename(string fileName)
        {
            if (fileName == null)
                throw new COMException("The output filename was not set");

            var tmpPath = PathSafe.GetDirectoryName(fileName);

            if (tmpPath == null || !_directory.Exists(tmpPath))
                throw new COMException("Invalid path. Please check if the directory exists.");

            Logger.Trace("COM: Setting the full name of the job:" + fileName);

            return _outputFormatHelper.EnsureValidExtension(fileName, Job.Profile.OutputFormat);
        }

        public void SortAndUpdateActions()
        {
            _actionOrderChecker.Check(new[] { Job.Profile });
        }
    }
}
