using System.Windows;

namespace pdfforge.PDFCreator.UI.Presentation.Styles.ContextMenuButton
{
    public class DataContextProxy<T> : Freezable
    {
        protected override Freezable CreateInstanceCore()
        {
            return new DataContextProxy<T>();
        }

        public T DataContext
        {
            get { return (T)GetValue(DataContextProperty); }
            set { SetValue(DataContextProperty, value); }
        }

        public static readonly DependencyProperty DataContextProperty =
            DependencyProperty.Register(nameof(DataContext), typeof(T), typeof(DataContextProxy<T>), new UIPropertyMetadata(null));
    }
}
