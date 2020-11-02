using pdfforge.DataStorage;

namespace pdfforge.PDFCreator.Conversion.Settings
{
    public partial class HttpAccount
    {
        public string AccountInfo => IsBasicAuthentication ? $"{UserName} | {Url}" : Url;
  
        public void CopyTo(HttpAccount targetAccount)
        {
            var data = Data.CreateDataStorage();
            StoreValues(data, "");
            targetAccount.ReadValues(data, "");
        }
    }
}
