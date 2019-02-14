using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using Pdftools.Pdf;
using Pdftools.PdfValidate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PDFCreator.TestUtilities
{
    public static class PDFValidation
    {
        public static void ValidatePdf(Job job)
        {
            ValidatePdf(job.OutputFiles.First(), job.Profile.OutputFormat);
        }

        public static void ValidatePdf(string file, OutputFormat outputFormat)
        {
            var compliance = DetermineCompliance(outputFormat);

            var key = ParameterHelper.GetPassword("pdfa_validator_key");
            PdfValidator.SetLicenseKey(key);

            using (PdfValidator validator = new PdfValidator())
            {
                validator.Open(file, "", compliance);
                validator.Validate();
                var errors = GetErrors(validator);
                var errorMessages = errors.Select(e => $"{e.ErrorCode}: {e.Message}").ToList();

                Assert.IsFalse(errorMessages.Any(), "The PDF/A file could not be validated! Following errors were found: \r\n" + string.Join("\r\n", errorMessages));
            }
        }

        private static PDFCompliance DetermineCompliance(OutputFormat format)
        {
            switch (format)
            {
                case OutputFormat.PdfA1B:
                    return PDFCompliance.ePDFA1b;

                case OutputFormat.PdfA2B:
                    return PDFCompliance.ePDFA2b;

                case OutputFormat.PdfA3B:
                    return PDFCompliance.ePDFA3b;

                case OutputFormat.Pdf:
                    return PDFCompliance.ePDF17; //todo: Is highest ok?
                default:
                    throw new Exception($"{format} is not supported for validation.");
            }
        }

        private static IEnumerable<PdfError> GetErrors(PdfValidator validator)
        {
            var error = validator.GetFirstError();
            if (error == null)
                yield break;

            while (error != null)
            {
                yield return error;
                error = validator.GetNextError();
            }
        }
    }
}
