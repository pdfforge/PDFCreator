using pdfforge.Obsidian.Interaction;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.Wrapper;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Printer
{
    public class EditPrinterProfileUserInteraction : IInteraction
    {
        public readonly PrinterMappingWrapper PrinterMappingWrapper;
        public ConversionProfileWrapper ResultProfile;
        public ObservableCollection<ConversionProfileWrapper> ProfileWrappers { get; set; }
        public bool Success = false;

        public EditPrinterProfileUserInteraction(PrinterMappingWrapper printerMappingWrapper, ObservableCollection<ConversionProfileWrapper> profiles)
        {
            PrinterMappingWrapper = printerMappingWrapper;
            ProfileWrappers = profiles;
        }

    }
}
