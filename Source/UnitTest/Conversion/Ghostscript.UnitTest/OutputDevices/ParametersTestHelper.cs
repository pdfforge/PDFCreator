using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Ghostscript;
using pdfforge.PDFCreator.Conversion.Ghostscript.OutputDevices;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Utilities;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Ghostscript.OutputDevices
{
    internal static class ParametersTestHelper
    {
        public const string WindowsFontsFolderDummie = "WindowsFontsFolderDummie";
        public const string JobTempFolderDummie = "JobTempFolderDummie";

        public static GhostscriptVersion GhostscriptVersionDummie
        {
            get { return new GhostscriptVersion("dummyVersion", "gsDummie.exe", "gsLibDummie"); }
        }

        public static OutputDevice GenerateDevice(OutputFormat outputFormat, IFile fileWrap = null)
        {
            var jobStub = GenerateJobStub(outputFormat);

            var fileStub = fileWrap ?? Substitute.For<IFile>();

            var osHelperStub = Substitute.For<IOsHelper>();
            osHelperStub.WindowsFontsFolder.Returns(WindowsFontsFolderDummie);

            var commandLineUtilStub = Substitute.For<ICommandLineUtil>();

            OutputDevice device = null;

            switch (outputFormat)
            {
                case OutputFormat.Jpeg:
                    device = new JpegDevice(jobStub, fileStub, osHelperStub, commandLineUtilStub);
                    break;

                case OutputFormat.Pdf:
                case OutputFormat.PdfA1B:
                case OutputFormat.PdfA2B:
                case OutputFormat.PdfX:
                    device = new PdfDevice(jobStub, fileStub, osHelperStub, commandLineUtilStub);
                    break;

                case OutputFormat.Png:
                    device = new PngDevice(jobStub, fileStub, osHelperStub, commandLineUtilStub);
                    break;

                case OutputFormat.Tif:
                    device = new TiffDevice(jobStub, fileStub, osHelperStub, commandLineUtilStub);
                    break;

                case OutputFormat.Txt:
                    device = new TextDevice(jobStub, fileStub, osHelperStub, commandLineUtilStub);
                    break;
            }

            return device;
        }

        public static Job GenerateJobStub(OutputFormat outputFormat)
        {
            var sourceFileInfo = new SourceFileInfo();
            sourceFileInfo.Filename = "SourceFileNameDummie.inf";
            var jobInfo = new JobInfo();
            jobInfo.SourceFiles.Add(sourceFileInfo);
            jobInfo.Metadata = new Metadata();
            var jobStub = new Job(jobInfo, new ConversionProfile(), new Accounts());
            jobStub.JobTempFileName = "JobTempFileNameDummie";
            jobStub.JobTempFolder = JobTempFolderDummie;
            jobStub.JobTempOutputFolder = "JobTempOutputFolderDummie";
            //jobStub.Stub(x => x.JobTempFileName).Return("JobTempFileNameDummie");
            jobStub.OutputFileTemplate = "OutputFilenameTemplateDummie";
            jobStub.Profile = new ConversionProfile();
            jobStub.Profile.OutputFormat = outputFormat;

            return jobStub;
        }

        public static void CheckDefaultParameters(Collection<string> parameters)
        {
            Assert.Contains("gs", parameters, "Missing default parameter.");
            Assert.AreEqual(0, parameters.IndexOf("gs"), "gs is not the first parameter.");

            Assert.IsNotNull(parameters.First(x => x.StartsWith("-I")), "Missing -I GhostscriptLib Parameter");
            var fontPathParameter = parameters.First(x => x.StartsWith("-sFONTPATH="));
            Assert.IsNotNull(fontPathParameter, "Missing -sFONTPATH= Parameter");
            Assert.AreEqual("-sFONTPATH=" + WindowsFontsFolderDummie, fontPathParameter, "Fontpath is not the given dummie.");

            Assert.Contains("-dNOPAUSE", parameters, "Missing default parameter.");
            Assert.Contains("-dBATCH", parameters, "Missing default parameter.");

            Assert.Contains("-f", parameters, "missing default parameter.");

            var metadataFile = parameters.FirstOrDefault(x => x.EndsWith("metadata.mtd"));
            Assert.IsNotNull(metadataFile, "Missing metadata file.");
            var expectedMetadataFile = Path.Combine(JobTempFolderDummie, "metadata.mtd");
            Assert.AreEqual(expectedMetadataFile, metadataFile, "Falsely set metadata file");
            Assert.AreEqual(parameters.Count - 1, parameters.IndexOf(metadataFile), "Metadata file is not the last parameter");
        }
    }
}
