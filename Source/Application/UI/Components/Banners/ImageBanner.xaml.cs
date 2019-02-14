using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Banners
{
    public partial class ImageBanner : UserControl
    {
        public ImageBanner()
        {
            InitializeComponent();
        }

        public void SetData(string imagePath, string clickUrl)
        {
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(imagePath, UriKind.Absolute);
            src.EndInit();
            Image.Source = src;

            Button.CommandParameter = clickUrl;
        }
    }
}
