using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using System;

namespace pdfforge.PDFCreator.Conversion.ConverterInterface
{
    public interface IConverterFactory
    {
        IConverter GetConverter(JobType jobType);
    }

    public class ConverterFactory : IConverterFactory
    {
        private readonly IPsConverterFactory _psConverterFactory;

        public ConverterFactory(IPsConverterFactory psConverterFactory)
        {
            _psConverterFactory = psConverterFactory;
        }

        public IConverter GetConverter(JobType jobType)
        {
            if (jobType == JobType.PsJob)
                return _psConverterFactory.BuildPsConverter();
            throw new NotImplementedException("Only JobType PS is supported so far!");
        }
    }
}
