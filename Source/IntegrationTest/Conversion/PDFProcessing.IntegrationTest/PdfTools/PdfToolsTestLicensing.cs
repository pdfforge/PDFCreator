using PDFCreator.TestUtilities;
using pdfforge.PDFCreator.Conversion.Processing.PdfToolsProcessing;
using pdfforge.PDFCreator.Utilities;
using System.Reflection;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.PDFProcessing.PdfTools
{
    public class PdfToolsTestLicensing : PdfToolsLicensing
    {
        public string _PdfToolboxKey
        {
            set { PdfToolboxKey = value; }
        }

        public string _PdfAConverterKey
        {
            set { PdfAConverterKey = value; }
        }

        public string _PdfSecureKey
        {
            set { PdfSecureKey = value; }
        }

        public string _PdfAValidatorKey
        {
            set { PdfAValidatorKey = value; }
        }

        public PdfToolsTestLicensing() : base(ParameterHelper.GetPassword("pdftoolbox_key"), ParameterHelper.GetPassword("pdfa_converter_key"), ParameterHelper.GetPassword("pdfsecure_key"), ParameterHelper.GetPassword("pdfa_validator_key"))
        {
            var assemblyHelper = new AssemblyHelper(Assembly.GetExecutingAssembly());
            var libPath = assemblyHelper.GetAssemblyDirectory() + "\\lib\\";

            var osHelper = new OsHelper();
            libPath += osHelper.Is64BitProcess ? "x64" : "x86";
            osHelper.AddDllDirectorySearchPath(libPath);
        }
    }
}
