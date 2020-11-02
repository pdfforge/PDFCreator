using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.ComImplementation;
using pdfforge.PDFCreator.Core.JobInfoQueue;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace pdfforge.PDFCreator.UI.COM
{
    [ComVisible(true)]
    [Guid("489689FE-E8AF-41FF-8D5A-8212DF2F013C")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IJobFinishedEvent
    {
        void JobFinished();
    }

    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [Guid("01E51AAE-D371-469A-A556-FC491A81778D")]
    public interface IPrintJob
    {
        bool IsFinished { get; }
        bool IsSuccessful { get; }

        void SetProfileByGuid(string profileGuid);

        OutputFiles GetOutputFiles { get; }

        void ConvertTo(string fullFileName);

        void ConvertToAsync(string fullFileName);

        void SetProfileSetting(string name, string value);

        void SetProfileListSetting(string name, ArrayList value);

        PrintJobInfo PrintJobInfo { get; }

        string GetProfileSetting(string propertyName);

        ArrayList GetProfileListSetting(string propertyName);

        void AddActionToPosition(string actionSettingsName, int addToPosition);

        void AddAction(string actionSettingsName);

        void RemoveAction(string actionSettingsName);
    }

    [ComVisible(true)]
    [ComSourceInterfaces(typeof(IJobFinishedEvent))]
    [Guid("9616B8B3-FE6E-4122-AC93-E46DBD571F87")]
    [ClassInterface(ClassInterfaceType.None)]
    public class PrintJob : IPrintJob
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly PrintJobAdapter _printJobAdapter;

        internal PrintJob(Job job, IJobInfoQueue comJobInfoQueue, IPrintJobAdapterFactory printJobAdapterFactory)
        {
            Logger.Trace("COM: Setting up the ComJob instance.");
            _printJobAdapter = printJobAdapterFactory.BuildPrintJobAdapter(job);
        }

        internal JobInfo JobInfo => _printJobAdapter.Job.JobInfo;

        /// <summary>
        ///     Informs about process state
        /// </summary>
        public bool IsFinished => _printJobAdapter.IsFinished;

        /// <summary>
        ///     Returns true, if the job finished successfully
        /// </summary>
        public bool IsSuccessful => _printJobAdapter.IsSuccessful;

        /// <summary>
        ///     Sets the profile by guid to use for COM conversion
        /// </summary>
        public void SetProfileByGuid(string profileGuid)
        {
            _printJobAdapter.SetProfileByGuid(profileGuid);
        }

        /// <summary>
        ///     Returns an object of type Outputfiles if the job contains any output file
        /// </summary>
        public OutputFiles GetOutputFiles => new OutputFiles(_printJobAdapter.Job.OutputFiles);

        /// <summary>
        ///     Converts the job to the specified location
        /// </summary>
        /// <param name="fullFileName">Specifies the location</param>
        public void ConvertTo(string fullFileName)
        {
            _printJobAdapter.ConvertTo(fullFileName);
        }

        /// <summary>
        ///     Converts the job to the specified location asynchronously
        /// </summary>
        /// <param name="fullFileName">Specifies the location and the file's name</param>
        public void ConvertToAsync(string fullFileName)
        {
            _printJobAdapter.ConvertToAsync(fullFileName);
        }

        /// <summary>
        ///     Set a conversion profile property using two strings: One for the name (i.e. PdfSettings.Security.Enable) and one
        ///     for the value
        /// </summary>
        /// <param name="name">Name of the setting. This can include subproperties (i.e. PdfSettings.Security.Enable)</param>
        /// <param name="value">A string that can be parsed to the type</param>
        public void SetProfileSetting(string name, string value)
        {
            _printJobAdapter.SetProfileSetting(name, value);
            SortAndUpdateActions();
        }

        /// <summary>
        ///     Set a conversion profile property using a string and a list of strings: the single string is used for the name (i.e. PdfSettings.Security.Enable)
        ///     and the list is used for value
        /// </summary>
        /// <param name="name">Name of the setting. This can include subproperties (i.e. PdfSettings.Security.Enable)</param>
        /// <param name="value">A IEnumerable of string to set the value</param>
        public void SetProfileListSetting(string name, ArrayList value)
        {
            var list = new List<string>();

            for (int i = 0; i < value.Count; i++)
            {
                var item = value[i].ToString();
                list.Add(item);
            }

            _printJobAdapter.SetProfileListSetting(name, list);
        }

        /// <summary>
        ///     Returns a PrintJobInfo object
        /// </summary>
        public PrintJobInfo PrintJobInfo => new PrintJobInfo(_printJobAdapter.Job.JobInfo.Metadata);

        /// <summary>
        ///     Gets the current value of a specific profile property using its name.
        /// </summary>
        /// <param name="propertyName">Name of the setting. This can include subproperties (i.e. PdfSettings.Security.Enable)</param>
        /// <returns>The value of the property</returns>
        public string GetProfileSetting(string propertyName)
        {
            return _printJobAdapter.GetProfileSetting(propertyName);
        }

        /// <summary>
        ///     Gets the current value of a specific profile property as string list using its name.
        /// </summary>
        /// <param name="propertyName">name of the setting. This can include subproperties (i.e. PdfSettings.Security.Enable)</param>
        /// <returns></returns>
        public ArrayList GetProfileListSetting(string propertyName)
        {
            var profileListSetting = _printJobAdapter.GetProfileListSetting(propertyName);
            var array = new ArrayList();
            foreach (var str in profileListSetting)
            {
                array.Add(str);
            }
            return array;
        }

        /// <summary>
        /// Run a helper to add all active actions into actionOrder list and sort it to default sorting
        /// </summary>
        private void SortAndUpdateActions()
        {
            _printJobAdapter.SortAndUpdateActions();
        }

        /// <summary>
        /// Activates an action and puts it in a certain execution position
        /// It only enables the execution all configurations must be done by user
        /// </summary>
        /// <param name="actionSettingsName">setting name of the action that should be activated</param>
        /// <param name="addToPosition">the position index in which the action gets executed</param>
        public void AddActionToPosition(string actionSettingsName, int addToPosition)
        {
            var profileListSetting = GetProfileListSetting("ActionOrder");
            if (addToPosition >= profileListSetting.Count || addToPosition < 0)
            {
                AddAction(actionSettingsName);
                return;
            }

            profileListSetting.Insert(addToPosition, actionSettingsName);
            SetProfileListSetting("ActionOrder", profileListSetting);
            SetActionEnabledState(actionSettingsName, true);
        }

        /// <summary>
        /// Activates an action and puts it at the end of the execution order
        /// It only enables the execution all configurations must be done by user
        /// </summary>
        /// <param name="actionSettingsName">setting name of the action that should be activated</param>
        public void AddAction(string actionSettingsName)
        {
            var propertyName = "ActionOrder";
            var profileListSetting = GetProfileListSetting(propertyName);
            profileListSetting.Add(actionSettingsName);
            SetProfileListSetting(propertyName, profileListSetting);
            SetActionEnabledState(actionSettingsName, true);
        }

        /// <summary>
        /// Removes an action from the execution list
        /// </summary>
        /// <param name="actionSettingsName">setting name of the action that should be deactivated</param>
        public void RemoveAction(string actionSettingsName)
        {
            var actionOrder = GetProfileListSetting("ActionOrder");
            actionOrder.Remove(actionSettingsName);
            SetProfileListSetting("ActionOrder", actionOrder);

            SetActionEnabledState(actionSettingsName, false);
        }

        private void SetActionEnabledState(string actionSettingsName, bool state)
        {
            var profileSetting = GetProfileSetting(actionSettingsName);
            if (profileSetting != null)
            {
                SetProfileSetting($"{actionSettingsName}.Enabled", state ? "True" : "False");
            }
        }

#pragma warning disable CS0067

        public event EventHandler JobFinished; //Extern event: For COM clients only

#pragma warning restore CS0067
    }
}
