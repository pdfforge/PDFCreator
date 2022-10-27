using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Assistants.Update;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using System.ComponentModel;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings
{
    public class GeneralSettingsViewModel: BindableBase, IMountable
    {
        public GeneralSettingsViewModel()
        {
        }
        
        public void MountView()
        {
        }

        public void UnmountView()
        {
        }
    }

    public class DesignTimeGeneralSettingsViewModel : GeneralSettingsViewModel
    {
        public DesignTimeGeneralSettingsViewModel() : base()
        {
        }
    }
}
