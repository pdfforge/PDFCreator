using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.ServiceLocator;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities.Web;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls
{
    public class LicenseExpirationReminderViewModel : TranslatableViewModelBase<LicenseExpirationReminderTranslation>, IWhitelisted
    {
        private readonly ILicenseExpirationReminder _licenseExpirationReminder;
        private readonly IWebLinkLauncher _webLinkLauncher;
        public ICommand RemindMeLaterCommand { get; }
        public ICommand ManageLicensesCommand { get; }
        public bool ShowLicenseExpireReminder { get; set; }

        public LicenseExpirationReminderViewModel(ILicenseExpirationReminder licenseExpirationReminder, ICommandLocator commandLocator,
            ITranslationUpdater translationUpdater, IWebLinkLauncher webLinkLauncher) : base(translationUpdater)
        {
            _licenseExpirationReminder = licenseExpirationReminder;
            _webLinkLauncher = webLinkLauncher;

            ManageLicensesCommand = commandLocator?.CreateMacroCommand()
                .AddCommand(new DelegateCommand(_ => ManageLicensesCommandExecute()))
                .AddCommand(new DelegateCommand(_ => SetReminderForLicenseExpiration()))
                .Build();

            ShowLicenseExpireReminder = _licenseExpirationReminder.IsExpirationReminderDue();

            RemindMeLaterCommand = new DelegateCommand(o => SetReminderForLicenseExpiration());
        }

        private void ManageLicensesCommandExecute()
        {
            _webLinkLauncher.Launch(Urls.LicenseServerUrl);
        }

        public string LicenseReminderInfo => Translation.FormatLicenseExpiryDate(_licenseExpirationReminder.DaysTillLicenseExpires);

        private void SetReminderForLicenseExpiration()
        {
            _licenseExpirationReminder.SetReminderForLicenseExpiration();
            ShowLicenseExpireReminder = false;
            RaisePropertyChanged(nameof(ShowLicenseExpireReminder));
        }
    }

    internal class DesignTimeLicenseExpirationReminderViewModel : LicenseExpirationReminderViewModel
    {
        public DesignTimeLicenseExpirationReminderViewModel() : base(new DesignTimeLicenseExpirationReminder(), new DesignTimeCommandLocator(), new DesignTimeTranslationUpdater(), null)
        {
        }
    }
}
