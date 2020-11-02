using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.Conversion.Settings
{
    public partial class ConversionProfile
    {
        public bool IsDefault => Guid == ProfileGuids.DEFAULT_PROFILE_GUID;

        public bool HasEnabledSecurity => PdfSettings.Security.Enabled && !HasNotSupportedEncryption();

        public bool HasNotSupportedEncryption()
        {
            return PdfSettings.Security.Enabled && OutputFormat != OutputFormat.Pdf;
        }

        public bool HasEnabledSendActions =>

            Ftp.Enabled
            || EmailClientSettings.Enabled
            || HttpSettings.Enabled
            || EmailSmtpSettings.Enabled
            || DropboxSettings.Enabled
            || Printing.Enabled;

    }

}


