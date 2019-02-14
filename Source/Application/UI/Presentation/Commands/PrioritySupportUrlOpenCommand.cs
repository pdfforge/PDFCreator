using pdfforge.LicenseValidator.Data;
using pdfforge.LicenseValidator.Interface;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class PrioritySupportUrlOpenCommand : UrlOpenCommand
    {
        private readonly IVersionHelper _versionHelper;

        public PrioritySupportUrlOpenCommand(IProcessStarter processStarter, IVersionHelper versionHelper,
            ILicenseChecker licenseChecker, LicenseKeySyntaxChecker licenseKeySyntaxChecker, Configuration config)
            : base(processStarter)
        {
            _versionHelper = versionHelper;
            var edition = LicenseValidator.Interface.Tools.StringValueAttribute.GetStringValue(config.Product);
            var key = licenseChecker.GetSavedLicenseKey().ValueOr("");
            var normKey = licenseKeySyntaxChecker.NormalizeLicenseKey(key);

            Url = BuildUrl(edition, normKey);
        }

        protected PrioritySupportUrlOpenCommand(IProcessStarter processStarter, IVersionHelper versionHelper)
            : base(processStarter)
        {
            _versionHelper = versionHelper;
        }

        private string GetNormalizedVersionString()
        {
            return _versionHelper.FormatWithBuildNumber()
                .Replace(" ", "_")
                .Replace("(", "_")
                .Replace(")", "_")
                .Replace("__", "_")
                .Trim('_');
            ;
        }

        protected string BuildUrl(string edition, string licenseKey)
        {
            var version = GetNormalizedVersionString();

            return $"{Urls.PrioritySupport}?edition={edition}&version={version}&license_key={licenseKey}";
        }
    }

    public class CustomPrioritySupportUrlOpenCommand : PrioritySupportUrlOpenCommand
    {
        public CustomPrioritySupportUrlOpenCommand(IProcessStarter processStarter, IVersionHelper versionHelper)
            : base(processStarter, versionHelper)
        {
            Url = BuildUrl("pdfcreator_custom", "");
        }
    }

    public class DisabledPrioritySupportUrlOpenCommand : PrioritySupportUrlOpenCommand
    {
        public DisabledPrioritySupportUrlOpenCommand(IProcessStarter processStarter, IVersionHelper versionHelper)
            : base(processStarter, versionHelper)
        {
            Url = "";
        }

        public override bool CanExecute(object parameter)
        {
            return false;
        }
    }
}