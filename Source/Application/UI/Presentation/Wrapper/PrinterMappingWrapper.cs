using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.Annotations;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace pdfforge.PDFCreator.UI.Presentation.Wrapper
{
    public class PrinterMappingWrapper : INotifyPropertyChanged
    {
        private ConversionProfileWrapper _conversionProfile;
        private string _primaryPrinter;
        private string _printerName;

        public PrinterMappingWrapper(PrinterMapping printerMapping, IEnumerable<ConversionProfileWrapper> profiles)
        {
            PrinterMapping = printerMapping;
            PrinterName = printerMapping.PrinterName;
            Profile = profiles.FirstOrDefault(p => p.ConversionProfile.Guid == printerMapping.ProfileGuid);
        }

        public PrinterMapping PrinterMapping { get; }

        public string PrinterName
        {
            get { return _printerName; }
            set
            {
                _printerName = value;
                PrinterMapping.PrinterName = _printerName;
                OnPropertyChanged(nameof(PrinterName));
                OnPropertyChanged(nameof(IsPrimaryPrinter));
            }
        }

        public ConversionProfileWrapper Profile
        {
            get { return _conversionProfile; }
            set
            {
                _conversionProfile = value;
                PrinterMapping.ProfileGuid = _conversionProfile == null ? "" : _conversionProfile.ConversionProfile.Guid;
                OnPropertyChanged(nameof(Profile));
            }
        }

        public bool IsPrimaryPrinter
        {
            get { return PrinterName != null && PrinterName == PrimaryPrinter; }
        }

        public string PrimaryPrinter
        {
            get { return _primaryPrinter; }
            set
            {
                //if (value == _primaryPrinter) return; //Don't! Primaryprinter must always be updated to prevent default behaviour of bound checkbox!
                _primaryPrinter = value;
                OnPropertyChanged(nameof(PrimaryPrinter));
                OnPropertyChanged(nameof(IsPrimaryPrinter));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
