using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.ComImplementation;
using pdfforge.PDFCreator.Core.JobInfoQueue;
using System;
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

        PrintJobInfo PrintJobInfo { get; }

        string GetProfileSetting(string propertyName);
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

#pragma warning disable CS0067

        public event EventHandler JobFinished; //Extern event: For COM clients only

#pragma warning restore CS0067
    }
}
