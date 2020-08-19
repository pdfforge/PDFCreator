using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Utilities;
using System.Collections.Generic;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Ghostscript.OutputDevices
{
    /// <summary>
    ///     Extends OutputDevice to create PNG files
    /// </summary>
    public class JpegDevice : OutputDevice
    {
        public JpegDevice(Job job, ConversionMode conversionMode) : base(job, conversionMode)
        {
        }

        public JpegDevice(Job job, ConversionMode conversionMode, IFile file, IOsHelper osHelper, ICommandLineUtil commandLineUtil) : base(job, conversionMode, file, osHelper, commandLineUtil)
        {
        }

        protected override void AddDeviceSpecificParameters(IList<string> parameters)
        {
            switch (Job.Profile.JpegSettings.Color)
            {
                case JpegColor.Gray8Bit:
                    parameters.Add("-sDEVICE=jpeggray");
                    break;

                default:
                    parameters.Add("-sDEVICE=jpeg");
                    break;
            }
            parameters.Add("-dJPEGQ=" + Job.Profile.JpegSettings.Quality);
            parameters.Add("-r" + Job.Profile.JpegSettings.Dpi);
            parameters.Add("-dTextAlphaBits=4");
            parameters.Add("-dGraphicsAlphaBits=4");
        }

        protected override string ComposeOutputFilename()
        {
            //%d for multiple Pages
            return Job.JobTempFileName + "%d.jpg";
        }
    }
}
