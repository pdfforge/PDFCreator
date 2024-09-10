using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.Helpers;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Ghostscript.OutputDevices
{
    /// <summary>
    ///     The abstract class OutputDevice holds methods and properties that handle the Ghostscript parameters. The device
    ///     independent elements are defined here.
    ///     Other classes inherit OutputDevice to extend the functionality with device-specific functionality, i.e. to create
    ///     PDF or PNG files.
    ///     Especially the abstract function AddDeviceSpecificParameters has to be implemented to add parameters that are
    ///     required to use a given device.
    /// </summary>
    public abstract class OutputDevice
    {
        public const string PasswordParameter = "-SPDFPassword";
        public ConversionMode ConversionMode { get; }

        private readonly ICommandLineUtil _commandLineUtil;

        protected readonly IFile FileWrap;
        protected readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly IOsHelper _osHelper;

        /// <summary>
        ///     A list of Distiller dictionary strings. They will be added after all parameters are set.
        /// </summary>
        protected IList<string> DistillerDictionaries = new List<string>();

        protected OutputDevice(Job job, ConversionMode conversionMode, IFile file, IOsHelper osHelper, ICommandLineUtil commandLineUtil)
        {
            Job = job;
            ConversionMode = conversionMode;
            FileWrap = file;
            _osHelper = osHelper;
            _commandLineUtil = commandLineUtil;
        }

        /// <summary>
        ///     The Job that is converted
        /// </summary>
        public Job Job { get; }

        /// <summary>
        ///     Get the list of Ghostscript Parameters. This List contains of a basic set of parameters together with some
        ///     device-specific
        ///     parameters that will be added by the device implementation
        /// </summary>
        /// <param name="ghostscriptVersion"></param>
        /// <returns>A list of parameters that will be passed to Ghostscript</returns>
        public IList<string> GetGhostScriptParameters(GhostscriptVersion ghostscriptVersion)
        {
            IList<string> parameters = new List<string>();

            var outputFormatHelper = new OutputFormatHelper();
            var jobTempFolder = PathHelper.GetShortPathName(Job.JobTempFolder);

            Logger.Info("Job folder:           " + Job.JobTempFolder);
            Logger.Info("Shortened Job folder: " + jobTempFolder);

            parameters.Add("--permit-file-all=" + jobTempFolder + "\\");
            SetLibPaths(parameters, ghostscriptVersion);
            SetFontPath(parameters);

            parameters.Add("-dNOPAUSE");
            parameters.Add("-dBATCH");

            if (!outputFormatHelper.HasValidExtension(Job.OutputFileTemplate, Job.Profile.OutputFormat))
                outputFormatHelper.EnsureValidExtension(Job.OutputFileTemplate, Job.Profile.OutputFormat);

            AddPasswordParameter(parameters);

            AddOutputFileParameter(parameters);

            AddDeviceSpecificParameters(parameters);

            // Add user-defined parameters
            if (!string.IsNullOrEmpty(Job.Profile.Ghostscript.AdditionalGsParameters))
            {
                var args = _commandLineUtil.CommandLineToArgs(Job.Profile.Ghostscript.AdditionalGsParameters);
                foreach (var s in args)
                    parameters.Add(s);
            }

            //Dictonary-Parameters must be the last Parameters
            if (DistillerDictionaries.Count > 0)
            {
                parameters.Add("-c");
                foreach (var parameter in DistillerDictionaries)
                {
                    parameters.Add(parameter);
                }
            }

            //Don't add further parameters here, since the distiller-parameters should be the last!

            parameters.Add("-f");

            SetSourceFiles(parameters, Job);

            // Compose name of the pdfmark file based on the location and name of the inf file
            var pdfMarkFileName = PathSafe.Combine(jobTempFolder, "metadata.mtd");
            CreatePdfMarksFile(pdfMarkFileName);

            // Add pdfmark file as input file to set metadata
            parameters.Add(pdfMarkFileName);

            return parameters;
        }

        private void SetLibPaths(ICollection<string> parameters, GhostscriptVersion ghostscriptVersion)
        {
            var shortLibPaths = ghostscriptVersion.LibPaths.Select(PathHelper.GetShortPathName);
            var shortLibPathString = string.Join(";", shortLibPaths);
            parameters.Add("-I" + shortLibPathString);
        }

        private void SetFontPath(IList<string> parameters)
        {
            var shortFontPath = PathHelper.GetShortPathName(_osHelper.WindowsFontsFolder);
            parameters.Add("-sFONTPATH=" + shortFontPath);
        }

        protected virtual void SetSourceFiles(IList<string> parameters, Job job)
        {
            if (ConversionMode == ConversionMode.IntermediateToTargetConversion)
                SetSourceFilesFromIntermediateFiles(parameters);
            else
                SetSourceFilesFromSourceFileInfo(parameters);
        }

        protected void SetSourceFilesFromIntermediateFiles(IList<string> parameters)
        {
            parameters.Add(PathHelper.GetShortPathName(Job.IntermediatePdfFile));
        }

        protected void SetSourceFilesFromSourceFileInfo(IList<string> parameters)
        {
            foreach (var sourceFileInfo in Job.JobInfo.SourceFiles)
            {
                parameters.Add(PathHelper.GetShortPathName(sourceFileInfo.Filename));
            }
        }

        protected virtual void AddPasswordParameter(IList<string> parameters)
        { }

        protected virtual void AddOutputFileParameter(IList<string> parameters)
        {
            parameters.Add("-sOutputFile=" + PathSafe.Combine(PathHelper.GetShortPathName(Job.JobTempOutputFolder), ComposeOutputFilename()));
        }

        /// <summary>
        ///     Create a file with metadata in the pdfmarks format. This file can be passed to Ghostscript to set Metadata of the
        ///     resulting document
        /// </summary>
        /// <param name="filename">Full path and filename of the resulting file</param>
        private void CreatePdfMarksFile(string filename)
        {
            var metadataContent = new StringBuilder();
            metadataContent.Append("/pdfmark where {pop} {userdict /pdfmark /cleartomark load put} ifelse\n[ ");
            metadataContent.Append("\n/Title " + EncodeGhostscriptParametersHex(Job.JobInfo.Metadata.Title));
            metadataContent.Append("\n/Author " + EncodeGhostscriptParametersHex(Job.JobInfo.Metadata.Author));
            metadataContent.Append("\n/Subject " + EncodeGhostscriptParametersHex(Job.JobInfo.Metadata.Subject));
            metadataContent.Append("\n/Keywords " + EncodeGhostscriptParametersHex(Job.JobInfo.Metadata.Keywords));
            metadataContent.Append("\n/Creator " + EncodeGhostscriptParametersHex(Job.Producer));
            metadataContent.Append("\n/Producer " + EncodeGhostscriptParametersHex(Job.Producer));
            metadataContent.Append("\n/DOCINFO pdfmark");

            AddViewerSettingsToMetadataContent(metadataContent);

            FileWrap.WriteAllText(filename, metadataContent.ToString());

            Logger.Debug("Created metadata file \"" + filename + "\"");
        }

        protected string EncodeGhostscriptParametersOctal(string String)
        {
            var sb = new StringBuilder();

            foreach (var c in String)
            {
                switch (c)
                {
                    case '\\':
                        sb.Append("\\\\");
                        break;

                    case '{':
                        sb.Append("\\{");
                        break;

                    case '}':
                        sb.Append("\\}");
                        break;

                    case '[':
                        sb.Append("\\[");
                        break;

                    case ']':
                        sb.Append("\\]");
                        break;

                    case '(':
                        sb.Append("\\(");
                        break;

                    case ')':
                        sb.Append("\\)");
                        break;

                    default:
                        int charCode = c;
                        if (charCode > 127)
                            sb.Append("\\" + Convert.ToString(Math.Min(charCode, 255), 8));
                        else sb.Append(c);
                        break;
                }
            }

            return sb.ToString();
        }

        protected string EncodeGhostscriptParametersHex(string String)
        {
            if (String == null)
                return "()";

            return "<FEFF" + BitConverter.ToString(Encoding.BigEndianUnicode.GetBytes(String)).Replace("-", "") + ">";
        }

        /// <summary>
        ///     This functions is called by inherited classes to add device-specific parameters to the Ghostscript parameter list
        /// </summary>
        /// <param name="parameters">The current list of parameters. This list may be modified in inherited classes.</param>
        protected abstract void AddDeviceSpecificParameters(IList<string> parameters);

        protected abstract string ComposeOutputFilename();

        private void AddViewerSettingsToMetadataContent(StringBuilder metadataContent)
        {
            metadataContent.Append("\n[\n/PageLayout ");

            switch (Job.Profile.PdfSettings.PageView)
            {
                case PageView.OneColumn:
                    metadataContent.Append("/OneColumn");
                    break;

                case PageView.TwoColumnsOddLeft:
                    metadataContent.Append("/TwoColumnLeft");
                    break;

                case PageView.TwoColumnsOddRight:
                    metadataContent.Append("/TwoColumnRight");
                    break;

                case PageView.TwoPagesOddLeft:
                    metadataContent.Append("/TwoPageLeft");
                    break;

                case PageView.TwoPagesOddRight:
                    metadataContent.Append("/TwoPageRight");
                    break;

                case PageView.OnePage:
                    metadataContent.Append("/SinglePage");
                    break;
            }

            metadataContent.Append("\n/PageMode ");
            switch (Job.Profile.PdfSettings.DocumentView)
            {
                case DocumentView.AttachmentsPanel:
                    metadataContent.Append("/UseAttachments");
                    break;

                case DocumentView.ContentGroupPanel:
                    metadataContent.Append("/UseOC");
                    break;

                case DocumentView.FullScreen:
                    metadataContent.Append("/FullScreen");
                    break;

                case DocumentView.Outline:
                    metadataContent.Append("/UseOutlines");
                    break;

                case DocumentView.ThumbnailImages:
                    metadataContent.Append("/UseThumbs");
                    break;

                default:
                    metadataContent.Append("/UseNone");
                    break;
            }

            var startPage = 1;
            if (Job.Profile.PdfSettings.ViewerStartsOnPage > Job.NumberOfPages)
                startPage = Job.NumberOfPages;
            else if (Job.Profile.PdfSettings.ViewerStartsOnPage > 0)
                startPage = Job.Profile.PdfSettings.ViewerStartsOnPage;

            metadataContent.Append(" /Page " + startPage);

            metadataContent.Append("\n/DOCVIEW pdfmark");
        }
    }
}
