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
    public class TiffDevice : OutputDevice
    {
        public TiffDevice(Job job, ConversionMode conversionMode) : base(job, conversionMode)
        {
        }

        public TiffDevice(Job job, ConversionMode conversionMode, IFile file, IOsHelper osHelper, ICommandLineUtil commandLineUtil) : base(job, conversionMode, file, osHelper, commandLineUtil)
        {
        }

        protected override void AddDeviceSpecificParameters(IList<string> parameters)
        {
            switch (Job.Profile.TiffSettings.Color)
            {
                case TiffColor.BlackWhiteG4Fax:
                    parameters.Add("-sDEVICE=tiffg4");
                    break;

                case TiffColor.BlackWhiteG3Fax:
                    parameters.Add("-sDEVICE=tiffg3");
                    break;

                case TiffColor.BlackWhiteLzw:
                    parameters.Add("-sDEVICE=tifflzw");
                    break;

                case TiffColor.Gray8Bit:
                    parameters.Add("-sDEVICE=tiffgray");
                    parameters.Add("-sCompression=lzw");
                    break;

                case TiffColor.Color12Bit:
                    parameters.Add("-sDEVICE=tiff12nc");
                    parameters.Add("-sCompression=lzw");
                    break;

                case TiffColor.Color24Bit:
                    parameters.Add("-sDEVICE=tiff24nc");
                    parameters.Add("-sCompression=lzw");
                    break;
            }

            parameters.Add("-r" + Job.Profile.TiffSettings.Dpi);
            parameters.Add("-dTextAlphaBits=4");
            parameters.Add("-dGraphicsAlphaBits=4");
        }

        protected override string ComposeOutputFilename()
        {
            return Job.JobTempFileName + ".tif";
        }
    }
}
