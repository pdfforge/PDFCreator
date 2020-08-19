using pdfforge.PDFCreator.Core.ServiceLocator;

namespace pdfforge.PDFCreator.Utilities
{
    public class ApplicationNameProvider : IWhitelisted
    {
        public ApplicationNameProvider(string editionName)
        {
            EditionName = editionName;
        }

        public string ApplicationName => "PDFCreator";
        public string EditionName { get; }
        public string ApplicationNameWithEdition => ApplicationName + " " + EditionName;

        /// <summary>
        /// Machine readable product name, e.g. pdfcreator_professional
        /// </summary>
        public string ProductIdentifier => ApplicationNameWithEdition.ToLowerInvariant().Replace(" ", "_");
    }
}
