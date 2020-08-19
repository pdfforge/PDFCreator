using pdfforge.PDFCreator.Conversion.Jobs.Annotations;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using pdfforge.PDFCreator.Core.Services;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public class ConversionProfileWrapper : INotifyPropertyChanged, IMountable
    {
        public ConversionProfile ConversionProfile { get; }
        private readonly Dictionary<string, HashSet<string>> _watchedProperties = new Dictionary<string, HashSet<string>>();

        public ConversionProfileWrapper(ConversionProfile conversionProfile)
        {
            ConversionProfile = conversionProfile;
            MountView();
        }

        public void MountView()
        {
            ConversionProfile.PdfSettings.Security.PropertyChanged += OnPropertyChangedHandler(nameof(ConversionProfile.PdfSettings.Security.Enabled), nameof(HasEnabledSecurity));

            var hasEnabledSendActionsName = nameof(HasEnabledSendActions);

            ConversionProfile.Ftp.PropertyChanged += OnPropertyChangedHandler(nameof(ConversionProfile.Ftp.Enabled), hasEnabledSendActionsName);
            ConversionProfile.EmailClientSettings.PropertyChanged += OnPropertyChangedHandler(nameof(ConversionProfile.EmailClientSettings.Enabled), hasEnabledSendActionsName);
            ConversionProfile.HttpSettings.PropertyChanged += OnPropertyChangedHandler(nameof(ConversionProfile.HttpSettings.Enabled), hasEnabledSendActionsName);
            ConversionProfile.EmailSmtpSettings.PropertyChanged += OnPropertyChangedHandler(nameof(ConversionProfile.EmailSmtpSettings.Enabled), hasEnabledSendActionsName);
            ConversionProfile.DropboxSettings.PropertyChanged += OnPropertyChangedHandler(nameof(ConversionProfile.DropboxSettings.Enabled), hasEnabledSendActionsName);
            ConversionProfile.Printing.PropertyChanged += OnPropertyChangedHandler(nameof(ConversionProfile.Printing.Enabled), hasEnabledSendActionsName);

            ConversionProfile.PropertyChanged += ConversionProfileOnPropertyChanged;
        }

        private void ConversionProfileOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ConversionProfile.Name):
                    OnPropertyChanged(nameof(Name));
                    break;

                case nameof(ConversionProfile.OutputFormat):
                    OnPropertyChanged(nameof(HasEnabledSecurity));
                    break;
            }
        }

        public void UnmountView()
        {
            ConversionProfile.Ftp.PropertyChanged -= OnSendActionPropertyChanged;
            ConversionProfile.EmailClientSettings.PropertyChanged -= OnSendActionPropertyChanged;
            ConversionProfile.HttpSettings.PropertyChanged -= OnSendActionPropertyChanged;
            ConversionProfile.EmailSmtpSettings.PropertyChanged -= OnSendActionPropertyChanged;
            ConversionProfile.DropboxSettings.PropertyChanged -= OnSendActionPropertyChanged;
            ConversionProfile.Printing.PropertyChanged -= OnSendActionPropertyChanged;

            ConversionProfile.PropertyChanged -= ConversionProfileOnPropertyChanged;
        }

        private void OnSendActionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var propertyName = e.PropertyName;
            if (_watchedProperties.ContainsKey(e.PropertyName))
            {
                foreach (var property in _watchedProperties[propertyName])
                {
                    OnPropertyChanged(property);
                }
            }
        }

        private PropertyChangedEventHandler OnPropertyChangedHandler(string settingsProperty, string wrapperProperty)
        {
            if (!_watchedProperties.ContainsKey(settingsProperty))
                _watchedProperties[settingsProperty] = new HashSet<string>();

            _watchedProperties[settingsProperty].Add(wrapperProperty);
            return OnSendActionPropertyChanged;
        }

        public bool HasEnabledSendActions =>
            ConversionProfile.Ftp.Enabled
            || ConversionProfile.EmailClientSettings.Enabled
            || ConversionProfile.HttpSettings.Enabled
            || ConversionProfile.EmailSmtpSettings.Enabled
            || ConversionProfile.DropboxSettings.Enabled
            || ConversionProfile.Printing.Enabled;

        public string Name => ConversionProfile.Name;

        public bool HasEnabledSecurity => ConversionProfile.PdfSettings.Security.Enabled && !HasNotSupportedEncryption();

        private bool HasNotSupportedEncryption()
        {
            return ConversionProfile.PdfSettings.Security.Enabled && ConversionProfile.OutputFormat != OutputFormat.Pdf;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
