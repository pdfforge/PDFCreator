using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Utilities;
using System.Collections.Generic;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Ghostscript.OutputDevices
{
    public class PdfIntermediateDevice : PdfDevice
    {
        public PdfIntermediateDevice(Job job) : base(job, ConversionMode.IntermediateConversion)
        {
        }

        public PdfIntermediateDevice(Job job, IFile file, IOsHelper osHelper, ICommandLineUtil commandLineUtil)
            : base(job, ConversionMode.IntermediateConversion, file, osHelper, commandLineUtil)
        {
        }

        protected override void AddDeviceSpecificParameters(IList<string> parameters)
        {
            parameters.Add("-sDEVICE=pdfwrite");
            parameters.Add("-dCompatibilityLevel=1.4");
            parameters.Add("-dPDFSETTINGS=/default");
            parameters.Add("-dEmbedAllFonts=true");

            SetPageOrientation(parameters, DistillerDictonaries);

            //Do not set ColorConversionStrategy to leave colors unchanged (by default)

            //Do not compress color and greyscale images
            parameters.Add("-dAutoFilterColorImages=false");
            parameters.Add("-dAutoFilterGrayImages=false");
            parameters.Add("-dEncodeColorImages=false");
            parameters.Add("-dEncodeGrayImages=false");

            //Do not compress mono images
            parameters.Add("-dEncodeMonoImages=false");
        }
    }
}
