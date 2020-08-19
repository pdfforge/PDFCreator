using Optional;
using System;
using System.IO;
using System.Text;

namespace pdfforge.PDFCreator.Utilities.Pdf
{
    public class PdfVersionHelper : IPdfVersionHelper
    {
        public Option<PdfVersion, PdfVersionError> GetPdfVersionFromFile(string file)
        {
            string version;

            if (string.IsNullOrEmpty(file))
                return Option.None<PdfVersion, PdfVersionError>(PdfVersionError.NoPdfFile);

            try
            {
                using (var streamReader = new StreamReader(file))
                using (var binaryReader = new BinaryReader(streamReader.BaseStream))
                {
                    var data = binaryReader.ReadBytes(8);
                    var pdfVersion = Encoding.ASCII.GetString(data);
                    version = pdfVersion.StartsWith("%") ? pdfVersion.Substring(1) : pdfVersion;
                }
            }
            catch (ArgumentException)
            {
                return Option.None<PdfVersion, PdfVersionError>(PdfVersionError.IllegalHeader);
            }
            catch (IOException)
            {
                return Option.None<PdfVersion, PdfVersionError>(PdfVersionError.FileNotReadable);
            }
            catch
            {
                return Option.None<PdfVersion, PdfVersionError>(PdfVersionError.UnknownError);
            }

            return ParsePdfVersion(version);
        }

        private Option<PdfVersion, PdfVersionError> ParsePdfVersion(string version)
        {
            switch (version)
            {
                case "PDF-2.0":
                    return PdfVersion.Pdf20.Some<PdfVersion, PdfVersionError>();

                case "PDF-1.7":
                    return PdfVersion.Pdf17.Some<PdfVersion, PdfVersionError>();

                case "PDF-1.6":
                    return PdfVersion.Pdf16.Some<PdfVersion, PdfVersionError>();

                case "PDF-1.5":
                    return PdfVersion.Pdf15.Some<PdfVersion, PdfVersionError>();

                case "PDF-1.4":
                    return PdfVersion.Pdf14.Some<PdfVersion, PdfVersionError>();

                case "PDF-1.3":
                    return PdfVersion.Pdf13.Some<PdfVersion, PdfVersionError>();

                case "PDF-1.2":
                    return PdfVersion.Pdf12.Some<PdfVersion, PdfVersionError>();

                case "PDF-1.1":
                    return PdfVersion.Pdf11.Some<PdfVersion, PdfVersionError>();

                case "PDF-1.0":
                    return PdfVersion.Pdf10.Some<PdfVersion, PdfVersionError>();

                default:
                    return Option.None<PdfVersion, PdfVersionError>(PdfVersionError.NoPdfFile);
            }
        }

        public bool CheckIfVersionIsPdf20(string file)
        {
            var pdfVersion = GetPdfVersionFromFile(file);

            return pdfVersion.Exists(v => v == PdfVersion.Pdf20);
        }
    }

    public enum PdfVersion
    {
        Pdf10,
        Pdf11,
        Pdf12,
        Pdf13,
        Pdf14,
        Pdf15,
        Pdf16,
        Pdf17,
        Pdf20
    }

    public enum PdfVersionError
    {
        IllegalHeader,
        FileNotReadable,
        NoPdfFile,
        UnknownError
    }

    public interface IPdfVersionHelper
    {
        Option<PdfVersion, PdfVersionError> GetPdfVersionFromFile(string file);

        bool CheckIfVersionIsPdf20(string file);
    }
}
