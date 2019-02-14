using pdfforge.DataStorage;
using pdfforge.DataStorage.Storage;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;

namespace pdfforge.PDFCreator.Conversion.Jobs.JobInfo
{
    public interface IJobInfoManager
    {
        /// <summary>
        ///     Creates a JobInfo based on the given Inf file
        /// </summary>
        /// <param name="infFile">full path to the Inf file to use</param>
        JobInfo ReadFromInfFile(string infFile);

        /// <summary>
        ///     Save the inf file to the path from the InfFile property
        /// </summary>
        void SaveToInfFile(JobInfo jobInfo);

        /// <summary>
        ///     Save the inf file to the given path
        ///     <param name="filename">filename where the inf will be stored</param>
        /// </summary>
        void SaveToInfFile(JobInfo jobInfo, string filename);

        /// <summary>
        ///     Deletes the inf file and also the source files
        /// </summary>
        void DeleteInfAndSourceFiles(JobInfo jobInfo);

        /// <summary>
        ///     Merge the second JobInfo into the first one
        /// </summary>
        void Merge(JobInfo jobInfo, JobInfo jobinfoToMerge);
    }

    public class JobInfoManager : IJobInfoManager
    {
        private readonly ITitleReplacerProvider _titleReplacerProvider;
        private readonly IStoredParametersManager _storedParametersManager;

        public JobInfoManager(ITitleReplacerProvider titleReplacerProvider, IStoredParametersManager storedParametersManager)
        {
            _titleReplacerProvider = titleReplacerProvider;
            _storedParametersManager = storedParametersManager;
        }

        public JobInfo ReadFromInfFile(string infFile)
        {
            var titleReplacer = _titleReplacerProvider.BuildTitleReplacer();
            return ReadFromInfFile(infFile, titleReplacer);
        }

        public void SaveToInfFile(JobInfo jobInfo)
        {
            if (string.IsNullOrEmpty(jobInfo.InfFile))
            {
                throw new InvalidOperationException("The inf file must not be empty");
            }

            var infData = Data.CreateDataStorage();
            var ini = new IniStorage(jobInfo.InfFile, Encoding.GetEncoding("Unicode"));

            var sourceFileReader = new SourceFileInfoDataReader();

            var sectionId = 0;
            foreach (var sourceFileInfo in jobInfo.SourceFiles)
            {
                var section = sectionId.ToString(CultureInfo.InvariantCulture) + "\\";
                sourceFileReader.WriteSourceFileInfoToData(infData, section, sourceFileInfo);
                sectionId++;
            }

            ini.WriteData(infData);
        }

        public void SaveToInfFile(JobInfo jobInfo, string filename)
        {
            jobInfo.InfFile = filename;
            SaveToInfFile(jobInfo);
        }

        public void DeleteInfAndSourceFiles(JobInfo jobInfo)
        {
            foreach (var sourceFileInfo in jobInfo.SourceFiles)
            {
                try
                {
                    if (File.Exists(sourceFileInfo.Filename))
                        File.Delete(sourceFileInfo.Filename);
                }
                catch (UnauthorizedAccessException) { }
                catch (IOException) { }
            }
            jobInfo.SourceFiles.Clear();
            DeleteInf(jobInfo);
        }

        public void Merge(JobInfo jobInfo, JobInfo jobinfoToMerge)
        {
            if (jobInfo.JobType != jobinfoToMerge.JobType)
                return;

            foreach (var sourceFile in jobinfoToMerge.SourceFiles)
            {
                jobInfo.SourceFiles.Add(sourceFile);
            }

            DeleteInf(jobinfoToMerge);
        }

        private void DeleteInf(JobInfo jobInfo)
        {
            try
            {
                if (!string.IsNullOrEmpty(jobInfo.InfFile))
                    File.Delete(jobInfo.InfFile);
            }
            catch (UnauthorizedAccessException) { }
            catch (IOException) { }

            jobInfo.InfFile = null;
        }

        private JobInfo ReadFromInfFile(string infFile, TitleReplacer titleReplacer)
        {
            var jobInfo = new JobInfo();

            jobInfo.InfFile = infFile;
            var infData = Data.CreateDataStorage();
            var ini = new IniStorage(jobInfo.InfFile, Encoding.GetEncoding("Unicode"));
            ini.ReadData(infData);

            var sourceFiles = new ObservableCollection<SourceFileInfo>();
            var sourceFileReader = new SourceFileInfoDataReader();
            foreach (var section in infData.GetSections())
            {
                var sfi = sourceFileReader.ReadSourceFileInfoFromData(infFile, infData, section);
                if (sfi != null)
                    sourceFiles.Add(sfi);
            }
            jobInfo.SourceFiles = sourceFiles;

            var metadata = new Metadata();
            if (sourceFiles.Count > 0)
            {
                metadata.PrintJobAuthor = sourceFiles[0].Author;
                metadata.PrintJobName = titleReplacer.Replace(sourceFiles[0].DocumentTitle);

                jobInfo.OriginalFilePath = sourceFiles[0].OriginalFilePath;
                jobInfo.PrinterName = sourceFiles[0].PrinterName;
                jobInfo.PrinterParameter = sourceFiles[0].PrinterParameter;
                jobInfo.ProfileParameter = sourceFiles[0].ProfileParameter;
                jobInfo.OutputFileParameter = sourceFiles[0].OutputFileParameter;

                jobInfo.JobType = sourceFiles[0].Type;
            }

            ConsiderStoredParameters(jobInfo);

            jobInfo.Metadata = metadata;

            jobInfo.PrintDateTime = File.GetCreationTime(infFile);

            return jobInfo;
        }

        private void ConsiderStoredParameters(JobInfo jobInfo)
        {
            //Required null check for server
            if (_storedParametersManager == null)
                return;

            //Check for PrintJob (PrinterName remains empty for DirectConversion)
            if (string.IsNullOrWhiteSpace(jobInfo.PrinterName))
                return;

            if (!_storedParametersManager.HasPredefinedParameters())
                return;

            var storedParameters = _storedParametersManager.GetAndResetParameters();
            jobInfo.ProfileParameter = storedParameters.Profile;
            jobInfo.OutputFileParameter = storedParameters.Outputfile;
            jobInfo.OriginalFilePath = storedParameters.OriginalFilePath;
        }
    }
}
