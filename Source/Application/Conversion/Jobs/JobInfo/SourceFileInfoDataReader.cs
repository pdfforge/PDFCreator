using pdfforge.DataStorage;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Utilities.Tokens;
using System;
using System.Collections.Generic;
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
                var unixTimestamp = int.Parse(data.GetValue(section + "Timestamp"));
                sfi.PrintedAt = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).LocalDateTime;

                if (sfi.PrintedAt.Year < 2000) // there was a bug in pdfcmon that truncated the last digit
                    sfi.PrintedAt = File.GetCreationTime(infFilename);
            }
            catch
            {
                sfi.PrintedAt = File.GetCreationTime(infFilename);
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

            var values = new SortedDictionary<string, string>();

            var type = sourceFileInfo.Type == JobType.XpsJob ? "xps" : "ps";
            values["SourceFileType"] = type;
            values["DocumentTitle"] = sourceFileInfo.DocumentTitle;
            values["OriginalFilePath"] = sourceFileInfo.OriginalFilePath;
            values["WinStation"] = sourceFileInfo.WinStation;
            values["UserName"] = sourceFileInfo.Author;
            values["ClientComputer"] = sourceFileInfo.ClientComputer;
            values["SpoolFileName"] = sourceFileInfo.Filename;
            values["PrinterName"] = sourceFileInfo.PrinterName;
            values["PrinterParameter"] = sourceFileInfo.PrinterParameter;
            values["ProfileParameter"] = sourceFileInfo.ProfileParameter;
            values["OutputFileParameter"] = sourceFileInfo.OutputFileParameter;
            values["SessionId"] = sourceFileInfo.SessionId.ToString(CultureInfo.InvariantCulture);
            values["JobCounter"] = sourceFileInfo.JobCounter.ToString(CultureInfo.InvariantCulture);
            values["JobId"] = sourceFileInfo.JobId.ToString(CultureInfo.InvariantCulture);
            values["Timestamp"] = new DateTimeOffset(sourceFileInfo.PrintedAt).ToUnixTimeSeconds().ToString();
            values["Copies"] = sourceFileInfo.Copies.ToString(CultureInfo.InvariantCulture);
            values["TotalPages"] = sourceFileInfo.TotalPages.ToString(CultureInfo.InvariantCulture);
            values["UserTokenEvaluated"] = sourceFileInfo.UserTokenEvaluated.ToString(CultureInfo.InvariantCulture);

            foreach (var kvp in values)
            {
                data.SetValue(section + kvp.Key, kvp.Value);
            }

            if (sourceFileInfo.UserToken != null)
            {
                foreach (var pair in sourceFileInfo.UserToken.KeyValueDict)
                {
                    data.SetValue("UserToken_" + section + pair.Key, pair.Value);
                }
            }
        }
    }
}
