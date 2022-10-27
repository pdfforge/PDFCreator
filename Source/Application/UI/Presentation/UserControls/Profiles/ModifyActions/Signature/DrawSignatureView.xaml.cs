using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Signature
{
    public partial class DrawSignatureView
    {
        private readonly DrawSignatureViewModel _viewModel;
        private Path _currentPath;
        private PathGeometry _currentPathGeometry;
        private PathFigure _currentPathFigure;

        public DrawSignatureView(DrawSignatureViewModel viewModel)
        {
            DataContext = viewModel;
            _viewModel = viewModel;

            InitializeComponent();
        }

        private void CanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || sender is not FrameworkElement frameworkElement)
                return;

            // Default of PathFigure.IsClosed is false
            // In order to draw a dot we need a temporary closed PathFigure
            var closedFigure = new PathFigure
            {
                StartPoint = e.GetPosition(frameworkElement),
                IsClosed = true
            };
            var closedSegment = new LineSegment { Point = e.GetPosition(frameworkElement) };
            closedFigure.Segments.Add(closedSegment);

            _currentPathGeometry = new PathGeometry();
            _currentPath = new Path { Data = _currentPathGeometry };
            _currentPathFigure = new PathFigure { StartPoint = e.GetPosition(frameworkElement) };
            _currentPathFigure.Segments.Add(closedSegment);
            _currentPathGeometry.Figures.Add(closedFigure);
            _currentPathGeometry.Figures.Add(_currentPathFigure);

            _currentPath.StrokeThickness = _viewModel.BrushSize;
            _currentPath.Stroke = new SolidColorBrush(_viewModel.BrushColor);

            PaintSurface.Children.Add(_currentPath);
        }

        private void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || sender is not FrameworkElement frameworkElement)
            {
                if (!_viewModel.CanSaveAndReset && PaintSurface.Children.Count > 0)
                {
                    _viewModel.CanSaveAndReset = true;
                    _viewModel.SaveCommand.RaiseCanExecuteChanged();
                    _viewModel.ResetCommand.RaiseCanExecuteChanged();
                }
                return;
            }

            var lineSegment = new LineSegment()
            {
                Point = e.GetPosition(frameworkElement),
                IsSmoothJoin = true
            };

            _currentPathFigure.Segments.Add(lineSegment);
        }

        private void CanvasMouseUp(object sender, MouseEventArgs e)
        {
            _viewModel.CanSaveAndReset = true;
            _viewModel.SaveCommand.RaiseCanExecuteChanged();
            _viewModel.ResetCommand.RaiseCanExecuteChanged();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _viewModel.PaintSurface = PaintSurface;
            PaintSurface = _viewModel.PaintSurface;
        }
    }
}
