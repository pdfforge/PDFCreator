using pdfforge.LicenseValidator.Interface;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.StartupInterface;

namespace pdfforge.PDFCreator.Core.Startup
{
    public class StaticPropertiesHack : IStaticPropertiesHack
    {
        private readonly ILicenseChecker _licenseChecker;

        public StaticPropertiesHack(ILicenseChecker licenseChecker)
        {
            _licenseChecker = licenseChecker;
        }

        public void SetStaticProperties()
        {
            // THIS SHOULD USUALLY NOT BE DONE!
            ErrorReportHelper.GetInstance().SetLicenseChecker(_licenseChecker);
        }
    }
}
