using Optional;
using pdfforge.LicenseValidator.Interface.Data;
using pdfforge.LicenseValidator.Interface;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeLicenseChecker : ILicenseChecker
    {
        private readonly Activation _activation;

        public DesignTimeLicenseChecker()
        {
            _activation = new Activation(true);
            _activation.SetResult(Result.OK, "Ok");
            _activation.TimeOfActivation = DateTime.Today;
            _activation.ActivatedTill = DateTime.Today.AddDays(7);
            _activation.LicenseExpires = DateTime.Today.AddDays(7);
            _activation.Key = "SOME-KEY";
            _activation.Licensee = "pdfforge";
            _activation.MachineId = "my-machine";
        }

        public Option<Activation, LicenseError> GetSavedActivation()
        {
            return _activation.Some<Activation, LicenseError>();
        }

        public Option<Activation, LicenseError> GetActivation()
        {
            return _activation.Some<Activation, LicenseError>();
        }

        public Option<string, LicenseError> GetSavedLicenseKey()
        {
            return _activation.Key.Some<string, LicenseError>();
        }

        public Option<Activation, LicenseError> ActivateWithKey(string key)
        {
            return _activation.Some<Activation, LicenseError>();
        }

        public Option<Activation, LicenseError> ActivateWithoutSaving(string key)
        {
            return _activation.Some<Activation, LicenseError>();
        }

        public void SaveActivation(Activation activation)
        {
        }
    }
}
