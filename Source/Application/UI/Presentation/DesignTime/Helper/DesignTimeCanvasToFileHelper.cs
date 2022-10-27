using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Signature;
using System.Drawing;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper
{
    public class DesignTimeCanvasToFileHelper : ICanvasToFileHelper
    {
        public Bitmap CanvasToBitmap(Canvas paintSurface)
        {
            return new Bitmap(1, 1);
        }

        public Bitmap CropSignature(Bitmap bitmap)
        {
            return new Bitmap(1, 1);
        }

        public bool SaveToFile(Bitmap croppedSignature, string path)
        {
            return false;
        }
    }
}
