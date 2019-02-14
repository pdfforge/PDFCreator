using pdfforge.DataStorage;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Utilities.Tokens;
using System;
using System.Globalization;
using System.IO;

namespace pdfforge.PDFCreator.Conversion.Jobs.JobInfo
{
    public class SourceFileInfoDataReader
    {
        /// <summary>
        ///     Read a single SourceFileInfo record from the given data section
        /// </summary>
        /// <param name="infFilename">full path to the inf file to read</param>
        /// <param name="data">Data set to use</param>
        /// <param name="section">Name of the section to process</param>
        /// <returns>A filled SourceFileInfo or null, if the data is invalid (i.e. no filename)</returns>
        public SourceFileInfo ReadSourceFileInfoFromData(string infFilename, Data data, string section)
        {
            if (!section.EndsWith("\\"))
                section = section + "\\";

            if (infFilename == null)
                throw new ArgumentNullException(nameof(infFilename));

            var sfi = new SourceFileInfo();

            sfi.DocumentTitle = data.GetValue(section + "DocumentTitle");
            sfi.OriginalFilePath = data.GetValue(section + "OriginalFilePath");
            sfi.WinStation = data.GetValue(section + "WinStation");
            sfi.Author = data.GetValue(section + "UserName");
            sfi.ClientComputer = data.GetValue(section + "ClientComputer");
            sfi.Filename = data.GetValue(section + "SpoolFileName");

            sfi.PrinterName = data.GetValue(section + "PrinterName");
            sfi.PrinterParameter = data.GetValue(section + "PrinterParameter");
            sfi.ProfileParameter = data.GetValue(section + "ProfileParameter");
            sfi.OutputFileParameter = data.GetValue(section + "OutputFileParameter");

            var type = data.GetValue(section + "SourceFileType");

            sfi.Type = type.Equals("xps", StringComparison.OrdinalIgnoreCase) ? JobType.XpsJob : JobType.PsJob;

            if (!Path.IsPathRooted(sfi.Filename))
            {
                sfi.Filename = Path.Combine(Path.GetDirectoryName(infFilename) ?? "", sfi.Filename);
            }

            sfi.PrinterName = data.GetValue(section + "PrinterName");

            try
            {
                sfi.SessionId = int.Parse(data.GetValue(section + "SessionId"));
            }
            catch
            {
                sfi.SessionId = 0;
            }

            try
            {
                sfi.JobCounter = int.Parse(data.GetValue(section + "JobCounter"));
            }
            catch
            {
                sfi.JobCounter = 0;
            }

            try
            {
                sfi.JobId = int.Parse(data.GetValue(section + "JobId"));
            }
            catch
            {
                sfi.JobId = 0;
            }

            try
            {
                sfi.TotalPages = int.Parse(data.GetValue(section + "TotalPages"));
            }
            catch
            {
                sfi.TotalPages = 0;
            }

            try
            {
                sfi.Copies = int.Parse(data.GetValue(section + "Copies"));
            }
            catch
            {
                sfi.Copies = 0;
            }

            try
            {
                sfi.UserTokenEvaluated = bool.Parse(data.GetValue(section + "UserTokenEvaluated"));
            }
            catch
            {
                sfi.UserTokenEvaluated = false;
            }

            try
            {
                var ut = data.GetValues("UserToken_" + section);
                sfi.UserToken = new UserToken();
                foreach (var keyValuePair in ut)
                {
                    sfi.UserToken.AddKeyValuePair(keyValuePair.Key, keyValuePair.Value);
                }
            }
            catch (Exception)
            {
                sfi.UserToken = new UserToken();
            }

            return string.IsNullOrEmpty(sfi.Filename) ? null : sfi;
        }

        public void WriteSourceFileInfoToData(Data data, string section, SourceFileInfo sourceFileInfo)
        {
            if (!section.EndsWith("\\"))
                section = section + "\\";

            data.SetValue(section + "DocumentTitle", sourceFileInfo.DocumentTitle);
            data.SetValue(section + "OriginalFilePath", sourceFileInfo.OriginalFilePath);
            data.SetValue(section + "WinStation", sourceFileInfo.WinStation);
            data.SetValue(section + "UserName", sourceFileInfo.Author);
            data.SetValue(section + "ClientComputer", sourceFileInfo.ClientComputer);
            data.SetValue(section + "SpoolFileName", sourceFileInfo.Filename);
            data.SetValue(section + "PrinterName", sourceFileInfo.PrinterName);
            data.SetValue(section + "PrinterParameter", sourceFileInfo.PrinterParameter);
            data.SetValue(section + "ProfileParameter", sourceFileInfo.ProfileParameter);
            data.SetValue(section + "OutputFileParameter", sourceFileInfo.OutputFileParameter);
            data.SetValue(section + "SessionId", sourceFileInfo.SessionId.ToString(CultureInfo.InvariantCulture));
            data.SetValue(section + "JobCounter", sourceFileInfo.JobCounter.ToString(CultureInfo.InvariantCulture));
            data.SetValue(section + "JobId", sourceFileInfo.JobId.ToString(CultureInfo.InvariantCulture));

            var type = sourceFileInfo.Type == JobType.XpsJob ? "xps" : "ps";
            data.SetValue(section + "SourceFileType", type);

            data.SetValue(section + "Copies", sourceFileInfo.Copies.ToString(CultureInfo.InvariantCulture));
            data.SetValue(section + "TotalPages", sourceFileInfo.TotalPages.ToString(CultureInfo.InvariantCulture));

            data.SetValue(section + "UserTokenEvaluated", sourceFileInfo.UserTokenEvaluated.ToString(CultureInfo.InvariantCulture));
            if (sourceFileInfo.UserToken != null)
            {
                foreach (var keyValuPair in sourceFileInfo.UserToken.KeyValueDict)
                {
                    data.SetValue("UserToken_" + section + keyValuPair.Key, keyValuPair.Value);
                }
            }
        }
    }
}
